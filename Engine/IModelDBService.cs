using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    public interface IModelDBService : IDisposable
    {
        bool hasObject(int objectID);
        BaseObject getObject(int objectID);
        void registerObject(BaseObject newObject);
        //bool removeObject(int objectID);
        bool isVisibleToPlayer(int objectID, int playerID);
        List<BaseObject> AllObjects { get; }

        int getNextOpenID();
    }
}
