using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Mammoth.Core
{
    /// <summary>
    /// This class encapsulates the idea of a Controller that modifies a DynamicObject.
    /// </summary>
    /// <typeparam name="T">The type of object that the controller modifies.</typeparam>
    public abstract class Controller<T> where T : DynamicObject<T>
    {
        public abstract void Update(GameTime gameTime);

        //TODO: Fill in the bodies for Register() and Unregister() (once we have a kernel class).
        internal void Register() { }
        internal void Unregister() { }

        #region Properties

        //TODO: We should have each controller store a list of models, not just one.
        private T Model
        {
            get;
            set;
        }

        #endregion
    }
}
