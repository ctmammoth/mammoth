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
    public class Bullet : Projectile, IEncodable
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

            // Fire the bullet
            FireBullet();
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
            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));

            // Just handle bullets by raycasting
            PhysicalObject objectHit = physics.RaycastClosestShape(InitialPosition, InitialDirection);
            // Damage the object that was hit if possible
            if (objectHit != null && objectHit is IDamageable)
            {
                Console.WriteLine("Damaging a mofo of type " + objectHit.getObjectType());
                ((IDamageable)objectHit).TakeDamage(GetDamage());
            }
        }

        public override string getObjectType()
        {
            return "Bullet";
        }        

        #region IEncodeable members
        public byte[] Encode()
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder();

            e.AddElement("InitialPosition", InitialPosition);
            e.AddElement("InitialDirection", InitialDirection);
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

            // Fire the bullet from the decoded position in the decoded direction
            FireBullet();
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
