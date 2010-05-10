using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine.Objects
{
    class SMGBullet : Bullet
    {
        public SMGBullet(Game game, Vector3 position, Quaternion orient, int creator)
            : base(game, position, orient, creator)
        {

        }

        #region BaseObject Properties

        public override string getObjectType()
        {
            return "SMGBullet";
        }
        
        #endregion

        #region Bullet Properties

        public override float Speed
        {
            get { return 100.0f; }
            protected set { }
        }

        public override float Damage
        {
            get { return 4.0f; }
            protected set { }
        }
        
        #endregion
    }
}
