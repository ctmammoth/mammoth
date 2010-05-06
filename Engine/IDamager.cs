using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine
{
    /// <summary>
    /// Represents anything which can deal damage.
    /// </summary>
    public interface IDamager
    {
        /// <summary>
        /// Returns the amount of damage dealt by this object.
        /// </summary>
        /// <returns>The amount of damage dealt by this object.  A positive value indicates damage being dealt
        /// whereas a negative value indicates health being given back to an object.</returns>
        float GetDamage();
    }
}
