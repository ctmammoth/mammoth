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
            Vector3 forward = Vector3.Transform(Vector3.Forward, HeadOrient) * 1000.0f;
            forward.Normalize();
            Vector3 position = Position + (Vector3.Up * Height / 4.0f);
            position = Vector3.Add(position, forward);

            // Make sure the bullet isn't spawned in the player: shift it by a bit
            Bullet b = new Bullet(Game, position, forward);

            // Give this projectile an ID
            IModelDBService modelDB = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            b.ID = modelDB.getNextOpenID();
            modelDB.registerObject(b);
            Console.WriteLine("Position vec: " + position);
            Console.WriteLine("Throwing bullet with position: " + b.Position);
            Console.WriteLine("Player position: " + Position);
            IServerNetworking net = (IServerNetworking)this.Game.Services.GetService(typeof(INetworkingService));
            //Console.WriteLine("Sending bullet (in throw)");
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
