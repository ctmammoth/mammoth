using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Physics;
using Mammoth.Engine.Networking;
using Mammoth.Engine.Objects;

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
    public class SimpleGun : BaseObject, IWeapon, IHoldeableItem
    {
        #region Properties

        // This gun's owner
        private Player Owner
        {
            get;
            set;
        }

        // This gun's orientation
        public Quaternion Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                if (Owner != null)
                    _orientation = Owner.HeadOrient;
                else
                    _orientation = value;
            }
        }

        // The gun's position
        public Vector3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (Owner != null)
                    _position = Owner.Position;
                else
                    _position = value;
            }
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

        // The gun's model
        public Microsoft.Xna.Framework.Graphics.Model Model3D
        {
            get;
            protected set;
        }

        #endregion

        private Vector3 _position;
        private Quaternion _orientation;

        private double _lastFiredTime;
        private double _lastReloadTime;

        private const double FIRE_RATE = 4.0;
        private const double RELOAD_TIME = 2000.0;

        // Perturbs the bullet's direction
        private Random directionPerturber;
        // Determines the amount by which to perturb the bullet's direction
        private float inaccuracy;

        public SimpleGun(Game game, Player owner)
            : base(game)
        {
            // Give each magazine 15 bullets
            Mag = new Magazine(game, 15);
            // Give the gun some magazines
            MagCount = 5;
            // Give the gun a model
            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));
            this.Model3D = r.LoadModel("soldier-low-poly");

            _lastFiredTime = 0;
            _lastReloadTime = -1;
            // Set the owner
            Owner = owner;
            // Set location
            Position = owner.Position;
            Orientation = owner.HeadOrient;
            // Set inaccuracy
            inaccuracy = 0.01f;
            // Make the randomizer
            directionPerturber = new Random();
        }

        #region IWeapon Members

        public void Shoot(Vector3 position, Vector3 direction, int shooterID, GameTime time)
        {
            // Make sure a shot can be fired
            double curTime = time.TotalRealTime.TotalMilliseconds;
            if (Mag.FireShot() > 0)
            {
                // Check whether it's too soon to fire
                if ((curTime - _lastFiredTime) >= (1000.0 / FIRE_RATE) && _lastReloadTime < 0)
                {
                    _lastFiredTime = curTime;

                    // Randomly perturb the bullet
                    direction = Vector3.Add(direction, new Vector3(directionPerturber.NextDouble() * inaccuracy,
                        directionPerturber.NextDouble() * inaccuracy,
                        directionPerturber.NextDouble() * inaccuracy));

                    SpawnBullet(position, direction, shooterID);
                }
            }
            else if (MagCount > 1)
            {
                _lastReloadTime = curTime;
                Reload(time);
                SpawnBullet(position, direction, shooterID);
            }
            else
            {
                Console.WriteLine("Out of ammo.");
            }
        }

        /// <summary>
        /// Spawns a bullet from this gun.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="shooterID"></param>
        private void SpawnBullet(Vector3 position, Vector3 direction, int shooterID)
        {
            IServerNetworking net = (IServerNetworking)this.Game.Services.GetService(typeof(INetworkingService));
            net.sendSound("Gunshot");

            // Make sure the bullet isn't spawned in the player: shift it by a bit
            Bullet b = new Bullet(Game, position, direction, shooterID >> 25);

            // Give this projectile an ID, but it's not really necessary since it gets shot instantaneously
            IModelDBService modelDB = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            b.ID = modelDB.getNextOpenID();

            // Send the bullet after it's created
            net.sendThing(b);

            Console.WriteLine("Shot a bullet with a SimpleGun; " + Mag.AmmoRemaining + " bullets left.");
        }

        public void Reload(GameTime time)
        {
            Console.WriteLine("SimpleGun is reloading!");
            _lastReloadTime = time.TotalRealTime.TotalMilliseconds;
            IServerNetworking net = (IServerNetworking)this.Game.Services.GetService(typeof(INetworkingService));
            net.sendSound("Reload", Owner.ID >> 25);

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

        #region BaseObject Members

        public override string getObjectType()
        {
            return "SimpleGun";
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            this.Position = Owner.Position;
            this.Orientation = Owner.HeadOrient;
            if (_lastReloadTime >= 0 && gameTime.TotalRealTime.TotalMilliseconds - _lastReloadTime >= RELOAD_TIME)
            {
                _lastReloadTime = -1;
                IServerNetworking net = (IServerNetworking)this.Game.Services.GetService(typeof(INetworkingService));
                net.sendSound("Reload", Owner.ID >> 25);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //Load services
            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));
            ICameraService cam = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));

            //Render the RemotePlayer
            r.DrawRenderable(this);
        }

        #endregion

        #region IHoldeableItem Members

        void IHoldeableItem.SetOwner(Player owner)
        {
            Owner = owner;
        }

        #endregion

        #region IRenderable Members

        Vector3 IRenderable.PositionOffset
        {
            get
            {
                return new Vector3(2.0f, 2.0f, 2.0f);
            }
        }

        #endregion
    }
}
