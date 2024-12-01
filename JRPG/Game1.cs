using System;
using JRPG.Assets.Sprites.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using JRPG.Engine;
using JRPG.UI.Screens.MainMenu;

namespace JRPG;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private CharacterSprite _character;
    private float _characterSpeed = 100f;
    private InputManager _inputManager;
    private bool _isFullscreen = false;
    private bool _canToggleFullscreen = true;
    private TimeSpan _fullscreenCooldown = TimeSpan.FromMilliseconds(500);
    private TimeSpan _lastFullscreenToggle = TimeSpan.Zero;
    private Texture2D _backgroundTexture;
    private Rectangle _backgroundRect;
    private MainMenu _mainMenu;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _inputManager = new InputManager();

        // Set initial window size
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        
        // Center the window on the screen
        Window.Position = new Point(
            (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - _graphics.PreferredBackBufferWidth) / 2,
            (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - _graphics.PreferredBackBufferHeight) / 2
        );

        _graphics.HardwareModeSwitch = false;
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        Window.Title = "JRPG";
        Window.AllowUserResizing = true;
        
        // Set up viewport
        float aspectRatio = (float)_graphics.PreferredBackBufferWidth / _graphics.PreferredBackBufferHeight;
        GraphicsDevice.Viewport = new Viewport(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

        // Initialize background rectangle to cover the entire screen
        _backgroundRect = new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

        Window.ClientSizeChanged += Window_ClientSizeChanged;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _backgroundTexture = Content.Load<Texture2D>("background");
        Texture2D characterTexture = Content.Load<Texture2D>("dummy_spritesheet");
        
        Vector2 initialPosition = new Vector2(
            _graphics.PreferredBackBufferWidth / 2,
            _graphics.PreferredBackBufferHeight / 2
        );
        _character = new CharacterSprite(characterTexture, initialPosition);
        
        // Initialize main menu
        _mainMenu = new MainMenu(this);
    }

    protected override void Update(GameTime gameTime)
    {
        if (_inputManager.IsExitRequested())
            Exit();

        // Handle fullscreen toggle with cooldown
        if (_canToggleFullscreen && Keyboard.GetState().IsKeyDown(Keys.F11))
        {
            ToggleFullscreen();
            _canToggleFullscreen = false;
            _lastFullscreenToggle = gameTime.TotalGameTime;
        }
        else if (!_canToggleFullscreen && gameTime.TotalGameTime - _lastFullscreenToggle > _fullscreenCooldown)
        {
            _canToggleFullscreen = true;
        }

        float deltaSpeed = _characterSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Vector2 movement = _inputManager.GetMovementDirection(deltaSpeed);
        
        // Update character position based on movement
        _character.Position += movement;
        _character.Update(movement);

        // Update main menu
        _mainMenu.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();
        // Draw the background first
        _spriteBatch.Draw(_backgroundTexture, _backgroundRect, Color.White);
        // Then draw the character
        _character.Draw(_spriteBatch);
        _spriteBatch.End();

        // Draw the menu last so it appears on top
        _mainMenu.Draw(gameTime);

        base.Draw(gameTime);
    }

    private void Window_ClientSizeChanged(object sender, EventArgs e)
    {
        _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
        _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
        _graphics.ApplyChanges();

        // Update background rectangle to match new window size
        _backgroundRect = new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height);

        // Update character position if needed
        if (_character != null)
        {
            _character.Position = new Vector2(
                _graphics.PreferredBackBufferWidth / 2,
                _graphics.PreferredBackBufferHeight / 2
            );
        }

        // Update menu if needed
        _mainMenu?.HandleResize(GraphicsDevice.Viewport);
    }

    public void ToggleFullscreen()
    {
        _isFullscreen = !_isFullscreen;
        
        if (_isFullscreen)
        {
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }
        else
        {
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            
            Window.Position = new Point(
                (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - _graphics.PreferredBackBufferWidth) / 2,
                (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - _graphics.PreferredBackBufferHeight) / 2
            );
        }

        _graphics.IsFullScreen = _isFullscreen;
        _graphics.ApplyChanges();

        // Update background rectangle after resolution change
        _backgroundRect = new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

        IsMouseVisible = true;

        // Update character position if needed
        if (_character != null)
        {
            _character.Position = new Vector2(
                _graphics.PreferredBackBufferWidth / 2,
                _graphics.PreferredBackBufferHeight / 2
            );
        }
    }
}