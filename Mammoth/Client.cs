using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mammoth;
using Mammoth.Engine;
using Mammoth.Engine.Input;
using Mammoth.Engine.Physics;
using Mammoth.Engine.Interface;
using Mammoth.Engine.Networking;

namespace Mammoth
{
    class Client : Game
    {
        #region Fields

        GraphicsDeviceManager graphics;

        #endregion

        public Client()
            : base()
        {
            // Create a graphics device manager to deal with graphics devices.
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.PreferredDepthStencilFormat = DepthFormat.Depth32;
            //graphics.IsFullScreen = true;
            //graphics.PreferredBackBufferWidth = 1024;
            //graphics.PreferredBackBufferHeight = 768;

            // Set the root directory from which to load game content files.
            Content.RootDirectory = "Content";

            // Register the exiting handler with the Exiting event.
            this.Exiting += new EventHandler(Client_Exiting);
        }

        protected override void Initialize()
        {
            // Create the renderer, and register it as a service.
            Renderer r = new Renderer(this);
            this.Services.AddService(typeof(IRenderService), r);

            // Next, initialize the PhysX subsystem and create the physics manager.
            PhysicsManagerService physics = new PhysicsManagerService(this);
            this.Components.Add(physics);

            // Add the input handler.
            this.Components.Add(new LocalInput(this)
            {
                UpdateOrder = 1
            });

            // We also add the screen manager to initialize and handle the entire interface.  Phew.
            TScreenManager screenManager = new TScreenManager(this)
            {
                UpdateOrder = 2,
                DrawOrder = 1
            };
            this.Components.Add(screenManager);

            // Create the networking component, and have it update after all of the rest of the code.
            //DummyClientNetworking net = new DummyClientNetworking(this)
            LidgrenClientNetworking net = new LidgrenClientNetworking(this)
            {
                UpdateOrder = 3
            };
            this.Components.Add(net);

            // Add the main menu as a screen for the screen manager.
            //screenManager.AddScreen(new MainMenuScreen(this));
            screenManager.AddScreen(new PrettyMenuScreen(this));

            // Add the audio system
            this.Components.Add(new Mammoth.Engine.Audio.Audio(this));

            // Initialize all of the components that we just added to the game.
            base.Initialize();
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clear the backbuffer - set it to a nice sky blue.
            this.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);

            // Draw all of this game's drawable components.
            base.Draw(gameTime);
        }

        #region Event Handlers

        /// <summary>
        /// Handle the Exiting event thrown before the game exits.
        /// </summary>
        void Client_Exiting(object sender, EventArgs e)
        {
            this.Dispose();
        }

        #endregion
    }
}
