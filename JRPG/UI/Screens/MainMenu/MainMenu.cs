using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JRPG.UI.Screens.MainMenu
{
    public class MainMenu
    {
        private SpriteBatch _spriteBatch;
        private Texture2D _menuTexture;
        private Rectangle _menuRect;
        private bool _isVisible = true;
        private bool _isActive = true;  // New flag to control menu state

        public MainMenu(Game game)
        {
            _spriteBatch = new SpriteBatch(game.GraphicsDevice);
            LoadContent(game);
            InitializeMenu(game.GraphicsDevice.Viewport);
        }

        private void LoadContent(Game game)
        {
            _menuTexture = game.Content.Load<Texture2D>("mainmenu");
        }

        private void InitializeMenu(Viewport viewport)
        {
            _menuRect = new Rectangle(0, 0, viewport.Width, viewport.Height);
        }

        public void Update(GameTime gameTime)
        {
            if (!_isVisible || !_isActive) return;

            MouseState mouseState = Mouse.GetState();
            Point mousePoint = mouseState.Position;

            // Using the exact coordinates you provided for the single player button
            Rectangle singlePlayerButton = new Rectangle(819, 564, 248, 50);

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (singlePlayerButton.Contains(mousePoint))
                {
                    HandleMenuSelection("SinglePlayer");
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (!_isVisible) return;

            _spriteBatch.Begin();
            _spriteBatch.Draw(_menuTexture, _menuRect, Color.White);
            _spriteBatch.End();
        }

        private void HandleMenuSelection(string selection)
        {
            if (selection == "SinglePlayer")
            {
                // Hide and deactivate the menu when entering the game
                _isVisible = false;
                _isActive = false;
            }
        }

        public void HandleResize(Viewport viewport)
        {
            _menuRect = new Rectangle(0, 0, viewport.Width, viewport.Height);
        }

        // Method to show/hide menu
        public void ToggleVisibility()
        {
            _isVisible = !_isVisible;
            _isActive = _isVisible;  // Only allow interaction when visible
        }
    }
}