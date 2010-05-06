﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Mammoth.Engine;
using Mammoth.Engine.Physics;
using Mammoth.Engine.Interface;
using Mammoth.Engine.Networking;
using Mammoth.Engine.Graphics;

namespace Mammoth
{
    class GameScreen : TScreen
    {
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

            // Now we'll add some PhysX objects for testing purposes.  Later, this should contain
            // code for loading the game data from the network as well as possibly loading some
            // local data.  In fact, this should probably get moved into LoadContent eventually.
            #region PhysX Code

            // Because I don't trust the ground plane, I'm making my own.
            ActorDescription boxActorDesc = new ActorDescription();
            boxActorDesc.Shapes.Add(new BoxShapeDescription()
            {
                Size = new Vector3(100.0f, 2.0f, 100.0f),
                LocalPosition = new Vector3(0.0f, -1.0f, 0.0f)
            });
            new Mammoth.Engine.Objects.MinimalPhysicalObject(this.Game, physics.CreateActor(boxActorDesc));

            // Just to test collisions...
            boxActorDesc = new ActorDescription();
            boxActorDesc.Shapes.Add(new BoxShapeDescription()
            {
                Size = new Vector3(0.5f, 0.5f, 0.5f),
                LocalPosition = new Vector3(-3.0f, 3.0f, 0.0f)
            });
            new Mammoth.Engine.Objects.MinimalPhysicalObject(this.Game, physics.CreateActor(boxActorDesc));

            #endregion

            // Add the model database.
            ModelDatabase modelDB = new ModelDatabase(this.Game)
            {
                UpdateOrder = 1
            };
            this.Components.Add(modelDB);

            // TODO: Change this to use a server lobby.
            // Join a game.
            IClientNetworking net = (IClientNetworking)this.Game.Services.GetService(typeof(INetworkingService));
            net.joinGame();

            // Create the local player and add it to the ModelDB.
            this.LocalPlayer = new LocalInputPlayer(this.Game, net.ClientID);
            modelDB.registerObject(this.LocalPlayer);

            if (net is DummyNetworking)
                this.Game.Services.AddService(typeof(InputPlayer), this.LocalPlayer);

            // Create the camera.  We want it to update after all of the models (especially the player)
            // have been updated.  Set its target to be the local player.
            Camera cam = new FirstPersonCamera(this.Game, this.LocalPlayer)
            {
                UpdateOrder = 2
            };
            this.Components.Add(cam);

            // Now, we want to initialize all of the components we just added.
            foreach (GameComponent component in this.Components)
                component.Initialize();
        }

        public override void LoadContent()
        {
            this.Components.Add(new Skybox(this.Game)
            {
                DrawOrder = 3
            });
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
            // that draws the HUD for us.  Some widget code, I suppose.
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

            // TODO: Clear/delete/dispose of the model database.
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
