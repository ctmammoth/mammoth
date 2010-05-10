using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using StillDesign.PhysX;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Mammoth.Engine.Input;
using Mammoth.Engine.Physics;
using Mammoth.Engine.Objects;
using Mammoth.Engine.Networking;
using Mammoth.Engine.Interface;
using Mammoth.Engine;

namespace Mammoth.Engine
{
    /// <summary>
    /// An InputPlayer is a Player whose properties may be modified by the IInputService.
    /// </summary>
    public abstract class InputPlayer : Player
    {

        #region Properties
        
        //CURRENT COLLISION
        public ControllerCollisionFlag CurrentCollision
        {
            get;
            private set;
        }
        //YAW
        private float Yaw
        {
            get;
            set;
        }

        //PITCH
        private float Pitch
        {
            get;
            set;
        }

        // The armed weapon
        protected IWeapon CurWeapon
        {
            get;
            set;
        }

        // The weapons owned by this player
        protected IWeapon[] Items
        {
            get;
            set;
        }

        protected GameStats GameStats
        {
            get;
            set;
        }

        protected Flag Flag
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// Initialize a new InputPlayer. Does nothing special.
        /// </summary>
        public InputPlayer(Game game) : base(game)
        {
            // Initializes PhysX of a player.
            InitializePhysX();

            this.Spawn(new Vector3(-3.0f, 10.0f, 0.0f), Quaternion.Identity);

            // Give the player 5 weapons, for now
            Items = new IWeapon[5];
            // Give the player a simple gun, for now
            Items[0] = new Revolver(game, this);
            CurWeapon = Items[0];

            // Give the player some stats
            GameStats = new GameStats();
        }

        /// <summary>
        /// Resets all the properties of an InputPlayer to default values. Spawns the player with passed in position and orientation.
        /// </summary>
        /// <param name="pos">Starting position.</param>
        /// <param name="orient">Starting orientation.</param>
        public override void Spawn(Vector3 pos, Quaternion orient)
        {
            // Set the spawn properties that are unique to all players.
            base.Spawn(pos, orient);

            // TODO: Fix this stuff.
            // Set properties specific to an input-based player.
            this.CurrentCollision = 0;
            this.Yaw = 0.0f;
            this.Pitch = 0.0f;
        }

        /// <summary>
        /// Sets up the PhysX of a player and "syncs" with model.
        /// </summary>
        public void InitializePhysX()
        {
            //Gives the player a bounding box and a physical description
            IPhysicsManagerService physics = (IPhysicsManagerService) this.Game.Services.GetService(typeof(IPhysicsManagerService));
            ControllerDescription desc = new CapsuleControllerDescription(1, this.Height - 2.0f)
            {
                UpDirection = Axis.Y,
                Position = Vector3.UnitY * (this.Height - 1.0f) / 2.0f,
                StepOffset = 0.55f
            };

            //Describe the offset of the model to sync model center with PhysX Controller center
            this.PositionOffset = -1.0f * desc.Position;
            
            //Set controllet to one defined above
            this.Controller = physics.CreateController(desc, this);
        }

        /// <summary>
        /// On every timestep, checks that player is still alive, checks queued input states and reacts.
        /// </summary>
        /// <param name="gameTime">The Game Time.</param>
        public override void Update(GameTime gameTime)
        {
            // Load services for use later.
            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));
            IInputService inputService = (IInputService)this.Game.Services.GetService(typeof(IInputService));

            // Go through queued InputStates and modify Player properties accordingly.
            foreach (var input in inputService.States)
            {
                // Get a dampened version of the mouse movement.
                Vector2 delta = input.MouseDelta * 0.0005f;
                // Modify the yaw based on horizontal mouse movement.
                this.Yaw = MathHelper.WrapAngle(this.Yaw - delta.X);
                // Modify the pitch based on vertical mouse movement.
                const float pitchClamp = 0.9876f;
                //Calculate pitch
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

                //TODO: Put this somewhere not sucky.
                if (input.KeyPressed(InputType.Stats) && this is LocalInputPlayer)
                {
                    TScreenManager t = (TScreenManager)this.Game.Services.GetService(typeof(TScreenManager));
                    t.AddScreen(new StatsScreen(this.Game, GameStats));
                }

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
                if (input.KeyPressed(InputType.Fire))
                    this.Shoot(gameTime);

                // Reload the user's gun
                if (input.KeyPressed(InputType.Reload))
                    this.Reload(gameTime);

                // Move the player's controller based on its velocity.
                this.CurrentCollision = (this.Controller.Move(Vector3.Transform(this.Velocity, this.Orientation))).CollisionFlag;

                // Now, we need to reset parts of the velocity so that we're not compounding it.
                if (InCollisionState(ControllerCollisionFlag.Down))
                    this.Velocity = Vector3.Zero;
                else
                    this.Velocity += physics.Scene.Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds - motion;
            }

            // Update main weapon
            ((BaseObject)CurWeapon).Update(gameTime);

            //Console.WriteLine("Weapon " + ((BaseObject)CurWeapon).getObjectType() + " has " + CurWeapon.ShotsLeft() + " shots left.");
        }

        /// <summary>
        /// Throws a "bullet" in the current direction of the player. Overridden in ProxyInputPlayer since shooting only happens on server-side.
        /// </summary>
        protected virtual void Shoot(GameTime time) {
            Console.WriteLine("Throwing, orientation is: " + this.HeadOrient);
        }

        /// <summary>
        /// Reloads the player's current weapon.  Overridden in ProxyInputPlayer since reloading only happens on server-side.
        /// </summary>
        protected virtual void Reload(GameTime time)
        {
            Console.WriteLine("Reloading.");
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
        /// Draws the InputPlayer.
        /// </summary>
        /// <param name="gameTime">The Game Time.</param>
        public override void Draw(GameTime gameTime)
        {
            //Call Player's Draw
            base.Draw(gameTime);

            //Load services
            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));
            ICameraService cam = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));

            //If you're using the first-person camera, don't draw your own geometry.
            if (cam.Type != Camera.CameraType.FIRST_PERSON)
                r.DrawRenderable(this);
            if (CurWeapon != null)
                ((BaseObject)CurWeapon).Draw(gameTime);
        }

        public override void RespondToTrigger(PhysicalObject obj)
        {
            Console.WriteLine("Responding to trigger.");
        }

        public override void TakeDamage(float damage, IDamager inflicter)
        {
            base.TakeDamage(damage, inflicter);
            Console.WriteLine("Health: " + this.Health);
            if (this.Health <= 0)
            {
                Die();
                //Get your own id
                int myID = this.ID >> 25;
                Projectile p = (Projectile)inflicter;
                Console.WriteLine("Player " + myID + " was killed by Player " + p.Creator);
            }
        }

        /// <summary>
        /// Respawns the player when the it dies.
        /// </summary>
        public override void Die()
        {
            base.Die();
            Console.WriteLine("Player " + ID + " died.");
            this.Spawn(new Vector3(-3.0f, 10.0f, 0.0f), Quaternion.Identity);
        }

        #region IEncodable Members

        public override byte[] Encode()
        {
            Networking.Encoder tosend = new Networking.Encoder();

            IGameLogic g = (IGameLogic)this.Game.Services.GetService(typeof(IGameLogic));
            int myID = ID >> 25;
            GameStats = new GameStats(NumKills, NumCaptures, NumDeaths, myID, g);

            //Console.WriteLine("Encoding: " + GameStats.ToString());

            tosend.AddElement("Position", Position);
            tosend.AddElement("Orientation", Orientation);
            tosend.AddElement("HeadOrient", HeadOrient);
            tosend.AddElement("Velocity", Velocity);
            tosend.AddElement("Health", Health);
            tosend.AddElement("GameStats", GameStats);
            tosend.AddElement("GunType", ((BaseObject)CurWeapon).getObjectType());
            tosend.AddElement("Gun", CurWeapon);

            return tosend.Serialize();
        }

        public override void Decode(byte[] serialized)
        {
            Networking.Encoder props = new Networking.Encoder(serialized);

            if (props.UpdatesFor("Position"))
                Position = (Vector3)props.GetElement("Position", Position);
            if (props.UpdatesFor("Orientation"))
                Orientation = (Quaternion)props.GetElement("Orientation", Orientation);
            if (props.UpdatesFor("HeadOrient"))
                HeadOrient = (Quaternion)props.GetElement("HeadOrient", HeadOrient);
            if (props.UpdatesFor("Velocity"))
                Velocity = (Vector3)props.GetElement("Velocity", Velocity);
            if (props.UpdatesFor("Health"))
                Health = (float)props.GetElement("Health", Health);
            if (props.UpdatesFor("GameStats"))
                props.UpdateIEncodable("GameStats", GameStats);

            string gunType = (string)props.GetElement("GunType", "Revolver");
            if (CurWeapon == null || !((BaseObject)CurWeapon).getObjectType().Equals(gunType))
            {
                switch (gunType)
                {
                    case "Revolver":
                        CurWeapon = new Revolver(this.Game, this);
                        break;
                }
            }
            if (props.UpdatesFor("Gun"))
                props.UpdateIEncodable("Gun", CurWeapon);

            //Console.WriteLine("Decoding: " + GameStats.ToString());
        }

        #endregion

    }
}
