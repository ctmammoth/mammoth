using System;

using StillDesign.PhysX;

using Mammoth.Engine.Physics;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    public abstract class PhysicalObject : BaseObject
    {
        public PhysicalObject(Game game)
            : base(game)
        { }
        
        /// <summary>
        /// Supplies an Actor for use by PhysX. Allows interactions with other objects such as collisions.
        /// </summary>
        public Actor Actor
        {
            get;
            protected set;
        }


        /// <summary>
        /// Defines the position of the object by the position of the PhysX Actor.  Note: this should not be called
        /// on PhysicalObjects with static Actors!
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
        /// Defines the orientation of the object by the orientation of the object according to PhysX.  Note: this 
        /// should not be called on PhysicalObjects with static Actors!
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

        /// <summary>
        /// Provides a way for objects to react to triggers.  This method is called whenever an object triggers
        /// another object.
        /// </summary>
        /// <param name="obj">The object triggering or being triggered.</param>
        public virtual void RespondToTrigger(PhysicalObject obj) { }

        public override void Dispose()
        {
            base.Dispose();
            if (this.Actor != null)
            {
                IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));
                physics.RemoveActor(this.Actor);
            }
        }
    }
}
