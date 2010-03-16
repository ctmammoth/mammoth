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
    public abstract class Controller<T> : IProcess where T : DynamicObject<T>
    {
        #region Variables

        private static int _nextID = 0;
        private readonly int _id;

        #endregion

        public Controller()
        {
            // Set the unique ID for this controller.
            _id = _nextID++;
        }

        public abstract void Update(GameTime gameTime);

        //TODO: Fill in the bodies for Register() and Unregister() (once we have a kernel class).
        public void Register() { }
        public void Unregister() { }

        #region Properties

        //TODO: We should have each controller store a list of models, not just one.
        private T Model
        {
            get;
            set;
        }

        public int ID
        {
            get
            {
                return _id;
            }
        }

        #endregion
    }
}
