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
        #region Variables
        protected bool Dead;
        public int NumKills;
        public int NumKilled;
        public int NumCaptures;
        public Team Team;
        #endregion

        

        public Player(Game game)
        {
            this.Game = game;

        }

        public virtual void Spawn(Vector3 pos, Quaternion orient)
        {
            this.Position = pos;
            this.Orientation = orient;
            this.HeadOrient = orient;
        }

        public override void InitializeDefault(int id)
        {
            
        }

        #region IEncodable Members

        public virtual byte[] Encode()
        {
            Networking.Encoder tosend = new Networking.Encoder();

            tosend.AddElement("Position", Position);
            tosend.AddElement("Orientation", Orientation);
            tosend.AddElement("Velocity", Velocity);


            return tosend.Serialize();
        }

        public virtual void Decode(byte[] serialized)
        {
            Networking.Encoder props = new Networking.Encoder(serialized);

            if(props.UpdatesFor("Position"))
                Position = (Vector3) props.GetElement("Position", Position);
            if (props.UpdatesFor("Velocity"))
                Orientation = (Quaternion) props.GetElement("Orientation", Orientation);
            if (props.UpdatesFor("Orientation"))
                Velocity = (Vector3) props.GetElement("Velocity", Velocity);
        }

        #endregion

        #region Properties

        public override Vector3 Position
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

        public override Quaternion Orientation
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
        Vector3 _velocity;
        public Vector3 Velocity
        {
            get
            {
                return _velocity;
            }

            protected set
            {
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
