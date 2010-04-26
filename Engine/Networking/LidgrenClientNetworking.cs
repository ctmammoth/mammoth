using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Lidgren.Network.Xna;
using Lidgren.Network;

namespace Mammoth.Engine.Networking
{
    public class LidgrenClientNetworking : LidgrenNetworking, IClientNetworking
    {
        private NetClient _client;
        private Queue<byte[]> _toSend;

        public LidgrenClientNetworking(Game game)
            : base(game)
        {
            _toSend = new Queue<byte[]>();
            NetConfiguration config = new NetConfiguration("Mammoth");
            _client = new NetClient(config);
        }

        #region IClientNetworking Members

        public void sendThing(IEncodable toSend)
        {
            Console.WriteLine("Sending thing");
            _toSend.Enqueue(toSend.Encode());
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            NetBuffer buffer;
            while (_toSend.Count != 0)
            {
                Console.WriteLine("Really sending thing");
                buffer = _client.CreateBuffer();
                buffer.Write(_toSend.Dequeue());
                _client.SendMessage(buffer, NetChannel.Unreliable);
            }
        }

        public void joinGame()
        {
            _client.DiscoverLocalServers(PORT);
            NetBuffer buffer = _client.CreateBuffer();
            NetMessageType type;
            while (true)
            {
                _client.ReadMessage(buffer, out type);
                switch (type)
                {
                    case NetMessageType.ServerDiscovered:
                        Console.WriteLine("Discovered network");
                        NetBuffer buf = _client.CreateBuffer();
                        buf.Write(Engine.Instance.LocalPlayer.ID);
                        _client.Connect(buffer.ReadIPEndPoint(), buf.ToArray());
                        return;
                }
            }
        }

        public void quitGame()
        {
            _client.Shutdown("Player Quit");
        }

        #endregion
    }
}
