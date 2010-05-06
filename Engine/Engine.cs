﻿#define PHYSX_DEBUG

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
using Mammoth.Engine.Physics;
using Mammoth.Engine.Networking;

namespace Mammoth.Engine
{
    public class Engine : Microsoft.Xna.Framework.Game
    {
        #region Fields

        private Texture2D _text;
        private bool _usingNetworking;

        #endregion

        

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
                //LocalPosition = new Vector3(-0.0f, 0.0f, 0.0f)
            });
            physics.CreateActor(boxActorDesc);

            #endregion

            // Create the renderer, and register it as a service.
            Renderer r = new Renderer(this);
            this.Services.AddService(typeof(IRenderService), r);


            //add Decoder as a service
            Mammoth.Engine.Networking.Decoder d = new Mammoth.Engine.Networking.Decoder(this);
            this.Services.AddService(typeof(IDecoder), d);

            // Add the input handler.
            this.Components.Add(new LocalInput(this)
            {
                UpdateOrder = 1
            });

            // Add the networking service.
            //this.Components.Add(new LidgrenClientNetworking(this));
            if (_usingNetworking)
                Networking.NetworkComponent.CreateClientNetworking(this);
            else
                Networking.NetworkComponent.CreateDummyClient(this);

            // Add the model database.
            ModelDatabase modelDB = new ModelDatabase(this)
            {
                UpdateOrder = 3
            };
            this.Components.Add(modelDB);

            // TODO: Remove this, and create the local player when the game screen is initialized.
            // Create the local player, and add it to the model DB.
            this.LocalPlayer = new LocalInputPlayer(this);
            IClientNetworking net = (IClientNetworking)this.Services.GetService(typeof(INetworkingService));
            this.LocalPlayer.ID = net.ClientID << 25;
            modelDB.registerObject(this.LocalPlayer);

            // Create the camera next, and have it update after the player.
            Camera cam = new FirstPersonCamera(this, this.LocalPlayer)
            {
                UpdateOrder = 5
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

            IModelDBService modelDB = (IModelDBService)this.Services.GetService(typeof(IModelDBService));


            /*
            
            ObjectParameters parameters = new ObjectParameters();
            parameters.AddAttribute("X", "8");
            parameters.AddAttribute("Y", "0");
            parameters.AddAttribute("Z", "0");
            parameters.AddAttribute("Crate_Type", "SMALL");
            
            
            ObjectParameters parameters2 = new ObjectParameters();
            parameters2.AddAttribute("X", "8");
            parameters2.AddAttribute("Y", "3.6");
            parameters2.AddAttribute("Z", "0");
            parameters2.AddAttribute("Crate_Type", "SMALL");

            ObjectParameters parameters3 = new ObjectParameters();
            parameters3.AddAttribute("X", "8");
            parameters3.AddAttribute("Y", "7.2");
            parameters3.AddAttribute("Z", "0");
            parameters3.AddAttribute("Crate_Type", "SMALL");

            ObjectParameters parameters4 = new ObjectParameters();
            parameters4.AddAttribute("X", "11.6");
            parameters4.AddAttribute("Y", "0");
            parameters4.AddAttribute("Z", "0");
            parameters4.AddAttribute("Crate_Type", "SMALL");



            

            int crateId = modelDB.getNextOpenID();
            Crate crate1 = (Crate)ObjectFactories.CreateObject("Crate", crateId, parameters, this);
            modelDB.registerObject(crate1);
            Console.WriteLine(crate1.Position);
            
            
            crateId = modelDB.getNextOpenID();
            Crate crate2 = (Crate)ObjectFactories.CreateObject("Crate", crateId, parameters2, this);
            modelDB.registerObject(crate2);

            crateId = modelDB.getNextOpenID();
            Crate crate3 = (Crate)ObjectFactories.CreateObject("Crate", crateId, parameters3, this);
            modelDB.registerObject(crate3);

            crateId = modelDB.getNextOpenID();
            Crate crate4 = (Crate)ObjectFactories.CreateObject("Crate", crateId, parameters4, this);
            modelDB.registerObject(crate4); */



            ObjectParameters parameters = new ObjectParameters();
            /*
            for (int i = 0; i < 25; i++)
            {
                parameters = new ObjectParameters();
                Double number1 = (3.6 * (i / 5));
                Double number2 = (3.6 * (i % 5));


                parameters.AddAttribute("X", number1.ToString());
                parameters.AddAttribute("Y", number2.ToString());
                parameters.AddAttribute("Z", "0.0");
                parameters.AddAttribute("Crate_Type", "SMALL");


                if ((!(i > 9 && i < 12)))
                {
                    int crateId = modelDB.getNextOpenID();
                    Crate crate1 = (Crate)ObjectFactories.CreateObject("Crate", crateId, parameters, this);
                    modelDB.registerObject(crate1);
                }

            }

            for (int i = 0; i < 25; i++)
            {
                parameters = new ObjectParameters();
                Double number1 = (3.6 * (i / 5));
                Double number2 = (3.6 * (i % 5));


                parameters.AddAttribute("X", number1.ToString());
                parameters.AddAttribute("Y", number2.ToString());
                parameters.AddAttribute("Z", "14.4");
                parameters.AddAttribute("Crate_Type", "DARK");


                if ((!(i > 9 && i < 12)))
                {
                    int crateId = modelDB.getNextOpenID();
                    Crate crate1 = (Crate)ObjectFactories.CreateObject("Crate", crateId, parameters, this);
                    modelDB.registerObject(crate1);
                }

            }

            for (int i = 0; i < 25; i++)
            {
                parameters = new ObjectParameters();
                Double number1 = (3.6 * (i / 5));
                Double number2 = (3.6 * (i % 5));


                parameters.AddAttribute("Z", number1.ToString());
                parameters.AddAttribute("Y", number2.ToString());
                parameters.AddAttribute("X", "0.0");
                parameters.AddAttribute("Crate_Type", "SMALL");


                if ((!(i > 9 && i < 12)))
                {
                    int crateId = modelDB.getNextOpenID();
                    Crate crate1 = (Crate)ObjectFactories.CreateObject("Crate", crateId, parameters, this);
                    modelDB.registerObject(crate1);
                }

            }


            for (int i = 0; i < 25; i++)
            {
                parameters = new ObjectParameters();
                Double number1 = (3.6 * (i / 5));
                Double number2 = (3.6 * (i % 5));


                parameters.AddAttribute("Z", number1.ToString());
                parameters.AddAttribute("Y", number2.ToString());
                parameters.AddAttribute("X", "14.4");
                parameters.AddAttribute("Crate_Type", "SMALL");


                if ((!(i > 9 && i < 12)))
                {
                    int crateId = modelDB.getNextOpenID();
                    Crate crate1 = (Crate)ObjectFactories.CreateObject("Crate", crateId, parameters, this);
                    modelDB.registerObject(crate1);
                }

            }
             * 
             * 


             * 
            


            for (int i = 0; i < 25; i++)
            {
                parameters = new ObjectParameters();
                Double number1 = (3.6 * (i / 5));
                Double number2 = (3.6 * (i % 5));


                parameters.AddAttribute("X", number1.ToString());
                parameters.AddAttribute("Y", "18"); 
                parameters.AddAttribute("Z", number2.ToString());                
                parameters.AddAttribute("Crate_Type", "SMALL");

                int crateId = modelDB.getNextOpenID();
                Crate crate1 = (Crate)ObjectFactories.CreateObject("Crate", crateId, parameters, this);
                modelDB.registerObject(crate1);

                

            }
             * */

            Room room = new Room(631, this);
            room.BuildWalls(0, 0 , 0);
            room.BuildWalls(0, 21, 0);
            room.BuildWalls(0, 42, 0);
            room.BuildWalls(0, 63, 0);
            room.BuildWalls(0, 0, 24);
            room.BuildWalls(24, 0, 24);

            // ObjectFactories.content_test(this);

            //_text = r.RenderFont("ABC", Vector2.Zero, Color.Maroon, Color.TransparentBlack);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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
            {
                this.Dispose();
                this.Exit();
                return;
            }

            // TODO: Add your update logic here

            // Now we update all of the GameComponents associated with the engine.
            base.Update(gameTime);

            // Let's have PhysX update itself.
            // This might need to be changed/optimized a bit if things are getting slow because they have
            // to wait for the physics calculations.
            IPhysicsManagerService physics = (IPhysicsManagerService) this.Services.GetService(typeof(IPhysicsManagerService));
            physics.Simulate((float)gameTime.ElapsedGameTime.TotalSeconds);
            physics.FetchResults();
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
            IPhysicsManagerService physics = (IPhysicsManagerService) this.Services.GetService(typeof(IPhysicsManagerService));
            r.DrawPhysXDebug(physics.Scene);
        #endif

            // Draw all of the objects in the scene.
            base.Draw(gameTime);

            // Test drawing a rectangle.
            //r.DrawFilledRectangle(new Rectangle(100, 100, 50, 50), Color.DarkMagenta);
            // Test drawing some text.
            //r.DrawTexturedRectangle(new Rectangle(110, 110, _text.Width, _text.Height), _text);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Console.WriteLine("Disposing");
            IPhysicsManagerService physics = (IPhysicsManagerService)this.Services.GetService(typeof(IPhysicsManagerService));
            physics.Dispose();
            IClientNetworking net = (IClientNetworking)this.Services.GetService(typeof(INetworkingService));
            net.quitGame();
        }

        #endregion
        
        #region Properties

        public InputPlayer LocalPlayer
        {
            get;
            private set;
        }


        public Engine(bool useNetworking)
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            
            Content.RootDirectory = "Content";
            _usingNetworking = useNetworking;
            //Initialize();
        }

        #endregion
    }
}
