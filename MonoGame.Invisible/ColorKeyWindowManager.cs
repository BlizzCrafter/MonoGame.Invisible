using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;
using Color = Microsoft.Xna.Framework.Color;
using Point = Microsoft.Xna.Framework.Point;

namespace MonoGame.Invisible
{
    /// <summary>
    /// Implementation for a ColorKey window Manager.
    /// In this mode, a specific color is set as transparent for the window.
    /// </summary>
    public class ColorKeyWindowManager
    {
        /// <summary>
        /// Gets the window handle.
        /// </summary>
        public nint WindowHandle { get; private set; }

        /// <summary>
        /// Gets the transparent color.
        /// </summary>
        public Color TransparentColor { get; private set; }

        /// <summary>
        /// Gets or sets the alpha threshold for mouse clicks.
        /// </summary>
        public byte MouseClickAlphaThreshold { get; set; } = 128;

        private readonly GraphicsDevice _graphicsDevice;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorKeyWindowManager"/> class.
        /// </summary>
        /// <param name="windowHandle">The window handle.</param>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="transparentColor">The transparent color.</param>
        /// <exception cref="InvalidOperationException">Thrown when the window handle is invalid or Win32 API calls fail.</exception>
        public ColorKeyWindowManager(nint windowHandle, GraphicsDevice graphicsDevice, Color transparentColor)
        {
            WindowHandle = windowHandle;
            _graphicsDevice = graphicsDevice;
            TransparentColor = transparentColor;

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

            // Set the transparent color key for the window.
            uint colorKey = (uint)System.Drawing.ColorTranslator.ToWin32(
                System.Drawing.Color.FromArgb(TransparentColor.R, TransparentColor.G, TransparentColor.B));
            bool result = Win32Helper.SetLayeredWindowAttributes(WindowHandle, colorKey, 0, Win32Helper.LWA_COLORKEY);
            if (!result)
            {
                int error = Marshal.GetLastWin32Error();
                throw new InvalidOperationException("SetLayeredWindowAttributes failed. Error code: " + error);
            }
        }

        /// <summary>
        /// Prepares the graphics device for drawing by clearing it with the transparent color.
        /// </summary>
        public void PrepareDraw()
        {
            _graphicsDevice.Clear(TransparentColor);
        }

        /// <summary>
        /// Determines whether the pixel at the specified mouse point is opaque.
        /// </summary>
        /// <param name="mousePoint">The mouse point.</param>
        /// <returns>
        ///   <c>true</c> if the pixel is opaque; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPixelOpaque(Point mousePoint)
        {
            if (mousePoint.X < 0 || mousePoint.Y < 0 ||
                mousePoint.X >= _graphicsDevice.PresentationParameters.BackBufferWidth || mousePoint.Y >= _graphicsDevice.PresentationParameters.BackBufferHeight)
                return false;

            var pixelData = new Color[_graphicsDevice.PresentationParameters.BackBufferWidth * _graphicsDevice.PresentationParameters.BackBufferHeight];
            _graphicsDevice.GetBackBufferData(pixelData);

            int index = mousePoint.Y * _graphicsDevice.PresentationParameters.BackBufferWidth + mousePoint.X;
            Color pixelColor = pixelData[index];

            return pixelColor.A >= MouseClickAlphaThreshold;
        }

        /// <summary>
        /// Brings the window to the front of the Z-order, making it the topmost window.
        /// </summary>
        public void BringToFront()
        {
            Win32Helper.BringToFront(WindowHandle);
        }

        /// <summary>
        /// Sends the window to the back of the Z-order, behind other windows.
        /// </summary>
        public void SendToBack()
        {
            Win32Helper.SendToBack(WindowHandle);
        }

        /// <summary>
        /// Keeps the window in the background, preventing it from being moved to the front.
        /// </summary>
        public void KeepInBackground()
        {
            Win32Helper.KeepInBackground(WindowHandle);
        }

        /// <summary>
        /// Keeps the window in the foreground, preventing it from being moved to the back.
        /// </summary>
        public void KeepInForeground()
        {
            Win32Helper.KeepInForeground(WindowHandle);
        }

        /// <summary>
        /// Checks if the window is the currently active (focused) window.
        /// </summary>
        /// <returns>True if the window is the foreground window; otherwise, false.</returns>
        public bool IsForegroundWindow()
        {
            return Win32Helper.IsForegroundWindow(WindowHandle);
        }

        /// <summary>
        /// Checks if the window is in the background (behind other windows).
        /// </summary>
        /// <returns>True if the window is behind another window; otherwise, false.</returns>
        public bool IsInBackground()
        {
            return Win32Helper.IsInBackground(WindowHandle);
        }

        /// <summary>
        /// Checks if the window is the topmost window on the screen.
        /// </summary>
        /// <returns>True if the window is the topmost; otherwise, false.</returns>
        public bool IsTopWindow()
        {
            return Win32Helper.IsTopWindow(WindowHandle);
        }
    }
}
