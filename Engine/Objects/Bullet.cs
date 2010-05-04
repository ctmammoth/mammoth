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
    public class Bullet : Projectile, IEncodable
    {
        /// <summary>
        /// Creates a new bullet at the specified position and gives it the required initial velocity.  It moves in the
        /// direction of the vector obtained by taking Vector3.Transform(Vector3.UnitZ, orientation).
        /// </summary>
        /// <param name="position">The location at which to spawn the bullet.</param>
        /// <param name="orientation">The direction in which to shoot the bullet.</param>
        public Bullet(Game game, Vector3 position, Quaternion orientation)
            : base(game)
        {
            InitialVelocityMagnitude = 2.0f;

            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));
            
            // TODO: get orientation from player
            Vector3 tempVelocity = Vector3.UnitZ;
            //velocity = Vector3.Transform(velocity, orientation);
            InitialVelocity = Vector3.Multiply(tempVelocity, InitialVelocityMagnitude);

            ActorDescription bulletActorDesc = new ActorDescription()
            {
                Shapes = { new SphereShapeDescription(2.0f) },
                // Add a body so the bullet moves
                BodyDescription = new BodyDescription(10.0f)
            };

            // Set the body's initial velocity
            bulletActorDesc.BodyDescription.LinearVelocity = InitialVelocity;

            // Create the actor
            this.Actor = physics.CreateActor(bulletActorDesc, this);

            // Add this to the model DB
            IModelDBService modelDB = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            modelDB.registerObject(this);

            Console.WriteLine("Making bullet");
        }

        public override void Update(GameTime gameTime)
        {
            // Send the bullet
            //IServerNetworking network = (IServerNetworking)this.Game.Services.GetService(typeof(INetworkingService));
            //network.sendThing(this);
        }


        // TODO
        public override void CollideWith(PhysicalObject obj)
        {
        }

        // TODO
        public override string getObjectType()
        {
            return "Bullet";
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
