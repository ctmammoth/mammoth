using System;
using StillDesign.PhysX;
using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    public abstract class PhysicalObject : BaseObject
    {
        # region Properties
        public Actor Actor
        {
            get;
            protected set;
        }

        public Vector3 Position
        {
            get
            {
                return Actor.GlobalPosition;
            }

            protected set
            {
                Actor.GlobalPosition = value;
            }
        }

        public Quaternion Orientation
        {
            get
            {
                return Actor.GlobalOrientationQuat;
            }

            protected set
            {
                Actor.GlobalOrientationQuat = value;
            }
        }
        # endregion

        public abstract void CollideWith(PhysicalObject obj);
    }
}
