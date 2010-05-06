using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine.ExtensionMethods
{
    public static class InterfaceExtensionMethods
    {
        public static Point ToPoint(this Vector2 vec)
        {
            return new Point((int) vec.X, (int) vec.Y);
        }

        public static Vector2 ToVector(this Point p)
        {
            return new Vector2(p.X, p.Y);
        }
    }
}
