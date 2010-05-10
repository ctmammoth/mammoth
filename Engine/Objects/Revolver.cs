using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Mammoth.Engine.Objects;

namespace Mammoth.Engine.Objects
{
    class Revolver : Gun
    {
        public Revolver(Game game, Player player)
            : base(game, player)
        {

        }

        protected override double FireRate
        {
            get { return 2.0; }
        }

        protected override double ReloadTime
        {
            get { return 3000.0; }
        }

        protected override float Inaccuracy
        {
            get { return 0.01f; }
        }

        protected override int MagazineCapacity
        {
            get { return 6; }
        }

        protected override int NumMagazines
        {
            get { return 50; }
        }

        protected override string FireSound
        {
            get { return "Gunshot"; }
        }

        public override string getObjectType()
        {
            return "Revolver";
        }

        protected override Bullet createBullet(Game game, Vector3 position, Quaternion orientation, int shooterID)
        {
            RevolverBullet b = new RevolverBullet(game, position, orientation, shooterID);
            IModelDBService mdb = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            b.ID = mdb.getNextOpenID();
            mdb.registerObject(b);
            return b;
        }
    }
}
