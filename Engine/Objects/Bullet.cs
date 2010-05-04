﻿using System;
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
        /// <param name="forward">A vector pointing in the direction in which to shoot the bullet.</param>
        public Bullet(Game game, Vector3 position, Vector3 forward)
            : base(game)
        {
            Console.WriteLine("Constructing a bullet...");
            InitialVelocityMagnitude = 10.0f;

            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));

            // Make the bullet's actor description
            ActorDescription bulletActorDesc = new ActorDescription()
            {
                Shapes = { new SphereShapeDescription(){ Radius = 1.0f, LocalPosition = position } },
                // Add a body so the bullet moves
                BodyDescription = new BodyDescription(10.0f)
            };

            // Set the body's initial velocity
            forward.Normalize();
            InitialVelocity = Vector3.Multiply(forward, InitialVelocityMagnitude);
            bulletActorDesc.BodyDescription.LinearVelocity = InitialVelocity;

            // Create the actor
            this.Actor = physics.CreateActor(bulletActorDesc, this);
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
