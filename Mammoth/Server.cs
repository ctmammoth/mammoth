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
using Mammoth.Engine.Objects;

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

            // Set up the emulated input.
            this.Components.Add(new EmulatedInput(this)
            {
                UpdateOrder = 1
            });

            NetworkComponent.CreateServerNetworking(this);

            //Add GameLogic
            GameLogic g = new GameLogic(this);
            this.Components.Add(g);
            this.Services.AddService(typeof(GameLogic), g);
            g.ResetServer += new EventHandler(g_ResetServer);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            SetupGame();
        }

        private void SetupGame()
        {
            IPhysicsManagerService phys = (IPhysicsManagerService)this.Services.GetService(typeof(IPhysicsManagerService));
            phys.CreateScene();

            // Add the model database.
            ModelDatabase modelDB = new ModelDatabase(this)
            {
                UpdateOrder = 3,
                Visible = false
            };
            this.Components.Add(modelDB);
            modelDB.registerObject(new Terrain(this));

            // LET'S TRY ADDING A ROOM!!!
            // TODO: find a better place for this?
            ObjectParameters stairRoom = new ObjectParameters();
            stairRoom.AddAttribute("X", "-50");
            stairRoom.AddAttribute("Y", "-23");
            stairRoom.AddAttribute("Z", "-50");
            stairRoom.AddAttribute("Special_Type", "STAIR_ROOM");
            Room room = new Room(modelDB.getNextOpenID(), stairRoom, this);

            ObjectParameters stairRoom2 = new ObjectParameters();
            stairRoom2.AddAttribute("X", "-193");
            stairRoom2.AddAttribute("Y", "-31");
            stairRoom2.AddAttribute("Z", "-118");
            stairRoom2.AddAttribute("Special_Type", "STAIR_ROOM");
            Room room2 = new Room(modelDB.getNextOpenID(), stairRoom2, this);

            IServerNetworking net = (IServerNetworking)this.Services.GetService(typeof(INetworkingService));

            // LET'S TRY ADDING A FLAG!!!
            Flag flag1 = new Flag(this, new Vector3(-45.0f, -23.0f, -45.0f), 1);
            flag1.ID = modelDB.getNextOpenID();
            modelDB.registerObject(flag1);
            // LET'S TRY ADDING A FLAG!!!
            Flag flag2 = new Flag(this, new Vector3(-65.0f, -23.0f, -45.0f), 2);
            flag2.ID = modelDB.getNextOpenID();
            modelDB.registerObject(flag2);
        }

        void g_ResetServer(object sender, EventArgs e)
        {
            TeardownGame();
        }

        private void TeardownGame()
        {
            IModelDBService modelDB = (IModelDBService)this.Services.GetService(typeof(IModelDBService));
            this.Components.Remove((IGameComponent)modelDB);
            modelDB.Dispose();

            IPhysicsManagerService phys = (IPhysicsManagerService)this.Services.GetService(typeof(IPhysicsManagerService));
            phys.RemoveScene();

            IServerNetworking net = (IServerNetworking)this.Services.GetService(typeof(INetworkingService));
            net.endGame();
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
