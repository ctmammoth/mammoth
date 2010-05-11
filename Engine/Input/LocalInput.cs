using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Mammoth.Engine.Input
{
    /// <summary>
    /// Represents the local input on client, providing methods for
    /// getting input states based on the mouse and keyboard input.
    /// </summary>
    public class LocalInput : GameComponent, IInputService
    {
        #region Fields

        private Dictionary<InputType, Keys> _keyMappings;

        private InputState _state;

        #endregion

        /// <summary>
        /// Initalizes the local input service by creating the containers and
        /// adding this to the services.
        /// </summary>
        /// <param name="game"></param>
        public LocalInput(Game game)
            : base(game)
        {
            _keyMappings = new Dictionary<InputType, Keys>(Enum.GetValues(typeof(InputType)).Length);
            _state = new InputState(InputType.None, InputType.None, Vector2.Zero, new GameTime());

            game.Services.AddService(typeof(IInputService), this);
        }

        /// <summary>
        /// Initializes the service by loading the 
        /// keyboard mappings and resetting the mouse.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            LoadSettings();

            CenterCursor();
        }

        /// <summary>
        /// Sets the keyboard mappings.
        /// </summary>
        public void LoadSettings()
        {
            _keyMappings.Add(InputType.Forward, Keys.W);
            _keyMappings.Add(InputType.Backward, Keys.S);
            _keyMappings.Add(InputType.Left, Keys.A);
            _keyMappings.Add(InputType.Right, Keys.D);
            _keyMappings.Add(InputType.Sprint, Keys.LeftShift);
            _keyMappings.Add(InputType.Jump, Keys.Space);
            _keyMappings.Add(InputType.Reload, Keys.R);
            _keyMappings.Add(InputType.Stats, Keys.Tab);
            _keyMappings.Add(InputType.Weapon1, Keys.D1);
            _keyMappings.Add(InputType.Weapon2, Keys.D2);
            _keyMappings.Add(InputType.Weapon3, Keys.D3);
            _keyMappings.Add(InputType.SpawnRoom, Keys.F);
        }

        /// <summary>
        /// Gets the input from the mouse and keyboard using XNA
        /// functions and creates an InputState out of them, which
        /// are added to the queue of unhandled states.
        /// </summary>
        /// <param name="gameTime"></param>
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

        /// <summary>
        /// Uses XNA to check whether the key mapping to the
        /// input type is down.
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Does nothing, as there aren't multiple local clients.
        /// </summary>
        /// <param name="clientID"></param>
        public void SetStateByClientID(int clientID)
        {
            throw new NotSupportedException("You cannot set the state for local input!");
        }

        /// <summary>
        /// See IInputService for descriptions.
        /// </summary>
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
                if (!this.InputHandled)
                    ret.Enqueue(_state);
                else
                    ret.Enqueue(new InputState());
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
