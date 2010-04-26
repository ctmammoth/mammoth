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
        public LocalPlayer(Engine game) : base(game)
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();

            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));

            this.Model3D = r.LoadModel("soldier-low-poly");
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
            this.Yaw = 0.0f;
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

        public override void Update(GameTime gameTime)
        {
            // Get an instance of the game window to calculate new values.
            GameWindow window = this.Game.Window;

            // Get the mouse's offset from the previous position (window center).
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Vector2 mouseCenter = new Vector2(window.ClientBounds.Width / 2, window.ClientBounds.Height / 2);
            Vector2 delta = (mousePosition - mouseCenter) * 0.0005f;
            CenterCursor();
            
            // Now we deal with the camera.
            // Modify the yaw based on horizontal mouse movement.
            this.Yaw = MathHelper.WrapAngle(this.Yaw - delta.X);
            // Modify the pitch based on vertical mouse movement.
            const float pitchClamp = 0.9876f;

            // TODO: There's probably a better (faster) way of doing this...
            this.Pitch = (float) Math.Asin(MathHelper.Clamp((float) Math.Sin(this.Pitch - delta.Y), -pitchClamp, pitchClamp));

            // Set the orientation of the player's body.  We only use the yaw, as we don't want the player model
            // rotating up and down.
            this.Orientation = Quaternion.CreateFromYawPitchRoll(this.Yaw, 0, 0);
            // Set the orientation of the player's view (head).
            this.HeadOrient = Quaternion.CreateFromYawPitchRoll(this.Yaw, this.Pitch, 0);
            
            // Set the base movement speed.
            const float baseSpeed = 6.0f;

            // Calculate the speed at which we travel based on speed and elapsed time.
            float speed = baseSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;  // dx = v*t

            // Yay, we can run now!
            if (InCollisionState(ControllerCollisionFlag.Down))
            {
                if (Input.IsKeyDown(Input.EvKeys.KEY_SPRINT))
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
                if (Input.IsKeyDown(Input.EvKeys.KEY_FORWARD)) // Forwards?
                    motion += Vector3.Forward;

                if (Input.IsKeyDown(Input.EvKeys.KEY_BACKWARD)) // Backwards?
                    motion += Vector3.Backward;

                if (Input.IsKeyDown(Input.EvKeys.KEY_LEFT)) // Left?
                    motion += Vector3.Left;

                if (Input.IsKeyDown(Input.EvKeys.KEY_RIGHT)) // Right?
                    motion += Vector3.Right;
            //}

            // Normalize the motion vector (so we don't move at twice the speed when moving diagonally).
            if(motion != Vector3.Zero)
                motion.Normalize();
            motion *= speed;

            // Add the calculated motion to the current velocity.
            this.Velocity += motion;

            // If the player presses space (and is on the ground), jump!
            if (InCollisionState(ControllerCollisionFlag.Down))
                if(Input.IsKeyDown(Input.EvKeys.KEY_JUMP))
                this.Velocity += Vector3.Up / 4.0f;

            // Move the player's controller based on its velocity.
            this.CurrentCollision = (this.Controller.Move(Vector3.Transform(this.Velocity, this.Orientation))).CollisionFlag;
            
            // Now, we need to reset parts of the velocity so that we're not compounding it.
            if(InCollisionState(ControllerCollisionFlag.Down))
                this.Velocity = Vector3.Zero;
            else
                // TODO: Change this so that it gets the scene from Adam's physics helper code.
                this.Velocity += ((Engine) this.Game).Scene.Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds - motion;
        }

        /// <summary>
        /// This helper function is used to determine whether or not the player is colliding with objects in a 
        /// certain fashion after Controller.Move() is called.  For example, if the player is standing on the ground
        /// after Move() is called, calling InCollisionState(ControllerCollisionFlag.Down) will return true.
        /// </summary>
        /// <param name="flag">The collision state that we're interested in.</param>
        /// <returns>Whether or not the player is colliding on that "side".</returns>
        private bool InCollisionState(ControllerCollisionFlag flag)
        {
            return (this.CurrentCollision & flag) == flag;
        }

        /// <summary>
        /// This warps the cursor to the center of the game window.  This is important, as without it, the player's
        /// mouse would hit the side of the screen and they wouldn't be able to turn any further.  Also, using this
        /// method, the distance moved by the mouse in each update loop is just the distance from the center of the
        /// window to the mouse location.
        /// </summary>
        public void CenterCursor()
        {
            GameWindow window = this.Game.Window;

            if(ClientState.Instance.CurrentState == ClientState.State.InGame)
                Mouse.SetPosition(window.ClientBounds.Width / 2, window.ClientBounds.Height / 2);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));
            ICameraService cam = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));

            // If you're using the first-person camera, don't draw your own geometry.
            if (cam.Type != Camera.CameraType.FIRST_PERSON)
                r.DrawRenderable(this);
        }

        #region Properties

        public ControllerCollisionFlag CurrentCollision
        {
            get;
            private set;
        }

        private float Yaw
        {
            get;
            set;
        }

        private float Pitch
        {
            get;
            set;
        }

        #endregion
    }
}
