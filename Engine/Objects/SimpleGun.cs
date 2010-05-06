using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Physics;

namespace Mammoth.Engine
{
    public class SimpleGunClip : BaseObject
    {
        private const int MaxRounds = 15;
        public int AmmoRemaining
        {
            get;
            private set;
        }

        public SimpleGunClip(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Removes a bullet from the magazine if possible.
        /// </summary>
        /// <returns>The number of bullets remain in the magazine after the shot is fired, or zero if a bullet is
        /// shot when no rounds are remaining.</returns>
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
            return "Simple gun clip";
        }
    }

    public class SimpleGun : BaseObject, IWeapon
    {
        #region Properties

        public Quaternion Orientation
        {
            get;
            set;
        }

        public Vector3 Position
        {
            get;
            set;
        }

        #endregion

        private SimpleGunClip Magazine;

        public SimpleGun(Game game)
            : base(game)
        {
            Magazine = new SimpleGunClip(game);
        }

        #region IWeapon Members

        public void Shoot(Vector3 position, Vector3 direction)
        {

        }

        public int AmmoRemainingInClip()
        {
            return 0;
        }

        public void Reload()
        {

        }

        #endregion

        public void InitializeDefault(int id)
        {
        }

        public override string getObjectType()
        {
            return "SimpleGun";
        }
    }
}
