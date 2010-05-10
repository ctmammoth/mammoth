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
        #region Variables

        // The magnitude of the bullet's velocity
        private const float speed = 50.0f;

        #endregion

        public Bullet(Game game, int creator)
            : base(game, creator)
        {
            Console.WriteLine("Constructing a bullet...");

            InitializePhysX();

            // Load a retarded model
            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));
            this.Model3D = r.LoadModel("bullet_low");
        }

        /// <summary>
        /// Creates a new bullet at the specified position and gives it the required initial velocity.  It moves in the
        /// direction of the vector obtained by taking Vector3.Transform(Vector3.UnitZ, orientation).
        /// </summary>
        /// <param name="position">The location at which to spawn the bullet.</param>
        /// <param name="forward">A unit vector pointing in the direction in which to shoot the bullet.</param>
        public Bullet(Game game, Vector3 position, Quaternion orient, int creator) 
            : base(game, creator)
        {
            InitializePhysX();

            // Set the initial position and direction
            this.Position = position;
            this.Orientation = orient;

            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));
            this.Model3D = r.LoadModel("bullet_low");

            IModelDBService mdb = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            this.ID = mdb.getNextOpenID();
            mdb.registerObject(this);
        }

        private void InitializePhysX()
        {
            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));

            SphereShapeDescription sDesc = new SphereShapeDescription()
            {
                Radius = 0.3f
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
        }

        public override void Update(GameTime gameTime)
        {
            //Console.WriteLine("I'm here with " + Velocity + " id=" + ID);

            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));

            // Move the position by the amount dictated by its velocity and the elapsed game time

            //Vector3 tempPos = Position + Vector3.Multiply(Velocity, (float)gameTime.ElapsedGameTime.TotalSeconds);

            // Perform the raycast
            Vector3 dir = Vector3.Transform(Vector3.Forward, this.Orientation);
            RaycastHit rayHit = physics.RaycastClosestShape(this.Position + dir * 0.35f, dir);

            // Get the difference in position
            float distanceMoved = speed * (float) gameTime.ElapsedGameTime.TotalSeconds;

            // Make sure the shape that was hit is between the current and previous positions, exists and that 
            // its actor has userdata
            if (rayHit.Shape != null)
            {
                Console.WriteLine("Bullet with : " + this.ID + " ray hit something with a shape of type " + rayHit.Shape.ToString());
                // Get the PhysicalObject that owns the Shape hit by the raycast
                PhysicalObject objHit = ((PhysicalObject)rayHit.Shape.Actor.UserData);

                if (objHit != null)
                {
                    Console.WriteLine("Bullet with ID: " + this.ID + " ray hit something with userdata of type: " + objHit.getObjectType());
                    // Make sure the collision is within the distance we've moved during this timestep.
                    if (rayHit.Distance <= distanceMoved)
                    {
                        Console.WriteLine("Bullet with ID: " + this.ID + " ray hit something in range");
                        // Make sure the object is damageable.
                        if (objHit.ID >> 25 != Creator)
                        {
                            Console.WriteLine("Bullet with ID: " + this.ID + " ray hit something that isn't its creator and isn't a bullet");
                            // Make sure the creator isn't the one being hit.
                            if (objHit is IDamageable)
                            {
                                Console.WriteLine("Bullet with ID: " + this.ID + " ray hit something damagable in range");
                                Console.WriteLine("Damaging a mofo of type " + objHit.getObjectType());
                                ((IDamageable)objHit).TakeDamage(this.GetDamage(), this);
                            }

                            Console.WriteLine("Bullet with ID: " + this.ID + " being removed");

                            // Have the bullet get removed at the next update step.
                            this.IsAlive = false;

                            // We collided with something legit, so let's get out of here.
                            return;
                        }
                    }
                }
            }

            // We didn't collide with something, so move the bullet.
            //Console.WriteLine("Moving bullet with ID: " + this.ID);
            this.Position += dir * distanceMoved;
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

        #region IEncodeable members

        public byte[] Encode()
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder();

            e.AddElement("Position", Position);
            e.AddElement("Orientation", Orientation);
            e.AddElement("Creator", Creator);

            return e.Serialize();
        }

        public void Decode(byte[] serialized)
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder(serialized);

            Console.WriteLine("Decoding a bullet...");

            Position = (Vector3)e.GetElement("Position", Position);
            Orientation = (Quaternion)e.GetElement("Orientation", Orientation);
            Creator = (int)e.GetElement("Creator", Creator);

            Console.WriteLine("Bullet Position received: " + Position);
            Console.WriteLine("Bullet Orientation received: " + Orientation);
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();

            Console.WriteLine("DISPOSE YOU MOTHERFUCKER!!!");
        }

        #region IDamager Members

        public override float GetDamage()
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

        public Microsoft.Xna.Framework.Graphics.Model Model3D
        {
            get;
            protected set;
        }

        #endregion
    }
}
