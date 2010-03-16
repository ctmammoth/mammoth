using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mammoth.Core
{
    /// <summary>
    /// This class encapsulates the idea of a dynamic object in the engine.  This means that it has
    /// a controller that can modify its properties.
    /// </summary>
    /// <typeparam name="T">The type of the object.  Subclasses of this class should set themselves as T.</typeparam>
    public abstract class DynamicObject<T> : BaseObject where T : DynamicObject<T>
    {
        #region Variables

        private Controller<T> _controller = null;

        #endregion

        public DynamicObject() : base()
        {
            
        }

        /// <summary>
        /// This method should be overridden so that it creates a Controller for the object,
        /// sets the Model's _controller variable to that Controller object, and registers
        /// the Controller with the Kernel.  It should be called at the end of the object's
        /// constructor.
        /// </summary>
        protected abstract void CreateController();

        #region Properties

        public Controller<T> Controller
        {
            get
            {
                return _controller;
            }
        }

        #endregion
    }
}
