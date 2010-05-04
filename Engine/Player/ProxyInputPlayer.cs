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

        protected override Bullet Throw()
        {
            Bullet bullet = base.Throw();
            IServerNetworking net = (IServerNetworking)this.Game.Services.GetService(typeof(INetworkingService));
            net.sendToAllBut(bullet, ClientID);
            return bullet;
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
