using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using StillDesign.PhysX;
using Mammoth.Engine.Physics;

namespace Mammoth.Engine.Objects
{
    /// <summary>
    /// Represents a flag that can be picked up by a player.
    /// </summary>
    public class Flag : PhysicalObject, IHoldeableItem, IRenderable
    {
        public Flag(Game game, Vector3 initialPosition)
            : base(game)
        {
            // Give this a sphere shape trigger
            SphereShapeDescription sDesc = new SphereShapeDescription()
            {
                Radius = 10.0f,
                Flags = ShapeFlag.TriggerEnable
            };

            // Make a body: flags should be kinematic since they should get their positions from the player carrying them.
            BodyDescription bDesc = new BodyDescription()
            {
                Mass = 10.0f,
                BodyFlags = BodyFlag.Kinematic
            };

            ActorDescription aDesc = new ActorDescription()
            {
                Shapes = { sDesc },
                BodyDescription = bDesc
            };

            IPhysicsManagerService physics = (IPhysicsManagerService)game.Services.GetService(typeof(IPhysicsManagerService));

            // Create the flag's Actor
            this.Actor = physics.CreateActor(aDesc, this);

            // Set the position to whereever the flag should be constructed
            this.Position = initialPosition;

            // Load a flag model
            // TODO: get a flag model or something
            this.Model3D = r.LoadModel("ammocrate");

            IModelDBService modelDB = (IModelDBService)game.Services.GetService(typeof(IModelDBService));
            // Add the flag to the model database
            modelDB.registerObject(this);
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

            Renderer renderer = (Renderer)this.Game.Services.GetService(typeof(Renderer));

            // Draw the flag
            renderer.DrawRenderable(this);    
        }

        #region Properties

        public Vector3 Position
        {
            get;
            protected set;
        }

        public Vector3 PositionOffset
        {
            get
            {
                if (this.Owner == null)
                    // If someone owns this flag, put it above them
                    return new Vector3(0.0f, 10.0f, 0.0f);
                else
                    // Otherwise draw it wherever it is
                    return Vector3.Zero;
            }
        }

        public Quaternion Orientation
        {
            get;
        }

        public Model Model3D
        {
            get;
        }

        public Player Owner
        {
            get;
            protected set;
        }

        #endregion

        #region IHoldeableItem Members

        public void SetOwner(Player owner)
        {
            this.Owner = owner;
        }

        #endregion

    }
}
