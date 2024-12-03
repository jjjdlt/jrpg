using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JRPG.Assets.Sprites.Characters
{
    public class CharacterSprite
    {
        private readonly Texture2D _texture;
        private Rectangle[][] _frames;
        private int _currentDirection = 0;    // 0 = down, 1 = left, 2 = right, 3 = up
        private int _currentAnimationFrame = 1;  // Start with neutral pose (middle frame)
        private float _animationTimer = 0f;
        private const float ANIMATION_FRAME_TIME = 0.15f;
        
        public Vector2 Position { get; set; }
        
        public CharacterSprite(Texture2D texture, Vector2 initialPosition)
        {
            _texture = texture;
            Position = initialPosition;
            
            _frames = new Rectangle[4][];
            for (int i = 0; i < 4; i++)
            {
                _frames[i] = new Rectangle[3];
            }
            
            // Down-facing frames with exact coordinates from your sprite sheet
            _frames[0][0] = new Rectangle(0, 10, 150, 190);      // Walking frame 1
            _frames[0][1] = new Rectangle(170, 0, 140, 190);     // Neutral pose
            _frames[0][2] = new Rectangle(330, 1, 188, 199);     // Walking frame 2
            
            // Left-facing frames
            _frames[1][0] = new Rectangle(160, 510, 140, 190);
            _frames[1][1] = new Rectangle(0, 520, 141, 181);
            _frames[1][2] = new Rectangle(320, 519, 141, 181);
            
            // Right-facing frames (uses left frames with horizontal flip)
            _frames[2] = _frames[1];
            
            // Up-facing frames
            _frames[3][0] = new Rectangle(170, 250, 140, 190);
            _frames[3][1] = new Rectangle(10, 260, 140, 190);
            _frames[3][2] = new Rectangle(330, 260, 140, 190);
        }
        
        public void Update(Vector2 movement, GameTime gameTime)
        {
            if (movement != Vector2.Zero)
            {
                // Determine direction based on movement
                if (movement.Y > 0)         // Moving down
                {
                    _currentDirection = 0;
                }
                else if (movement.Y < 0)    // Moving up
                {
                    _currentDirection = 3;
                }
                else if (movement.X != 0)   // Moving horizontally
                {
                    _currentDirection = movement.X > 0 ? 2 : 1;
                }
                
                // Update animation timing
                _animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_animationTimer >= ANIMATION_FRAME_TIME)
                {
                    _animationTimer = 0f;
                    _currentAnimationFrame = (_currentAnimationFrame + 1) % 3;
                }
            }
            else
            {
                // Return to neutral pose when not moving
                _currentAnimationFrame = 1;
                _animationTimer = 0f;
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle currentFrame = _frames[_currentDirection][_currentAnimationFrame];
            
            // Calculate origin point based on the current frame's dimensions
            Vector2 origin = new Vector2(currentFrame.Width / 2, currentFrame.Height / 2);
            
            spriteBatch.Draw(
                _texture,
                Position,
                currentFrame,
                Color.White,
                0f,
                origin,
                Vector2.One,
                (_currentDirection == 2) ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f
            );
        }
    }
}