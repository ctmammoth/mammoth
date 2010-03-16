using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth.Core
{
    public abstract class BaseObject
    {
        #region Variables

        private readonly int _id;
        private static int _nextID = 0;

        #endregion

        public BaseObject()
        {
            // Set the unique ID for this object.
            _id = _nextID;
            _nextID++;
        }

        #region Properties

        public int ID
        {
            get
            {
                return _id;
            }
        }

        //TODO: Write an interface that provides position and orientation.
        // We're not going to be using Pos/Orient, but an interface that allows
        // us to put stuff into an octree (scene manager of some sort).  That interface
        // will probably define the same basic properties, though.
        public Vector3 Position
        {
            get;
            set;
        }

        public Quaternion Orientation
        {
            get;
            set;
        }

        //TODO: We should probably move visual representation out of the base class.
        // We will probably want to have objects that don't get drawn, so they shouldn't contain
        // a model.  Then again, it could always just never be set and never be used...  Should we
        // make some sort of IDrawable interface?
        /// <summary>
        /// This is the 3D model used to draw this object.
        /// </summary>
        public Model Model3D
        {
            get;
            protected set;
        }

        #endregion
    }
}
