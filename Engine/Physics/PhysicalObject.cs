using System;
using StillDesign.PhysX;
using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    public abstract class PhysicalObject : BaseObject
    {
        public PhysicalObject(Game game)
            : base(game)
        {

        }

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
                if (Actor.IsDynamic)
                    Actor.GlobalPosition = value;
                else
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
                if (Actor.IsDynamic)
                    Actor.GlobalOrientationQuat = value;
                else
                    Actor.MoveGlobalOrientationTo(value);
            }
        }
        # endregion

        public virtual void CollideWith(PhysicalObject obj) { }


    }
}
