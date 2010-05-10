using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine.Objects
{
    class RevolverBullet : Bullet
    {
        public RevolverBullet(Game game, Vector3 position, Quaternion orient, int creator)
            : base(game, position, orient, creator)
        {

        }

        public override string getObjectType()
        {
            return "RevolverBullet";
        }

        public override float Speed
        {
            get { return 150.0f; }
            protected set { }
        }

        public override float Damage
        {
            get { return 14.0f; }
            protected set { }
        }
    }
}
