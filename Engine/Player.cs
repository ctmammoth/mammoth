using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;

using Mammoth.Engine.Networking;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth.Engine
{
    public abstract class Player : PhysicalObject, IRenderable, IEncodable
    {
        public Player(Game game)
        {
            this.Game = game;

            //Declare ID number
            IModelDBService mdb = (IModelDBService) this.Game.Services.GetService(typeof(IModelDBService));
            ID = mdb.getNextOpenID();
        }

        public virtual void Spawn(Vector3 pos, Quaternion orient)
        {
            this.Position = pos;
            this.Orientation = orient;
            this.HeadOrient = orient;
        }

        #region IEncodable Members

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

        #endregion

        #region Properties

        public Vector3 Position
        {
            get
            {
                return this.Controller.Position;
            }
            protected set
            {
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

        public Game Game
        {
            get;
            protected set;
        }

        public float Height
        {
            get;
            protected set;
        }

        // Not sure what the permissions of this should be.
        internal Controller Controller
        {
            get;
            set;
        }

        public int ID
        {
            get;
            protected set;
        }

        #endregion
    }
}
