using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StillDesign.PhysX;

using Mammoth.Engine.Physics;
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

            // Give this a sphere shape trigger
            SphereShapeDescription trigShapeDesc = new SphereShapeDescription()
            {
                Radius = 7.0f,
                Flags = ShapeFlag.TriggerOnEnter,
                LocalPosition = Vector3.Zero
            };

            // Give this a sphere shape trigger
            SphereShapeDescription sDesc = new SphereShapeDescription()
            {
                Radius = 0.3f,
                LocalPosition = Vector3.Zero
            };

            ActorDescription aDesc = new ActorDescription()
            {
                Shapes = { sDesc, trigShapeDesc },
                BodyDescription = new BodyDescription()
                {
                    Mass = 1.0f,
                    MassSpaceInertia = Vector3.Zero,
                    BodyFlags = BodyFlag.Kinematic
                }
            };

            IPhysicsManagerService physics = (IPhysicsManagerService)game.Services.GetService(typeof(IPhysicsManagerService));

            // Create the flag's Actor
            this.Actor = physics.CreateActor(aDesc, this);

            // Set the position to wherever the flag should be constructed
            this.Position = initialPosition;

            // No owner, initially
            this.Owner = null;

            // Load a flag model
            // TODO: get a flag model or something
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
            this.PositionOffset = Vector3.Zero;
        }

        public override string getObjectType()
        {
            return "Flag";
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Console.WriteLine("Flag " + this.ID + " old pos: " + this.Position); // WHY

            if (this.Owner != null)
                this.Position = Owner.Position;

            //Console.WriteLine("Flag " + this.ID + " new pos: " + this.Position);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Renderer renderer = (Renderer)this.Game.Services.GetService(typeof(IRenderService));
            // Draw the flag
            renderer.DrawRenderable(this);
        }

        #region Properties

        private Vector3 posOffset;

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
                    posOffset = Vector3.Zero;
                else
                    posOffset = new Vector3(0.0f, Owner.Height + 4.0f, 0.0f);
            }
        }

        #endregion

        #region IEncodable Members

        public byte[] Encode()
        {
            Console.WriteLine("Encoding a flag.");

            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder();

            e.AddElement("ID", ID);
            e.AddElement("Position", Position);
            e.AddElement("PositionOffset", PositionOffset);

            return e.Serialize();
        }

        public void Decode(byte[] serialized)
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder(serialized);

            this.ID = (int)e.GetElement("ID", ID);
            this.Position = (Vector3)e.GetElement("Position", Position);
            this.PositionOffset = (Vector3)e.GetElement("PositionOffset", PositionOffset);

            Console.WriteLine("Decoding a flag with id: " + this.ID + " and pos: " + this.Position);
        }

        #endregion
    }
}
