using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Mammoth.Engine.Input
{
    public class LocalInput : GameComponent, IInputService
    {
        #region Fields

        private Dictionary<InputType, Keys> _keyMappings;

        private InputState _state;

        #endregion

        public LocalInput(Game game)
            : base(game)
        {
            _keyMappings = new Dictionary<InputType, Keys>(Enum.GetValues(typeof(InputType)).Length);
            _state = new InputState(InputType.None, InputType.None);

            game.Services.AddService(typeof(IInputService), this);
        }

        public override void Initialize()
        {
            base.Initialize();

            LoadSettings();
        }

        public void LoadSettings()
        {
            _keyMappings.Add(InputType.Forward, Keys.W);
            _keyMappings.Add(InputType.Backward, Keys.S);
            _keyMappings.Add(InputType.Left, Keys.A);
            _keyMappings.Add(InputType.Right, Keys.D);
            _keyMappings.Add(InputType.Sprint, Keys.LeftShift);
            _keyMappings.Add(InputType.Jump, Keys.Space);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            InputType newState = InputType.None;

            foreach(var input in _keyMappings.Where((pair) => IsKeyDown(pair.Key)))
                newState |= input.Key;

            _state = new InputState(_state.CurrentState, newState);
        }

        private bool IsKeyDown(InputType k)
        {
            return Keyboard.GetState().IsKeyDown(_keyMappings[k]);
        }

        #region Properties

        public bool IsLocal
        {
            get { return true; }
        }

        public InputState State
        {
            get { return _state; }
        }

        #endregion
    }
}
