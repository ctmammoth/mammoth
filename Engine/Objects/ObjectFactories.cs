using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mammoth.Engine.Networking;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth.Engine
{
    public class ObjectFactories
    {

        private static IModelDBService registeredObjects;

        public static void InitializeDB(IModelDBService db)
        {
            registeredObjects = db;
        }


        public static IEncodable CreateObjectFromNetwork(String type, int id, Byte[] data, Game game)
        {
            IEncodable theObject = (IEncodable)CreateObject(type, id, null, game);
            if (data != null)
            {
                theObject.Decode(data);
            }
            return theObject;
        }

        public static IEncodable CreateLocalObject(String type, ObjectParameters parameters, Game game)
        {
            int id = registeredObjects.getNextOpenID();
            IEncodable theObject = CreateObjectFromNetwork(type, id, null, game);
            return theObject;

        }

        
        public static int content_test(Game game)
        {
            /*
            ObjectParameters parameters = new ObjectParameters();
            parameters.AddAttribute("X", "666");
            parameters.AddAttribute("Y", "420");
            parameters.AddAttribute("Z", "69");
            parameters.AddAttribute("Crate_Type", "SMALL");
            Crate testCrate = (Crate)CreateObject("Crate", 69, null, game);
            Crate testCrate2 = (Crate)CreateObject("Crate", 631, parameters, game);
            int i = 0;
             * * */
            return 0;
             

        }

        public static BaseObject CreateObject(String type, int id, ObjectParameters parameters, Game game)
        {
            BaseObject theObject = null;
            switch (type)
            {
                case "Crate":
                    theObject = CrateFactory(id, parameters, game);
                    break;
                case "Fortress":
                    theObject = FortressFactory(id, parameters, game);
                    break;
            }
            return theObject;

        }

        public static Crate CrateFactory(int id, ObjectParameters parameters, Game game)
        {
            if (parameters != null)
            {
                return new Crate(id, parameters, game);
            }
            else
            {
                return new Crate(id, game);
            }


        }

        public static Fortress FortressFactory(int id, ObjectParameters parameters, Game game)
        {
            return null;
        }








    }
}
