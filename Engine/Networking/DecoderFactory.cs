using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Mammoth.Engine.Networking
{
    static class DecoderFactory
    {
        public static Hashtable registeredObjects = new Hashtable();

        //when we have an object interface, we should make sure it has a constructor that registers that object with the factory
        public static void RegisterObject(int id, object theobject)
        {
            if(!registeredObjects.ContainsKey(id))
                registeredObjects.Add(id, theobject);
        }

        public static void UnregisterObject(int id)
        {
            registeredObjects.Remove(id);
        }

        public static void AnalyzeObjects(string type, int id, byte[] properties)
        {
            if (registeredObjects.ContainsKey(id))
                UpdateObject(id, properties);
            else
                CreateObject(type, id, properties); //how do we know where we want this object to go...
        }

        public static object CreateObject(string type, int id, byte[] properties)
        {
            switch (type)
            {
                case "Car":
                //do some stuff
                break;

                case "Driver":
                //do some stuff
                break;

                default:
                Console.WriteLine("Object type was not recognized");
                break;
            }
        }

        public static void UpdateObject(int id, byte[] properties)
        {

        }
    }
}
