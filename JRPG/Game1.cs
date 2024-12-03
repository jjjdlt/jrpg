using System;
using JRPG.Assets.Sprites.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using JRPG.Engine;
using JRPG.UI.Screens.MainMenu;
using JRPG.World.Maps;

namespace JRPG
{
    public class Game1 : Game
    {
        // Core graphics and management systems
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private InputManager _inputManager;
        private GameState _gameState;

        // Game objects and map system
        private CharacterSprite _character;
        private MapManager _mapManager;
        private float _characterSpeed = 100f;
        private MainMenu _mainMenu;

        // Camera and view control
        private Vector2 _cameraOffset;
        private Vector2 _screenCenter;

        // Fullscreen handling
        private bool _isFullscreen = false;
        private bool _isResizing = false;
        private bool _canToggleFullscreen = true;
        private TimeSpan _fullscreenCooldown = TimeSpan.FromMilliseconds(500);
        private TimeSpan _lastFullscreenToggle = TimeSpan.Zero;
        private Vector2 _lastWindowedSize;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _inputManager = new InputManager();
            _gameState = new GameState();

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _lastWindowedSize = new Vector2(1280, 720);

            // Calculate screen center for camera positioning
            _screenCenter = new Vector2(_graphics.PreferredBackBufferWidth / 2f,
                _graphics.PreferredBackBufferHeight / 2f);

            Window.Position = new Point(
                (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - _graphics.PreferredBackBufferWidth) / 2,
                (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - _graphics.PreferredBackBufferHeight) / 2
            );

            _graphics.HardwareModeSwitch = false;
            _graphics.ApplyChanges();

            _gameState.OnStateChanged += HandleGameStateChanged;
        }

        protected override void Initialize()
        {
            Window.Title = "JRPG";
            Window.AllowUserResizing = true;

            _canToggleFullscreen = true;
            _lastFullscreenToggle = TimeSpan.Zero;

            GraphicsDevice.Viewport = new Viewport(0, 0, _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight);

            Window.ClientSizeChanged += Window_ClientSizeChanged;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _mapManager = new MapManager(GraphicsDevice, Content);

            Texture2D characterTexture = Content.Load<Texture2D>("test_spritesheet");

            // Initialize character at the center of the visible map
            Vector2 initialPosition = new Vector2(
                32 * 2, // Center horizontally (32 is TILE_DISPLAY_SIZE)
                32 * 2 // Center vertically
            );

            _character = new CharacterSprite(characterTexture, initialPosition);
            _mainMenu = new MainMenu(this, _gameState);

            UpdateCameraOffset();
        }

        private void UpdateCameraOffset()
        {
            _cameraOffset = _character.Position - _screenCenter;
        }

        protected override void Update(GameTime gameTime)
        {
            if (_inputManager.IsExitRequested())
                Exit();

            KeyboardState currentKeyboardState = Keyboard.GetState();
            if (_canToggleFullscreen && currentKeyboardState.IsKeyDown(Keys.F11))
            {
                ToggleFullscreen();
                _canToggleFullscreen = false;
                _lastFullscreenToggle = gameTime.TotalGameTime;
            }
            else if (!_canToggleFullscreen && gameTime.TotalGameTime - _lastFullscreenToggle > _fullscreenCooldown)
            {
                _canToggleFullscreen = true;
            }

            switch (_gameState.CurrentState)
            {
                case GameStateType.MainMenu:
                    _mainMenu.Update(gameTime);
                    break;

                case GameStateType.InGame:
                    float deltaSpeed = _characterSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Vector2 movement = _inputManager.GetMovementDirection(deltaSpeed);

                    Vector2 proposedPosition = _character.Position + movement;
                    Rectangle characterBounds = new Rectangle(
                        (int)proposedPosition.X - 16,
                        (int)proposedPosition.Y - 16,
                        32,
                        32
                    );

                    Rectangle mapBounds = _mapManager.GetMapBounds();
                    bool isWithinBounds = mapBounds.Contains(characterBounds);
                    bool hasCollision = _mapManager.CheckCollision(characterBounds);

                    if (isWithinBounds && !hasCollision)
                    {
                        _character.Position = proposedPosition;
                        UpdateCameraOffset();
                    }

                    _character.Update(movement, gameTime);
                    _mapManager.Update(gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            switch (_gameState.CurrentState)
            {
                case GameStateType.MainMenu:
                    _mainMenu.Draw(gameTime);
                    break;

                case GameStateType.InGame:
                    _spriteBatch.Begin(
                        SpriteSortMode.Deferred,
                        BlendState.AlphaBlend,
                        SamplerState.PointClamp,
                        null,
                        null,
                        null,
                        Matrix.CreateTranslation(new Vector3(-_cameraOffset, 0f))
                    );

                    _mapManager.DrawCurrentMap(_spriteBatch);
                    _mapManager.DrawDebugGrid(_spriteBatch);
                    _character.Draw(_spriteBatch);

                    _spriteBatch.End();
                    break;
            }

            base.Draw(gameTime);
        }

        private void HandleGameStateChanged(GameStateType newState)
        {
            switch (newState)
            {
                case GameStateType.InGame:
                    IsMouseVisible = true;
                    break;
                case GameStateType.MainMenu:
                    IsMouseVisible = true;
                    break;
            }
        }

        private void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            if (_isResizing)
                return;

            _isResizing = true;
            try
            {
                if (!_isFullscreen)
                {
                    _lastWindowedSize = new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height);
                }

                _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                _graphics.ApplyChanges();

                _screenCenter = new Vector2(_graphics.PreferredBackBufferWidth / 2f,
                    _graphics.PreferredBackBufferHeight / 2f);

                if (_character != null)
                {
                    UpdateCameraOffset();
                }

                _mainMenu?.HandleResize(GraphicsDevice.Viewport);
            }
            finally
            {
                _isResizing = false;
            }
        }

        public void ToggleFullscreen()
        {
            // Prevent recursive resizing operations
            if (_isResizing)
                return;

            _isResizing = true;
            try
            {
                // Toggle fullscreen state
                _isFullscreen = !_isFullscreen;

                if (_isFullscreen)
                {
                    // Get the current display's resolution for fullscreen mode
                    DisplayMode currentDisplay = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
                    _graphics.PreferredBackBufferWidth = currentDisplay.Width;
                    _graphics.PreferredBackBufferHeight = currentDisplay.Height;
                }
                else
                {
                    // Return to previous windowed size
                    _graphics.PreferredBackBufferWidth = (int)_lastWindowedSize.X;
                    _graphics.PreferredBackBufferHeight = (int)_lastWindowedSize.Y;
                }

                // Apply the fullscreen setting
                _graphics.IsFullScreen = _isFullscreen;
                _graphics.ApplyChanges();

                if (!_isFullscreen)
                {
                    // Center the window on screen when returning to windowed mode
                    Window.Position = new Point(
                        (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - _graphics.PreferredBackBufferWidth) /
                        2,
                        (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height -
                         _graphics.PreferredBackBufferHeight) / 2
                    );
                }

                // Update the screen center point for camera calculations
                _screenCenter = new Vector2(
                    _graphics.PreferredBackBufferWidth / 2f,
                    _graphics.PreferredBackBufferHeight / 2f
                );

                // Update camera position if character exists
                if (_character != null)
                {
                    UpdateCameraOffset();
                }

                // Ensure mouse remains visible
                IsMouseVisible = true;
            }
            finally
            {
                // Always make sure to reset the resizing flag
                _isResizing = false;
            }
        }
    }
}