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
    /// <summary>
    /// Server side networking system. Provides for receiving message from 
    /// clients and sending messages to all or specific clients.
    /// </summary>
    public class LidgrenServerNetworking : LidgrenNetworking, IServerNetworking
    {
        private NetServer _server;

        // Dictionary from ClientID to client connection
        private Dictionary<int, NetConnection> _connections;

        private Queue<DataGram> _toSend;

        // Dictionary from ClientID to the queue of InputStates for that client
        private Dictionary<int, Queue<InputState>> _inputStates;

        // The next ID to assign a newly connecting client
        private int _nextID;

        /// <summary>
        /// Creates the server networking system, initializing the
        /// instance variables.
        /// </summary>
        /// <param name="game"></param>
        public LidgrenServerNetworking(Game game)
            : base(game)
        {
            _toSend = new Queue<DataGram>();
            _connections = new Dictionary<int, NetConnection>();
            _inputStates = new Dictionary<int, Queue<InputState>>();
            _nextID = 1;
        }

        #region IServerNetworking Members

        /// <summary>
        /// Sends an Encodable thing to a specified client by
        /// putting a Datagram in the queue of things to send.
        /// </summary>
        /// <param name="toSend">the Encodable to send</param>
        /// <param name="target">the ID of the client to send it to</param>
        public void sendThing(IEncodable toSend, int target)
        {
            _toSend.Enqueue(new DataGram(MessageType.ENCODABLE, toSend.getObjectType(), 
                (toSend is BaseObject) ? ((BaseObject)toSend).ID : -1, toSend.Encode(), target, -1, null));
        }

        /// <summary>
        /// Sends an Encodable thing to all connected clients by
        /// putting a Datagram in the queue of things to send.
        /// </summary>
        /// <param name="toSend"></param>
        public void sendThing(IEncodable toSend)
        {
            _toSend.Enqueue(new DataGram(MessageType.ENCODABLE, toSend.getObjectType(),
                (toSend is BaseObject) ? ((BaseObject)toSend).ID : -1, toSend.Encode(), -1, -1, null));
        }

        /// <summary>
        /// Sends an Encodable thing to all connected clients except for
        /// the specified client by putting a Datagram in the queue of things to send.
        /// </summary>
        /// <param name="toSend">the Encodable to send</param>
        /// <param name="excludeTarget">the ID of the client to exclude</param>
        public void sendToAllBut(IEncodable toSend, int excludeTarget)
        {
            _toSend.Enqueue(new DataGram(MessageType.ENCODABLE, toSend.getObjectType(),
                (toSend is BaseObject) ? ((BaseObject)toSend).ID : -1, toSend.Encode(), -1, excludeTarget, null));
        }

        /// <summary>
        /// Sends a sound event to all players.
        /// </summary>
        /// <param name="soundToPlay"></param>
        public void sendEvent(string eventType, string param)
        {
            _toSend.Enqueue(new DataGram(MessageType.EVENT, eventType, -1, null, -1, -1, param));
        }

        /// <summary>
        /// Sends a sound event to the specified player.
        /// </summary>
        /// <param name="soundToPlay"></param>
        /// <param name="target"></param>
        public void sendEvent(string eventType, string param, int target)
        {
            _toSend.Enqueue(new DataGram(MessageType.EVENT, eventType, -1, null, target, -1, param));
        }

        /// <summary>
        /// Starts by making sure that all clients are still connected, removing
        /// thos thate aren't anymore. Then eends everything in the queue of messages 
        /// to send, then clears the InputState queues for each client and reads in 
        /// all the new messages.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Check for and remove disconnected players
            checkForDisconnectedPlayers();

            // Send all the queued up messages
            while (_toSend.Count != 0)
                sendMessage(_toSend.Dequeue());

            // Clear all the InputState queues
            foreach (Queue<InputState> q in _inputStates.Values)
                q.Clear();

            // Read in all the new messages, calling the appropriate handle methods
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

        /// <summary>
        /// Sends a message in the form of a DataGram to the recipients specified by
        /// the DataGram.
        /// </summary>
        /// <param name="message"></param>
        private void sendMessage(DataGram message)
        {
            // Write the data to a buffer
            NetBuffer buffer = _server.CreateBuffer();
            buffer.WriteVariableInt32((int)message.MessageType);
            switch (message.MessageType)
            {
                case MessageType.ENCODABLE:
                    buffer.Write(message.ObjectType);
                    buffer.WriteVariableInt32(message.ID);
                    buffer.WriteVariableInt32(message.Data.Length);
                    buffer.WritePadBits();
                    buffer.Write(message.Data);
                    break;
                case MessageType.EVENT:
                    buffer.Write(message.ObjectType);
                    buffer.Write(message.Params);
                    break;
            }

            // If there is more than one specified target, send 
            if (message.Recipient < 0)
            {
                // If there is no client to exclude, send to all clients
                if (message.Exclude < 0)
                    _server.SendToAll(buffer, NetChannel.Unreliable);
                // Else, exclude the client to exclude
                else
                    _server.SendToAll(buffer, NetChannel.Unreliable, _connections[message.Exclude]);
            }
            // If there is one specified target, send to that target
            else
                _server.SendMessage(buffer, _connections[message.Recipient], NetChannel.Unreliable);
        }

        /// <summary>
        /// Check all clients to make sure their status is still connected. If a client
        /// is not connected, remove its player from the model database, its connection
        /// from the connections dictionary, and its queue of input states.
        /// </summary>
        private void checkForDisconnectedPlayers()
        {
            foreach (int id in _connections.Keys)
            {
                NetConnectionStatus status = _connections[id].Status;
                if (status != NetConnectionStatus.Connected)
                {
                    // If the client is no longer connected, remove it from the 
                    // connections, input states, and model database
                    Console.WriteLine("Removing disconnected client " + id);
                    _connections.Remove(id);
                    _inputStates.Remove(id);
                    IModelDBService mdb = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
                    if (mdb.hasObject(id << 25))
                        //mdb.removeObject(id << 25);
                        mdb.getObject(id << 25).IsAlive = false;
                    sendEvent("PlayerLeft", id.ToString());
                    return;
                }
            }
        }

        /// <summary>
        /// Handles a player joining message (technically a "Connection Approved"
        /// message). Approves the connection attempt of the player, then
        /// sends it a ClientID and creates a new ProxyInputPlayer and places
        /// it in the model database.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="sender"></param>
        private void handlePlayerJoin(NetBuffer buffer, NetConnection sender)
        {
            try
            {
                sender.Approve();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failure to join. Please try again.");
                return;
            }
            // Wait for the client to be fully connected
            while (sender.Status != NetConnectionStatus.Connected) ;
            
            // Choose a client ID
            int id = _nextID++;
            //Console.WriteLine("The value of id: " + id);
            _connections.Add(id, sender);
            _inputStates[id] = new Queue<InputState>();

            IModelDBService mdb = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));

            // Send every encodable in the model database to the new player
            var encodables = from obj in mdb.AllObjects
                             where obj is IEncodable
                             select obj as IEncodable;
            foreach (IEncodable toSend in encodables)
            {
                //Console.WriteLine("Initial send of " + ((BaseObject)toSend).getObjectType() + " with ID: " + ((BaseObject)toSend).ID);
                DataGram message = new DataGram(MessageType.ENCODABLE, ((BaseObject)toSend).getObjectType(),
                    ((BaseObject)toSend).ID, toSend.Encode(), id, -1, null);
                sendMessage(message);
            }

            // Send the client ID to the client
            buffer = _server.CreateBuffer();
            buffer.WriteVariableInt32((int)MessageType.CLIENT_ID);
            buffer.WriteVariableInt32(id);
            _server.SendMessage(buffer, sender, NetChannel.ReliableInOrder2);

            // Create ProxyInputPlayer to represent the new client and add it to
            // the model database.
            InputPlayer player = new ProxyInputPlayer(this.Game, id);
            // Give the player object the object ID of the ClientID bitshifted to the front of
            // an integer (i.e. the zero-ith object created by the client).
            player.ID = id << 25;
            //TODO: change where the player spawns to?
            player.Spawn(new Vector3(-3.0f, 10.0f, 0.0f), Quaternion.Identity);
            mdb.registerObject(player);

            // Send the player to the client
            DataGram playerMessage = new DataGram(MessageType.ENCODABLE, ((BaseObject)player).getObjectType(),
                    ((BaseObject)player).ID, player.Encode(), id, -1, null);
            sendMessage(playerMessage);
        }

        /// <summary>
        /// Handles a Lidgren status change message. At the moment just
        /// prints out the status change.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="sender"></param>
        public void handleStatusChange(NetBuffer buffer, NetConnection sender)
        {
            string statusMessage = buffer.ReadString();
            NetConnectionStatus newStatus = (NetConnectionStatus)buffer.ReadByte();
            Console.WriteLine("New status for " + sender + ": " + newStatus + " (" + statusMessage + ")");
        }

        /// <summary>
        /// Handles a data message, which can be of several
        /// types, though at the moment only Encodables are
        /// supported.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="sender"></param>
        public void handleData(NetBuffer buffer, NetConnection sender)
        {
            int senderID = buffer.ReadVariableInt32();
            MessageType type = (MessageType)buffer.ReadVariableInt32();
            switch (type)
            {
                case MessageType.ENCODABLE:
                    handleEncodable(buffer, senderID);
                    break;
            }
        }

        /// <summary>
        /// Handles an encodable by switching on the type
        /// of the sent thing and handling it appropriately.
        /// At the moment only supports InputStates, which
        /// it adds to the appropriate queue.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="senderID"></param>
        private void handleEncodable(NetBuffer buffer, int senderID)
        {
            string objectType = buffer.ReadString();
            switch (objectType)
            {
                case "Mammoth.Engine.Input.InputState":
                    // Decode the InputState and add it to the sender's InputState queue
                    if (!_inputStates.ContainsKey(senderID))
                        return;
                    IDecoder decoder = (IDecoder)this.Game.Services.GetService(typeof(IDecoder));
                    int length = buffer.ReadVariableInt32();
                    buffer.SkipPadBits();
                    byte[] data = buffer.ReadBytes(length);
                    InputState state = decoder.DecodeInputState(data);
                    _inputStates[senderID].Enqueue(state);
                    break;
            }
        }

        /// <summary>
        /// Returns the queue of InputStates for the specified
        /// client.
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public Queue<InputState> getInputStateQueue(int playerID)
        {
            if (!_inputStates.ContainsKey(playerID))
                return null;
            return _inputStates[playerID];
        }

        /// <summary>
        /// Creates the game session, which at the moment
        /// allows up to 32 client connections.
        /// </summary>
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

        /// <summary>
        /// Ends the game by shutting down the server.
        /// </summary>
        public void endGame()
        {
            sendEvent("EndGame", "EndGame");
            bool keepGoing = true;
            while (keepGoing)
            {
                Console.WriteLine("Waiting for clients to disconnect");
                keepGoing = false;
                foreach (NetConnection c in _connections.Values)
                    if (c.Status != NetConnectionStatus.Disconnected)
                        keepGoing = true;
            }
            Console.WriteLine("All clients disconnected");
            _connections.Clear();
            _inputStates.Clear();
            _nextID = 1;
            _toSend.Clear();
        }

        /// <summary>
        /// The ClientID of the server, which is always 0.
        /// </summary>
        public override int ClientID
        {
            get { return 0; }
        }

        #endregion

        /// <summary>
        /// Small storage class which stores an object to be sent
        /// in serialized form along with the type, ID, and 
        /// recipient information.
        /// </summary>
        private class DataGram
        {
            public MessageType MessageType;
            public string ObjectType;
            public int ID;
            public int Exclude;
            public byte[] Data;
            public int Recipient;
            public string Params;

            /// <summary>
            /// Creates a DataGram.
            /// </summary>
            /// <param name="objectType">The type of the object to be sent</param>
            /// <param name="id">The object ID of the object to be sent</param>
            /// <param name="data">The serialized data to be sent</param>
            /// <param name="recipient">The recipient of the data, -1 to send to all</param>
            /// <param name="exclude">A client to exclude from receiving the message, -1 to not exclude any</param>
            /// <param name="parameters">Any additional string params to send</param>
            public DataGram(MessageType messageType, string objectType, int id, byte[] data, int recipient, int exclude, string parameters)
            {
                MessageType = messageType;
                ObjectType = objectType;
                ID = id;
                Data = data;
                Recipient = recipient;
                Exclude = exclude;
                Params = parameters;
            }
        }
    }
}
