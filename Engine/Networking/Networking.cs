using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Input;

namespace Mammoth.Engine.Networking
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
        NetworkComponent.NetworkingType getType();
        void Update(GameTime gameTime);

        int ClientID
        {
            get;
        } 
    }

    public interface IServerNetworking : INetworkingService
    {
        void sendThing(IEncodable toSend, int target);
        void sendThing(IEncodable toSend);
        Queue<InputState> getInputStateQueue(int playerID);
        void createSession();
        void endGame();
    }

    public interface IClientNetworking : INetworkingService
    {
        void sendThing(IEncodable toSend);
        void joinGame();
        void quitGame();
    }

    public class InputStateUpdate
    {
        public uint _bitmask;
        public double _elapsedTime;

        public InputStateUpdate(uint bitmask, double elapsedTime)
        {
            _bitmask = bitmask;
            _elapsedTime = elapsedTime;
        }

        public uint Bitmask
        {
            get
            {
                return _bitmask;
            }
        }

        public double ElapsedTime
        {
            get
            {
                return _elapsedTime;
            }
        }
    }

    /**
     * Abstract class which defines a networking system.
     */
    public abstract class NetworkComponent : GameComponent, INetworkingService
    {
        public enum NetworkingType
        {
            XNA,
            LIDGREN,
            DUMMY
        }

        public abstract bool isLANCapable();

        public abstract bool isNetCapable();

        public abstract NetworkingType getType();

        public NetworkComponent(Game game) : base(game)
        {
            this.Game.Services.AddService(typeof(INetworkingService), this);
        }

        public static void CreateClientNetworking(Game game)
        {
            LidgrenClientNetworking client = new LidgrenClientNetworking(game);
            game.Components.Add(client);
            client.UpdateOrder = 7;
            client.joinGame();
        }

        public static void CreateDummyClient(Game game)
        {
            DummyClientNetworking client = new DummyClientNetworking(game);
            game.Components.Add(client);
            client.UpdateOrder = 7;
        }

        public static void CreateServerNetworking(Game game)
        {
            LidgrenServerNetworking server = new LidgrenServerNetworking(game);
            game.Components.Add(server);
            server.UpdateOrder = 7;
            server.createSession();
        }

        public abstract int ClientID
        {
            get;
        }
    }
}
