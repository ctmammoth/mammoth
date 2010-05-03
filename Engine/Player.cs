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
        [Flags]
        public enum EncodableProperties
        {
            None = 0x00,
            Position = 0x01,
            Orientation = 0x02,
            Velocity = 0x04
        }

        EncodableProperties dirty;
        long counter = 0;

        public Player(Game game)
        {
            this.Game = game;
            dirty = EncodableProperties.None;

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

            //if ((dirty & EncodableProperties.Position) == dirty)
            //{
                //Console.WriteLine("Sending updated position, " + counter++ + "; ");
                //Console.Write(Position.ToString());
                tosend.AddElement("Position", Position);
            //}
            //if((dirty & EncodableProperties.Orientation) == dirty)
                tosend.AddElement("Orientation", Orientation);
            //if ((dirty & EncodableProperties.Velocity) == dirty)
                tosend.AddElement("Velocity", Velocity);

            tosend.AddElement("ID", ID);

            //reset DIRTY
            dirty = EncodableProperties.None;

            return tosend.Serialize();
        }

        public void Decode(byte[] serialized)
        {
            Networking.Encoder props = new Networking.Encoder(serialized);

            Position = (Vector3) props.GetElement("Position", Position);
            Orientation = (Quaternion) props.GetElement("Orientation", Orientation);
            Velocity = (Vector3) props.GetElement("Velocity", Velocity);
            ID = (int) props.GetElement("ID", ID);
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
                dirty |= EncodableProperties.Position;
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
                dirty |= EncodableProperties.Orientation;
            }
        }

        public Quaternion HeadOrient
        {
            get;
            protected set;
        }

        // This is the velocity of the player in the player's local coordinate system.
        Vector3 _velocity;
        public Vector3 Velocity
        {
            get
            {
                return _velocity;
            }

            protected set
            {
                dirty |= EncodableProperties.Velocity;
                _velocity = value;
            }
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

        #endregion
    }
}
