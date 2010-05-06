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
            _state = new InputState(InputType.None, InputType.None, Vector2.Zero, new GameTime());

            game.Services.AddService(typeof(IInputService), this);
        }

        public override void Initialize()
        {
            base.Initialize();

            LoadSettings();

            CenterCursor();
        }

        public void LoadSettings()
        {
            _keyMappings.Add(InputType.Forward, Keys.W);
            _keyMappings.Add(InputType.Backward, Keys.S);
            _keyMappings.Add(InputType.Left, Keys.A);
            _keyMappings.Add(InputType.Right, Keys.D);
            _keyMappings.Add(InputType.Sprint, Keys.LeftShift);
            _keyMappings.Add(InputType.Jump, Keys.Space);
            _keyMappings.Add(InputType.Stats, Keys.Tab);
        }

        public override void Update(GameTime gameTime)
        {
            InputType newState = InputType.None;

            foreach(var input in _keyMappings.Where((pair) => IsKeyDown(pair.Key)))
                newState |= input.Key;

            // Add in mouse-based input.
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
                newState |= InputType.Fire;
            if (mouseState.RightButton == ButtonState.Pressed)
                newState |= InputType.Zoom;

            // Get the mouse movement.
            Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Vector2 mouseCenter = new Vector2(this.Game.Window.ClientBounds.Width / 2, this.Game.Window.ClientBounds.Height / 2);
            Vector2 delta = (mousePosition - mouseCenter);

            // Reset the mouse cursor to the center.
            //this.CenterCursor();

            // Set the current state.
            _state = new InputState(_state.CurrentState, newState, delta, gameTime);

            // Reset the handled flag.
            this.InputHandled = false;
        }

        private bool IsKeyDown(InputType k)
        {
            return Keyboard.GetState().IsKeyDown(_keyMappings[k]);
        }

        /// <summary>
        /// This warps the cursor to the center of the game window.  This is important, as without it, the player's
        /// mouse would hit the side of the screen and they wouldn't be able to turn any further.  Also, using this
        /// method, the distance moved by the mouse in each update loop is just the distance from the center of the
        /// window to the mouse location.
        /// </summary>
        private void CenterCursor()
        {
            GameWindow window = this.Game.Window;

            if (ClientState.Instance.CurrentState == ClientState.State.InGame)
                Mouse.SetPosition(window.ClientBounds.Width / 2, window.ClientBounds.Height / 2);
        }

        public void SetStateByClientID(int clientID)
        {
            throw new NotSupportedException("You cannot set the state for local input!");
        }

        #region Properties

        public bool IsLocal
        {
            get { return true; }
        }

        public Queue<InputState> States
        {
            get
            {
                Queue<InputState> ret = new Queue<InputState>();
                ret.Enqueue(_state);
                return ret;
            }
        }

        public bool InputHandled
        {
            get;
            set;
        }

        #endregion
    }
}
