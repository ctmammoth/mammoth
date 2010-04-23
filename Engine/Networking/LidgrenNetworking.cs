using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Lidgren.Network.Xna;
using Lidgren.Network;

namespace Mammoth.Engine.Networking
{
    public abstract class LidgrenNetworking : AbstractNetworking
    {
        public const int PORT = 3333;

        public LidgrenNetworking(Game game)
            : base(game)
        {

        }

        public override bool isLANCapable()
        {
            return true;
        }

        public override bool isNetCapable()
        {
            return false;
        }

        public override AbstractNetworking.NetworkingType getType()
        {
            return NetworkingType.LIDGREN;
        }

        public override void Update(GameTime gameTime)
        {
            return;
        }
    }
}
