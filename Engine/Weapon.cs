using System;
using System.Collections.Generic;
using System.Text;

namespace Mammoth.Engine
{
    public abstract class Weapon : IRenderable
    {
        public void Fire()
        {
            throw new System.NotImplementedException();
        }

        public void Reload()
        {
            throw new System.NotImplementedException();
        }

        #region IRenderable Members

        public Microsoft.Xna.Framework.Vector3 Position
        {
            get { throw new NotImplementedException(); }
        }

        public Microsoft.Xna.Framework.Quaternion Orientation
        {
            get { throw new NotImplementedException(); }
        }

        public Microsoft.Xna.Framework.Graphics.Model Model3D
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
