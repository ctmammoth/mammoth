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
        // The projectile's velocity in the global coordinate system
        public Vector3 GlobalVelocity
        {
            get
            {
                return Actor.LinearVelocity;
            }
            protected set
            {
                Actor.LinearVelocity = value;
            }
        }

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

        protected float InitialVelocity;
        #endregion

        // TODO
        public override void CollideWith(PhysicalObject obj)
        {
            throw new NotImplementedException();
        }

        // TODO
        public override string getObjectType()
        {
            throw new NotImplementedException();
        }

        #region IDamager Members

        float IDamager.GetDamage()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
