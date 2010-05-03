using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Engine
{
    public class ObjectFactories
    {
        private static IModelDBService registeredObjects = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
 
        public static BaseObject CreateObjectFromNetwork(String type, int id, Byte[] data)
        {
            BaseObject theObject = CreateObject(type,id,null);
            if (data != null)
            {
                theObject.Decode(data);
            }            
            return theObject;
        }

        public static BaseObject CreateLocalObject(String type, ObjectParameters parameters)
        {
            int id = registeredObjects.getNextOpenID();
            BaseObject theObject = CreateObjectFromNetwork(type, id, null);
            return theObject;

        }



        public BaseObject CreateObject(String type, int id, ObjectParameters parameters)
        {
            BaseObject theObject;
            switch (type)
            {
                case "Crate":
                    theObject = CrateFactory(id,parameters);
                    break;
                case "Fortress":
                    theObject = FortressFactory(id,parameters);
                    break;
            }
            
        }

        public Crate CrateFactory(int id, ObjectParameters parameters)
        {
            return new Crate(id,parameters);
        }





        public static Crate makeCrate(double x, double y, double z, double w, double h, double l)
        {
            String parameters = "";
            parameters = parameters + "x:" + x + " y:" + y + "z: " + z + " w:" + w + " h:" + h + " l:" + l;

            XmlHandler.CreateFromXml("Crate", parameters);
        }

    }
}
