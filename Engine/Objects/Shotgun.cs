using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Mammoth.Engine.Objects;
using Mammoth.Engine.Input;
using Mammoth.Engine.Networking;

namespace Mammoth.Engine.Objects
{
    class Shotgun : Gun
    {
        public Shotgun(Game game, Player player)
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
            get { return 0.025f; }
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
            get { return "ShotgunShot"; }
        }

        public override string getObjectType()
        {
            return "Shotgun";
        }

        /// <summary>
        /// Spawns five bullets from this shotgun.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="shooterID"></param>
        protected virtual void SpawnBullet(Vector3 position, Quaternion orientation, int shooterID)
        {
            IServerNetworking net = (IServerNetworking)this.Game.Services.GetService(typeof(INetworkingService));
            net.sendEvent("Sound", FireSound);

            for (int i = 0; i < 5; i++)
            {
                Mag.FireShot();

                // Randomly perturb the bullet
                Quaternion perturbation =
                    Quaternion.CreateFromYawPitchRoll(((float)directionPerturber.NextDouble() - 0.5f) * Inaccuracy,
                                                      ((float)directionPerturber.NextDouble() - 0.5f) * Inaccuracy,
                                                      0.0f);

                Bullet b = createBullet(Game, position, orientation * perturbation, shooterID >> 25);

                // Send the bullet after it's created
                net.sendThing(b);

                Console.WriteLine("Shot a bullet with a " + getObjectType() + "; " + Mag.AmmoRemaining + " bullets left.");
            }
        }

        protected override Bullet createBullet(Game game, Vector3 position, Quaternion orientation, int shooterID)
        {
            Bullet b = new ShotgunBullet(game, position, orientation, shooterID);
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
