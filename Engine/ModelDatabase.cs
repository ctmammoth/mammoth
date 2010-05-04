using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Mammoth.Engine.Networking;

namespace Mammoth.Engine
{
    public class ModelDatabase : DrawableGameComponent, IModelDBService
    {
        // This must not be zero since the player will have nextID = 0!
        private static int nextID = 1;
        // Used to avoid modifying _objects during an Update() call
        // TODO: may need to do something similar when removing objects
        private bool isUpdating;
        private Queue<BaseObject> toRegister;
        private Queue<int> toRemove;
        // The objects in this database
        private Dictionary<int, BaseObject> _objects;

        public ModelDatabase(Game game) : base(game)
        {
            // Not updating initially
            isUpdating = false;
            toRegister = new Queue<BaseObject>();
            toRemove = new Queue<int>();

            // Add this service to the game
            this.Game.Services.AddService(typeof(IModelDBService), this);
            _objects = new Dictionary<int, BaseObject>();
        }

        /// <summary>
        /// Updates all objects in the database.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // Currently updating
            isUpdating = true;

            // Update all objects
            foreach (var obj in _objects.Values)
                obj.Update(gameTime);

            // Done updating
            isUpdating = false;

            // Add objects that are waiting to be registered
            while (toRegister.Count > 0)
                registerObject(toRegister.Dequeue());

            while (toRemove.Count > 0)
                removeObject(toRemove.Dequeue());
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var obj in _objects.Values)
                obj.Draw(gameTime);
        }

        #region IModelDBService Members

        public bool hasObject(int objectID)
        {
            //Console.WriteLine("has object?");
            return _objects.ContainsKey(objectID);
        }

        public BaseObject getObject(int objectID)
        {
            return _objects[objectID];
        }

        public void registerObject(BaseObject newObject)
        {
            if (isUpdating)
                toRegister.Enqueue(newObject);
            else
                _objects.Add(newObject.ID, newObject);
        }

        public bool removeObject(int objectID)
        {
            if (isUpdating)
            {
                if (_objects.ContainsKey(objectID))
                {
                    toRemove.Enqueue(objectID);
                    return true;
                }
                return false;
            }
            else
                return _objects.Remove(objectID);
        }

        public int getNextOpenID()
        {
            INetworkingService net = (INetworkingService)this.Game.Services.GetService(typeof(INetworkingService));
            Console.WriteLine("ID = " + (net.ClientID << 25 | (nextID + 1)));
            return net.ClientID << 25 | nextID++;
        }

        public bool isVisibleToPlayer(int objectID, int playerID)
        {
            //TODO: make this do something
            return true;
        }

        #endregion
    }
}
