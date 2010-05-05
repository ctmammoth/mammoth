using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mammoth.Engine.Networking;
using StillDesign.PhysX;
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
            InitialVelocityMagnitude = 500.0f;

            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));

            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));

            this.Model3D = r.LoadModel("soldier-low-poly");
            this.PositionOffset = Vector3.Zero;

            // Make the bullet's actor description
            BodyDescription bodyDesc = new BodyDescription()
            {
                Mass = 1.0f//,
                //BodyFlags = BodyFlag.Kinematic
            };
            ActorDescription bulletActorDesc = new ActorDescription()
            {
                Shapes = { new SphereShapeDescription() { Radius = 1.0f } },
                // Add a body so the bullet moves
                BodyDescription = bodyDesc
            };

            // Set the body's initial velocity
            forward.Normalize();
            InitialVelocity = Vector3.Multiply(forward, InitialVelocityMagnitude);
            bulletActorDesc.BodyDescription.LinearVelocity = InitialVelocity;

            // Create the actor
            this.Actor = physics.CreateActor(bulletActorDesc, this);
            Console.Write("Constructing a bullet: position = " + position);
            Position = position;
            Console.WriteLine("; my position is now " + Position);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));

            r.DrawRenderable(this);
        }

        public override void InitializeDefault(int id)
        {
            this.ID = id;
            InitialVelocityMagnitude = 10.0f;
        }

        // TODO
        public override void CollideWith(PhysicalObject obj)
        {
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

            Position = (Vector3)e.GetElement("Position", Position);
            InitialVelocity = (Vector3)e.GetElement("InitialVelocity", InitialVelocity);
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
