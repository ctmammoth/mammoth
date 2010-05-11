using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using Mammoth.Engine.Input;
using Mammoth.Engine.Objects;

namespace Mammoth.Engine.Networking
{
    public class Decoder : GameComponent, IDecoder
    {
        //define short-hand access to the master hashtable of objects
        //public IModelDBService registeredObjects;
        public Game game;

        public Decoder(Game dagame) : base(dagame)
        {
            //get game
            game = dagame;

            //add this as a service
            this.Game.Services.AddService(typeof(IDecoder), this);

            //get list of registered objects
            //registeredObjects = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
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
            IModelDBService ro = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));

            //Console.WriteLine("Receiving item of type " + type + " with id: " + id);

            //if (registeredObjects.hasObject(id) && registeredObjects.getObject(id) is IEncodable)
            if (ro.hasObject(id))
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
            IModelDBService ro = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));

            switch (type)
            {
                case "Player":
                    RemotePlayer p = new RemotePlayer(game);
                    p.Decode(properties);
                    p.ID = id;
                    ro.registerObject(p);
                break;

                //TODO: make sure any new bullet types are put in here
                case "RevolverBullet":
                    Bullet rb = new RevolverBullet(this.Game, Vector3.Zero, Quaternion.Identity, 0);
                    rb.Decode(properties);
                    rb.ID = id;
                    ro.registerObject(rb);
                    //Console.WriteLine("Bullet received, position: " + rb.Position + ", orientation: " + rb.Orientation + 
                        //", ID = " + id);
                break;

                case "SMGBullet":
                    Bullet sb = new SMGBullet(this.Game, Vector3.Zero, Quaternion.Identity, 0);
                    sb.Decode(properties);
                    sb.ID = id;
                    ro.registerObject(sb);
                    //Console.WriteLine("Bullet received, position: " + sb.Position + ", orientation: " + sb.Orientation +
                        //", ID = " + id);
                break;

                case "ShotgunBullet":
                    Bullet shb = new ShotgunBullet(this.Game, Vector3.Zero, Quaternion.Identity, 0);
                    shb.Decode(properties);
                    shb.ID = id;
                    ro.registerObject(shb);
                    //Console.WriteLine("Bullet received, position: " + shb.Position + ", orientation: " + shb.Orientation +
                        //", ID = " + id);
                break;

                case "Flag":
                    Flag flag = new Flag(this.Game, Vector3.Zero, 0);
                    flag.Decode(properties);
                    ro.registerObject(flag);
                    //Console.WriteLine("Flag received, position: " + flag.Position + ", positionoffset: " + flag.PositionOffset +
                     //   ", ID = " + id);
                break;

                case "GameStats":
                    //Console.WriteLine("Game stats recieved");
                    GameStats g = (GameStats)this.Game.Services.GetService(typeof(GameStats));
                    g.Decode(properties);
                break;

                case "Room":
                    //Console.WriteLine("Room recieved");
                    Room r = new Room(id, this.Game);
                    r.Decode(properties);
                break;

                case "RealStaticObject":
                Console.WriteLine("RSO recieved");
                RealStaticObject rso = new RealStaticObject(this.game);
                rso.Decode(properties);
                break;

                default:
                Console.WriteLine("Object type was not recognized: " + type);
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
            IModelDBService ro = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            IEncodable toupdate = (IEncodable)ro.getObject(id);
            toupdate.Decode(properties);
        }

    }
}
