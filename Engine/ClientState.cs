using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine
{
    class ClientState
    {
        #region Fields

        // This is subject to change.
        public enum State
        {
            MainMenu,
            OptionsMenu,
            ServerList,
            GameLobby,
            InGame
        }

        #endregion

        private ClientState()
        {
            _currentState = State.MainMenu;
        }

        static ClientState()
        {
            _instance = new ClientState();
        }

        public delegate void ClientStateEventHandler(ClientState sender, State oldState, State newState);
        public event ClientStateEventHandler ClientStateChanged;

        #region Properties

        public static ClientState Instance
        {
            get { return _instance; }
        }
        static ClientState _instance;

        public State CurrentState
        {
            get { return _currentState; }
            set
            {
                this.PreviousState = _currentState;
                _currentState = value;

                // Let people know that the client state changed.
                if(ClientStateChanged != null)
                    ClientStateChanged(this, this.PreviousState, _currentState);
            }
        }
        State _currentState;

        public State PreviousState
        {
            get;
            private set;
        }

        #endregion
    }
}
