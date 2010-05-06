using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;

namespace Mammoth.Engine.Networking
{
    public abstract class XNANetworking : Networking
    {
        #region Variables

        public const int MAX_PLAYERS = 2;

        protected NetworkSession _session;

        #endregion

        public XNANetworking(Game game) : base(game)
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
            createSession();
            _toSend = new Queue<DataGram>();
        }

        #region IServerNetworking Members

        public void createSession()
        {
            Console.WriteLine("Began creating session.");
            _session = NetworkSession.Create(NetworkSessionType.SystemLink, 1, MAX_PLAYERS);
            _session.AllowHostMigration = false;
            _session.AllowJoinInProgress = false;
            _session.GamerJoined += new EventHandler<GamerJoinedEventArgs>(session_GamerJoined);
            //_session.GamerLeft += new EventHandler<GamerLeftEventArgs>(session_GamerLeft);
            //_session.GameStarted += new EventHandler<GameStartedEventArgs>(session_GameStarted);
            //_session.GameEnded += new EventHandler<GameEndedEventArgs>(session_GameEnded);
            //_session.SessionEnded +=new EventHandler<NetworkSessionEndedEventArgs>(session_SessionEnded);
            _server = _session.LocalGamers[0];
            Console.WriteLine("Finished creating session.");
        }

        void session_GamerJoined(object sender, GamerJoinedEventArgs e)
        {
            
        }

        public void endGame()
        {
            if (_session != null)
            {
                _session.EndGame();
                _session.Update();
            }
        }

        public void sendThing(IEncodable toSend, int target)
        {
            _toSend.Enqueue(new DataGram(toSend.Encode(), target));
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
                    Console.WriteLine("Read something");
                    Console.WriteLine(reader.ReadBytes(reader.Length));
                }
            }
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
            if (availableSessions.Count == 0)
                Console.WriteLine("Unable to find game.");
            else
            {
                _session = NetworkSession.Join(availableSessions[0]);
                _localGamer = _session.LocalGamers[0];
            }
        }

        public void quitGame()
        {
            _session.EndGame();
            _session.Update();
        }

        public void sendThing(IEncodable toSend)
        {
            _toSend.Enqueue(toSend.Encode());
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
