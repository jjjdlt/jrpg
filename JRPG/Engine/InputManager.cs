using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace JRPG.Engine
{
    public class InputManager
    {
        // Add a speed multiplier that can be adjusted as needed
        private const float BASE_SPEED_MULTIPLIER = 2.5f;
        
        // Add sprint functionality
        private const float SPRINT_MULTIPLIER = 1.5f;

        public Vector2 GetMovementDirection(float deltaSpeed)
        {
            Vector2 movement = Vector2.Zero;
            
            // Apply our base speed multiplier to make movement naturally faster
            float adjustedSpeed = deltaSpeed * BASE_SPEED_MULTIPLIER;
            
            // Check for sprint key (Left Shift)
            var kstate = Keyboard.GetState();
            if (kstate.IsKeyDown(Keys.LeftShift))
            {
                adjustedSpeed *= SPRINT_MULTIPLIER;
            }
            
            // Keyboard handling with adjusted speed
            if (kstate.IsKeyDown(Keys.W))
                movement.Y -= adjustedSpeed;
            if (kstate.IsKeyDown(Keys.S))
                movement.Y += adjustedSpeed;
            if (kstate.IsKeyDown(Keys.A))
                movement.X -= adjustedSpeed;
            if (kstate.IsKeyDown(Keys.D))
                movement.X += adjustedSpeed;

            // Joystick handling with adjusted speed
            if (Joystick.LastConnectedIndex >= 0)
            {
                JoystickState jstate = Joystick.GetState(0);
                
                if (jstate.Axes.Length >= 2)
                {
                    float deadzone = 0.2f;
                    
                    // Apply adjusted speed to joystick movement
                    if (Math.Abs(jstate.Axes[1]) > deadzone)
                    {
                        movement.Y += jstate.Axes[1] * adjustedSpeed;
                    }
                    
                    if (Math.Abs(jstate.Axes[0]) > deadzone)
                    {
                        movement.X += jstate.Axes[0] * adjustedSpeed;
                    }
                }
            }

            // Normalize movement if moving diagonally
            if (movement != Vector2.Zero)
            {
                movement.Normalize();
                movement *= adjustedSpeed;
            }

            return movement;
        }

        public bool IsExitRequested()
        {
            return GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                   Keyboard.GetState().IsKeyDown(Keys.Escape);
        }
    }
}