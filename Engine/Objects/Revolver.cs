using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Mammoth.Engine.Objects;
using Mammoth.Engine.Input;

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
            get { return 3.0; }
        }

        protected override double ReloadTime
        {
            get { return 2500.0; }
        }

        protected override float Inaccuracy
        {
            get { return 0.015f; }
        }

        protected override int MagazineCapacity
        {
            get { return 10; }
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
            Bullet b = new RevolverBullet(game, position, orientation, shooterID);
            IModelDBService mdb = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            b.ID = mdb.getNextOpenID();
            mdb.registerObject(b);
            return b;
        }

        public override bool ShouldShoot(InputState input)
        {
            return input.KeyPressed(InputType.Fire);
        }
    }
}
