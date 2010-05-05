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

        public virtual Vector3 Position
        {
            get
            {
                return Actor.GlobalPosition;
            }

            protected set
            {
                Actor.MoveGlobalPositionTo(value);
            }
        }

        public virtual Quaternion Orientation
        {
            get
            {
                return Actor.GlobalOrientationQuat;
            }

            protected set
            {
                Actor.MoveGlobalOrientationTo(value);
            }
        }
        # endregion

        public virtual void CollideWith(PhysicalObject obj) { }
    }
}
