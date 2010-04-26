using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using System.Configuration;

namespace Mammoth.Engine
{
    public class Input
    {
        #region Fields

        public enum EvKeys
        {
            KEY_FORWARD,
            KEY_BACKWARD,
            KEY_LEFT,
            KEY_RIGHT,
            KEY_SPRINT,
            KEY_JUMP,
            KEY_CROUCH,
            KEY_RELOAD
        }

        private static Dictionary<EvKeys, Keys> _keyMappings;

        #endregion

        static Input()
        {
            _keyMappings = new Dictionary<EvKeys, Keys>();
        }

        public static void LoadSettings()
        {
            _keyMappings.Add(EvKeys.KEY_FORWARD, Keys.W);
            _keyMappings.Add(EvKeys.KEY_BACKWARD, Keys.S);
            _keyMappings.Add(EvKeys.KEY_LEFT, Keys.A);
            _keyMappings.Add(EvKeys.KEY_RIGHT, Keys.D);
            _keyMappings.Add(EvKeys.KEY_SPRINT, Keys.LeftShift);
            _keyMappings.Add(EvKeys.KEY_JUMP, Keys.Space);
        }

        public static bool IsKeyDown(EvKeys k)
        {
            return Keyboard.GetState().IsKeyDown(_keyMappings[k]);
        }
    }
}
