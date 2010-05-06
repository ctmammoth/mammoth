using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    public abstract class BaseObject
    {
        /// <summary>
        /// Updates for the object for every timestep.
        /// </summary>
        /// <param name="gameTime">The official GameTime.</param>
        public virtual void Update(GameTime gameTime) { }

        /// <summary>
        /// How to draw the object.
        /// </summary>
        /// <param name="gameTime">The official GameTime.</param>
        public virtual void Draw(GameTime gameTime) { }


        /// <summary>
        /// Supplies every object with a unique ID which can be used to access the object in the ModelDB.
        /// </summary>
        public int ID
        {
            get;
            internal set;
        }
        
        /// <summary>
        /// Returns a string that identifies the object type. Used in sending an IEncodable and reconstructing it to the proper type.
        /// </summary>
        /// <returns></returns>
        public abstract String getObjectType();
    }
}
