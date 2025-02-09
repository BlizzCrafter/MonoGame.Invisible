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
        public Color TransparentColor { get; private set; } = Color.Transparent;
        public bool SwapRedBlueChannels { get; set; } = true;

        private readonly nint _hWnd;
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
            _hWnd = windowHandle;
            _graphicsDevice = graphicsDevice;
            _width = width;
            _height = height;
        }

        public void Initialize()
        {
            if (_hWnd == nint.Zero)
                throw new InvalidOperationException("Invalid window handle.");

            int exStyle = Win32Helper.GetWindowLong(_hWnd, Win32Helper.GWL_EXSTYLE);
            if (exStyle == 0)
            {
                int error = Marshal.GetLastWin32Error();
                throw new InvalidOperationException("GetWindowLong failed. Error code: " + error);
            }

            int newExStyle = exStyle | Win32Helper.WS_EX_LAYERED;
            int setResult = Win32Helper.SetWindowLong(_hWnd, Win32Helper.GWL_EXSTYLE, newExStyle);
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

                nint screenDC = NativeMethods.GetDC(nint.Zero);
                nint memDC = NativeMethods.CreateCompatibleDC(screenDC);
                nint hBitmap = bmp.GetHbitmap(System.Drawing.Color.FromArgb(0));
                nint oldBitmap = NativeMethods.SelectObject(memDC, hBitmap);

                Win32Native.POINT topPos = new Win32Native.POINT { x = 0, y = 0 };
                Win32Native.SIZE size = new Win32Native.SIZE { cx = width, cy = height };
                Win32Native.POINT srcPos = new Win32Native.POINT { x = 0, y = 0 };

                Win32Native.BLENDFUNCTION blend = new Win32Native.BLENDFUNCTION
                {
                    BlendOp = Win32Native.AC_SRC_OVER,
                    BlendFlags = 0,
                    SourceConstantAlpha = 255, // Global alpha is ignored; per-pixel alpha is used.
                    AlphaFormat = Win32Native.AC_SRC_ALPHA
                };

                bool result = Win32Native.UpdateLayeredWindow(_hWnd, screenDC, ref topPos, ref size,
                    memDC, ref srcPos, 0, ref blend, Win32Native.ULW_ALPHA);

                if (!result)
                {
                    int error = Marshal.GetLastWin32Error();
                    System.Diagnostics.Debug.WriteLine("UpdateLayeredWindow failed. Error code: " + error);
                }

                NativeMethods.SelectObject(memDC, oldBitmap);
                NativeMethods.DeleteObject(hBitmap);
                NativeMethods.DeleteDC(memDC);
                NativeMethods.ReleaseDC(nint.Zero, screenDC);
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