using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    public interface IModelDBService
    {
        // TODO: change object to our object class
        bool hasObject(int objectID);
        object getObject(int objectID);
        void registerObject(object newObject);
        bool removeObject(int objectID);
    }
}
