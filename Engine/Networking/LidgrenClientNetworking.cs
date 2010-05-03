﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Microsoft.Xna.Framework;
using Lidgren.Network.Xna;
using Lidgren.Network;

using Mammoth.Engine.Input;

namespace Mammoth.Engine.Networking
{
    public class LidgrenClientNetworking : LidgrenNetworking, IClientNetworking
    {
        private int _clientID;
        private NetClient _client;
        private Queue<DataGram> _toSend;

        public LidgrenClientNetworking(Game game)
            : base(game)
        {
            _toSend = new Queue<DataGram>();
            NetConfiguration config = new NetConfiguration("Mammoth");
            _client = new NetClient(config);
        }

        #region IClientNetworking Members

        public void sendThing(IEncodable toSend)
        {
            //Console.WriteLine("Sending thing");
            if (toSend is BaseObject)
                _toSend.Enqueue(new DataGram(toSend.GetType().ToString(), ((BaseObject)toSend).ID, toSend.Encode()));
            else
                _toSend.Enqueue(new DataGram(toSend.GetType().ToString(), toSend.Encode()));
        }

        public override void Update(GameTime gameTime)
        {
            if (_client == null || _client.Status != NetConnectionStatus.Connected)
                return;
            base.Update(gameTime);

            if (_client.ServerConnection.Status == NetConnectionStatus.Disconnected)
            {
                Console.WriteLine("Server disconnected.");
                quitGame();
            }
            else if (_client.ServerConnection.Status != NetConnectionStatus.Connected)
                return;

            IInputService inputServer = (IInputService)this.Game.Services.GetService(typeof(IInputService));
            InputState state = inputServer.States.Peek();
            sendThing(state);
            NetBuffer buffer;

            while (_toSend.Count != 0)
            {
                //Console.WriteLine("Really sending thing");
                buffer = _client.CreateBuffer();
                buffer.WriteVariableInt32(_clientID);
                DataGram data = _toSend.Dequeue();
                buffer.Write(data.Type);
                if (data.ID >= 0)
                    buffer.WriteVariableInt32(data.ID);
                buffer.WriteVariableInt32(data.Data.Length);
                buffer.WritePadBits();
                buffer.Write(data.Data);
                _client.SendMessage(buffer, NetChannel.ReliableInOrder1);
            }

            IDecoder decode = (IDecoder)this.Game.Services.GetService(typeof(IDecoder));
            buffer = _client.CreateBuffer();
            NetMessageType type;
            while (_client.ReadMessage(buffer, out type))
            {
                switch (type)
                {
                    case NetMessageType.DebugMessage:
                        Console.WriteLine(buffer.ReadString());
                        break;
                    case NetMessageType.StatusChanged:
                        string statusMessage = buffer.ReadString();
                        NetConnectionStatus newStatus = (NetConnectionStatus)buffer.ReadByte();
                        Console.WriteLine("New status for the server: " + newStatus + " (" + statusMessage + ")");
                        if (newStatus == NetConnectionStatus.Disconnecting || newStatus == NetConnectionStatus.Disconnected)
                        {
                            Console.WriteLine("Server has disconnected.");
                            quitGame();
                        }
                        break;
                    case NetMessageType.Data:
                        //Console.WriteLine("Data received");
                        string objectType = buffer.ReadString();
                        int id = buffer.ReadVariableInt32();
                        int length = buffer.ReadVariableInt32();
                        buffer.SkipPadBits();
                        byte[] data = buffer.ReadBytes(length);
                        decode.AnalyzeObjects(objectType, id, data);
                        break;
                }
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
                    case NetMessageType.DebugMessage:
                        Console.WriteLine(buffer.ReadString());
                        break;
                    case NetMessageType.StatusChanged:
                        string statusMessage = buffer.ReadString();
                        NetConnectionStatus newStatus = (NetConnectionStatus)buffer.ReadByte();
                        Console.WriteLine("New status: " + newStatus + " (" + statusMessage + ")");
                        break;
                    case NetMessageType.ServerDiscovered:
                        Console.WriteLine("Discovered network");
                        _client.Connect(buffer.ReadIPEndPoint());
                        while (_client.Status != NetConnectionStatus.Connected) ;
                        Console.WriteLine("Connected to server");
                        while (true)
                        {
                            buffer = _client.CreateBuffer();
                            NetMessageType type2;
                            while (!_client.ReadMessage(buffer, out type2))
                                Console.WriteLine("No message");
                            switch (type2)
                            {
                                case NetMessageType.Data:
                                    byte[] bytes = buffer.PeekBytes(buffer.LengthBytes);
                                    foreach (byte b in bytes)
                                        Console.Write(b + " ");
                                    Console.WriteLine();
                                    //Console.WriteLine(buffer.PeekString());
                                    string messageType = buffer.ReadString();
                                    if (!messageType.Equals("ClientID"))
                                    {
                                        Console.WriteLine("Non client id message");
                                        break;
                                    }
                                    int id = buffer.ReadVariableInt32();
                                    //if (id > 0)
                                        _clientID = id;
                                    //else
                                    //{
                                    //    //TODO: hack
                                    //    Console.WriteLine("Ignoring negative ID");
                                    //    break;
                                    //}
                                    Console.WriteLine("My ID is: " + _clientID);
                                    return;
                            }
                        }
                }
            }
        }

        public void quitGame()
        {
            Console.WriteLine("Quitting the game, my id is: " + _clientID.ToString());
            _client.Shutdown(_clientID.ToString());
        }

        public override int ClientID
        {
            get { return _clientID; }
        } 

        #endregion

        private class DataGram
        {
            public string Type;
            public int ID;
            public byte[] Data { get; set; }

            public DataGram(string type, int id, byte[] data)
            {
                Type = type;
                ID = id;
                Data = data;
            }

            public DataGram(string type, byte[] data)
            {
                Type = type;
                ID = -1;
                Data = data;
            }
        }
    }
}
