using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Lidgren.Network.Xna;
using Lidgren.Network;

namespace Mammoth.Engine.Networking
{
    public class LidgrenServerNetworking : LidgrenNetworking, IServerNetworking
    {
        private NetServer _server;
        private Dictionary<int, NetConnection> _connections;
        private Queue<DataGram> _toSend;

        public LidgrenServerNetworking(Game game)
            : base(game)
        {
            _toSend = new Queue<DataGram>();
            _connections = new Dictionary<int, NetConnection>();
            createSession();
        }

        #region IServerNetworking Members

        public void sendThing(IEncodable toSend, int target)
        {
            _toSend.Enqueue(new DataGram(toSend.Encode(), target));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Console.WriteLine("Updating server");

            NetBuffer buffer;

            while (_toSend.Count != 0)
            {
                DataGram message = _toSend.Dequeue();
                buffer = _server.CreateBuffer();
                buffer.Write(message.Data);
                _server.SendMessage(buffer, _connections[message.Recipient], NetChannel.Unreliable);
            }

            buffer = _server.CreateBuffer();
            NetMessageType type;
            NetConnection sender;
            while (_server.ReadMessage(buffer, out type, out sender))
            {
                switch (type)
                {
                    case NetMessageType.DebugMessage:
                        Console.WriteLine(buffer.ReadString());
                        break;
                    case NetMessageType.ConnectionApproval:
                        int id = buffer.ReadInt16();
                        Console.WriteLine("Approval; id is " + id);
                        sender.Approve();
                        _connections.Add(id, sender);
                        break;
                    case NetMessageType.StatusChanged:
                        string statusMessage = buffer.ReadString();
                        NetConnectionStatus newStatus = (NetConnectionStatus)buffer.ReadByte();
                        Console.WriteLine("New status for " + sender + ": " + newStatus + " (" + statusMessage + ")");
                        break;
                    case NetMessageType.Data:
                        // A client sent this data!
                        Console.WriteLine("Data recieved from " + sender);
                        string msg = buffer.ReadString();

                        // send to everyone, including sender
                        NetBuffer sendBuffer = _server.CreateBuffer();
                        sendBuffer.Write(sender.RemoteEndpoint.ToString() + " wrote: " + msg);

                        // send using ReliableInOrder
                        _server.SendToAll(sendBuffer, NetChannel.ReliableInOrder1);
                        break;
                }
            }
        }

        public void createSession()
        {
            NetConfiguration config = new NetConfiguration("Mammoth");
            config.MaxConnections = 32;
            config.Port = PORT;

            _server = new NetServer(config);
            _server.SetMessageTypeEnabled(NetMessageType.ConnectionApproval, true);
            _server.Start();
        }

        public void endGame()
        {
            _server.Shutdown("Game ended.");
        }

        #endregion

        private class DataGram
        {
            public byte[] Data { get; set;  }
            public int Recipient { get; set;  }

            public DataGram(byte[] data, int recipient)
            {
                Data = data;
                Recipient = recipient;
            }
        }
    }
}
