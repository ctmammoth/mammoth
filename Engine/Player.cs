using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth.Engine
{
    public class Player : DrawableGameComponent, IRenderable, Networking.IEncodable
    {
        private int ID;
        public Player(Engine game) : base(game)
        {
            // TODO: Change this to use Adam's physics helper functions.
            // TODO: uncomment this (and add "Engine game" back to params)
            //Player.ControllerManager = game.Scene.CreateControllerManager();
            //Declare ID number
            IModelDBService mdb = (IModelDBService) this.Game.Services.GetService(typeof(IModelDBService));
            ID = mdb.getNextOpenID();
        }

        public int GetID()
        {
            return ID;
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

            Position = (Vector3) props.GetElement("Position");
            Orientation = (Quaternion) props.GetElement("Orientation");
            Velocity = (Vector3) props.GetElement("Velocity");
        }

        #region Properties

        public Vector3 Position
        {
            get
            {
                return this.Controller.Position;
            }
            set
            {
                // TODO: this should be protected
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
