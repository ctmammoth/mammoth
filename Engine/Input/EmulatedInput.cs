using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Networking;

namespace Mammoth.Engine.Input
{
    public class EmulatedInput : GameComponent, IInputService
    {
        #region Fields

        Queue<InputState> _emulatedState = null;

        #endregion

        public EmulatedInput(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IInputService), this);
        }

        public void SetStateByClientID(int clientID)
        {
            IServerNetworking serverNet = (IServerNetworking)this.Game.Services.GetService(typeof(INetworkingService));

            _emulatedState = serverNet.getInputStateQueue(clientID);
        }

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
