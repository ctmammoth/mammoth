using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StillDesign.PhysX;

using Mammoth.Engine.Physics;
using Mammoth.Engine.Graphics;
using Mammoth.Engine.Networking;

namespace Mammoth.Engine.Objects
{
    /// <summary>
    /// Represents a flag that can be picked up by a player.
    /// </summary>
    public class Flag : PhysicalObject, IHoldableItem, IRenderable, IEncodable
    {
        public Flag(Game game, Vector3 initialPosition, int team)
            : base(game)
        {
            // Set this flag's team
            this.Team = team;

            // Give this a capsule shape trigger
            SphereShapeDescription trigShapeDesc = new SphereShapeDescription()
            {
                Radius = 0.5f,
                LocalPosition = Vector3.Zero,
                Flags = ShapeFlag.DisableCollision
            };

            // Placate PhysX by giving it a real shape
            BoxShapeDescription cDesc = new BoxShapeDescription()
            {
                Size = new Vector3(3.0f, 4.0f, 3.0f),
                LocalPosition = Vector3.Zero,
                Flags = ShapeFlag.TriggerOnEnter
            };

            ActorDescription aDesc = new ActorDescription()
            {
                Shapes = { cDesc, trigShapeDesc },
                BodyDescription = new BodyDescription()
                {
                    Mass = 1.0f,
                    // This tensor shall placate PhysX
                    MassSpaceInertia = Vector3.Zero,
                    BodyFlags = BodyFlag.Kinematic
                }
            };

            // Create this Flag's Actor
            IPhysicsManagerService physics = (IPhysicsManagerService)game.Services.GetService(typeof(IPhysicsManagerService));
            this.Actor = physics.CreateActor(aDesc, this);

            // Set the position to wherever the flag should be constructed
            this.Position = initialPosition;

            // The Flag initially has no owner
            this.Owner = null;

            // Load a flag model
            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));
            this.Model3D = r.LoadModel("banner01");
        }

        /// <summary>
        /// Causes the flag to be dropped by the owner at its last position.
        /// </summary>
        public void GetDropped()
        {
            this.Owner = null;
            // Draw the flag on the ground

            //HACK: hardcoded locations
            switch (this.Team)
            {
                case 1:
                    this.Position = new Vector3(-45.0f, -3.0f, -45.0f);
                    break;
                case 2:
                    this.Position = new Vector3(199.0f, -11.0f, 121.0f);
                    break;
            }

            this.PositionOffset = Vector3.Zero;
        }

        public override string getObjectType()
        {
            return "Flag";
        }

        /// <summary>
        /// This flag's update function.  Only performs any actual updates if the flag has an owner.
        /// </summary>
        /// <param name="gameTime">The game time at which this update is occurring.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // If the flag has an owner, update the flag's position
            if (this.Owner != null)
            {
                this.Position = Owner.Position;
            }

            INetworkingService server = (INetworkingService)this.Game.Services.GetService(typeof(INetworkingService));
            if (server is IServerNetworking)
            {
                //Send over the network
                ((IServerNetworking)server).sendThing(this);
            }
        }

        /// <summary>
        /// The flag's draw method.  Draws the flag's 
        /// </summary>
        /// <param name="gameTime">The game time at which this update is occurring.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Renderer renderer = (Renderer)this.Game.Services.GetService(typeof(IRenderService));
            // Draw the flag
            renderer.DrawRenderable(this);
        }

        #region Properties

        private Vector3 posOffset;

        /// <summary>
        /// The offset of the model within this PhysicalObject.
        /// </summary>
        public Vector3 PositionOffset
        {
            get
            {
                return posOffset;
            }
            protected set
            {
                this.posOffset = value;
            }
        }

        /// <summary>
        /// The Flag's model.
        /// </summary>
        public Model Model3D
        {
            get;
            protected set;
        }

        /// <summary>
        /// The team that owns this flag.
        /// </summary>
        public int Team
        {
            get;
            protected set;
        }

        #endregion

        #region IHoldableItem Members

        private Player owner;

        /// <summary>
        /// This flag's owner.
        /// </summary>
        public Player Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;

                if (value == null)
                    // If there is now no owner, the model should be drawn on the ground.
                    posOffset = Vector3.Zero;
                else
                    // Otherwise draw the flag above the owner's head.
                    posOffset = new Vector3(0.0f, Owner.Height + 4.0f, 0.0f);
            }
        }

        #endregion

        #region IEncodable Members

        /// <summary>
        /// Encodes a flag's position, positionoffset and ID.
        /// </summary>
        /// <returns>A byte array containing the encoded flag.</returns>
        public byte[] Encode()
        {
            //Console.WriteLine("Encoding a flag.  Position = " + Position + ", PositionOffset = " + PositionOffset +
             //   ", ID = " + ID);

            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder();

            e.AddElement("Position", Position);
            e.AddElement("PositionOffset", PositionOffset);
            e.AddElement("ID", ID);

            return e.Serialize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialized"></param>
        public void Decode(byte[] serialized)
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder(serialized);

            this.Position = (Vector3)e.GetElement("Position", Position);
            this.PositionOffset = (Vector3)e.GetElement("PositionOffset", PositionOffset);
            this.ID = (int)e.GetElement("ID", ID);

            // Console.WriteLine("Decoding a flag with id: " + this.ID + " and pos: " + this.Position);
        }

        #endregion
    }
}
