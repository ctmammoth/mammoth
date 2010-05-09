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
    public abstract class Projectile : PhysicalObject, IDamager, System.IDisposable
    {
        #region Properties

        public int Creator
        {
            get;
            protected set;
        }

        #endregion

        // Default
        protected Projectile(Game game, int creator) 
            : base(game)
        {
            Creator = creator;
        }

        public abstract float GetDamage();

        public override string getObjectType()
        {
            return "Projectile";
        }
    }
}
