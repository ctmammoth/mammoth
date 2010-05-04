using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine.Networking
{
    public abstract class DummyNetworking : NetworkComponent
    {
        public DummyNetworking(Game game)
            : base(game)
        {

        }

        public override bool isLANCapable()
        {
            return false;
        }

        public override bool isNetCapable()
        {
            return false;
        }

        public override NetworkComponent.NetworkingType getType()
        {
            return NetworkingType.DUMMY;
        }

        public override void Update(GameTime gameTime)
        {
            return;
        }

        public override int ClientID
        {
            get { return 1; }
        }
    }

    public class DummyClientNetworking : DummyNetworking, IClientNetworking
    {
        #region IClientNetworking Members

        public DummyClientNetworking(Game game)
            : base(game)
        {

        }

        public void sendThing(IEncodable toSend)
        {
            return;
        }

        public void joinGame()
        {
            return;
        }

        public void quitGame()
        {
            return;
        }

        #endregion
    }

    public class DummyServerNetworking : DummyNetworking, IServerNetworking
    {
        #region IServerNetworking Members

        public DummyServerNetworking(Game game)
            : base(game)
        {

        }

        public void sendThing(IEncodable toSend, int target)
        {
            return;
        }

        public void sendThing(IEncodable toSend)
        {
            return;
        }

        public Queue<Mammoth.Engine.Input.InputState> getInputStateQueue(int playerID)
        {
            return null;
        }

        public void createSession()
        {
            return;
        }

        public void endGame()
        {
            return;
        }

        #endregion
    }
}
