using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

            // TODO: This should probably be moved into the networking code.
            // TODO: Decoder should register itself with the Game as providing the IDecoder service.
            // We only need to create a decoder if we're doing networking.
            Mammoth.Engine.Networking.Decoder d = new Mammoth.Engine.Networking.Decoder(this);
            this.Services.AddService(typeof(IDecoder), d);

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

            // Add the main menu as a screen for the screen manager.
            screenManager.AddScreen(new MainMenuScreen(this));

            // Initialize all of the components that we just added to the game.
            base.Initialize();
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clear the backbuffer - set it to a nice sky blue.
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw all of this game's drawable components.
            base.Draw(gameTime);
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
