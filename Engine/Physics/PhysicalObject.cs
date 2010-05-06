using System;
using StillDesign.PhysX;
using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    public abstract class PhysicalObject : BaseObject
    {
        /// <summary>
        /// Supplies an Actor for use by PhysX. Allows interactions with other objects such as collisions.
        /// </summary>
        public Actor Actor
        {
            get;
            protected set;
        }


        /// <summary>
        /// Defines the position of the object by the position of the object according to PhysX.
        /// </summary>
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

        /// <summary>
        /// Defines the orientation of the object by the orientation of the object according to PhysX.
        /// </summary>
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

        /// <summary>
        /// Provides the reaction for a collision with the passed in object.
        /// </summary>
        /// <param name="obj">The object with which the collision is occuring.</param>
        public virtual void CollideWith(PhysicalObject obj) { }

    }
}
