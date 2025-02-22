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
    }
}