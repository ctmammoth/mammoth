﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;

using Mammoth.Engine;

namespace Mammoth.Engine
{
    /// <summary>
    /// Represents any object which can be destroyed.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// The quantified health of the object.
        /// </summary>
        float Health
        {
            get;
            set;
        }

        /// <summary>
        /// Deals damage to this damageable.
        /// </summary>
        /// <param name="damage">The amount of damage to deal: negative damage adds health to the damageable.</param>
        void TakeDamage(float damage, IDamager inflicter);

        /// <summary>
        /// Defines the action of an object once Health reaches 0.
        /// </summary>
        void Die();
    }
}
