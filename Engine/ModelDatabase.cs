using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Mammoth.Engine.Networking;

namespace Mammoth.Engine
{
    public class ModelDatabase : DrawableGameComponent, IModelDBService, IDisposable
    {
        // This must not be zero since the player will have ID = 0
        private static int nextID = 1;

        // The objects in this database
        private Dictionary<int, BaseObject> _objects;

        /// <summary>
        /// Creates the dictionary of objects and adds this DB as
        /// a service.
        /// </summary>
        /// <param name="game"></param>
        public ModelDatabase(Game game) : base(game)
        {
            // Add this service to the game
            this.Game.Services.AddService(typeof(IModelDBService), this);
            _objects = new Dictionary<int, BaseObject>();
        }

        /// <summary>
        /// Updates all objects in the database and
        /// removes any which are no longer alive.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // Update all objects
            var toUpdate = new List<BaseObject>(_objects.Values);

            // Remove any objects which are no longer alive.
            foreach (var obj in toUpdate)
            {
                if (obj.IsAlive)
                    obj.Update(gameTime);
                else
                {
                    _objects.Remove(obj.ID);
                    obj.Dispose();
                }
            }
        }

        /// <summary>
        /// Draws all the alive objects in the DB.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            foreach (var obj in _objects.Values)
                if(obj.IsAlive)
                    obj.Draw(gameTime);
        }

        /// <summary>
        /// See IModelDBService for method descriptions.
        /// </summary>
        /// <param name="objectID"></param>
        /// <returns></returns>
        #region IModelDBService Members

        public bool hasObject(int objectID)
        {
            return _objects.ContainsKey(objectID);
        }

        public BaseObject getObject(int objectID)
        {
            return _objects[objectID];
        }

        public void registerObject(BaseObject newObject)
        {
            if (!_objects.ContainsKey(newObject.ID))
                _objects.Add(newObject.ID, newObject);
        }

        public int getNextOpenID()
        {
            INetworkingService net = (INetworkingService)this.Game.Services.GetService(typeof(INetworkingService));
            return net.ClientID << 25 | nextID++;
        }

        public List<BaseObject> AllObjects
        {
            // TODO: Make this a read only list
            get { return _objects.Values.ToList<BaseObject>(); }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Calls dispose on everything in the DB.
        /// </summary>
        void IDisposable.Dispose()
        {
            foreach (BaseObject obj in _objects.Values)
                obj.Dispose();

            this.Game.Services.RemoveService(typeof(IModelDBService));

            base.Dispose();
        }

        #endregion
    }
}
