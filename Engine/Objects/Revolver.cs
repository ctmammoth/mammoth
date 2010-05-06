﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Mammoth.Engine.Objects;

namespace Mammoth.Engine.Objects
{
    class Revolver : SimpleGun
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
            get { return 3.0; }
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
            get { return 8; }
        }

        protected override string FireSound
        {
            get { return "Gunshot"; }
        }

        public override string getObjectType()
        {
            return "Revolver";
        }
    }
}
