using Microsoft.Xna.Framework;

namespace MonoGame.Invisible
{
    /// <summary>
    /// Common interface for transparent window managers.
    /// Provides methods for updating the layered window and wrapping drawing code.
    /// </summary>
    public interface ITransparentWindowManager
    {
        /// <summary>
        /// The handle of the created game window.
        /// </summary>
        nint WindowHandle { get; }

        /// <summary>
        /// Gets the color used for transparency.
        /// </summary>
        Color TransparentColor { get; }

        /// <summary>
        /// Performs necessary initialization.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Updates the window content.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        void Update(GameTime gameTime);

        /// <summary>
        /// Begins the drawing phase.
        /// </summary>
        void BeginDraw();

        /// <summary>
        /// Ends the drawing phase.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        void EndDraw(GameTime gameTime);

        /// <summary>
        /// Brings the window to the front of the Z-order, making it the topmost window.
        /// </summary>
        void BringToFront()
        {
            Win32Helper.BringToFront(WindowHandle);
        }

        /// <summary>
        /// Sends the window to the back of the Z-order, behind other windows.
        /// </summary>
        void SendToBack()
        {
            Win32Helper.SendToBack(WindowHandle);
        }

        /// <summary>
        /// Keeps the window in the background, preventing it from being moved to the front.
        /// </summary>
        void KeepInBackground()
        {
            Win32Helper.KeepInBackground(WindowHandle);
        }

        /// <summary>
        /// Checks if the window is the currently active (focused) window.
        /// </summary>
        /// <returns>True if the window is the foreground window; otherwise, false.</returns>
        bool IsForegroundWindow()
        {
            return Win32Helper.IsForegroundWindow(WindowHandle);
        }

        /// <summary>
        /// Checks if the window is in the background (behind other windows).
        /// </summary>
        /// <returns>True if the window is behind another window; otherwise, false.</returns>
        bool IsInBackground()
        {
            return Win32Helper.IsInBackground(WindowHandle);
        }

        /// <summary>
        /// Checks if the window is the topmost window on the screen.
        /// </summary>
        /// <returns>True if the window is the topmost; otherwise, false.</returns>
        bool IsTopWindow()
        {
            return Win32Helper.IsTopWindow(WindowHandle);
        }
    }
}
