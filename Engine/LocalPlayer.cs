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
            
        }

        public override void Initialize()
        {
            base.Initialize();

            this.Model3D = Renderer.Instance.LoadModel("soldier-low-poly");
            this.Height = 6.0f;

            InitializePhysX();

            /// TODO: Change LocalPlayer so that it can be given starting values.
            /// It's probably best to put a specialized factory in this file and give it things
            /// like position and orientation values.  It could then be part of an umbrella
            /// factory that allows the game to create objects of various types.

            this.Position = new Vector3(-3.0f, 10.0f, 0.0f);
            this.Orientation = Quaternion.Identity;
            this.HeadOrient = Quaternion.Identity;
            this.CurrentCollision = 0;
            this.Pitch = 0.0f;

            CenterCursor();
        }

        // TODO: There has to be a better way to do this.
        public void InitializePhysX()
        {
            ControllerDescription desc = new CapsuleControllerDescription(1, this.Height - 2.0f)
            {
                UpDirection = Axis.Y,
                Position = Vector3.UnitY * (this.Height - 1.0f) / 2.0f
            };
            this.PositionOffset = -1.0f * desc.Position;
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
            this.Orientation *= Quaternion.CreateFromYawPitchRoll(-delta.X, 0, 0);

            // Now we deal with the camera - let's change the pitch based on vertical mouse movement.
            float oldPitch = this.Pitch;
            this.Pitch = MathHelper.Clamp(this.Pitch - delta.Y, -MathHelper.PiOver4, MathHelper.PiOver4);

            // TODO: Fix pitch clamping - it doesn't work all the time.  Not sure why.  Rotation screws with stuff.
            // This could be done by looking at the y component of Vector3.Transform(Vector3.Forward, this.HeadOrient), and
            // making sure that it doesn't get too large or small.
            // TODO: We might want to change pi/4 to something else, depends on play-testing.

            // Modify delta.Y if we ended up clamping the pitch.
            if (this.Pitch == -MathHelper.PiOver4)
                delta.Y = -MathHelper.PiOver4 - oldPitch;
            if (this.Pitch == MathHelper.PiOver4)
                delta.Y = MathHelper.PiOver4 - oldPitch;
            
            // Set the orientation of the player's view (head).
            this.HeadOrient *= Quaternion.CreateFromYawPitchRoll(-delta.X, -delta.Y, 0);
            
            // Set the base movement speed.
            const float baseSpeed = 4.0f;

            // Calculate the speed at which we travel based on speed and elapsed time.
            float speed = baseSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;  // dx = v*t

            // Yay, we can run now!
            if (InCollisionState(ControllerCollisionFlag.Down))
            {
                if (states.IsKeyDown(Keys.LeftShift))
                    speed *= 1.5f;
            }
            //else
                //speed *= 0.3f;

            // And use that to determine which directions to move in.
            Vector3 motion = Vector3.Zero;

            // TODO: We probably want to make it so you can make some adjustments to your motion when in mid-air.
            // For example, instead of only doing this when we are on the ground, we could do it whenever but lower
            // the speed if we are NOT on the ground.

            // We only want to take key-presses into account when the player is on the ground.
            //if (InCollisionState(ControllerCollisionFlag.Down))
            //{
                if (states.IsKeyDown(Keys.W)) // Forwards?
                    motion += Vector3.Forward;

                if (states.IsKeyDown(Keys.S)) // Backwards?
                    motion += Vector3.Backward;

                if (states.IsKeyDown(Keys.A)) // Left?
                    motion += Vector3.Left;

                if (states.IsKeyDown(Keys.D)) // Right?
                    motion += Vector3.Right;
            //}

            // Normalize the motion vector (so we don't move at twice the speed when moving diagonally).
            if(motion != Vector3.Zero)
                motion.Normalize();
            motion *= speed;

            // Add the calculated motion to the current velocity.
            this.Velocity += motion;

            // If the player presses space (and is on the ground), jump!
            if (InCollisionState(ControllerCollisionFlag.Down) && states.IsKeyDown(Keys.Space))
                this.Velocity += Vector3.Up / 4.0f;

            // Move the player's controller based on its velocity.
            ControllerCollisionFlag prevCollState = this.CurrentCollision;
            this.CurrentCollision = (this.Controller.Move(Vector3.Transform(this.Velocity, this.Orientation))).CollisionFlag;
            
            // Now, we need to reset parts of the velocity so that we're not compounding it.
            if(InCollisionState(ControllerCollisionFlag.Down))
                this.Velocity = Vector3.Zero;
            else
                this.Velocity += Engine.Instance.Scene.Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds - motion;
        }

        private bool InCollisionState(ControllerCollisionFlag flag)
        {
            return ((byte) this.CurrentCollision & (byte) flag) == (byte) flag;
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

            if(Engine.Instance.Camera.Type != Camera.CameraType.FIRST_PERSON)
                Renderer.Instance.DrawObject(this);
        }

        #region Properties

        public ControllerCollisionFlag CurrentCollision
        {
            get;
            private set;
        }

        private float Pitch
        {
            get;
            set;
        }

        #endregion
    }
}
