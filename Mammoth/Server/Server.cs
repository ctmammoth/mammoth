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
            // Set the content directory
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // HACK: HACK.
            // Let's create/initialize the PhysX subsystem.
            PhysicsManagerService physics = new PhysicsManagerService(this);
            this.Components.Add(physics);

            // Give the server a renderer
            Renderer r = new Renderer(this);
            this.Services.AddService(typeof(IRenderService), r);
            // TODO: Change this to create a new scene when a game screen is created.
            physics.CreateScene();

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
            modelDB.registerObject(new Terrain(this));

            NetworkComponent.CreateServerNetworking(this);

            //Add GameLogic
            GameLogic g = new GameLogic();
            this.Services.AddService(typeof(IGameLogic), g);

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            // Now we update all of the GameComponents associated with the engine.
            base.Update(gameTime);

            // Let's have PhysX update itself.
            // This might need to be changed/optimized a bit if things are getting slow because they have
            // to wait for the physics calculations.
            IPhysicsManagerService physics = (IPhysicsManagerService)this.Services.GetService(typeof(IPhysicsManagerService));
            physics.Simulate((float)gameTime.ElapsedGameTime.TotalSeconds);
            physics.FetchResults();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Console.WriteLine("Disposing");
            IPhysicsManagerService physics = (IPhysicsManagerService)this.Services.GetService(typeof(IPhysicsManagerService));
            physics.Dispose();
            IServerNetworking net = (IServerNetworking)this.Services.GetService(typeof(INetworkingService));
            net.endGame();
        }
       
    }
}
