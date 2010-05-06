using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth.Engine
{
    public interface IRenderable
    {
        #region Properties

        Vector3 Position
        {
            get;
        }

        Vector3 PositionOffset
        {
            get;
        }

        Quaternion Orientation
        {
            get;
        }

        Model Model3D
        {
            get;
        }

        #endregion
    }
}
