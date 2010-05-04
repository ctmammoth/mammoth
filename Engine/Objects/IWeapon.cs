using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    public interface IWeapon
    {
        void Shoot(Quaternion orientation);
    }
}
