using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Mammoth.GameWidgets;
using Mammoth.Screens;
using Mammoth.Engine;
using Mammoth.Engine.Input;
using Mammoth.Engine.Physics;
using Mammoth.Engine.Interface;
using Mammoth.Engine.Networking;
using Mammoth.Engine.Graphics;
using Mammoth.Engine.Audio;
using Mammoth.Engine.Objects;

namespace Mammoth
{
    class GameScreen : TScreen
    {
        #region Fields

        TWidget baseWidget;

        #endregion

        public GameScreen(Game game)
            : base(game)
        {
            this.Components = new GameComponentCollection();

            this.Exiting += new EventHandler(GameScreen_Exiting);
        }

        public override void Initialize()
        {
            // This is where we create all of the services that are needed specifically by the GameScreen - 
            // this includes networking services, model databases, all of that fun stuff.
            // Further down the line, when we have server lobbies and the like, we should change over
            // from making the networking here to simply calling join game on it here.  (Maybe that would work already?)

            // Let's get the physics manager so we can set up PhysX.
            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));

            // Let's create a PhysX scene for the game.
            physics.CreateScene();

            // Add the model database.
            ModelDatabase modelDB = new ModelDatabase(this.Game)
            {
                UpdateOrder = 1,
                DrawOrder = 2
            };
            this.Components.Add(modelDB);

            // TODO: Change this to use a server lobby.
            // Join a game.
            IClientNetworking net = (IClientNetworking)this.Game.Services.GetService(typeof(INetworkingService));
            net.joinGame();
            net.EndGameEvent += new EventHandler(net_EndGameEvent);

            // Create the local player and add it to the ModelDB.
            this.LocalPlayer = new LocalInputPlayer(this.Game, net.ClientID);
            modelDB.registerObject(this.LocalPlayer);

            // HACK: change this.
            if (net is DummyNetworking)
                this.Game.Services.AddService(typeof(InputPlayer), this.LocalPlayer);

            // Clear the current input state.
            IInputService input = (IInputService)this.Game.Services.GetService(typeof(IInputService));
            input.InputHandled = true;

            // Create the camera.  We want it to update after all of the models (especially the player)
            // have been updated.  Set its target to be the local player.
            Camera cam = new FirstPersonCamera(this.Game, this.LocalPlayer)
            {
                UpdateOrder = 2
            };
            this.Components.Add(cam);

            // Play game music
            IAudioService audio = (IAudioService)Game.Services.GetService(typeof(IAudioService));
            audio.playMusic("In_Game");
            audio.loopSound("Ambient");

            // Now, we want to initialize all of the components we just added.
            foreach (GameComponent component in this.Components)
                component.Initialize();
        }

        public override void LoadContent()
        {
            this.Components.Add(new Skybox(this.Game)
            {
                DrawOrder = 1
            });

            IModelDBService modelDB = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            modelDB.registerObject(new Terrain(this.Game));

            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));

            //create baseWidget
            baseWidget = new TWidget(this.Game)
                {
                    Size = new Vector2(this.Game.Window.ClientBounds.Width / 2, this.Game.Window.ClientBounds.Height / 2),
                    Center = new Vector2(this.Game.Window.ClientBounds.Width / 2, this.Game.Window.ClientBounds.Height / 2)
                };

            // Add the crosshairs to the game.
            TImage cross = new TImage(this.Game, r.LoadTexture("cross"))
             {
                 Size = new Vector2(50, 50),
                 Center = new Vector2(this.Game.Window.ClientBounds.Width / 2, this.Game.Window.ClientBounds.Height / 2)
             };
            baseWidget.Add(cross);

            // Add the timer
            GameTimeWidget timer = new GameTimeWidget(this.Game)
            {
                Size = new Vector2(50, 50),
                Center = new Vector2(this.Game.Window.ClientBounds.Width / 2, 25)
            };
            baseWidget.Add(timer);

            // Add the gun
            GunWidget gunner = new GunWidget(this.Game, this.LocalPlayer)
            {
                Size = new Vector2(50, 50),
                Center = new Vector2(this.Game.Window.ClientBounds.Width - 100, this.Game.Window.ClientBounds.Height - 150)
            };
            baseWidget.Add(gunner);

            // Add the ammo counter
            AmmoWidget ammo = new AmmoWidget(this.Game, this.LocalPlayer)
            {
                Size = new Vector2(50, 50),
                Center = new Vector2(this.Game.Window.ClientBounds.Width - 100, this.Game.Window.ClientBounds.Height - 100)
            };
            baseWidget.Add(ammo);

            // Add the health counter
            HealthWidget health = new HealthWidget(this.Game, this.LocalPlayer)
            {
                Size = new Vector2(50, 50),
                Center = new Vector2(this.Game.Window.ClientBounds.Width - 100, this.Game.Window.ClientBounds.Height - 50)
            };
            baseWidget.Add(health);

            //Add FlagIndicator
            FlagIndicatorWidget f = new FlagIndicatorWidget(this.Game, this.LocalPlayer)
            {
                Size = new Vector2(75, 111),
                Center = new Vector2(50, this.Game.Window.ClientBounds.Height - 65)
            };
            baseWidget.Add(f);

            // LET'S TRY ADDING A ROOM!!!
            // HACK: bad
            ObjectParameters stairRoom = new ObjectParameters();
            stairRoom.AddAttribute("X", "-50");
            stairRoom.AddAttribute("Y", "-23");
            stairRoom.AddAttribute("Z", "-50");
            stairRoom.AddAttribute("Special_Type", "STAIR_ROOM");
            ObjectParameters stairRoom2 = new ObjectParameters();
            stairRoom2.AddAttribute("X", "193");
            stairRoom2.AddAttribute("Y", "-31");
            stairRoom2.AddAttribute("Z", "118");
            stairRoom2.AddAttribute("Special_Type", "STAIR_ROOM");
            Room room = new Room(modelDB.getNextOpenID(), stairRoom2, this.Game);
            Flag flag1 = new Flag(this.Game, new Vector3(-45.0f, -3.0f, -45.0f), 1);
            Flag flag2 = new Flag(this.Game, new Vector3(193.0f, -11.0f, 118.0f), 2);
        }

        public override void Update(GameTime gameTime, bool hasFocus, bool visible)
        {
            // If we press escape, kill off the game screen.  Later, we'll want to open up a popup menu.
            if (hasFocus && Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.IsExiting = true;

            // Update the screen (visibility and transition state and the like).
            base.Update(gameTime, hasFocus, visible);

            // Don't do any of the updating if we're exiting.
            if (!this.IsExiting)
            {
                // (Stuff to set up if we're handling input events - i.e. if the game window is on top.)
                if (hasFocus)
                {
                    // Make the mouse invisible - we only want it to appear if we're in a menu.
                    this.Game.IsMouseVisible = false;

                    // This is where we should reset the cursor's position to the middle of the screen.  We can also
                    // send the local input state to the server in this block (for now, though, this gets handled
                    // after all screens are run in the network component's update call).
                    // TODO: Call CenterCursor as soon as the game screen gets focus.
                    // If we don't, we'll end up sending very incorrect input states (especially mouse deltas) whenever
                    // we switch back "down" to the game screen from a popup.
                    CenterCursor();
                }

                // Update all of the game components that are part of the game (in the logical sense, not the XNA sense).
                // TODO: IMPORTANT: We need to make sure that input events are only handled if the game has focus.
                // I might have fixed the above already, not sure.  There might be a "block" on giving out input events
                // that is set properly by the screen manager - can't be sure though.

                // Get the list of game components in the component collection (as IUpdateables).
                var updateList = from c in this.Components
                                 where c is IUpdateable
                                 select c as IUpdateable;

                // Update them according to their UpdateOrder.  Yes, this doesn't need to sort every time.
                foreach (IUpdateable component in updateList.OrderBy((comp) => comp.UpdateOrder))
                    component.Update(gameTime);

                // Send the current input state to the server.
                IInputService input = (IInputService)this.Game.Services.GetService(typeof(IInputService));
                IClientNetworking net = (IClientNetworking)this.Game.Services.GetService(typeof(INetworkingService));
                net.sendThing(input.States.Peek());

                // TODO: Do this correctly?
                // Update the HUD.
                baseWidget.Update(gameTime);

                // Let's have PhysX update itself.
                // This might need to be changed/optimized a bit if things are getting slow because they have
                // to wait for the physics calculations.
                IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));
                physics.Simulate((float)gameTime.ElapsedGameTime.TotalSeconds);
                physics.FetchResults();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // Get the list of drawable game components in the component collection.
            var drawList = from c in this.Components
                             where c is IDrawable
                             select c as IDrawable;

            // Draw them according to their DrawOrder.  Yes, this doesn't need to sort every time.
            foreach (IDrawable component in drawList.OrderBy((comp) => comp.DrawOrder))
                component.Draw(gameTime);

            // Draw the PhysX debug geometry if we're doing PhysX debugging.
        #if PHYSX_DEBUG
            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));
            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));
            r.DrawPhysXDebug(physics.Scene);
        #endif

            // Maybe here is where we would draw HUD stuff?  Or maybe an explicit call to something
            // that draws the HUD for us.  Some widget code, I suppose.\

            // Draw the HUD
            baseWidget.Draw(gameTime);
        }

        /// <summary>
        /// This warps the cursor to the center of the game window.  This is important, as without it, the player's
        /// mouse would hit the side of the screen and they wouldn't be able to turn any further.  Also, using this
        /// method, the distance moved by the mouse in each update loop is just the distance from the center of the
        /// window to the mouse location.
        /// </summary>
        private void CenterCursor()
        {
            GameWindow window = this.Game.Window;

            Mouse.SetPosition(window.ClientBounds.Width / 2, window.ClientBounds.Height / 2);
        }

        #region Event Handlers

        /// <summary>
        /// An event handler for cleaning up the game screen before it exits.  This needs to delete all information
        /// related to this specific match (game screen instance), like objects, physics information, etc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void GameScreen_Exiting(object sender, EventArgs e)
        {
            // Disconnect from the server.
            IClientNetworking net = (IClientNetworking)this.Game.Services.GetService(typeof(INetworkingService));
            net.quitGame();

            // Remove the scene from the physics subsystem.
            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));
            physics.RemoveScene();

            IModelDBService mdb = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            mdb.Dispose();

            ICameraService cam = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));
            this.Game.Components.Remove((GameComponent)cam);
            this.Game.Services.RemoveService(typeof(ICameraService));

            // Play the menu music
            IAudioService audio = (IAudioService)Game.Services.GetService(typeof(IAudioService));
            audio.stopSound("Heartbeat");
            audio.stopSound("Ambient");
            audio.playMusic("Main_Menu");
        }

        void net_EndGameEvent(object sender, EventArgs e)
        {
            GameStats g = (GameStats)this.Game.Services.GetService(typeof(GameStats));
            this.ScreenManager.AddScreen(new EndScreen(this.Game, g));
            this.IsExiting = true;
        }

        #endregion

        #region Properties

        public GameComponentCollection Components
        {
            get;
            set;
        }

        public InputPlayer LocalPlayer
        {
            get;
            private set;
        }

        #endregion
    }
}
