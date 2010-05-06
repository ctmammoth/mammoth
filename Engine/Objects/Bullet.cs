using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mammoth.Engine.Networking;

using StillDesign.PhysX;
using StillDesign.PhysX.Utilities;
using Microsoft.Xna.Framework;

using Mammoth.Engine.Physics;
using Mammoth.Engine.Audio;

namespace Mammoth.Engine
{
    //TODO: Make bullet drawable
    public class Bullet : Projectile, IEncodable, IRenderable
    {
        /// <summary>
        /// Creates a new bullet at the specified position and gives it the required initial velocity.  It moves in the
        /// direction of the vector obtained by taking Vector3.Transform(Vector3.UnitZ, orientation).
        /// </summary>
        /// <param name="position">The location at which to spawn the bullet.</param>
        /// <param name="forward">A unit vector pointing in the direction in which to shoot the bullet.</param>
        public Bullet(Game game, Vector3 position, Vector3 direction, int creator)
            : base(game, creator)
        {
            Console.WriteLine("Constructing a bullet...");
            
            // Set the initial position and direction
            InitialPosition = position;
            InitialDirection = direction;

            // Set the velocity to point the same way as direction
            Velocity = Vector3.Multiply(InitialDirection, VelocityMagnitude);

            // Load a retarded model
            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));
            this.Model3D = r.LoadModel("soldier-low-poly");

            IModelDBService mdb = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            this.ID = mdb.getNextOpenID();
            mdb.registerObject(this);

            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));

            SphereShapeDescription sDesc = new SphereShapeDescription()
            {
                Radius = 1.0f
            };

            ActorDescription aDesc = new ActorDescription()
            {
                Shapes = { sDesc },
                BodyDescription = new BodyDescription()
                {
                    Mass = 1.0f,
                    // Make this kinematic so we can do our raycasted "ccd"
                    BodyFlags = BodyFlag.Kinematic
                }
            };

            // Create the bullet's actor
            this.Actor = physics.CreateActor(aDesc, this);

            // Create the actor at the specified location
            this.Position = InitialPosition;
        }

        public Bullet(Game game)
            : base(game, 0)
        {
        }

        /// <summary>
        /// Performs the ray cast to shoot the bullet.
        /// </summary>
        private void FireBullet()
        {
        }

        public override void Update(GameTime gameTime)
        {
            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));


            // Move the position by the amount dictated by its velocity and the elapsed game time
            Vector3 tempPos = Vector3.Multiply(Velocity, (float)gameTime.ElapsedGameTime.TotalSeconds);

            // Perform the raycast
            RaycastHit rayHit = physics.RaycastClosestShape(Position + (new Vector3(0.0f, 8.0f, 0.0f)), InitialDirection);

            // Get the difference in position
            float distanceMoved = Vector3.Subtract(tempPos, Position).Length();

            // Make sure the shape that was hit is between the current and previous positions, exists and that 
            // its actor has userdata
            if (rayHit.Distance <= distanceMoved && rayHit.Shape != null)
            {
                Console.WriteLine("Raycasting a bullet.");
                // Get the PhysicalObject that owns the Shape hit by the raycast
                PhysicalObject objHit = ((PhysicalObject)rayHit.Shape.Actor.UserData);

                // Try damaging the object
                if (objHit != null && objHit is IDamageable)
                {
                    Console.WriteLine("Damaging a mofo of type " + objHit.getObjectType());
                    //((IDamageable)objHit).TakeDamage(GetDamage(), this);
                }
            }
            else
            {
                Position = tempPos;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));
            r.DrawRenderable(this);
        }

        public override string getObjectType()
        {
            return "Bullet";
        }

        #region Properties
        // We will raycast between the bullet's current position and here to determine whether a collision occurred.
        private Vector3 LastPosition
        {
            get;
            set;
        }

        // This bullet's velocity vector
        public Vector3 Velocity
        {
            get;
            protected set;
        }
        #endregion

        #region Variables
        // The magnitude of the bullet's velocity
        private const float VelocityMagnitude = 0.0f;
        #endregion

        #region IEncodeable members
        public byte[] Encode()
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder();

            e.AddElement("InitialPosition", InitialPosition);
            e.AddElement("Velocity", Velocity);
            e.AddElement("Creator", Creator);

            return e.Serialize();
        }

        public void Decode(byte[] serialized)
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder(serialized);

            Console.WriteLine("Constructing a bullet...");

            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));

            InitialPosition = (Vector3)e.GetElement("InitialPosition", InitialPosition);
            InitialDirection = (Vector3)e.GetElement("InitialDirection", InitialDirection);
            Creator = (int)e.GetElement("Creator", Creator);


            Console.WriteLine("Bullet InitialPosition received: " + InitialPosition);
            Console.WriteLine("Bullet InitialDirection received: " + InitialDirection);

            FireBullet();
        }
        #endregion

        #region IDamager Members

        float GetDamage()
        {
            return 10.0f;
        }

        #endregion

        #region IRenderable Members

        public Vector3 PositionOffset
        {
            get
            {
                return Vector3.Zero;
            }
        }

        public Quaternion Orientation
        {
            get
            {
                return Quaternion.Identity;
            }
        }

        public Microsoft.Xna.Framework.Graphics.Model Model3D
        {
            get;
            protected set;
        }

        #endregion
    }
}
