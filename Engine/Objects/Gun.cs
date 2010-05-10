using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Physics;
using Mammoth.Engine.Networking;
using Mammoth.Engine.Objects;
using Mammoth.Engine.Input;

namespace Mammoth.Engine.Objects
{
    /// <summary>
    /// Represents a magazine for a gun.  Acts as a bullet container.
    /// </summary>
    public class Magazine : BaseObject, IEncodable
    {
        /// <summary>
        /// The maximum number of rounds that this magazine can hold.
        /// </summary>
        public int MaxRounds;

        /// <summary>
        /// The ammo remaining in this magazine.
        /// </summary>
        public int AmmoRemaining
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="maxRounds">The maximum number of rounds this magazine should hold.</param>
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
        public bool CanFireShot()
        {
            return AmmoRemaining > 0;
        }

        public void FireShot()
        {
            AmmoRemaining = AmmoRemaining - 1;
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

        #region IEncodable Members

        public byte[] Encode()
        {
            Networking.Encoder tosend = new Networking.Encoder();

            tosend.AddElement("MaxRounds", MaxRounds);
            tosend.AddElement("AmmoRemaining", AmmoRemaining);

            return tosend.Serialize();
        }

        public void Decode(byte[] serialized)
        {
            Networking.Encoder props = new Networking.Encoder(serialized);

            if (props.UpdatesFor("MaxRounds"))
                MaxRounds = (int)props.GetElement("MaxRounds", MaxRounds);
            if (props.UpdatesFor("AmmoRemaining"))
                AmmoRemaining = (int)props.GetElement("AmmoRemaining", AmmoRemaining);
        }

        #endregion
    }

    /// <summary>
    /// A simple weapon which shoots Bullets.
    /// </summary>
    public abstract class Gun : BaseObject, IHoldableItem, IEncodable, IRenderable
    {
        #region Properties

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
        protected Magazine Mag
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
        private double _reloadStartTime;

        // The maximum rate of fire in bullets per second
        protected abstract double FireRate { get; }

        // The time a reload takes in milliseconds
        protected abstract double ReloadTime { get; }

        // Determines the amount by which to perturb the bullet's direction
        protected abstract float Inaccuracy { get; }

        // The magazine size of the gun
        protected abstract int MagazineCapacity { get; }

        // The number of magazines to hold for this gun
        protected abstract int NumMagazines { get; }

        // The fire sound for this gun
        protected abstract string FireSound { get; }

        // Perturbs the bullet's direction
        protected Random directionPerturber;

        public Gun(Game game, Player owner)
            : base(game)
        {
            // Give each magazine 15 bullets
            Mag = new Magazine(game, MagazineCapacity);
            // Give the gun some magazines
            MagCount = NumMagazines;
            // Give the gun a model
            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));
            this.Model3D = r.LoadModel("soldier-low-poly");

            _lastFiredTime = 0;
            _reloadStartTime = -1;
            // Set the owner
            Owner = owner;
            // Set location
            Position = owner.Position;
            Orientation = owner.HeadOrient;
            // Make the randomizer
            directionPerturber = new Random();
        }

        public int ShotsLeft()
        {
            return Mag.AmmoRemaining;
        }

        public int MagsLeft()
        {
            return MagCount;
        }

        public abstract bool ShouldShoot(InputState input);

        public void Shoot(Vector3 position, Quaternion orientation, int shooterID, GameTime time)
        {
            // Make sure a shot can be fired
            double curTime = time.TotalRealTime.TotalMilliseconds;
            if (Mag.CanFireShot())
            {
                // Check whether it's too soon to fire
                if ((curTime - _lastFiredTime) >= (1000.0 / FireRate) && _reloadStartTime < 0)
                {
                    _lastFiredTime = curTime;

                    SpawnBullet(position, orientation, shooterID);
                }
            }
            else if (MagCount > 1)
            {
                Reload(time);
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
        protected virtual void SpawnBullet(Vector3 position, Quaternion orientation, int shooterID)
        {
            Mag.FireShot();

            IServerNetworking net = (IServerNetworking)this.Game.Services.GetService(typeof(INetworkingService));
            net.sendEvent("Sound", FireSound);

            // Randomly perturb the bullet
            Quaternion perturbation =
                Quaternion.CreateFromYawPitchRoll(((float)directionPerturber.NextDouble() - 0.5f) * Inaccuracy,
                                                  ((float)directionPerturber.NextDouble() - 0.5f) * Inaccuracy,
                                                  0.0f);  

            Bullet b = createBullet(Game, position, orientation * perturbation, shooterID >> 25);

            // Send the bullet after it's created
            net.sendThing(b);

            //Console.WriteLine("Shot a bullet with a " + getObjectType() + "; " + Mag.AmmoRemaining + " bullets left.");
        }

        /// <summary>
        /// Create a bullet of the appropriate type.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="position"></param>
        /// <param name="orientation"></param>
        /// <param name="shooterID"></param>
        /// <returns></returns>
        protected abstract Bullet createBullet(Game game, Vector3 position, Quaternion orientation, int shooterID);

        public void Reload(GameTime time)
        {
            if (MagCount != 0 && _reloadStartTime < 0)
            {
                Console.WriteLine(getObjectType() + " is reloading!");
                _reloadStartTime = time.TotalRealTime.TotalMilliseconds;
                IServerNetworking net = (IServerNetworking)this.Game.Services.GetService(typeof(INetworkingService));
                net.sendEvent("Sound", "GunEmpty", Owner.ID >> 25);
                // Create a new magazine (really just refill the current one)
                Mag.Refill();
                MagCount -= 1;
            }
            else
            {
                // Out of magazines
            }
        }

        #region BaseObject Members

        public abstract override string getObjectType();

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            this.Position = Owner.Position;
            this.Orientation = Owner.HeadOrient;
            if (_reloadStartTime >= 0 && gameTime.TotalRealTime.TotalMilliseconds - _reloadStartTime >= ReloadTime)
            {
                _reloadStartTime = -1;
                IServerNetworking net = (IServerNetworking)this.Game.Services.GetService(typeof(INetworkingService));
                net.sendEvent("Sound", "Reload", Owner.ID >> 25);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // TODO: actually draw the gun
            ////Load services
            //IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));
            //ICameraService cam = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));

            ////Render the RemotePlayer
            //r.DrawRenderable(this);
        }

        #endregion

        #region IHoldableItem Members

        public Player Owner
        {
            get;
            protected set;
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

        #region IEncodable Members

        public byte[] Encode()
        {
            Networking.Encoder tosend = new Networking.Encoder();

            tosend.AddElement("Magazine", Mag);
            tosend.AddElement("MagCount", MagCount);
            tosend.AddElement("Position", _position);
            tosend.AddElement("Orientation", _orientation);

            return tosend.Serialize();
        }

        public void Decode(byte[] serialized)
        {
            Networking.Encoder props = new Networking.Encoder(serialized);

            if (Mag == null)
                Mag = new Magazine(this.Game, MagazineCapacity);
            if (props.UpdatesFor("Magazine"))
                props.UpdateIEncodable("Magazine", Mag);
            if (props.UpdatesFor("MagCount"))
                MagCount = (int)props.GetElement("MagCount", MagCount);
            if (props.UpdatesFor("Position"))
                Position = (Vector3)props.GetElement("Position", Position);
            if (props.UpdatesFor("Orientation"))
                Orientation = (Quaternion)props.GetElement("Orientation", Orientation);
        }

        #endregion

    }
}
