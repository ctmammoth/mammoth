using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Input;
using Mammoth.Engine.Interface;

namespace Mammoth.Engine.Interface
{
    public class TScreenManager : DrawableGameComponent
    {
        #region Fields

        List<TScreen> _screens = new List<TScreen>();
        Stack<TScreen> _screensToUpdate = new Stack<TScreen>();

        bool isInitialized = false;

        #endregion

        public TScreenManager(Game game)
            : base(game)
        {
            this.Game.Services.AddService(typeof(TScreenManager), this);
        }

        /// <summary>
        /// Initialize all of the screens.
        /// </summary>
        public override void Initialize()
        {
            isInitialized = true;

            foreach (TScreen screen in _screens)
            {
                screen.Initialize();
            }
        }

        /// <summary>
        /// Have all screens load any content that they need.
        /// </summary>
        protected override void LoadContent()
        {
            if(isInitialized)
                foreach (TScreen screen in _screens)
                    screen.LoadContent();
        }

        /// <summary>
        /// Tells all screens to update and lets them know if they need to handle input.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // Get the input service.
            IInputService input = (IInputService)this.Game.Services.GetService(typeof(IInputService));

            // Empty the list of screens to update.
            _screensToUpdate.Clear();

            // Add each screen to the end of the list of ones to update.
            foreach(TScreen screen in _screens)
                _screensToUpdate.Push(screen);

            // Keep track of whether screens have focus (and should handle input).  If the game isn't active, all
            // screens are out of focus.
            bool hasFocus = !this.Game.IsActive;
            // Keep track of whether screens are visible or not.  The topmost screen is visible, so we initialize
            // this to true.
            bool visible = true;

            // Loop through the list of screens.
            while (_screensToUpdate.Count > 0)
            {
                // Pop the topmost screen.
                TScreen screen = _screensToUpdate.Pop();

                // Have the screen update its state.
                screen.Update(gameTime, hasFocus, visible);

                // If the screen is shown on-screen...
                if (screen.ScreenState == ScreenState.Active ||
                    screen.ScreenState == ScreenState.TransitionOn)
                {
                    // If this is the top-most screen, make sure lower screens don't receive input events.
                    if (hasFocus)
                        input.InputHandled = true;

                    // If this screen is active and not a popup, let the rest
                    // of the screens know that they are covered up.
                    if (!screen.IsPopup)
                        visible = false;
                }
            }
        }

        /// <summary>
        /// Draw all of the screens managed by this screen manager.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            foreach (TScreen screen in _screens)
                if (screen.ScreenState != ScreenState.Hidden)
                    screen.Draw(gameTime);
        }

        /// <summary>
        /// Adds a screen to the screen manager.
        /// </summary>
        /// <param name="screen">The screen to add.</param>
        public void AddScreen(TScreen screen)
        {
            // Set the screen manager for the screen.
            screen.ScreenManager = this;
            
            // Have the screen initialize itself and load its content.
            if (isInitialized)
            {
                screen.Initialize();
                screen.LoadContent();
            }

            // Add the screen to the list.
            _screens.Add(screen);
        }

        /// <summary>
        /// Removes a screen from this screen manager.
        /// </summary>
        /// <param name="screen">The screen to remove.</param>
        public void RemoveScreen(TScreen screen)
        {
            // Have the screen unload its content.
            // TODO: Add this, if necessary.

            _screens.Remove(screen);
        }

        #region Properties

        public TScreen[] Screens
        {
            get { return _screens.ToArray(); }
        }

        #endregion
    }
}
