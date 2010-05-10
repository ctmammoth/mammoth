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

            // Set the position to that of the owner
            if (this.Owner != null)
                this.Position = Owner.Position;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Renderer renderer = (Renderer)this.Game.Services.GetService(typeof(IRenderService));
            // Draw the flag
            renderer.DrawRenderable(this);
        }

        #region Properties

        public Vector3 PositionOffset
        {
            get
            {
                if (Owner == null)
                    return Vector3.Zero;
                else
                    // Draw it above the player's head
                    return new Vector3(0.0f, Owner.Height + 4.0f, 0.0f);
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

        public Player Owner
        {
            get;
            set;
        }

        #endregion

        #region IEncodable Members

        public byte[] Encode()
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder();

            e.AddElement("Position", Position);
            e.AddElement("PositionOffset", PositionOffset);

            return e.Serialize();
        }

        public void Decode(byte[] serialized)
        {
            Mammoth.Engine.Networking.Encoder e = new Mammoth.Engine.Networking.Encoder(serialized);

            Console.WriteLine("Decoding a flag...");

            Position = (Vector3)e.GetElement("Position", Position);
            PositionOffset = (Vector3)e.GetElement("PositionOffset", PositionOffset);
        }

        #endregion
    }
}
