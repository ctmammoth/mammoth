using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mammoth.Engine.Networking;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth.Engine
{
    /// <summary>
    /// Provides a way to create BaseObjects both locally and from the Networks
    /// </summary>
    public class ObjectFactories
    {

        
        private static IModelDBService modelDB;

        /// <summary>
        /// Sets the current modelDB to that of this game
        /// </summary>
        /// <param name="db">The Model Database of the current Game</param>
        public static void InitializeDB(IModelDBService db)
        {
            modelDB = db;
        }


        /// <summary>
        /// Creates a blank BaseObject with the Specified ID, but does not initialize its fields, and instead
        /// it uses decode to get proper properties.
        /// </summary>
        /// <param name="type">Type of object to be created</param>
        /// <param name="id">ID Number to give this object</param>
        /// <param name="data">What is to be decoded</param>
        /// <param name="game">The current game</param>
        /// <returns>The object that the network sent you</returns>
        public static IEncodable CreateObjectFromNetwork(String type, int id, Byte[] data, Game game)
        {
            IEncodable theObject = (IEncodable)CreateObject(type, id, null, game);
            if (data != null)
            {
                theObject.Decode(data);
            }
            return theObject;
        }

        /// <summary>
        /// Creates an object locally, using set parameters that you give it, instead of 
        /// decoding one from the network
        /// </summary>
        /// <param name="type">Type of the object to create</param>
        /// <param name="parameters">The paramaters that you want this object to have</param>
        /// <param name="game">the current game</param>
        /// <returns>An object with the specified parameters</returns>
        public static IEncodable CreateLocalObject(String type, ObjectParameters parameters, Game game)
        {
            int id = modelDB.getNextOpenID();
            IEncodable theObject = CreateObjectFromNetwork(type, id, null, game);
            return theObject;

        }
        
        

        /// <summary>
        /// Determines which type of object is to be created, and then delegates to the 
        /// correct factory, and returns its output
        /// </summary>
        /// <param name="type">Type of requested object</param>
        /// <param name="id">ID of the new object</param>
        /// <param name="parameters">parameters the new object is to have. if null,
        /// (i.e. this is being created from the network), 
        /// the factory will take care of initializing a blank object  </param>
        /// <param name="game">the current game</param>
        /// <returns>The object the type's factory returns</returns>
        public static BaseObject CreateObject(String type, int id, ObjectParameters parameters, Game game)
        {
            BaseObject theObject = null;
            switch (type)
            {
                case "Static_Object":
                    theObject = RealStaticObjectFactory(id, parameters, game);
                    break;
                case "WallBlock":
                    theObject = WallBlockFactory(id, parameters, game);
                    break;
                case "Fortress":
                    theObject = FortressFactory(id, parameters, game);
                    break;
            }
            return theObject;

        }

        /// <summary>
        /// Creates a WallBlock object with the specified parameters, and if parameters are
        /// null it initializes a blank WallBlock
        /// </summary>
        /// <param name="id">ID of the wallblock</param>
        /// <param name="parameters">Parameters to give the new WallBlock (if null, 
        /// this returns a blank WallBlock</param>
        /// <param name="game">The current game</param>
        /// <returns>The requested WallBlcok</returns>
        public static WallBlock WallBlockFactory(int id, ObjectParameters parameters, Game game)
        {
            if (parameters != null)
            {
                return new WallBlock(id, parameters, game);
            }
            else
            {
                return new WallBlock(id, game);
            }

        }

        /// <summary>
        /// Creates a RealStaticObject object with the specified parameters, and if parameters are
        /// null it initializes a blank RealStaticObject
        /// </summary>
        /// <param name="id">ID of the RealStaticObject</param>
        /// <param name="parameters">Parameters to give the new RealStaticObject (if null, 
        /// this returns a blank RealStaticObject</param>
        /// <param name="game">The current game</param>
        /// <returns>The requested RealStaticObject</returns>
        public static RealStaticObject RealStaticObjectFactory(int id, ObjectParameters parameters, Game game)
        {
            if (parameters != null)
            {
                return new RealStaticObject(id, parameters, game, false);
            }
            else
            {
                return new RealStaticObject(id, parameters, game, true);
            }

        }

        /// <summary>
        /// Currently, Fortresses are implemented as Individual rooms, so this isn't Used
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public static Fortress FortressFactory(int id, ObjectParameters parameters, Game game)
        {
            return null;
        }








    }
}
