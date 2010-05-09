using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;

using Mammoth.Engine.Input;
using Mammoth.Engine.Interface;
using Mammoth.Engine.Physics;
using Mammoth.Engine.Networking;

namespace Mammoth.Engine.Objects
{
    class CompoundStaticObject : PhysicalObject, IEncodable, IRenderable
    {

        public CompoundStaticObject(Game game) : base(game)
        {

        }

        public override string getObjectType()
        {
            throw new NotImplementedException();
        }

        #region IEncodable Members

        public byte[] Encode()
        {
            throw new NotImplementedException();
        }

        public void Decode(byte[] serialized)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IRenderable Members


        public Vector3 PositionOffset
        {
            get { throw new NotImplementedException(); }
        }

        public Model Model3D
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
