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

        // Logo position and dragging variables.
        Vector2 _logoPosition;
        bool _isDragging = false;
        Vector2 _dragOffset;
        MouseState _previousMouseState;

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
            TransparentWindowManager.Init(this, graphics);

            // The window will stay in the background - even on user interaction.
            TransparentWindowManager.Window.KeepInBackground();

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
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                ExitApplication();

            HandleLogoDragging();

            // Ensure window stays in the back.
            if (TransparentWindowManager.Window.IsForegroundWindow())
            {
                TransparentWindowManager.Window.SendToBack();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            TransparentWindowManager.Window.PrepareDraw();

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
                // Start dragging only if the click is within the logo and the pixel is opaque.
                if (logoRect.Contains(mousePoint) && TransparentWindowManager.Window.IsPixelOpaque(mousePoint))
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
    }
}
