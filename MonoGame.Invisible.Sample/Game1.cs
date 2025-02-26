using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Color = Microsoft.Xna.Framework.Color;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MonoGame.Invisible.Sample
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Example texture (e.g. a logo)
        Texture2D _logo;

        // Render target for global alpha checking.
        RenderTarget2D _sceneRenderTarget;
        // Array to hold render target pixel data.
        Color[] _sceneData;

        // Logo position and dragging variables.
        Vector2 _logoPosition;
        bool _isDragging = false;
        Vector2 _dragOffset;
        MouseState _previousMouseState;

        // The transparent window manager.
        ITransparentWindowManager _windowManager;

        public Game1()
        {
            Window.Title = "MonoGame Invisible Sample";

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / 60);
            graphics.SynchronizeWithVerticalRetrace = true;
        }

        protected override void Initialize()
        {
            // Setup the TransparentWindowManager.
            TransparentWindowManager.Setup(this, graphics);

            // Choose the desired transparency mode:
            // Option 1: ColorKey mode (e.g. using 'Magenta' as the transparent color)
            // Fast, prefered and recommended way of making your MonoGame window transparent!
            _windowManager = TransparentWindowManager.Create(this, TransparencyMode.ColorKey, new Color(1, 1, 1));

            // Option 2: Per-Pixel Alpha mode
            // Slow and NOT the prefered way of making your MonoGame window transparent!
            // Use it for tinkering or if the ColorKey version does not satisfy you.
            //_windowManager = TransparentWindowManager.Create(this, TransparencyMode.PerPixelAlpha);

            // Initialize the TransparentWindowManager.
            _windowManager.Initialize();

            // The window will stay in the background - even on user interaction.
            _windowManager.KeepInBackground();

            TrayIconManager.Init(
                Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location));

            var runOnBoot = new ToolStripMenuItem("Run On Boot", null, RunOnBoot) { Checked = StartupManager.IsAutostartEnabled() };
            var exitItem = new ToolStripMenuItem("Exit", null, ExitApplication);

            TrayIconManager.ContextMenu.Items.AddRange(
                [runOnBoot, new ToolStripSeparator(), exitItem]);

            _previousMouseState = Mouse.GetState();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _logo = Content.Load<Texture2D>("Logo");

            // Initialize logo position.
            _logoPosition = new Vector2(
                (graphics.PreferredBackBufferWidth / 2) - (_logo.Width / 2),
                (graphics.PreferredBackBufferHeight / 2) - (_logo.Height / 2));

            // Create a render target matching the window size.
            _sceneRenderTarget = new RenderTarget2D(GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            _sceneData = new Color[graphics.PreferredBackBufferWidth * graphics.PreferredBackBufferHeight];
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                ExitApplication();

            HandleLogoDragging();

            // Update the transparent window manager.
            _windowManager.Update(gameTime);

            // Ensure window stays in the back.
            if (_windowManager.IsForegroundWindow())
            {
                _windowManager.SendToBack();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _windowManager.BeginDraw();
            DrawScene();
            _windowManager.EndDraw(gameTime);
        }

        /// <summary>
        /// Draws the current scene.
        /// This method is used in both the Draw() method and for updating the alpha data.
        /// </summary>
        private void DrawScene()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_logo, _logoPosition, Color.White);
            spriteBatch.End();
        }

        private void RunOnBoot(object? sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item)
            {
                item.Checked = !item.Checked;
                if (item.Checked)
                    StartupManager.SetAutostart(true);
                else
                    StartupManager.SetAutostart(false);
            }
        }

        private void ExitApplication(object? sender, EventArgs e)
        {
            ExitApplication();
        }

        private void ExitApplication()
        {
            TrayIconManager.Dispose();
            Exit();
            Application.Exit();
        }

        /// <summary>
        /// Handles the dragging logic for the logo texture.
        /// Dragging is only activated when the initial click is on an opaque part of the current scene.
        /// </summary>
        private void HandleLogoDragging()
        {
            MouseState currentMouseState = Mouse.GetState();
            Point mousePoint = currentMouseState.Position;
            Rectangle logoRect = new Rectangle(_logoPosition.ToPoint(), new Point(_logo.Width, _logo.Height));

            // Check if the left button transitioned from released to pressed.
            if (currentMouseState.LeftButton == ButtonState.Pressed &&
                _previousMouseState.LeftButton == ButtonState.Released)
            {
                // Update the alpha data using the current drawing from DrawScene.
                UpdateAlphaData();

                // Start dragging only if the click is within the logo and the pixel is opaque.
                if (logoRect.Contains(mousePoint) && IsGlobalPixelOpaque(mousePoint))
                {
                    _isDragging = true;
                    _dragOffset = new Vector2(mousePoint.X, mousePoint.Y) - _logoPosition;
                }
                else
                {
                    _isDragging = false;
                }
            }

            if (currentMouseState.LeftButton == ButtonState.Pressed && _isDragging)
            {
                _logoPosition = new Vector2(mousePoint.X, mousePoint.Y) - _dragOffset;
            }
            else if (currentMouseState.LeftButton == ButtonState.Released)
            {
                _isDragging = false;
            }

            _previousMouseState = currentMouseState;
        }

        /// <summary>
        /// Returns true if the pixel in the render target at the given mouse position is opaque.
        /// </summary>
        /// <param name="mousePoint">The mouse position in window coordinates.</param>
        /// <returns>True if the pixel’s alpha is at least 128; otherwise, false.</returns>
        private bool IsGlobalPixelOpaque(Point mousePoint)
        {
            if (mousePoint.X < 0 || mousePoint.Y < 0 ||
                mousePoint.X >= _sceneRenderTarget.Width || mousePoint.Y >= _sceneRenderTarget.Height)
                return false;

            int index = mousePoint.Y * _sceneRenderTarget.Width + mousePoint.X;
            Color pixelColor = _sceneData[index];

            return pixelColor.A >= 128;
        }

        /// <summary>
        /// Updates the render target with the current scene drawing and retrieves its pixel data.
        /// This is called only on an initial mouse click, so that the drawing is always current.
        /// </summary>
        private void UpdateAlphaData()
        {
            GraphicsDevice.SetRenderTarget(_sceneRenderTarget);
            GraphicsDevice.Clear(Color.Transparent);
            DrawScene();
            GraphicsDevice.SetRenderTarget(null);
            _sceneRenderTarget.GetData(_sceneData);
        }
    }
}
