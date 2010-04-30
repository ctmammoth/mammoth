using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Networking;

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
        Reload = 0x80,
        Shoot = 0x100,
        Zoom = 0x200
    }

    public class InputState : IEncodable
    {
        #region Fields

        InputType _curState, _prevState;
        Vector2 _mouseDelta;
        float _elapsedSeconds;

        #endregion

        public InputState()
        {
            _curState = InputType.None;
            _prevState = InputType.None;
            _mouseDelta = Vector2.Zero;
            _elapsedSeconds = 0.0f;
        }

        public InputState(InputType prevState, InputType curState, Vector2 delta, GameTime time)
        {
            _curState = curState;
            _prevState = prevState;
            _mouseDelta = delta;
            _elapsedSeconds = (float) time.ElapsedGameTime.TotalSeconds;
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

        #region IEncodable Members

        public byte[] Encode()
        {
            Mammoth.Engine.Networking.Encoder tosend = new Mammoth.Engine.Networking.Encoder();

            tosend.AddElement("_curState", _curState);
            tosend.AddElement("_prevState", _prevState);
            tosend.AddElement("_mouseDelta", _mouseDelta);
            tosend.AddElement("_elapsedSeconds", _elapsedSeconds);

            return tosend.Serialize();
        }

        public void Decode(byte[] serialized)
        {
            Mammoth.Engine.Networking.Encoder getdata = new Mammoth.Engine.Networking.Encoder(serialized);

            _curState = (InputType) getdata.GetElement("_curState");
            _prevState = (InputType) getdata.GetElement("_prevState");
            _mouseDelta = (Vector2) getdata.GetElement("_mouseDelta");
            _elapsedSeconds = (float) getdata.GetElement("_elapsedSeconds");
        }

        public int ID
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Properties

        public InputType CurrentState
        {
            get { return _curState; }
        }

        public InputType PreviousState
        {
            get { return _prevState; }
        }

        public Vector2 MouseDelta
        {
            get { return _mouseDelta; }
        }

        public float ElapsedSeconds
        {
            get { return _elapsedSeconds; }
        }

        #endregion
    }
}
