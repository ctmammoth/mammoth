using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Networking;

namespace Mammoth.Engine.Input
{
    /// <summary>
    /// The different types of inputs (e.g. move forward, fire, etc.),
    /// along with their values to use in a bitmask.
    /// </summary>
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
        Fire = 0x100,
        Zoom = 0x200,
        Stats = 0x400,
        Weapon1 = 0x800,
        Weapon2 = 0x1000,
        Weapon3 = 0x2000,
        SpawnRoom = 0x4000
    }

    /// <summary>
    /// Represents the state of a client's input at 
    /// a timestep, containing information about
    /// whether the key corresponding to different
    /// input types were held or pressed along with
    /// the movement of the mouse during the timestep
    /// and the amount of time the state was valid for.
    /// Encodable so that it can be sent from the clients
    /// to the server.
    /// </summary>
    public class InputState : IEncodable
    {
        #region Fields

        private InputType _curState, _prevState;
        private Vector2 _mouseDelta;
        private float _elapsedSeconds;

        #endregion

        public String getObjectType()
        {
            return "InputState";
        }

        /// <summary>
        /// Creates an essentially empty InputState.
        /// Used by the decoder, which then decodes
        /// the new empty state to get the true state.
        /// </summary>
        public InputState()
        {
            _curState = InputType.None;
            _prevState = InputType.None;
            _mouseDelta = Vector2.Zero;
            _elapsedSeconds = 0.0f;
        }

        /// <summary>
        /// Creates a new InputState with the specified
        /// bitmask values for the current and previous
        /// states along with the mouse movement and time
        /// the state was valid for.
        /// </summary>
        /// <param name="prevState"></param>
        /// <param name="curState"></param>
        /// <param name="delta"></param>
        /// <param name="time"></param>
        public InputState(InputType prevState, InputType curState, Vector2 delta, GameTime time)
        {
            _curState = curState;
            _prevState = prevState;
            _mouseDelta = delta;
            _elapsedSeconds = (float) time.ElapsedGameTime.TotalSeconds;
        }

        /// <summary>
        /// Returns whether the key corresponding to the
        /// specified input type was down.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool IsKeyDown(InputType input)
        {
            return (_curState & input) == input;
        }

        /// <summary>
        /// Returns whether the key corresponding to the
        /// specified input type was pressed, i.e. was
        /// down in the current timestep but not the last
        /// one.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool KeyPressed(InputType input)
        {
            return ((_prevState & input) != input) && ((_curState & input) == input);
        }

        /// <summary>
        /// Returns whether the key corresponding to the
        /// specified input type was held, i.e. was down
        /// in both the previous and current timesteps.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool KeyHeld(InputType input)
        {
            return ((_prevState & input) == input) && ((_curState & input) == input);
        }

        /// <summary>
        /// Returns whether the key corresponding to the
        /// specified input type was released, i.e. was down
        /// in the last timestep but not the current one.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool KeyReleased(InputType input)
        {
            return ((_prevState & input) == input) && ((_curState & input) != input);
        }

        /// <summary>
        /// Prints out which keys were down in this state.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string toReturn = "INPUT: ";
            foreach (InputType t in Enum.GetValues(InputType.Forward.GetType()))
                if (IsKeyDown(t))
                    toReturn += t.ToString() + " ";
            return toReturn;
        }

        #region IEncodable Members

        /// <summary>
        /// Returns this state encoded as a byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] Encode()
        {
            Mammoth.Engine.Networking.Encoder tosend = new Mammoth.Engine.Networking.Encoder();

            tosend.AddElement("_curState", _curState);
            tosend.AddElement("_prevState", _prevState);
            tosend.AddElement("_mouseDelta", _mouseDelta);
            tosend.AddElement("_elapsedSeconds", _elapsedSeconds);

            return tosend.Serialize();
        }

        /// <summary>
        /// Sets the values of this state based on a byte array
        /// created by another state's encode method.
        /// </summary>
        /// <param name="serialized"></param>
        public void Decode(byte[] serialized)
        {
            Mammoth.Engine.Networking.Encoder getdata = new Mammoth.Engine.Networking.Encoder(serialized);

            _curState = (InputType) getdata.GetElement("_curState", _curState);
            _prevState = (InputType) getdata.GetElement("_prevState", _prevState);
            _mouseDelta = (Vector2) getdata.GetElement("_mouseDelta", _mouseDelta);
            _elapsedSeconds = (float) getdata.GetElement("_elapsedSeconds", _elapsedSeconds);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Bitmask of the keyboard state of the timestep
        /// this state represents.
        /// </summary>
        public InputType CurrentState
        {
            get { return _curState; }
        }

        /// <summary>
        /// Bitmask of the keyboard state of the timestep
        /// before the one this state represents.
        /// </summary>
        public InputType PreviousState
        {
            get { return _prevState; }
        }

        /// <summary>
        /// Vector of the mouse movement during the
        /// timestep this state represents.
        /// </summary>
        public Vector2 MouseDelta
        {
            get { return _mouseDelta; }
        }

        /// <summary>
        /// The time (in seconds) the timestep this
        /// state represents lasted.
        /// </summary>
        public float ElapsedSeconds
        {
            get { return _elapsedSeconds; }
        }

        #endregion
    }
}
