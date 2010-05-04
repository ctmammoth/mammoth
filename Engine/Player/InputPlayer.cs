using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Mammoth.Engine.Input;
using Mammoth.Engine.Physics;

namespace Mammoth.Engine
{
    public abstract class InputPlayer : Player, IDestructable
    {
        public InputPlayer(Game game) : base(game)
        {
            Init();
        }

        protected void Init()
        {
            this.Height = 6.0f;

            InitializePhysX();

            /// TODO: Remove this call - the player will get spawned by the game logic.
            this.Spawn(new Vector3(-3.0f, 10.0f, 0.0f), Quaternion.Identity);

            this.CurrentCollision = 0;
        }


        public override void Spawn(Vector3 pos, Quaternion orient)
        {
            base.Spawn(pos, orient);

            this.Yaw = 0.0f;
            this.Pitch = 0.0f;
        }

        // TODO: Clean this up a bit?
        public void InitializePhysX()
        {
            IPhysicsManagerService physics = (IPhysicsManagerService) this.Game.Services.GetService(typeof(IPhysicsManagerService));

            ControllerDescription desc = new CapsuleControllerDescription(1, this.Height - 2.0f)
            {
                UpDirection = Axis.Y,
                Position = Vector3.UnitY * (this.Height - 1.0f) / 2.0f
            };
            this.PositionOffset = -1.0f * desc.Position;

            this.Controller = physics.CreateController(desc, this);
        }

        public void Die()
        {
            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));
            IModelDBService modelDB = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));

            // Remove the physx controller
            physics.RemoveController(this.Controller);
            // Remove the model
            modelDB.removeObject(this.ID);
        }

        public override void Update(GameTime gameTime)
        {
            // Check whether the player is dead
            if (Dead)
            {
                Die();
                return;
            }

            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));
            IInputService inputService = (IInputService)this.Game.Services.GetService(typeof(IInputService));

            foreach (var input in inputService.States)
            {
                // Get a dampened version of the mouse movement.
                Vector2 delta = input.MouseDelta * 0.0005f;
                // Modify the yaw based on horizontal mouse movement.
                this.Yaw = MathHelper.WrapAngle(this.Yaw - delta.X);
                // Modify the pitch based on vertical mouse movement.
                const float pitchClamp = 0.9876f;

                // TODO: There's probably a better (faster) way of doing this...
                this.Pitch = (float)Math.Asin(MathHelper.Clamp((float)Math.Sin(this.Pitch - delta.Y), -pitchClamp, pitchClamp));

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
                    if (input.IsKeyDown(InputType.Sprint))
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
                if (input.IsKeyDown(InputType.Forward)) // Forwards?
                    motion += Vector3.Forward;

                if (input.IsKeyDown(InputType.Backward)) // Backwards?
                    motion += Vector3.Backward;

                if (input.IsKeyDown(InputType.Left)) // Left?
                    motion += Vector3.Left;

                if (input.IsKeyDown(InputType.Right)) // Right?
                    motion += Vector3.Right;
                //}

                // Normalize the motion vector (so we don't move at twice the speed when moving diagonally).
                if (motion != Vector3.Zero)
                    motion.Normalize();
                motion *= speed;

                // Add the calculated motion to the current velocity.
                this.Velocity += motion;

                // If the player presses space (and is on the ground), jump!
                if (InCollisionState(ControllerCollisionFlag.Down))
                    if (input.IsKeyDown(InputType.Jump))
                        this.Velocity += Vector3.Up / 4.0f;

                // TODO: FIX TO HANDLE THROWING GRENADES vs SHOOTING!
                if (input.IsKeyDown(InputType.Shoot))
                    this.Throw();

                // Move the player's controller based on its velocity.
                this.CurrentCollision = (this.Controller.Move(Vector3.Transform(this.Velocity, this.Orientation))).CollisionFlag;

                // Now, we need to reset parts of the velocity so that we're not compounding it.
                if (InCollisionState(ControllerCollisionFlag.Down))
                    this.Velocity = Vector3.Zero;
                else
                    this.Velocity += physics.Scene.Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds - motion;
            }
        }

        // TODO: MAKE THIS LEGITIMATE
        protected void Throw()
        {
            Console.WriteLine("Throwing...");
            Bullet bullet = new Bullet(Game, Vector3.Zero, new Quaternion());
            Console.WriteLine("Thrown.");
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

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));
            ICameraService cam = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));

            // If you're using the first-person camera, don't draw your own geometry.
            if (cam.Type != Camera.CameraType.FIRST_PERSON)
                r.DrawRenderable(this);
        }

        public override string getObjectType()
        {
            //return typeof(InputPlayer).ToString();
            return "Player";
        }

        public override void CollideWith(PhysicalObject obj)
        {

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
