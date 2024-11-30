namespace JRPG.Assets.Sprites.Characters;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class CharacterSprite
{
    private readonly Texture2D _texture;
    private Rectangle[] _frames;
    private int _currentFrame = 0; // 0 = down, 1 = left, 2 = right, 3 = up
    public Vector2 Position { get; set; }
    
    public CharacterSprite(Texture2D texture, Vector2 initialPosition)
    {
        _texture = texture;
        Position = initialPosition;
        
        _frames = new Rectangle[4]; // Just the 4 directional frames we're using
        
        // Down sprite
        _frames[0] = new Rectangle(172, 241, 75, 107);
        
        // Left sprite
        _frames[1] = new Rectangle(42, 125, 56, 107);
        
        // Right sprite
        _frames[2] = new Rectangle(349, 124, 56, 108);
        
        // Up sprite
        _frames[3] = new Rectangle(189, 26, 72, 106);
    }
    
    public void Update(Vector2 movement)
    {
        // Change the sprite based on movement direction
        if (movement.Y > 0) _currentFrame = 0;      // Moving down
        else if (movement.X < 0) _currentFrame = 1; // Moving left
        else if (movement.X > 0) _currentFrame = 2; // Moving right
        else if (movement.Y < 0) _currentFrame = 3; // Moving up
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(
            _texture,
            Position,
            _frames[_currentFrame],
            Color.White,
            0f,
            new Vector2(_frames[_currentFrame].Width / 2, _frames[_currentFrame].Height / 2),
            Vector2.One,
            SpriteEffects.None,
            0f
        );
    }
}