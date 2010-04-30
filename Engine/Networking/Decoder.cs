using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using Mammoth.Engine.Input;

namespace Mammoth.Engine.Networking
{
    public class Decoder : GameComponent, Mammoth.Engine.Networking.IDecoder
    {
        //define short-hand access to the master hashtable of objects
        public IModelDBService registeredObjects;
        public Game game;

        public Decoder(Game dagame) : base(dagame)
        {
            //get game
            game = dagame;

            //add this as a service
            //this.Game.Services.AddService(typeof(IDecoder), this);

            //get list of registered objects
            registeredObjects = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
        }

        public InputState DecodeInputState(byte[] data)
        {
            InputState state = new InputState();
            state.Decode(data);
            return state;
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
                case "Player":
                    RemotePlayer p = new RemotePlayer(game);
                    p.Decode(properties);
                    registeredObjects.registerObject(p);
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
