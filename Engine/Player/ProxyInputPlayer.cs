using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Input;
using Mammoth.Engine.Networking;

namespace Mammoth.Engine
{
    class ProxyInputPlayer : InputPlayer
    {
        public ProxyInputPlayer(Game game, int clientID)
            : base(game)
        {
            this.ClientID = clientID;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // Get the input state.
            IInputService input = (IInputService) this.Game.Services.GetService(typeof(IInputService));
            input.SetStateByClientID(this.ClientID);

            // Update player using emulated input state.
            base.Update(gameTime);

            IServerNetworking network = (IServerNetworking)this.Game.Services.GetService(typeof(INetworkingService));
            //Console.WriteLine("sending proxy player");
            //Console.WriteLine(this.Position);
            network.sendThing(this);
        }

        protected override void Throw()
        {
            Vector3 direction = Vector3.Transform(Vector3.Forward, HeadOrient) * 1000.0f;
            direction.Normalize();
            Vector3 position = Position + (Vector3.Up * Height / 4.0f);
            Vector3 offset = Vector3.Multiply(direction, 2.0f);
            position = Vector3.Add(position, offset);

            // Make sure the bullet isn't spawned in the player: shift it by a bit
            Bullet b = new Bullet(Game, position, direction);

            // Give this projectile an ID, but it's not really necessary since it gets shot instantaneously
            IModelDBService modelDB = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            b.ID = modelDB.getNextOpenID();

            // Debug info
            Console.WriteLine("Position vec: " + position);
            Console.WriteLine("Throwing bullet with position: " + b.InitialPosition + ", direction: " + b.InitialDirection);
            Console.WriteLine("Player position: " + Position);

            // Send the bullet after it's created
            IServerNetworking net = (IServerNetworking)this.Game.Services.GetService(typeof(INetworkingService));
            net.sendThing(b);
        }

        #region Properties

        public int ClientID
        {
            get;
            protected set;
        }

        #endregion
    }
}
