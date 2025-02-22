using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoGame.Invisible.Sample
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Example texture (e.g. a logo)
        Texture2D _logo;

        // Logo position and dragging variables
        Vector2 _logoPosition;
        bool _isDragging = false;
        Vector2 _dragOffset;
        MouseState _previousMouseState;

        // The transparent window manager (depending on the mode)
        ITransparentWindowManager _windowManager;

        public Game1()
        {
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

            // Send the window to the back.
            //_windowManager.SendToBack();

            // Bring the window to the front.
            //_windowManager.BringToFront();

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
                Exit();

            HandleLogoDragging();

            // Not needed in ColorKey mode.
            _windowManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _windowManager.BeginDraw();

            spriteBatch.Begin();
            spriteBatch.Draw(_logo, _logoPosition, Color.White);
            spriteBatch.End();

            // Not needed in ColorKey mode (could still stay here to keep the structure).
            _windowManager.EndDraw(gameTime);
        }

        /// <summary>
        /// Handles the dragging logic for the logo texture.
        /// </summary>
        private void HandleLogoDragging()
        {
            MouseState currentMouseState = Mouse.GetState();
            Point mousePoint = currentMouseState.Position;

            // Define a rectangle representing the logo's area.
            Rectangle logoRect = new Rectangle(_logoPosition.ToPoint(), new Point(_logo.Width, _logo.Height));

            // Check if the left button is pressed.
            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (!_isDragging)
                {
                    // If we are not already dragging, check if the mouse is over the logo.
                    if (logoRect.Contains(mousePoint))
                    {
                        _isDragging = true;
                        // Record the offset from the top-left corner of the logo to the mouse cursor.
                        _dragOffset = new Vector2(mousePoint.X, mousePoint.Y) - _logoPosition;
                    }
                }
                else
                {
                    // If dragging, update the logo position based on the current mouse position and offset.
                    _logoPosition = new Vector2(mousePoint.X, mousePoint.Y) - _dragOffset;
                }
            }
            else
            {
                // When the left button is released, stop dragging.
                _isDragging = false;
            }

            _previousMouseState = currentMouseState;
        }
    }
}
