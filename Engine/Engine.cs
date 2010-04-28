#define PHYSX_DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;

using Mammoth.Engine.Input;
using Mammoth.Engine.Interface;

namespace Mammoth.Engine
{
    public class Engine : Microsoft.Xna.Framework.Game
    {
        #region Fields

        private Texture2D _text;

        #endregion

        public Engine()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        #region XNA-Game

        GraphicsDeviceManager graphics;

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add more initialization logic here.

            // TODO: Design and implement the PhysX interactions.
            // Let's create the PhysX stuff here.  This needs to be changed though.
            #region PhysX Code
            this.Core = new Core(new CoreDescription(), new ConsoleOutputStream());

        #if PHYSX_DEBUG
            this.Core.SetParameter(PhysicsParameter.VisualizationScale, 2.0f);
            this.Core.SetParameter(PhysicsParameter.VisualizeCollisionShapes, true);
        #endif

            SimulationType hworsw = (this.Core.HardwareVersion == HardwareVersion.None ? SimulationType.Software : SimulationType.Hardware);
            Console.WriteLine("PhysX Acceleration Type: " + hworsw);

            this.Scene = this.Core.CreateScene(new SceneDescription()
            {
                GroundPlaneEnabled = false,
                Gravity = new Vector3(0.0f, -9.81f, 0.0f) / 9.81f,
                SimulationType = hworsw,
                // Use variable timesteps for the simulation to make sure that it's syncing with the refresh rate.
                // (We might want to change this later.)
                TimestepMethod = TimestepMethod.Variable
            });
            
            // Because I don't trust the ground plane, I'm making my own.
            ActorDescription boxActorDesc = new ActorDescription();
            boxActorDesc.Shapes.Add(new BoxShapeDescription()
            {
                Size = new Vector3(100.0f, 2.0f, 100.0f),
                LocalPosition = new Vector3(0.0f, -1.0f, 0.0f)
            });
            this.Scene.CreateActor(boxActorDesc);

            // Just to test collisions...
            boxActorDesc = new ActorDescription();
            boxActorDesc.Shapes.Add(new BoxShapeDescription()
            {
                Size = new Vector3(2.0f, 2.0f, 2.0f),
                LocalPosition = new Vector3(-3.0f, 3.0f, 0.0f)
            });
            this.Scene.CreateActor(boxActorDesc);

        #if PHYSX_DEBUG
            this.Core.Foundation.RemoteDebugger.Connect("localhost");
        #endif

            #endregion

            // Create the renderer, and register it as a service.
            Renderer r = new Renderer(this);
            this.Services.AddService(typeof(IRenderService), r);

            this.Components.Add(new LocalInput(this));
            this.Components.Add(new ModelDatabase(this));

            // Create the local player, and have it update after all important components.
            this.LocalPlayer = new LocalPlayer(this);
            this.LocalPlayer.UpdateOrder = 2;
            this.Components.Add(this.LocalPlayer);

            // Create the camera next, and have it update after the player.
            Camera cam = new FirstPersonCamera(this, this.LocalPlayer)
            {
                UpdateOrder = 3
            };
            this.Components.Add(cam);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here

            // Let's create a soldier so we can see something.
            this.Components.Add(new SoldierObject(this));

            Renderer r = (Renderer)this.Services.GetService(typeof(IRenderService));
            SpriteFont calibri = r.LoadFont("calibri");
            _text = r.RenderFont("ABC", Vector2.Zero, Color.Maroon, Color.TransparentBlack);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

            // This shouldn't be in here, but let's see if this prevents it from crashing.
            this.Scene.Dispose();
            this.Core.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here

            // Now we update all of the GameComponents associated with the engine.
            base.Update(gameTime);

            // Let's have PhysX update itself.
            // This might need to be changed/optimized a bit if things are getting slow because they have
            // to wait for the physics calculations.
            this.Scene.Simulate((float)gameTime.ElapsedGameTime.TotalSeconds);
            this.Scene.FlushStream();
            this.Scene.FetchResults(SimulationStatus.RigidBodyFinished, true);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here.
            Renderer r = (Renderer)this.Services.GetService(typeof(IRenderService));

        #if PHYSX_DEBUG
            r.DrawPhysXDebug(this.Scene);
        #endif

            // Draw all of the objects in the scene.
            base.Draw(gameTime);

            // Test drawing a rectangle.
            //r.DrawFilledRectangle(new Rectangle(100, 100, 50, 50), Color.DarkMagenta);
            // Test drawing some text.
            //r.DrawTexturedRectangle(new Rectangle(110, 110, _text.Width, _text.Height), _text);
        }

        #endregion

        #region Properties

        public Core Core
        {
            get;
            private set;
        }

        public Scene Scene
        {
            get;
            private set;
        }

        public LocalPlayer LocalPlayer
        {
            get;
            private set;
        }

        #endregion
    }
}
