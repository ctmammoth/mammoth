using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Input;

namespace Mammoth.Engine.Networking
{
    /// <summary>
    /// Interface which defines a networking service
    /// which can send encodable objects and farms out
    /// updates.
    /// </summary>
    public interface INetworkingService
    {
        NetworkComponent.NetworkingType getType();

        /// <summary>
        /// A unique identifier for a client (0 for the server)
        /// </summary>
        int ClientID
        {
            get;
        } 
    }

    /// <summary>
    /// Specifies methods for sending to different recipients,
    /// managing sessions, and retrieving InputState queues.
    /// </summary>
    public interface IServerNetworking : INetworkingService
    {
        void sendThing(IEncodable toSend, int target);
        void sendThing(IEncodable toSend);
        void sendToAllBut(IEncodable toSend, int excludeTarget);
        Queue<InputState> getInputStateQueue(int playerID);
        void createSession();
        void endGame();
    }

    /// <summary>
    /// Specifies methods for sending to the server and
    /// joining and quitting games.
    /// </summary>
    public interface IClientNetworking : INetworkingService
    {
        void sendThing(IEncodable toSend);
        void joinGame();
        void quitGame();
    }

    /**
     * Abstract class which defines a networking system.
     */
    public abstract class NetworkComponent : GameComponent, INetworkingService
    {
        /// <summary>
        /// Defines the different available networking types
        /// </summary>
        public enum NetworkingType
        {
            LIDGREN,
            DUMMY
        }

        public abstract NetworkingType getType();


        /// <summary>
        /// Adds this networking component to the game services.
        /// </summary>
        /// <param name="game"></param>
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

        /// <summary>
        /// A unique identifier for a client (0 for the server)
        /// </summary>
        public abstract int ClientID
        {
            get;
        }
    }
}
