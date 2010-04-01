using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Mammoth.Core;

namespace Mammoth.Engine
{
    class LocalPlayerController : Controller<LocalPlayer>
    {
        public LocalPlayerController(LocalPlayer player) : base()
        {
            this.Model = player;
        }

        public override void Update(GameTime gameTime)
        {
            // Get an instance of the game window to calculate new values.
            GameWindow window = Engine.Instance.Game.Window;

            // Get the mouse's offset from the previous position (window center).
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Vector2 mouseCenter = new Vector2(window.ClientBounds.Width / 2, window.ClientBounds.Height / 2);
            Vector2 delta = (mousePosition - mouseCenter) * 0.0005f;
            CenterCursor();

            // Let's add this new rotation onto the end of the current rotation.
            Model.Orientation = Quaternion.Concatenate(Model.Orientation,
                                                       Quaternion.CreateFromYawPitchRoll(-delta.X, -delta.Y, 0));

            // Now we can get the new look vector.
            Vector3 newForward = Vector3.Transform(Vector3.Forward, Model.Orientation);
            Vector3 newRight = Vector3.Cross(newForward, Vector3.Up);

            const float speed = 20.0f; // Movement is 20 units per second.
            float distance = speed * (float) gameTime.ElapsedGameTime.TotalSeconds;  // dx = v*t

            // The amount to shift the position by starts at 0.
            Vector3 translateDirection = Vector3.Zero;

            // TODO: At some point, we need to set up the use of a settings/options file, and read controls from there.
            // We get the current state of the keyboard...
            KeyboardState states = Keyboard.GetState();

            // And use that to determine which directions to move in.
            if (states.IsKeyDown(Keys.W)) // Forwards?
                translateDirection += newForward;

            if (states.IsKeyDown(Keys.S)) // Backwards?
                translateDirection -= newForward;

            if (states.IsKeyDown(Keys.A)) // Left?
                translateDirection -= newRight;

            if (states.IsKeyDown(Keys.D)) // Right?
                translateDirection += newRight;

            // Now we modify the position by the calculated amount.
            Vector3 newPosition = Model.Position;
            if (translateDirection.LengthSquared() > 0) // Kinda pointless, as it won't ever be less than 0...
                newPosition += Vector3.Normalize(translateDirection) * distance;

            // Set the new position of the player.
            Model.Position = newPosition;
        }

        /**
         * Warp the cursor to the center of the window.
         */
        public void CenterCursor()
        {
            GameWindow window = Engine.Instance.Game.Window;

            Mouse.SetPosition(window.ClientBounds.Width / 2, window.ClientBounds.Height / 2);
        }
    }
}
