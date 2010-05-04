using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mammoth.Engine.Networking;

namespace Mammoth.Engine
{
    public class ObjectFactories
    {
        
        private static IModelDBService registeredObjects;
        
        public static void InitializeDB(IModelDBService db) 
        {
            registeredObjects = db;
        }
        
 
        public static IEncodable CreateObjectFromNetwork(String type, int id, Byte[] data)
        {
            IEncodable theObject = CreateObject(type,id,null);
            if (data != null)
            {
                theObject.Decode(data);
            }            
            return theObject;
        }

        public static IEncodable CreateLocalObject(String type, ObjectParameters parameters)
        {
            int id = registeredObjects.getNextOpenID();
            IEncodable theObject = CreateObjectFromNetwork(type, id, null);
            return theObject;

        }



        public static IEncodable CreateObject(String type, int id, ObjectParameters parameters)
        {
            IEncodable theObject = null;
            switch (type)
            {
                case "Crate":
                    theObject = CrateFactory(id,parameters);
                    break;
                case "Fortress":
                    theObject = FortressFactory(id,parameters);
                    break;
            }
            return theObject;
            
        }

        public static Crate CrateFactory(int id, ObjectParameters parameters)
        {
            return new Crate(id,parameters);
        }

        public static Fortress FortressFactory(int id, ObjectParameters parameters)
        {
            return null;
        }






      

    }
}
