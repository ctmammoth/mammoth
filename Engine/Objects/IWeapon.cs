using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mammoth.Engine.Networking;

namespace Mammoth.Engine
{
    public interface IWeapon
    {
        void Shoot(Vector3 position, Vector3 direction);
        void Reload();
    }
}
