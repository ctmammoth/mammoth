using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine.Interface
{
    /// <summary>
    /// An enumeration of the states that a screen can be in.
    /// </summary>
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden
    }

    /// <summary>
    /// A shameless rip of the GameScreen class from the XNA NetworkStateManagement sample.
    /// Some things have been changed, although for the most part, it's a copy.
    /// </summary>
    public abstract class TScreen
    {
        #region Widget Code

        GameComponentCollection _components = new GameComponentCollection();

        #endregion

        public TScreen(Game game)
        {
            this.Game = game;
        }

        public virtual void Initialize() { }

        public virtual void LoadContent() { }

        public virtual void Update(GameTime gameTime, bool hasFocus, bool visible)
        {
            // Keep track of whether this screen has focus or not.
            this.otherScreenHasFocus = !hasFocus;

            if (isExiting)
            {
                // Exiting involves transitioning off.
                screenState = ScreenState.TransitionOff;

                if (!UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // When the transition's done, remove the screen.
                    // TODO: Remove the screen.
                }
            }
            else if (!visible)
            {
                // If the screen is covered up, it should transition off-screen.
                screenState = ScreenState.TransitionOff;

                if (!UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // Once the transition's done, this screen's hidden.
                    screenState = ScreenState.Hidden;
                }
            }
            else
            {
                // The screen is therefore uncovered, and should transition on.
                if (UpdateTransition(gameTime, transitionOnTime, -1))
                {
                    // Keep transitioning.
                    screenState = ScreenState.TransitionOn;
                }
                else
                {
                    // We're done transitioning!
                    screenState = ScreenState.Active;
                }
            }
        }

        /// <summary>
        /// A helper function to determine whether we're still transitioning or have finished already.
        /// </summary>
        /// <param name="gameTime">The length of this update step.</param>
        /// <param name="time">The total time that this transition takes.</param>
        /// <param name="direction">The direction in which we're transitioning.</param>
        /// <returns>True if we're still transitioning, false otherwise.</returns>
        private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // Calculate how much to move by.
            float delta;
            if (time == TimeSpan.Zero)
                delta = 1;
            else
                delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                    time.TotalMilliseconds);

            transitionPosition += delta * direction;

            // Check to see if we're done transitioning yet.
            if ((direction < 0) && (transitionPosition <= 0) ||
                (direction > 0) && (transitionPosition >= 1))
            {
                transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
                return false;
            }
            else
                return true;
        }

        public virtual void Draw(GameTime gameTime) { }

        #region Properties

        /// <summary>
        /// A popup window doesn't fully obstruct things below it, so they should still be drawn and shouldn't
        /// transition offscreen.
        /// </summary>
        public bool IsPopup
        {
            get { return isPopup; }
            protected set { isPopup = value; }
        }

        bool isPopup = false;


        /// <summary>
        /// The amount of time that this screen should take to transition on once it's been activated.
        /// </summary>
        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }

        TimeSpan transitionOnTime = TimeSpan.Zero;


        /// <summary>
        /// The amount of time that this screen should take to transition off once it's been deactivated.
        /// </summary>
        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }

        TimeSpan transitionOffTime = TimeSpan.Zero;

        /// <summary>
        /// The current position of the screen's transition: 0 is fully on-screen,
        /// 1 is fully off-screen.
        /// </summary>
        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }

        float transitionPosition = 1.0f;


        /// <summary>
        /// The current alpha value for the transition (based on 
        /// </summary>
        public byte TransitionAlpha
        {
            get { return (byte)(255 - this.TransitionPosition * 255); }
        }

        /// <summary>
        /// The screen's current state.
        /// </summary>
        public ScreenState ScreenState
        {
            get { return screenState; }
            protected set { screenState = value; }
        }

        ScreenState screenState = ScreenState.TransitionOn;


        /// <summary>
        /// Some screens are just being hidden, some are getting removed.  If the screen
        /// is exiting for good, it will remove itself when its transition completes.
        /// Useful for things like in-game screens, where you'd want to free all of that
        /// memory when the game completes.
        /// </summary>
        public bool IsExiting
        {
            get { return isExiting; }
            protected internal set { isExiting = value; }
        }

        bool isExiting = false;


        public bool IsActive
        {
            get
            {
                return !otherScreenHasFocus &&
                    (this.ScreenState == ScreenState.TransitionOn || this.ScreenState == ScreenState.Active);
            }
        }

        bool otherScreenHasFocus;

        /// <summary>
        /// This returns the TScreenManager that this screen belongs to.
        /// </summary>
        public TScreenManager ScreenManager
        {
            get { return screenManager; }
            internal set { screenManager = value; }
        }

        TScreenManager screenManager;

        /// <summary>
        /// Stores the Game.  Useful for stuff like getting window/viewport dimensions.
        /// </summary>
        public Game Game
        {
            get;
            private set;
        }

        #endregion
    }
}
