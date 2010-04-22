using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace Mammoth.Engine
{
    /**
     * Interface which defines a networking service
     * which can send encodable objects and farms out
     * updates;
     */
    public interface INetworkingService
    {
        bool isLANCapable();
        bool isNetCapable();
        Networking.NetworkingType getType();
    }

    public interface IServerNetworking : INetworkingService
    {
        void sendThing(IEncodable toSend, String target);
        void createGame();
        void endGame();
    }

    public interface IClientNetworking : INetworkingService
    {
        void sendThing(IEncodable toSend);
        void joinGame();
        void quitGame();
    }

    /**
     * Abstract class which defines a networking system.
     */
    public abstract class Networking : GameComponent, INetworkingService
    {
        public enum NetworkingType
        {
            XNA,
            LIDGREN
        }

        public abstract bool isLANCapable();

        public abstract bool isNetCapable();

        public abstract NetworkingType getType();

        public Networking(Game game) : base(game)
        {
            game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public abstract override void Update(GameTime gameTime);
    }
}
