using Microsoft.Xna.Framework;
using Color = Microsoft.Xna.Framework.Color;

namespace MonoGame.Invisible
{
    /// <summary>
    /// Factory for creating a TransparentWindowManager.
    /// </summary>
    public static class TransparentWindowManager
    {
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

        private static bool _setupCalled = false;
        private static int _width;
        private static int _height;

        private static void _Setup(Game game, GraphicsDeviceManager graphics, int width, int height, bool fullscreen)
        {
            _setupCalled = true;

            _width = width;
            _height = height;

            game.IsMouseVisible = true;
            game.Window.IsBorderless = true;
            game.Window.AllowUserResizing = false;

            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.HardwareModeSwitch = false; // Important!
            graphics.IsFullScreen = fullscreen;
            graphics.ApplyChanges();
        }

        public static void Setup(Game game, GraphicsDeviceManager graphics)
        {
            var screenWidth = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            var screenHeight = graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;

            _Setup(game, graphics, screenWidth, screenHeight, true);
        }

        public static void Setup(Game game, GraphicsDeviceManager graphics, int width, int height)
        {
            _Setup(game, graphics, width, height, false);
        }

        /// <summary>
        /// Creates a TransparentWindowManager based on the selected transparency mode.
        /// </summary>
        /// <param name="game">The current Game instance (to retrieve the window handle and GraphicsDevice).</param>
        /// <param name="mode">The desired transparency mode.</param>
        /// <param name="transparentColor">For the ColorKey mode: the color that should be transparent.</param>
        /// <returns>An instance of ITransparentWindowManager.</returns>
        public static ITransparentWindowManager Create(Game game, TransparencyMode mode, Color? transparentColor = null)
        {
            if (!_setupCalled)
            {
                throw new InvalidOperationException("You need to setup the library first! Call: 'TransparentWindowManager.Setup()'");
            }

            AppName = game.Window.Title;

            switch (mode)
            {
                case TransparencyMode.ColorKey:
                    if (!transparentColor.HasValue)
                        throw new ArgumentException("A transparent color must be provided for the ColorKey mode.");
                    return new ColorKeyWindowManager(game.Window.Handle, game.GraphicsDevice, transparentColor.Value);

                case TransparencyMode.PerPixelAlpha:
                    return new PerPixelAlphaWindowManager(game.Window.Handle, game.GraphicsDevice, _width, _height);

                default:
                    throw new ArgumentException("Unsupported transparency mode.");
            }
        }
    }
}
