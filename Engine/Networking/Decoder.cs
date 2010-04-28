using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Mammoth.Engine.Networking
{
    class Decoder : GameComponent
    {
        //define short-hand access to the master hashtable of objects
        public IModelDBService registeredObjects;

        public Decoder(Game game) : base(game)
        {
            registeredObjects = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
        }

        /// <summary>
        /// Checks if the object described in a packet exists and then performs an action. If the object does exist, the function updates
        /// the object's properties. If an object does not exist, it creates the object with the properties specified.
        /// </summary>
        /// <param name="type">A string representing the class of object received</param>
        /// <param name="id">An int representing the unique id of the object received</param>
        /// <param name="properties">A byte array that can be decoded by the IEncodable class of type "type" and contains the properties of the object</param>
        public void AnalyzeObjects(string type, int id, byte[] properties)
        {
            if (registeredObjects.hasObject(id) && registeredObjects.getObject(id) is IEncodable)
                UpdateObject(id, properties);
            else
                CreateObject(type, id, properties);
        }

        /// <summary>
        /// Creates new objects based on their type. This is temporary until a new part of the program does this.
        /// </summary>
        /// <param name="type">A string representing the class of object received</param>
        /// <param name="id">An int representing the unique id of the object received</param>
        /// <param name="properties">A byte array that can be decoded by the IEncodable class of type "type" and contains the properties of the object</param>
        public void CreateObject(string type, int id, byte[] properties)
        {
            switch (type)
            {
                case "Car":
                //do some stuff and get a default
                //decode properties
                //register with game
                break;

                case "Driver":
                //do some stuff and get a default
                //decode properties
                //register with game
                break;

                default:
                Console.WriteLine("Object type was not recognized");
                break;
            }
        }

        /// <summary>
        /// Updates an object with the id given by decoding the byte array.
        /// </summary>
        /// <param name="id">An int representing the unique id of the object received</param>
        /// <param name="properties">A byte array that can be decoded by the IEncodable class of type "type" and contains the properties of the object</param>
        public void UpdateObject(int id, byte[] properties)
        {
            IEncodable toupdate = (IEncodable) registeredObjects.getObject(id);
            toupdate.Decode(properties);
        }

    }
}
