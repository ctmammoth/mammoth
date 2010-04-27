using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine.Input
{
    [Flags]
    public enum InputType
    {
        None = 0x00,
        Forward = 0x01,
        Backward = 0x02,
        Left = 0x04,
        Right = 0x08,
        Sprint = 0x10,
        Jump = 0x20,
        Crouch = 0x40,
        Reload = 0x80
    }

    public class InputState
    {
        #region Fields

        InputType _curState, _prevState;

        #endregion

        public InputState(InputType prevState, InputType curState)
        {
            _curState = curState;
            _prevState = prevState;
        }

        public bool IsKeyDown(InputType input)
        {
            return (_curState & input) == input;
        }

        public bool KeyPressed(InputType input)
        {
            return ((_prevState & input) != input) && ((_curState & input) == input);
        }

        public bool KeyHeld(InputType input)
        {
            return ((_prevState & input) == input) && ((_curState & input) == input);
        }

        public bool KeyReleased(InputType input)
        {
            return ((_prevState & input) == input) && ((_curState & input) != input);
        }

        #region Properties

        public InputType CurrentState
        {
            get { return _curState; }
        }

        public InputType PreviousState
        {
            get { return _prevState; }
        }

        #endregion
    }
}
