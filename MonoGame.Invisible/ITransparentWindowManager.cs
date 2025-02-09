using Microsoft.Xna.Framework;

namespace MonoGame.Invisible
{
    /// <summary>
    /// Common interface for transparent window managers.
    /// Provides methods for updating the layered window and wrapping drawing code.
    /// </summary>
    public interface ITransparentWindowManager
    {
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
        /// <param name="gameTime">Current game time.</param>
        void BeginDraw(GameTime gameTime);

        /// <summary>
        /// Ends the drawing phase.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        void EndDraw(GameTime gameTime);
    }
}