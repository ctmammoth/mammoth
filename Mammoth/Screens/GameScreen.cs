﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;

using Microsoft.Xna.Framework;

using Mammoth.Engine;
using Mammoth.Engine.Physics;
using Mammoth.Engine.Interface;
using Mammoth.Engine.Networking;

namespace Mammoth
{
    class GameScreen : TScreen
    {
        public GameScreen(Game game)
            : base(game)
        {

        }

        public override void Initialize()
        {
            // This is where we create all of the services that are needed specifically by the GameScreen - 
            // this includes networking services, model databases, all of that fun stuff.
            // Further down the line, when we have server lobbies and the like, we should change over
            // from making the networking here to simply calling join game on it here.  (Maybe that would work already?)

            // Create the client networking and connect to a server.
            Mammoth.Engine.Networking.NetworkComponent.CreateClientNetworking(this.Game);

            // Add the model database.
            ModelDatabase modelDB = new ModelDatabase(this.Game)
            {
                UpdateOrder = 1
            };
            this.Components.Add(modelDB);

            // Create the local player, set its client ID, and add it to the ModelDB.
            this.LocalPlayer = new LocalInputPlayer(this.Game);
            IClientNetworking net = (IClientNetworking)this.Game.Services.GetService(typeof(INetworkingService));
            // TODO: Uncomment this when I merge into the latest codebase.
            //this.LocalPlayer.ID = net.ClientID << 25;
            modelDB.registerObject(this.LocalPlayer);

            // Create the camera.  We want it to update after all of the models (especially the player)
            // have been updated.  Set its target to be the local player.
            Camera cam = new FirstPersonCamera(this.Game, this.LocalPlayer)
            {
                UpdateOrder = 2
            };
            this.Components.Add(cam);

            // Let's get the physics manager so we can set up PhysX.
            IPhysicsManagerService physics = (IPhysicsManagerService) this.Game.Services.GetService(typeof(IPhysicsManagerService));

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
            // TODO: Uncomment when we merge.
            //physics.CreateActor(boxActorDesc);

            // Just to test collisions...
            boxActorDesc = new ActorDescription();
            boxActorDesc.Shapes.Add(new BoxShapeDescription()
            {
                Size = new Vector3(0.5f, 0.5f, 0.5f),
                LocalPosition = new Vector3(-3.0f, 3.0f, 0.0f)
            });
            // TODO: Uncomment when we merge.
            //physics.CreateActor(boxActorDesc);

            #endregion

            // Now, we want to initialize all of the components we just added.
            foreach (GameComponent component in this.Components)
                component.Initialize();
        }

        public override void Update(GameTime gameTime, bool hasFocus, bool visible)
        {
            // Update the screen (visibility and transition state and the like).
            base.Update(gameTime, hasFocus, visible);

            // (Stuff to set up if we're handling input events - i.e. if the game window is on top.)
            if (hasFocus)
            {
                // If we press escape, kill off the game screen.  Later, we'll want to open up a popup menu.
                this.IsExiting = true;

                // Make the mouse invisible - we only want it to appear if we're in a menu.
                this.Game.IsMouseVisible = false;

                // This is where we should reset the cursor's position to the middle of the screen.  We can also
                // send the local input state to the server in this block.
            }

            // Update all of the game components that are part of the game (in the logical sense, not the XNA sense).
            // TODO: IMPORTANT: We need to make sure that input events are only handled if the game has focus.
            // I might have fixed the above already, not sure.  There might be a "block" on giving out input events
            // that is set properly by the screen manager - can't be sure though.
            foreach (GameComponent component in this.Components)
                component.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (DrawableGameComponent component in this.Components)
                component.Draw(gameTime);

            // Maybe here is where we would draw HUD stuff?  Or maybe an explicit call to something
            // that draws the HUD for us.  Some widget code, I suppose.
        }

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
