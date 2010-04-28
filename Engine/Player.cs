using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth.Engine
{
    public abstract class Player : DrawableGameComponent, IRenderable, Networking.IEncodable
    {
        public Player(Engine game) : base(game)

        {
            // TODO: Change this to use Adam's physics helper functions.
            Player.ControllerManager = game.Scene.CreateControllerManager();
        }

        public byte[] Encode()
        {
            Networking.Encoder tosend = new Networking.Encoder();

            tosend.AddElement("Position", Position);
            tosend.AddElement("Orientation", Orientation);
            tosend.AddElement("Velocity", Velocity);

            return tosend.Serialize();
        }

        public void Decode(byte[] serialized)
        {
            Networking.Encoder props = new Networking.Encoder(serialized);

            Position = props.GetElement("Position");
            Orientation = props.GetElement("Orientation");
            Velocity = props.GetElement("Velocity");
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
