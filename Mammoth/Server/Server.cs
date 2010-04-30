using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Mammoth.Engine;
using Mammoth.Engine.Networking;

namespace Mammoth.Server
{
    public class Server : Game
    {
        public Server()
        {
            NetworkComponent.CreateServerNetworking(this);
            this.Services.AddService(typeof(IDecoder), new Mammoth.Engine.Networking.Decoder(this));

        }

    }
}
