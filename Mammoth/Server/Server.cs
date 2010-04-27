using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Server
{
    class Server : Game
    {
        public Server()
        {
            Mammoth.Engine.Networking.Networking.CreateServerNetworking(this);
        }

    }
}
