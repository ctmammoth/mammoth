using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    class ModelDatabase : GameComponent, IModelDBService
    {
        private Dictionary<int, object> _objects;

        public ModelDatabase(Game game) : base(game)
        {
            this.Game.Services.AddService(typeof(IModelDBService), this);
            _objects = new Dictionary<int, object>();
        }

        #region IModelDBService Members

        public bool hasObject(int objectID)
        {
            return _objects.ContainsKey(objectID);
        }

        public object getObject(int objectID)
        {
            return _objects[objectID];
        }

        public void registerObject(object newObject)
        {
            int id = 42;
            // TODO: use real objects with ids
            // int id = newObject.getID();
            _objects.Add(id, newObject);
        }

        public bool removeObject(int objectID)
        {
            return _objects.Remove(objectID);
        }

        #endregion
    }
}
