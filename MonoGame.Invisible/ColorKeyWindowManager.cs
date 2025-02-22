using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace MonoGame.Invisible
{
    /// <summary>
    /// Implementation for the ColorKey mode.
    /// The window background is made transparent using SetLayeredWindowAttributes.
    /// This is the fast and prefered way of making your MonoGame window transparent!
    /// </summary>
    public class ColorKeyWindowManager : ITransparentWindowManager
    {
        public nint WindowHandle { get; private set; }
        public Color TransparentColor { get; private set; }

        private readonly GraphicsDevice _graphicsDevice;

        public ColorKeyWindowManager(nint windowHandle, GraphicsDevice graphicsDevice, Color transparentColor)
        {
            WindowHandle = windowHandle;
            _graphicsDevice = graphicsDevice;
            TransparentColor = transparentColor;
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

            int newExStyle = exStyle | Win32Helper.WS_EX_LAYERED;
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
        /// No update logic is needed for ColorKey mode.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            
        }

        public void BeginDraw()
        {
            // Clear the GraphicsDevice with the transparent color key.
            _graphicsDevice.Clear(TransparentColor);
        }

        /// <summary>
        /// Nothing to do in EndDraw for ColorKey mode.
        /// </summary>
        public void EndDraw(GameTime gameTime)
        {
        }
    }
}
