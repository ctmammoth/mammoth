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
        // HACK HACK HACK HACK HACK HACK HACK: HACK
        /// <summary>
        /// This sets the current input state to be the input state of the specified client.  This is
        /// unsupported for local-based input.
        /// </summary>
        /// <param name="clientID">The ID of the client whose state we want to store.</param>
        void SetStateByClientID(int clientID);

        #region Properties

        bool IsLocal
        {
            get;
        }

        Queue<InputState> States
        {
            get;
        }

        bool InputHandled
        {
            get;
            set;
        }

        #endregion
    }
}
