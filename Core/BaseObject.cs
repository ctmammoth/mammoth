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
            _id = _nextID++;
        }

        #region Properties

        public int ID
        {
            get
            {
                return _id;
            }
        }

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

        #endregion
    }
}
