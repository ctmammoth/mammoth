using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Input;

using Lidgren.Network.Xna;
using Lidgren.Network;

namespace Mammoth.Engine.Networking
{
    public class LidgrenServerNetworking : LidgrenNetworking, IServerNetworking
    {
        private NetServer _server;
        private Dictionary<int, NetConnection> _connections;
        private Queue<DataGram> _toSend;
        private Dictionary<int, Queue<InputState>> _inputStates;

        private int _nextID;

        public LidgrenServerNetworking(Game game)
            : base(game)
        {
            _toSend = new Queue<DataGram>();
            _connections = new Dictionary<int, NetConnection>();
            _inputStates = new Dictionary<int, Queue<InputState>>();
            _nextID = 1;
        }

        #region IServerNetworking Members

        public void sendThing(IEncodable toSend, int target)
        {
            if (toSend is BaseObject)
                _toSend.Enqueue(new DataGram(((BaseObject)toSend).getObjectType(), ((BaseObject)toSend).ID, toSend.Encode(), target));
        }

        public void sendThing(IEncodable toSend)
        {
            // TODO: HACK: Not all things are players
            if (toSend is BaseObject)
                _toSend.Enqueue(new DataGram(((BaseObject)toSend).getObjectType(), ((BaseObject)toSend).ID, toSend.Encode(), -1));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (Queue<InputState> q in _inputStates.Values)
                q.Clear();

            NetBuffer buffer = _server.CreateBuffer();

            //Console.WriteLine("Things to send: " + _toSend.Count);
            while (_toSend.Count != 0)
            {
                DataGram message = _toSend.Dequeue();
                if (message.Recipient >= 0)
                {
                    if (!_connections.ContainsKey(message.Recipient))
                        continue;
                    if (_connections[message.Recipient].Status == NetConnectionStatus.Disconnected)
                        _connections.Remove(message.Recipient);
                    else if (_connections[message.Recipient].Status != NetConnectionStatus.Connected)
                        continue;
                }
                buffer = _server.CreateBuffer();
                buffer.Write(message.Type);
                buffer.WriteVariableInt32(message.ID);
                buffer.WriteVariableInt32(message.Data.Length);
                buffer.WritePadBits();
                buffer.Write(message.Data);
                if (message.Recipient < 0)
                    _server.SendMessage(buffer, _connections.Values, NetChannel.Unreliable);
                else
                    _server.SendMessage(buffer, _connections[message.Recipient], NetChannel.Unreliable);
            }

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
                        acceptClient(buffer, sender);
                        break;
                    case NetMessageType.StatusChanged:
                        handleStatusChange(buffer, sender);
                        break;
                    case NetMessageType.Data:
                        handleData(buffer, sender);
                        break;
                }
            }
        }

        private void acceptClient(NetBuffer buffer, NetConnection sender)
        {
            Console.WriteLine("Connection Approval");
            sender.Approve();
            while (sender.Status != NetConnectionStatus.Connected) ;
            int id = _nextID++;
            Console.WriteLine("The value of id: " + id);
            _connections.Add(id, sender);
            _inputStates[id] = new Queue<InputState>();
            buffer = _server.CreateBuffer();
            buffer.Write("ClientID");
            buffer.WriteVariableInt32(id);
            _server.SendMessage(buffer, sender, NetChannel.ReliableInOrder2);

            //TODO: TEST CODE
            //TODO: object ID of localplayer should be client ID + 0
            IModelDBService mdb = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            InputPlayer player = new ProxyInputPlayer(this.Game, id);
            player.ID = id << 25;
            mdb.registerObject(player);
        }

        private void handleStatusChange(NetBuffer buffer, NetConnection sender)
        {
            String message = buffer.ReadString();
            Console.WriteLine("Message: " + message);
            NetConnectionStatus newStatus = (NetConnectionStatus)buffer.ReadByte();
            int senderID = -1;
            if (int.TryParse(message, out senderID));
            
            if (senderID == -1)
                Console.WriteLine("New status for " + sender + ": " + newStatus);
            if (newStatus == NetConnectionStatus.Disconnected || newStatus == NetConnectionStatus.Disconnecting)
            {
                Console.WriteLine("Client " + senderID + " has disconnected.");
                _connections.Remove(senderID);
            }
        }

        private void handleData(NetBuffer buffer, NetConnection sender)
        {
            // A client sent this data!
            //Console.WriteLine("Data received from " + sender);
            int senderID = buffer.ReadVariableInt32();
            //Console.WriteLine("Sender ID: " + senderID);
            switch (buffer.ReadString())
            {
                case "Mammoth.Engine.Input.InputState":
                    if (!_inputStates.ContainsKey(senderID))
                        throw new Exception("Invalid player id: " + senderID);
                    IDecoder decoder = (IDecoder)this.Game.Services.GetService(typeof(IDecoder));
                    int length = buffer.ReadVariableInt32();
                    buffer.SkipPadBits();
                    byte[] data = buffer.ReadBytes(length);
                    InputState state = decoder.DecodeInputState(data);
                    _inputStates[senderID].Enqueue(state);
                    break;
            }
            //byte[] data = buffer.ReadBytes(buffer.LengthBytes);
            //Console.WriteLine(data);
            //_data.Add(data);
        }

        public Queue<InputState> getInputStateQueue(int playerID)
        {
            if (_inputStates[playerID] == null)
            {
                //TODO add good exceptions
                throw new Exception("Invalid player id: " + playerID);
            }
            return _inputStates[playerID];
        }

        public void createSession()
        {
            Console.WriteLine("Creating session...");
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

        public override int ClientID
        {
            get { return 0; }
        } 

        #endregion

        private class DataGram
        {
            public string Type;
            public int ID;
            public byte[] Data { get; set;  }
            public int Recipient { get; set;  }

            public DataGram(string type, int id, byte[] data, int recipient)
            {
                Type = type;
                ID = id;
                Data = data;
                Recipient = recipient;
            }
        }
    }
}
