using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mammoth.Engine.Networking;

using StillDesign.PhysX;
using StillDesign.PhysX.Utilities;
using Microsoft.Xna.Framework;

using Mammoth.Engine.Physics;

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
        /// <param name="forward">A vector pointing in the direction in which to shoot the bullet.</param>
        public Bullet(Game game, Vector3 position, Vector3 forward)
            : base(game)
        {
            Console.WriteLine("Constructing a bullet...");
            InitialVelocityMagnitude = 500.0f;

            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));

            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));

            this.Model3D = r.LoadModel("soldier-low-poly");
            this.PositionOffset = Vector3.Zero;

            // Make the bullet's actor description
            BodyDescription bodyDesc = new BodyDescription()
            {
                //BodyFlags = BodyFlag.Kinematic,
                Mass = 1.0f
            };


            TriangleMeshDescription trimeshDesc = new TriangleMeshDescription();
            trimeshDesc.AllocateVertices<Vector3>(1);
            trimeshDesc.AllocateTriangles<int>(1);
            // Add a single vertex somewhere
            trimeshDesc.VerticesStream.Write<Vector3>(new Vector3(0.0f, 0.0f, 0.0f));
            trimeshDesc.VertexCount = 1;
            trimeshDesc.TriangleCount = 1;

            SphereShapeDescription sphereShapeDesc = new SphereShapeDescription()
            {
                Radius = 1.0f,
                CCDSkeleton = physics.CreateCCDSkeleton(trimeshDesc)
            };

            ActorDescription bulletActorDesc = new ActorDescription()
            {
                Shapes = { sphereShapeDesc },
                // Add a body so the bullet moves
                BodyDescription = bodyDesc,
                UserData = this
            };

            // Set the body's initial velocity
            forward.Normalize();
            InitialVelocity = Vector3.Multiply(forward, InitialVelocityMagnitude);
            bulletActorDesc.BodyDescription.LinearVelocity = InitialVelocity;

            // Create the actor
            this.Actor = physics.CreateActor(bulletActorDesc, this);

            Position = position;
        }

        public Bullet(Game game)
            : base(game)
        {
            InitializeDefault(0);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));

            r.DrawRenderable(this);
        }

        public override void InitializeDefault(int id)
        {
            InitialVelocityMagnitude = 10.0f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //Console.WriteLine("Actor exists? " + (Actor != null));
            //Console.WriteLine("Bullet pos: " + Position);
        }

        // TODO
        public override void CollideWith(PhysicalObject obj)
        {
            // Check whether obj is damageable
            if (obj is IDamageable)
                ((IDamageable)obj).TakeDamage(GetDamage());

            // Destroy this bullet on impact
            IModelDBService mdb = (IModelDBService)this.Game.Services.GetService(typeof (IModelDBService));
            mdb.removeObject(ID);
            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));
            physics.RemoveActor(Actor);
        }

        // TODO
        public override string getObjectType()
        {
            return "Bullet";
        }

        public Vector3 PositionOffset
        {
            get;
            set;
        }


        #region IEncodeable members
        public byte[] Encode()
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder();

            e.AddElement("Position", Position);
            e.AddElement("InitialVelocity", InitialVelocity);

            return e.Serialize();
        }

        public void Decode(byte[] serialized)
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder(serialized);

            Console.WriteLine("Constructing a bullet...");
            InitialVelocityMagnitude = 500.0f;

            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));

            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));

            this.Model3D = r.LoadModel("soldier-low-poly");
            this.PositionOffset = Vector3.Zero;

            // Make the bullet's actor description
            BodyDescription bodyDesc = new BodyDescription()
            {
                //BodyFlags = BodyFlag.Kinematic,
                Mass = 1.0f
            };
            ActorDescription bulletActorDesc = new ActorDescription()
            {
                Shapes = { new SphereShapeDescription() { Radius = 1.0f } },
                // Add a body so the bullet moves
                BodyDescription = bodyDesc,
                UserData = this
            };

            InitialVelocity = (Vector3)e.GetElement("InitialVelocity", InitialVelocity);
            bulletActorDesc.BodyDescription.LinearVelocity = InitialVelocity;

            // Create the actor
            this.Actor = physics.CreateActor(bulletActorDesc, this);

            Position = (Vector3)e.GetElement("Position", Position);          

            Console.WriteLine("Bullet position received: " + Position);
            Console.WriteLine("Initial Velocity received: " + InitialVelocity);
        }
        #endregion

        #region IDamager Members

        float GetDamage()
        {
            return 10.0f;
        }

        #endregion
    }
}
