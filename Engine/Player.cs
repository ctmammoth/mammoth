using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth.Engine
{
    public abstract class Player : DrawableGameComponent, IRenderable
    {
        public Player(Game game) : base(game)
        {
            
        }

        #region Properties

        public Vector3 Position
        {
            get;
            internal set;
        }

        public Quaternion Orientation
        {
            get;
            internal set;
        }

        public Model Model3D
        {
            get;
            internal set;
        }

        public float Height
        {
            get;
            protected set;
        }

        #endregion
    }
}
