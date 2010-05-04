﻿using System;
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
    public abstract class Projectile : PhysicalObject, IDamager
    {
        #region Properties

        // Not needed for sufficiently fast-moving projectiles
        public Model Model3D
        {
            get;
            protected set;
        }

        public Game Game
        {
            get;
            protected set;
        }

        #endregion

        #region Variables
        protected float InitialVelocity;
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