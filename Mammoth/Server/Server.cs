using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Mammoth.Engine;
using Mammoth.Engine.Networking;

namespace Mammoth.Server
{
    class Server : Game
    {
        public Server()
        {
            Mammoth.Engine.Networking.Networking.CreateServerNetworking(this);
            IServerNetworking server = (IServerNetworking)this.Services.GetService(typeof(INetworkingService));
            Player player = new Player(this);
            server.sendThing(player, 42);

            while (true)
            {
                Vector3 pos = player.Position;
                pos.X += 0.001f;
                player.Position = pos;
                server.sendThing(player, 42);
            }
        }

    }
}
