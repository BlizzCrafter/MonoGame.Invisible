using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;

namespace MonoGame.Invisible
{
    /// <summary>
    /// Factory for creating a TransparentWindowManager.
    /// </summary>
    public static class TransparentWindowManager
    {
        public static ColorKeyWindowManager Window => _window;
        private static ColorKeyWindowManager _window;

        /// <summary>
        /// The name of the application that will be used as the "Tray-Icon-Name" and "Registry-Key-Name" (Auto-Start / StartOnBoot).
        /// </summary>
        /// <remarks>
        /// This name will be auto-set from the <c>GameWindow.Title</c> property, but can (and probably should!) be reset here manually (e.g. to avoid special characters in the registry key).
        /// </remarks>
        public static string AppName
        {
            get { return _appName; }
            set
            {
                if (StartupManager.IsAutostartEnabled())
                {
                    StartupManager.SetAutostart(false);
                    _appName = value;
                    StartupManager.SetAutostart(true);
                }
                else _appName = value;
            }
        }
        private static string _appName = "MonoGame.Invisible";

        /// <summary>
        /// Initializes the TransparentWindowManager with the specified game, graphics device manager, and transparent color.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="graphics">The graphics device manager.</param>
        /// <param name="transparentColor">The transparent color to use.</param>
        public static void Init(Game game, GraphicsDeviceManager graphics, Color transparentColor = default)
        {
            AppName = game.Window.Title;

            game.IsMouseVisible = true;
            game.Window.IsBorderless = true;
            game.Window.AllowUserResizing = false;

            graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            graphics.HardwareModeSwitch = false; // Important!
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            var color = transparentColor == default ? new Color(1, 1, 1, 0) : transparentColor;
            _window = new ColorKeyWindowManager(game.Window.Handle, game.GraphicsDevice, color);
        }
    }
}
