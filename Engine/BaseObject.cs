using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    public abstract class BaseObject
    {   
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }

        public int ID
        {
            get;
            internal set;
        }

        public abstract String getObjectType();



        



    }
}
