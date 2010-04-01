using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    class Engine
    {
        // Needs a camera instance variable.
        // Needs a group of methods (or one with an enum) for determining which type of camera to use.
        #region Variables

        private static Engine _instance = null;

        #endregion

        public Engine(Game game)
        {
            this.Game = game;
            _instance = this;
        }

        #region Properties

        public static Engine Instance
        {
            get
            {
                return _instance;
            }
        }

        public Game Game
        {
            get;
            private set;
        }

        public LocalPlayer LocalPlayer
        {
            get;
            private set;
        }

        public bool PhysXDebug
        {
            get;
            set;
        }

        #endregion
    }
}
