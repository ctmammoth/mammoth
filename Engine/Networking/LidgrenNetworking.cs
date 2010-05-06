using System;
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
        // The port to be used by both the clients and server
        public const int PORT = 5555;

        public LidgrenNetworking(Game game)
            : base(game)
        {

        }

        /// <returns>NetworkingType.LIDGREN</returns>
        public override NetworkComponent.NetworkingType getType()
        {
            return NetworkingType.LIDGREN;
        }
    }

    /// <summary>
    /// An enum to describe a connection beyond just "Data"
    /// </summary>
    public enum MessageType
    {
        ENCODABLE, STATUS_CHANGE, CLIENT_ID
    }
}
