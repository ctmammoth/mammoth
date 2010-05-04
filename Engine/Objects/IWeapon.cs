using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    public interface IWeapon
    {
        void Shoot(Vector3 position, Quaternion orientation);
        void Reload();
    }
}
