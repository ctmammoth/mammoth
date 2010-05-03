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
        private static int nextID = 0;

        private Dictionary<int, BaseObject> _objects;

        public ModelDatabase(Game game) : base(game)
        {
            this.Game.Services.AddService(typeof(IModelDBService), this);
            _objects = new Dictionary<int, BaseObject>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var obj in _objects.Values)
                obj.Update(gameTime);
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
            _objects.Add(newObject.ID, newObject);
        }

        public bool removeObject(int objectID)
        {
            return _objects.Remove(objectID);
        }

        public int getNextOpenID()
        {
            INetworkingService net = (INetworkingService)this.Game.Services.GetService(typeof(INetworkingService));
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
