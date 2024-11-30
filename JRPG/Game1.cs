using System.IO;
using JRPG.Assets.Sprites.Characters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JRPG;

// Game1.cs
public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private CharacterSprite _character;
    private float _characterSpeed = 100f;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Texture2D characterTexture = Content.Load<Texture2D>("dummy_spritesheet");
        Vector2 initialPosition = new Vector2(
            _graphics.PreferredBackBufferWidth / 2,
            _graphics.PreferredBackBufferHeight / 2
        );
        _character = new CharacterSprite(characterTexture, initialPosition);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        float deltaSpeed = _characterSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Vector2 movement = Vector2.Zero;

        // Keyboard controls
        var kstate = Keyboard.GetState();
        
        if (kstate.IsKeyDown(Keys.Up))
        {
            movement.Y -= deltaSpeed;
            _character.Position = new Vector2(_character.Position.X, _character.Position.Y - deltaSpeed);
        }
        if (kstate.IsKeyDown(Keys.Down))
        {
            movement.Y += deltaSpeed;
            _character.Position = new Vector2(_character.Position.X, _character.Position.Y + deltaSpeed);
        }
        if (kstate.IsKeyDown(Keys.Left))
        {
            movement.X -= deltaSpeed;
            _character.Position = new Vector2(_character.Position.X - deltaSpeed, _character.Position.Y);
        }
        if (kstate.IsKeyDown(Keys.Right))
        {
            movement.X += deltaSpeed;
            _character.Position = new Vector2(_character.Position.X + deltaSpeed, _character.Position.Y);
        }

        // Joystick controls
        if(Joystick.LastConnectedIndex == 0)
        {
            JoystickState jstate = Joystick.GetState((int)PlayerIndex.One);
            if (jstate.Axes[1] < 0)
            {
                movement.Y -= deltaSpeed;
                _character.Position = new Vector2(_character.Position.X, _character.Position.Y - deltaSpeed);
            }
            else if (jstate.Axes[1] > 0)
            {
                movement.Y += deltaSpeed;
                _character.Position = new Vector2(_character.Position.X, _character.Position.Y + deltaSpeed);
            }
            if (jstate.Axes[0] < 0)
            {
                movement.X -= deltaSpeed;
                _character.Position = new Vector2(_character.Position.X - deltaSpeed, _character.Position.Y);
            }
            else if (jstate.Axes[0] > 0)
            {
                movement.X += deltaSpeed;
                _character.Position = new Vector2(_character.Position.X + deltaSpeed, _character.Position.Y);
            }
        }

        _character.Update(movement);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        _character.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}