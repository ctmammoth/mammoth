using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mammoth.Engine.Physics;

using StillDesign.PhysX;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth.Engine
{
    /// <summary>
    /// Represents any projectile in the game.
    /// </summary>
    public abstract class Projectile : BaseObject, IDamager
    {
        #region Properties

        public Game Game
        {
            get;
            protected set;
        }

        public Vector3 InitialPosition
        {
            get;
            protected set;
        }

        public Vector3 InitialDirection
        {
            get;
            protected set;
        }

        #endregion

        // Default
        protected Projectile(Game game)
        {
            Game = game;

        }

        #region IDamager Members

        // Default
        float IDamager.GetDamage()
        {
            return 0.0f;
        }

        #endregion
    }
}
