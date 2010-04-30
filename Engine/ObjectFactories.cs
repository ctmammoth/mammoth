using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine
{
    class ObjectFactories
    {
        public static Crate makeCrate(double x, double y, double z, double w, double h, double l)
        {
            String parameters = "";
            parameters = parameters + "x:" + x + " y:" + y + "z: " + z + " w:" + w + " h:" + h + " l:" + l;

            XmlHandler.CreateFromXml("Crate", parameters);
        }

    }
}
