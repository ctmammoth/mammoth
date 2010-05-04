using System;
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
    public interface IDestructable
    {
        /// <summary>
        /// Peforms any tasks necessary to destroy this object.
        /// </summary>
        void Die();
    }
}
