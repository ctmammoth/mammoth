using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Networking;

namespace Mammoth.Engine.Input
{
    /// <summary>
    /// Represents input on the server by emulating the input on a
    /// specified client. When told to emulate a specific client, it
    /// gets the queue of unhandled input states for that client from
    /// the network and makes that available to the server.
    /// </summary>
    public class EmulatedInput : GameComponent, IInputService
    {
        #region Fields

        Queue<InputState> _emulatedState = null;

        #endregion

        /// <summary>
        /// Initialize this service by adding it to the list of services.
        /// </summary>
        /// <param name="game"></param>
        public EmulatedInput(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IInputService), this);
        }

        /// <summary>
        /// Tells this EmulatedInput which client to emulate (until this method
        /// is called again). Gets the input for the specified client from the
        /// networking and makes it available through the States property.
        /// </summary>
        /// <param name="clientID"></param>
        public void SetStateByClientID(int clientID)
        {
            IServerNetworking serverNet = (IServerNetworking)this.Game.Services.GetService(typeof(INetworkingService));

            _emulatedState = serverNet.getInputStateQueue(clientID);
        }

        /// <summary>
        /// See IInputService for descriptions.
        /// </summary>
        #region Properties

        public bool IsLocal
        {
            get { return false; }
        }

        public Queue<InputState> States
        {
            get { return _emulatedState; }
        }

        public bool InputHandled
        {
            get { return false; }
            set { }
        }

        #endregion
    }
}
