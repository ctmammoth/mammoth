using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth.Engine
{
    public abstract class Player : DrawableGameComponent, IRenderable
    {
        public Player(Game game) : base(game)
        {
            Player.ControllerManager = Engine.Instance.Scene.CreateControllerManager();
        }

        #region Properties

        public Vector3 Position
        {
            get
            {
                return this.Controller.Position;
            }
            protected set
            {
                //this.Controller.Actor.MoveGlobalPositionTo(value);
                this.Controller.Position = value;
            }
        }

        public Vector3 PositionOffset
        {
            get;
            protected set;
        }

        public Quaternion Orientation
        {
            get
            {
                return this.Controller.Actor.GlobalOrientationQuat;
            }
            protected set
            {
                this.Controller.Actor.MoveGlobalOrientationTo(Matrix.CreateFromQuaternion(value));
            }
        }

        public Quaternion HeadOrient
        {
            get;
            protected set;
        }

        // This is the velocity of the player in the player's local coordinate system.
        public Vector3 Velocity
        {
            get;
            protected set;
        }

        public Model Model3D
        {
            get;
            protected set;
        }

        public float Height
        {
            get;
            protected set;
        }

        static protected ControllerManager ControllerManager
        {
            get;
            private set;
        }

        // Not sure what the permissions of this should be.
        internal Controller Controller
        {
            get;
            set;
        }

        #endregion
    }
}
