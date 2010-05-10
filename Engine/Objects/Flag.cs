using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using StillDesign.PhysX;
using Mammoth.Engine.Physics;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth.Engine.Objects
{
    /// <summary>
    /// Represents a flag that can be picked up by a player.
    /// </summary>
    public class Flag : PhysicalObject, IHoldableItem, IRenderable
    {
        public Flag(Game game, Vector3 initialPosition)
            : base(game)
        {
            // Give this a sphere shape trigger
            SphereShapeDescription trigShapeDesc = new SphereShapeDescription()
            {
                Radius = 7.0f,
                Flags = ShapeFlag.TriggerEnable,
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

            // HACK HACK HACK
            IModelDBService mdb = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            this.ID = mdb.getNextOpenID();
            mdb.registerObject(this);
        }

        public void GetDropped(Vector3 position)
        {
            this.Position = position;
            this.Owner = null;
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
            if (Owner == null)
                renderer.DrawRenderable(this);
            else
                Console.WriteLine("I have an owner!");
        }

        #region Properties

        public Vector3 PositionOffset
        {
            get
            {
                return Vector3.Zero;
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

    }
}
