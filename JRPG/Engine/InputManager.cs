// Engine/InputManager.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace JRPG.Engine
{
    public class InputManager
    {
        public Vector2 GetMovementDirection(float deltaSpeed)
        {
            Vector2 movement = Vector2.Zero;
            
            // Keyboard handling
            var kstate = Keyboard.GetState();
            
            if (kstate.IsKeyDown(Keys.Up))
                movement.Y -= deltaSpeed;
            if (kstate.IsKeyDown(Keys.Down))
                movement.Y += deltaSpeed;
            if (kstate.IsKeyDown(Keys.Left))
                movement.X -= deltaSpeed;
            if (kstate.IsKeyDown(Keys.Right))
                movement.X += deltaSpeed;

            // Joystick handling
            if(Joystick.LastConnectedIndex == 0)
            {
                JoystickState jstate = Joystick.GetState((int)PlayerIndex.One);
                if (jstate.Axes[1] < 0)
                    movement.Y -= deltaSpeed;
                else if (jstate.Axes[1] > 0)
                    movement.Y += deltaSpeed;
                if (jstate.Axes[0] < 0)
                    movement.X -= deltaSpeed;
                else if (jstate.Axes[0] > 0)
                    movement.X += deltaSpeed;
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