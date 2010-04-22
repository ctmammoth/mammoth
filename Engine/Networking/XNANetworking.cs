using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;

namespace Mammoth.Engine
{
    public abstract class XNANetworking : Networking
    {
        #region Variables

        protected NetworkSession _session;

        #endregion

        public XNANetworking(Game game) : base(game)
        {
            game.Components.Add(new GamerServicesComponent(game));
        }

        public override bool isLANCapable()
        {
            return true;
        }

        public override bool isNetCapable()
        {
            return false;
        }

        public override Networking.NetworkingType getType()
        {
            return NetworkingType.XNA;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            _session.Update();
        }
    }

    public class XNAServerNetworking : XNANetworking, IServerNetworking
    {
        #region Variables

        private LocalNetworkGamer _server;
        private Queue<DataGram> _toSend;

        #endregion

        public XNAServerNetworking(Game game)
            : base(game)
        {
            createGame();
            _toSend = new Queue<DataGram>();
        }

        #region IServerNetworking Members

        public void createGame()
        {
            _session = NetworkSession.Create(NetworkSessionType.SystemLink, 1, 2);
            _session.AllowHostMigration = true;
            _session.AllowJoinInProgress = true;
            _server = _session.LocalGamers[0];
        }

        public void endGame()
        {
            _session.EndGame();
            _session.Update();
        }

        public void sendThing(IEncodable toSend, string target)
        {
            _toSend.Enqueue(new DataGram(toSend.encode(), target));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            while (_toSend.Count != 0)
            {
                _server.SendData(_toSend.Dequeue().Data, SendDataOptions.None);
            }
            PacketReader reader = new PacketReader();
            while (_server.IsDataAvailable)
            {
                NetworkGamer sender;
                _server.ReceiveData(reader, out sender);
                if (!sender.IsLocal)
                {
                    Console.WriteLine(reader.ReadBytes(reader.Length));
                }
            }
        }

        #endregion

        private class DataGram
        {
            public byte[] Data { get; set;  }
            public string Recipient { get; set;  }

            public DataGram(byte[] data, string recipient)
            {
                Data = data;
                Recipient = recipient;
            }
        }
    }

    public class XNAClientNetworking : XNANetworking, IClientNetworking
    {
        #region Variables

        private LocalNetworkGamer _localGamer;
        private Queue<byte[]> _toSend;

        #endregion

        public XNAClientNetworking(Game game)
            : base(game)
        {
            _toSend = new Queue<byte[]>();
        }

        #region IClientNetworking Members

        public void joinGame()
        {
            AvailableNetworkSessionCollection availableSessions = 
                NetworkSession.Find(NetworkSessionType.SystemLink, 1, null);
            if (availableSessions.Count != 0)
                _session = NetworkSession.Join(availableSessions[0]);
            else
                Console.WriteLine("Unable to find game.");
            _localGamer = _session.LocalGamers[0];
        }

        public void quitGame()
        {
            _session.EndGame();
            _session.Update();
        }

        public void sendThing(IEncodable toSend)
        {
            _toSend.Enqueue(toSend.encode());
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            while(_toSend.Count != 0)
            {
                _localGamer.SendData(_toSend.Dequeue(), SendDataOptions.None);
            }
            PacketReader reader = new PacketReader();
            while (_localGamer.IsDataAvailable)
            {
                NetworkGamer sender;
                _localGamer.ReceiveData(reader, out sender);
                if (!sender.IsLocal)
                {
                    Console.WriteLine(reader.ReadBytes(reader.Length));
                }
            }
        }

        #endregion
    }
}
