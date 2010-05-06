using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Physics;
using Mammoth.Engine.Networking;

namespace Mammoth.Engine
{
    public class Magazine : BaseObject
    {
        public readonly int MaxRounds;
        public int AmmoRemaining
        {
            get;
            private set;
        }

        public Magazine(Game game, int maxRounds)
            : base(game)
        {
            MaxRounds = maxRounds;
            AmmoRemaining = maxRounds;
        }

        /// <summary>
        /// Removes a bullet from the magazine if possible.
        /// </summary>
        /// <returns>The number of bullets remain in the magazine after the shot is fired, or zero if a bullet is
        /// shot when no rounds are remaining.</returns>
        public int FireShot()
        {
            if (AmmoRemaining == 0)
                return 0;

            AmmoRemaining = AmmoRemaining - 1;
            return AmmoRemaining;
        }

        /// <summary>
        /// Refills this magazine.
        /// </summary>
        /// <returns>The number of bullets in the magazine after refilling it.</returns>
        public void Refill()
        {
            AmmoRemaining = MaxRounds;
        }

        public override string getObjectType()
        {
            return "Magazine";
        }
    }

    /// <summary>
    /// A simple weapon which shoots Bullets.
    /// </summary>
    public class SimpleGun : BaseObject, IWeapon
    {
        #region Properties

        // This gun's orientation
        public Quaternion Orientation
        {
            get;
            set;
        }

        // The gun's position
        public Vector3 Position
        {
            get;
            set;
        }

        // The magazine used by this gun
        private Magazine Mag
        {
            get;
            set;
        }

        // The number of magazines remaining for this gun
        public int MagCount
        {
            get;
            private set;
        }

        #endregion

        public SimpleGun(Game game)
            : base(game)
        {
            // Give each magazine 15 bullets
            Mag = new Magazine(game, 15);
            // Give the gun some magazines
            MagCount = 5;
        }

        #region IWeapon Members

        public void Shoot(Vector3 position, Vector3 direction, int shooterID)
        {
            // Make sure a shot can be fired
            if (Mag.FireShot() > 0)
            {
                SpawnBullet(position, direction, shooterID);
            }
            else if (MagCount > 1)
            {
                Reload();
            }
            else
            {
                Console.WriteLine("Out of ammo.");
            }
        }

        private void SpawnBullet(Vector3 position, Vector3 direction, int shooterID)
        {
            // Make sure the bullet isn't spawned in the player: shift it by a bit
            Bullet b = new Bullet(Game, position, direction, shooterID >> 25);

            // Give this projectile an ID, but it's not really necessary since it gets shot instantaneously
            IModelDBService modelDB = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            b.ID = modelDB.getNextOpenID();

            // Send the bullet after it's created
            IServerNetworking net = (IServerNetworking)this.Game.Services.GetService(typeof(INetworkingService));
            net.sendThing(b);

            Console.WriteLine("Shot a bullet with a SimpleGun; " + Mag.AmmoRemaining + " bullets left.");
        }

        public void Reload()
        {
            if (MagCount != 0)
            {
                // Create a new magazine (really just refill the current one)
                Mag.Refill();
                MagCount -= 1;
            }
            else
            {
                // Out of magazines
            }
        }

        #endregion

        public override string getObjectType()
        {
            return "SimpleGun";
        }
    }
}
