using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using StillDesign.PhysX;

using Mammoth.Engine;
using Mammoth.Engine.Input;
using Mammoth.Engine.Networking;
using Mammoth.Engine.Physics;

namespace Mammoth.Server
{
    public class Server : Game
    {
        public Server()
        {
            new GraphicsDeviceManager(this);
        }

        protected override void Initialize()
        {
            // HACK: HACK.
            // Let's create/initialize the PhysX subsystem.
            PhysicsManagerService physics = new PhysicsManagerService(this);
            this.Components.Add(physics);
            // TODO: Change this to create a new scene when a game screen is created.
            physics.CreateScene();
            #region PhysX Code

            // Because I don't trust the ground plane, I'm making my own.
            ActorDescription boxActorDesc = new ActorDescription();
            boxActorDesc.Shapes.Add(new BoxShapeDescription()
            {
                Size = new Vector3(100.0f, 2.0f, 100.0f),
                LocalPosition = new Vector3(0.0f, -1.0f, 0.0f)
            });
            physics.CreateActor(boxActorDesc);

            // Just to test collisions...
            boxActorDesc = new ActorDescription();
            boxActorDesc.Shapes.Add(new BoxShapeDescription()
            {
                Size = new Vector3(0.5f, 0.5f, 0.5f),
                LocalPosition = new Vector3(-3.0f, 3.0f, 0.0f)
            });
            physics.CreateActor(boxActorDesc);

            #endregion

            // Set up the emulated input.
            this.Components.Add(new EmulatedInput(this)
            {
                UpdateOrder = 1
            });

            // Add the model database.
            ModelDatabase modelDB = new ModelDatabase(this)
            {
                UpdateOrder = 3,
                Visible = false
            };
            this.Components.Add(modelDB);

            NetworkComponent.CreateServerNetworking(this);
            this.Services.AddService(typeof(IDecoder), new Mammoth.Engine.Networking.Decoder(this));

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Now we update all of the GameComponents associated with the engine.
            base.Update(gameTime);

            // Let's have PhysX update itself.
            // This might need to be changed/optimized a bit if things are getting slow because they have
            // to wait for the physics calculations.
            IPhysicsManagerService physics = (IPhysicsManagerService)this.Services.GetService(typeof(IPhysicsManagerService));
            physics.Simulate((float)gameTime.ElapsedGameTime.TotalSeconds);
            physics.FetchResults();
        }
    }
}
