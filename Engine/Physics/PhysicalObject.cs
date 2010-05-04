using System;
using StillDesign.PhysX;
using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    public abstract class PhysicalObject : BaseObject
    {
        # region Properties
        public Actor actor
        {
            get;
            protected set;
        }

        public Vector3 position
        {
            get
            {
                return actor.GlobalPosition;
            }

            protected set
            {
                actor.GlobalPosition = value;
            }
        }

        public Quaternion orientation
        {
            get
            {
                return actor.GlobalOrientationQuat;
            }

            protected set
            {
                actor.GlobalOrientationQuat = value;
            }
        }
        # endregion

      


        public abstract void collideWith(PhysicalObject obj);
    }
}
