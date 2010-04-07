using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Mammoth.Engine
{
    public class LocalPlayer : Player
    {
        public LocalPlayer(Game game) : base(game)
        {
            /// TODO: Change LocalPlayer so that it can be given starting values.
            /// It's probably best to put a specialized factory in this file and give it things
            /// like position and orientation values.  It could then be part of an umbrella
            /// factory that allows the game to create objects of various types.

            this.Position = new Vector3(0.0f, 0.0f, 10.0f);
            this.Orientation = Quaternion.Identity;
            this.HeadOrient = Quaternion.Identity;
            this.Height = 6.0f;
        }

        public override void Initialize()
        {
            base.Initialize();

            this.Model3D = Renderer.Instance.LoadModel("soldier-low-poly");

            InitializePhysX();

            CenterCursor();
        }

        // TODO: There has to be a better way to do this.
        public void InitializePhysX()
        {
            ControllerDescription desc = new CapsuleControllerDescription(1, this.Height - 2.0f)
            {
                UpDirection = Axis.Y,
                Position = this.Position + Vector3.UnitY * (this.Height - 1.0f) / 2.0f
            };
            this.Controller = Player.ControllerManager.CreateController(desc);
            this.Controller.SetCollisionEnabled(true);
            this.Controller.Name = "Local Player Controller";
            this.Controller.Actor.Name = "Local Player Actor";
        }

        // TODO: Add a PhysX character controller to the LocalPlayer update code.
        public override void Update(GameTime gameTime)
        {
            // Get an instance of the game window to calculate new values.
            GameWindow window = this.Game.Window;

            // TODO: At some point, we need to set up the use of a settings/options file, and read controls from there.
            // We get the current state of the keyboard...
            KeyboardState states = Keyboard.GetState();

            // Get the mouse's offset from the previous position (window center).
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Vector2 mouseCenter = new Vector2(window.ClientBounds.Width / 2, window.ClientBounds.Height / 2);
            Vector2 delta = (mousePosition - mouseCenter) * 0.0005f;
            CenterCursor();

            // Let's add this new rotation onto the end of the current rotation.  We only add the yaw as we don't want
            // the player model to rotate up and down.
            this.Orientation = Quaternion.Concatenate(this.Orientation,
                                                       Quaternion.CreateFromYawPitchRoll(-delta.X, 0, 0));

            // Now we deal with the camera.
            float pitch = (float)Math.Asin(this.HeadOrient.W * this.HeadOrient.Y - this.HeadOrient.X * this.HeadOrient.Z);
            float newPitch = pitch - delta.Y;
            // TODO: Edge cases for mouse-look.
            // We need to make sure that the player can't look down enough that they look backwards.
            if (newPitch < -1.5f)
                delta.Y = -1.5f - pitch;
            if (newPitch > 1.5f)
                delta.Y = 1.5f - pitch;
            this.HeadOrient = Quaternion.Concatenate(this.HeadOrient,
                                                        Quaternion.CreateFromYawPitchRoll(-delta.X, -delta.Y, 0));

            float speed = 4.0f; // Movement is 20 units per second.

            // Yay, we can run now!
            if (states.IsKeyDown(Keys.LeftShift))
                speed *= 1.5f;

            // Calculate the distance we travel based on speed and elapsed time.
            float distance = speed * (float)gameTime.ElapsedGameTime.TotalSeconds;  // dx = v*t

            // And use that to determine which directions to move in.
            if (states.IsKeyDown(Keys.W)) // Forwards?
                this.Velocity += Vector3.Forward * distance;

            if (states.IsKeyDown(Keys.S)) // Backwards?
                this.Velocity += Vector3.Backward * distance;

            if (states.IsKeyDown(Keys.A)) // Left?
                this.Velocity += Vector3.Left * distance;

            if (states.IsKeyDown(Keys.D)) // Right?
                this.Velocity += Vector3.Right * distance;

            // Now we modify the position by the calculated amount.
            Vector3 newPosition = this.Position + Vector3.Transform(this.Velocity, this.Orientation);

            // Set the new position of the player.
            this.Position = newPosition;

            // Move the player's controller based on its velocity.
            ControllerMoveResult result = this.Controller.Move(Vector3.Transform(this.Velocity, this.Orientation));
            //if (result.CollisionFlag == ControllerCollisionFlag.Down)
                this.Velocity = Vector3.Zero;
            //else
            //    this.Velocity += Engine.Instance.Scene.Gravity * (float) gameTime.ElapsedGameTime.TotalSeconds;
        }

        /**
         * Warp the cursor to the center of the window.
         */
        public void CenterCursor()
        {
            GameWindow window = this.Game.Window;
            
            Mouse.SetPosition(window.ClientBounds.Width / 2, window.ClientBounds.Height / 2);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Renderer.Instance.DrawObject(this);
        }
    }
}
