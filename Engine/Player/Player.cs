using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;

using Mammoth.Engine.Networking;
using Mammoth.Engine.Physics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth.Engine
{
    /// <summary>
    /// Abstraction of player. Defines some physical aspects all players have as well as providing code for Spawning.
    /// A player can not be instantiated.
    /// </summary>
    public abstract class Player : PhysicalObject, IRenderable, IEncodable, IDamageable
    {
        /// <summary>
        /// Constructs a new player allowing access to the Game.
        /// </summary>
        /// <param name="game">The current Game.</param>
        public Player(Game game)
            : base(game)
        {
            //Give players access to game.
            this.Game = game;

            //Defines model height which is used by the camera
            this.Height = 6.0f;

        }

        /// <summary>
        /// Spawns the player with a given position and orientation. Also changes dead to false and restores full health.
        /// </summary>
        /// <param name="pos">The position of the spawn</param>
        /// <param name="orient">The initial orientation of the spawn.</param>
        public virtual void Spawn(Vector3 pos, Quaternion orient)
        {
            //Spawn position
            this.Position = pos;
            this.Orientation = orient;
            this.HeadOrient = orient;

            //Revive life
            this.Health = 100;
        }

        #region IDamageable Members

        public float Health
        {
            get;
            set;
        }

        public virtual void TakeDamage(float damage, IDamager inflicter)
        {
            this.Health -= damage;
        }

        public virtual void Die()
        {
            this.NumDeaths++;
        }

        #endregion
        
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
                Velocity = (Vector3) props.GetElement("Velocity", Velocity);
            if (props.UpdatesFor("Orientation"))
                Orientation = (Quaternion)props.GetElement("Orientation", Orientation);
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();

            IPhysicsManagerService phys = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));
            phys.RemoveController(this.Controller);
        }

        #region Properties

        //For PhysicalObject
        public override string getObjectType()
        {
            return "Player";
        }

        //POSITION
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

        //POSITION OFFSET
        public Vector3 PositionOffset
        {
            get;
            protected set;
        }

        //ORIENTATION
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

        //HEAD ORIENT
        public Quaternion HeadOrient
        {
            get;
            protected set;
        }

        //VELOCITY
        public Vector3 Velocity
        {
            get;
            set;
        }

        //MODEL
        public Model Model3D
        {
            get;
            protected set;
        }

        //HEIGHT
        public float Height
        {
            get;
            protected set;
        }

        //CONTROLLER
        //Not sure what the permissions of this should be.
        internal Controller Controller
        {
            get;
            set;
        }

        //-----------------------STATISTICAL VARIABLES-----------------------//

        //NUM KILLS
        protected int NumKills
        {
            get;
            set;
        }

        //NUM DEATHS
        protected int NumDeaths
        {
            get;
            set;
        }

        //NUM CAPTURES
        protected int NumCaptures
        {
            get;
            set;
        }

        #endregion
    }
}
