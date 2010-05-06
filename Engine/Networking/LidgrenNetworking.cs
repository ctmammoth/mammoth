﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Lidgren.Network.Xna;
using Lidgren.Network;

namespace Mammoth.Engine.Networking
{
    public abstract class LidgrenNetworking : NetworkComponent
    {
        public const int PORT = 5555;

        public LidgrenNetworking(Game game)
            : base(game)
        {
            Mammoth.Engine.Networking.Decoder d = new Mammoth.Engine.Networking.Decoder(this.Game);
        }

        public override bool isLANCapable()
        {
            return true;
        }

        public override bool isNetCapable()
        {
            return false;
        }

        public override NetworkComponent.NetworkingType getType()
        {
            return NetworkingType.LIDGREN;
        }

        public override void Update(GameTime gameTime)
        {
            return;
        }
    }

    public enum MessageType
    {
        ENCODABLE, STATUS_CHANGE, CLIENT_ID
    }
}
