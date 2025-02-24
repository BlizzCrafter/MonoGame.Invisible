using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MonoGame.Invisible
{
    /// <summary>
    /// Implementation for Per-Pixel Alpha mode.
    /// In this mode, each frame the render target is converted into a GDI Bitmap and written to the window via UpdateLayeredWindow.
    /// This is a slow and NOT prefered way of making your MonoGame window transparent!
    /// Use it for tinkering or if the ColorKey version does not satisfy you.
    /// </summary>
    public class PerPixelAlphaWindowManager : ITransparentWindowManager
    {
        public nint WindowHandle { get; private set; }
        public Color TransparentColor { get; private set; } = Color.Transparent;
        public bool SwapRedBlueChannels { get; set; } = true;

        private readonly GraphicsDevice _graphicsDevice;
        public RenderTarget2D _renderTarget;

        // Update interval for the bitmap transfer (e.g., 100 ms)
        // Reduce to get more smoothness with the (high!) cost of performance.
        public TimeSpan UpdateInterval
        {
            get { return _updateInterval; }
            set { _updateInterval = value; }
        }
        private TimeSpan _updateInterval = TimeSpan.FromMilliseconds(100);
        private TimeSpan _timeSinceLastUpdate = TimeSpan.Zero;

        private int _width;
        private int _height;

        /// <summary>
        /// Gets the internally managed render target.
        /// </summary>
        public RenderTarget2D RenderTarget
        {
            get { return _renderTarget; }
            set { _renderTarget = value; }
        }

        public PerPixelAlphaWindowManager(nint windowHandle, GraphicsDevice graphicsDevice, int width, int height)
        {
            WindowHandle = windowHandle;
            _graphicsDevice = graphicsDevice;
            _width = width;
            _height = height;
        }

        public void Initialize()
        {
            if (WindowHandle == nint.Zero)
                throw new InvalidOperationException("Invalid window handle.");

            int exStyle = Win32Helper.GetWindowLong(WindowHandle, Win32Helper.GWL_EXSTYLE);
            if (exStyle == 0)
            {
                int error = Marshal.GetLastWin32Error();
                throw new InvalidOperationException("GetWindowLong failed. Error code: " + error);
            }
            
            int newExStyle = (exStyle & ~Win32Helper.WS_EX_APPWINDOW) | Win32Helper.WS_EX_LAYERED | Win32Helper.WS_EX_TOOLWINDOW | Win32Helper.WS_EX_NOACTIVATE;
            int setResult = Win32Helper.SetWindowLong(WindowHandle, Win32Helper.GWL_EXSTYLE, newExStyle);
            if (setResult == 0)
            {
                int error = Marshal.GetLastWin32Error();
                throw new InvalidOperationException("SetWindowLong failed. Error code: " + error);
            }

            // Create the render target internally using the provided dimensions.
            _renderTarget = new RenderTarget2D(_graphicsDevice, _width, _height, false, SurfaceFormat.Color, DepthFormat.None);
        }

        public void Update(GameTime gameTime)
        {
            // Accumulate elapsed time for the update interval
            _timeSinceLastUpdate += gameTime.ElapsedGameTime;
        }

        public void UpdateWindow(GameTime gameTime)
        {
            int width = _renderTarget.Width;
            int height = _renderTarget.Height;

            Color[] pixelData = new Color[width * height];
            _renderTarget.GetData(pixelData);

            // Need to swap red and blue channels.
            // It takes additional performance.
            // You probably want to adjust your used colors instead (Swap Red for Blue).
            if (SwapRedBlueChannels)
            {
                for (int i = 0; i < pixelData.Length; i++)
                {
                    byte temp = pixelData[i].B;
                    pixelData[i].B = pixelData[i].R;
                    pixelData[i].R = temp;
                }
                _renderTarget.SetData(pixelData);
            }

            using (var bmp = new System.Drawing.Bitmap(width, height, PixelFormat.Format32bppArgb))
            {
                var rect = new System.Drawing.Rectangle(0, 0, width, height);
                BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

                int[] pixelInts = new int[width * height];
                for (int i = 0; i < pixelData.Length; i++)
                {
                    pixelInts[i] = (int)pixelData[i].PackedValue;
                }
                Marshal.Copy(pixelInts, 0, bmpData.Scan0, pixelInts.Length);
                bmp.UnlockBits(bmpData);

                nint screenDC = Win32Helper.GetDC(nint.Zero);
                nint memDC = Win32Helper.CreateCompatibleDC(screenDC);
                nint hBitmap = bmp.GetHbitmap(System.Drawing.Color.FromArgb(0));
                nint oldBitmap = Win32Helper.SelectObject(memDC, hBitmap);

                Win32Helper.POINT topPos = new Win32Helper.POINT { x = 0, y = 0 };
                Win32Helper.SIZE size = new Win32Helper.SIZE { cx = width, cy = height };
                Win32Helper.POINT srcPos = new Win32Helper.POINT { x = 0, y = 0 };

                Win32Helper.BLENDFUNCTION blend = new Win32Helper.BLENDFUNCTION
                {
                    BlendOp = Win32Helper.AC_SRC_OVER,
                    BlendFlags = 0,
                    SourceConstantAlpha = 255, // Global alpha is ignored; per-pixel alpha is used.
                    AlphaFormat = Win32Helper.AC_SRC_ALPHA
                };

                bool result = Win32Helper.UpdateLayeredWindow(WindowHandle, screenDC, ref topPos, ref size,
                    memDC, ref srcPos, 0, ref blend, Win32Helper.ULW_ALPHA);

                if (!result)
                {
                    int error = Marshal.GetLastWin32Error();
                    System.Diagnostics.Debug.WriteLine("UpdateLayeredWindow failed. Error code: " + error);
                }

                Win32Helper.SelectObject(memDC, oldBitmap);
                Win32Helper.DeleteObject(hBitmap);
                Win32Helper.DeleteDC(memDC);
                Win32Helper.ReleaseDC(nint.Zero, screenDC);
            }
        }

        public void BeginDraw()
        {
            // Render into the offscreen render target.
            _graphicsDevice.SetRenderTarget(_renderTarget);

            // IMPORTANT: In PerPixel mode, the background must be cleared with the regular "Transparent" color.
            _graphicsDevice.Clear(Color.Transparent);
        }

        public void EndDraw(GameTime gameTime)
        {
            // Reset the render target.
            _graphicsDevice.SetRenderTarget(null);

            // Update the window content (bitmap transfer) only if the interval has been reached.
            if (_timeSinceLastUpdate >= _updateInterval)
            {
                UpdateWindow(gameTime);
                _timeSinceLastUpdate = TimeSpan.Zero;
            }
        }
    }
}