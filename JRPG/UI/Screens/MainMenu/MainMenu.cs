using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using JRPG.Engine;

namespace JRPG.UI.Screens.MainMenu
{
    public class MainMenu
    {
        private readonly SpriteBatch _spriteBatch;
        private Texture2D _menuTexture;  // Removed readonly since we need to load it after construction
        private Rectangle _menuRect;
        private readonly GameState _gameState;

        public MainMenu(Game game, GameState gameState)
        {
            _spriteBatch = new SpriteBatch(game.GraphicsDevice);
            _gameState = gameState;
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
            // Only update if we're in the menu state
            if (_gameState.CurrentState != GameStateType.MainMenu)
                return;

            MouseState mouseState = Mouse.GetState();
            Point mousePoint = mouseState.Position;

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
            // Only draw if we're in the menu state
            if (_gameState.CurrentState != GameStateType.MainMenu)
                return;

            _spriteBatch.Begin();
            _spriteBatch.Draw(_menuTexture, _menuRect, Color.White);
            _spriteBatch.End();
        }

        private void HandleMenuSelection(string selection)
        {
            if (selection == "SinglePlayer")
            {
                _gameState.ChangeState(GameStateType.InGame);
            }
        }

        public void HandleResize(Viewport viewport)
        {
            _menuRect = new Rectangle(0, 0, viewport.Width, viewport.Height);
        }
    }
}