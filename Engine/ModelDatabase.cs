using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Mammoth.Engine.Networking;

namespace Mammoth.Engine
{
    class ModelDatabase : GameComponent, IModelDBService
    {
        private static int nextID = 0;

        private Dictionary<int, IEncodable> _objects;

        public ModelDatabase(Game game) : base(game)
        {
            this.Game.Services.AddService(typeof(IModelDBService), this);
            _objects = new Dictionary<int, IEncodable>();
        }

        #region IModelDBService Members

        public bool hasObject(int objectID)
        {
            return _objects.ContainsKey(objectID);
        }

        public IEncodable getObject(int objectID)
        {
            return _objects[objectID];
        }

        public void registerObject(IEncodable newObject)
        {
            _objects.Add(newObject.GetID(), newObject);
        }

        public bool removeObject(int objectID)
        {
            return _objects.Remove(objectID);
        }

        public int getNextOpenID()
        {
            return nextID++;
        }

        #endregion
    }
}
