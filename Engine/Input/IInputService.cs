using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using System.Configuration;

namespace Mammoth.Engine.Input
{
    public interface IInputService
    {
        /// <summary>
        /// This sets the current input state to be the input state of the specified client.  This is
        /// unsupported for local-based input.
        /// </summary>
        /// <param name="clientID">The ID of the client whose state we want to store.</param>
        void SetStateByClientID(int clientID);

        #region Properties

        /// <summary>
        /// Whether this input represents the
        /// local client input or is remote
        /// (i.e. on the server).
        /// </summary>
        bool IsLocal
        {
            get;
        }

        /// <summary>
        /// Queue of input states which have
        /// yet to be handled.
        /// </summary>
        Queue<InputState> States
        {
            get;
        }

        /// <summary>
        /// Whether the current queue of input
        /// states have been handled yet.
        /// </summary>
        bool InputHandled
        {
            get;
            set;
        }

        #endregion
    }
}
