using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;
using Microsoft.Xna.Framework;

using Mammoth.Engine.Physics;

namespace Mammoth.Engine
{
    public class Bullet : Projectile
    {
        public Bullet(Quaternion orientation)
        {
            Vector3 velocity = Vector3.UnitZ;
            velocity = Vector3.Transform(velocity, orientation);
            velocity = Vector3.Multiply(velocity, InitialVelocity);
        }
    }
}
