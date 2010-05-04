using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine
{
    public class Crate : PhysicalObject
    {
        private int width, height, length, x, y, z;

        public override void CollideWith(PhysicalObject obj)
        {
        }

        public override string getObjectType()
        {
            throw new NotImplementedException();
        }
    }
}
