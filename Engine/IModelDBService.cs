using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    public interface IModelDBService : IDisposable
    {
        /// <summary>
        /// Returns whether the model DB has an object with
        /// the specified ID.
        /// </summary>
        /// <param name="objectID"></param>
        /// <returns></returns>
        bool hasObject(int objectID);

        /// <summary>
        /// Returns the object associated with the given ID
        /// if there is one (throws an exception if no such
        /// object exists).
        /// </summary>
        /// <param name="objectID"></param>
        /// <returns></returns>
        BaseObject getObject(int objectID);

        /// <summary>
        /// Adds the specified object to the DB.
        /// </summary>
        /// <param name="newObject"></param>
        void registerObject(BaseObject newObject);

        /// <summary>
        /// Gets the list of all objects in the DB.
        /// </summary>
        List<BaseObject> AllObjects { get; }

        /// <summary>
        /// Returns an ID which is not in use in the DB, and
        /// is unique across the network (i.e. the model DB
        /// on any two different clients will never return the
        /// same value from this method).
        /// </summary>
        /// <returns></returns>
        int getNextOpenID();
    }
}
