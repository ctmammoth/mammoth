using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;

namespace Mammoth.Core
{
    /// <summary>
    /// This class encapsulates the idea of a Controller that modifies a single DynamicObject.
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

            // Register the controller.
            this.Register();
        }

        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// This registers the controller with the Kernel so that it can receive update events.
        /// </summary>
        public void Register()
        {
            Kernel.Instance.RegisterProcess(this);
        }

        #region Properties

        public bool IsAlive
        {
            get
            {
                return this.Model.IsAlive;
            }
        }

        public T Model
        {
            get;
            protected set;
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
