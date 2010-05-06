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

            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));

            // Just handle bullets by raycasting
            PhysicalObject objectHit = physics.RaycastClosestShape(InitialPosition, InitialDirection);
            // Damage the object that was hit if possible
            if (objectHit != null && objectHit is IDamageable)
                ((IDamageable)objectHit).TakeDamage(GetDamage());
        }

        public Bullet(Game game)
            : base(game, 0)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
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

            e.AddElement("InitialPosition", InitialPosition);
            e.AddElement("InitialDirection", InitialDirection);

            return e.Serialize();
        }

        public void Decode(byte[] serialized)
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder(serialized);

            Console.WriteLine("Constructing a bullet...");

            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));

            InitialPosition = (Vector3)e.GetElement("InitialPosition", InitialPosition);
            InitialDirection = (Vector3)e.GetElement("InitialDirection", InitialDirection);

            Console.WriteLine("Bullet InitialPosition received: " + InitialPosition);
            Console.WriteLine("Bullet InitialDirection received: " + InitialDirection);
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
