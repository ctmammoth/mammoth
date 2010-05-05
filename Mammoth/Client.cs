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

            // Only do this stuff if we're drawing PhysX debug geometry.
        #if PHYSX_DEBUG
            Renderer r = (Renderer)this.Services.GetService(typeof(IRenderService));
            IPhysicsManagerService physics = (IPhysicsManagerService) this.Services.GetService(typeof(IPhysicsManagerService));
            r.DrawPhysXDebug(physics.Scene);
        #endif

            // Draw all of this game's drawable components.
            base.Draw(gameTime);
        }
    }
}
