using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Physics;

namespace Mammoth.Engine
{
    public class SimpleGun : BaseObject, IWeapon
    {
        #region Properties
        private int AmmoLeft;
        private int MaxAmmo;
        private Projectile Projectile;

        public Quaternion Orientation
        {
            get;
            protected set;
        }

        public Game Game
        {
            get;
            protected set;
        }
        #endregion

        public SimpleGun(Game game)
            : base(game)
        {
            
        }

        #region IWeapon Members

        public void Shoot(Vector3 position, Quaternion orientation)
        {
            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));

        }

        public void Reload()
        {

        }

        #endregion

        public override void InitializeDefault(int id)
        {
            AmmoLeft = 0;
            MaxAmmo = 0;
        }

        public override string getObjectType()
        {
            return "SimpleGun";
        }
    }
}
