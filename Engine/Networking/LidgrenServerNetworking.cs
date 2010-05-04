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
                _toSend.Enqueue(new DataGram(((BaseObject)toSend).getObjectType(), 
                    ((BaseObject)toSend).ID, toSend.Encode(), target));
        }

        public void sendThing(IEncodable toSend)
        {
            if (toSend is BaseObject)
                _toSend.Enqueue(new DataGram(((BaseObject)toSend).getObjectType(), 
                    ((BaseObject)toSend).ID, toSend.Encode(), -1));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            while (_toSend.Count != 0)
                sendMessage(_toSend.Dequeue());

            foreach (Queue<InputState> q in _inputStates.Values)
                q.Clear();

            NetBuffer buffer = _server.CreateBuffer();
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
                        handlePlayerJoin(buffer, sender);
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

        private void sendMessage(DataGram message)
        {
            if (message.Recipient >= 0)
            {
                NetConnectionStatus status = _connections[message.Recipient].Status;
                if (status != NetConnectionStatus.Connected)
                {
                    Console.WriteLine("Removing disconnected client " + message.Recipient);
                    _connections.Remove(message.Recipient);
                    IModelDBService mdb = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
                    if (mdb.hasObject(message.Recipient << 25))
                        mdb.removeObject(message.Recipient << 25);
                    return;
                }
            }
            NetBuffer buffer = _server.CreateBuffer();
            buffer.WriteVariableInt32((int)MessageType.ENCODABLE);
            buffer.Write(message.ObjectType);
            buffer.WriteVariableInt32(message.ID);
            buffer.WriteVariableInt32(message.Data.Length);
            buffer.WritePadBits();
            buffer.Write(message.Data);
            if (message.Recipient < 0)
            {
                foreach (NetConnection c in _connections.Values)
                    if (c.Status == NetConnectionStatus.Connected)
                        _server.SendMessage(buffer, c, NetChannel.Unreliable);
            }
            else
                _server.SendMessage(buffer, _connections[message.Recipient], NetChannel.Unreliable);
        }

        private void handlePlayerJoin(NetBuffer buffer, NetConnection sender)
        {
            Console.WriteLine("Connection Approval");
            sender.Approve();
            while (sender.Status != NetConnectionStatus.Connected) ;
            int id = _nextID++;
            Console.WriteLine("The value of id: " + id);
            _connections.Add(id, sender);
            _inputStates[id] = new Queue<InputState>();
            buffer = _server.CreateBuffer();
            buffer.WriteVariableInt32((int)MessageType.CLIENT_ID);
            buffer.WriteVariableInt32(id);
            _server.SendMessage(buffer, sender, NetChannel.ReliableInOrder2);

            //TODO: TEST CODE
            IModelDBService mdb = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            InputPlayer player = new ProxyInputPlayer(this.Game, id);
            player.ID = id << 25;
            mdb.registerObject(player);
        }

        public void handleStatusChange(NetBuffer buffer, NetConnection sender)
        {
            string statusMessage = buffer.ReadString();
            NetConnectionStatus newStatus = (NetConnectionStatus)buffer.ReadByte();
            Console.WriteLine("New status for " + sender + ": " + newStatus + " (" + statusMessage + ")");
        }

        public void handleData(NetBuffer buffer, NetConnection sender)
        {
            int senderID = buffer.ReadVariableInt32();
            MessageType type = (MessageType)buffer.ReadVariableInt32();
            switch (type)
            {
                case MessageType.ENCODABLE:
                    handleEncodable(buffer, senderID);
                    break;
                case MessageType.STATUS_CHANGE:
                    switch (buffer.ReadString())
                    {
                        case "CLIENT_QUIT":
                            int clientID = buffer.ReadVariableInt32();
                            Console.WriteLine("Client " + clientID + " has quit.");
                            _connections.Remove(clientID);
                            IModelDBService mdb = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
                            if (mdb.hasObject(clientID << 25))
                                mdb.removeObject(clientID << 25);
                            break;
                    }
                    break;
            }
        }

        private void handleEncodable(NetBuffer buffer, int senderID)
        {
            string objectType = buffer.ReadString();
            switch (objectType)
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
            NetBuffer buffer = _server.CreateBuffer();
            buffer.WriteVariableInt32((int)MessageType.STATUS_CHANGE);
            buffer.Write("SERVER_QUIT");
            _server.SendMessage(buffer, _connections.Values, NetChannel.ReliableInOrder1);
            _server.Shutdown("Game ended.");
        }

        public override int ClientID
        {
            get { return 0; }
        }

        #endregion

        private class DataGram
        {
            public string ObjectType;
            public int ID;
            public byte[] Data { get; set; }
            public int Recipient { get; set; }

            public DataGram(string objectType, int id, byte[] data, int recipient)
            {
                ObjectType = objectType;
                ID = id;
                Data = data;
                Recipient = recipient;
            }
        }
    }
}
