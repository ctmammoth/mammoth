using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

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
        Networking.NetworkingType getType();
        void Update(GameTime gameTime);
    }

    public interface IServerNetworking : INetworkingService
    {
        void sendThing(IEncodable toSend, int target);
        Queue<InputStateUpdate> getInputStateQueue(int playerID);
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
            this.Game.Services.AddService(typeof(INetworkingService), this);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public abstract override void Update(GameTime gameTime);

        public static void CreateClientNetworking(Game game)
        {
            IClientNetworking client = new LidgrenClientNetworking(game);
            client.joinGame();
        }

        public static void CreateServerNetworking(Game game)
        {
            IServerNetworking server = new LidgrenServerNetworking(game);
            server.createSession();
        }
    }
}
