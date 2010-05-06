using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mammoth.Engine;
using Mammoth.Engine.Graphics;
using Mammoth.Engine.ContentProcessors;

namespace Mammoth
{
    public class Terrain : BaseObject, IRenderable
    {
        public Terrain(Game game)
            : base(game)
        {
            this.Model3D = this.Game.Content.Load<Model>("textures\\terrain");
            this.Position = new Vector3(0.0f, 25.0f, 0.0f);
            this.Orientation = Quaternion.Identity;
        }

        public override void Draw(GameTime gameTime)
        {
            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));
            this.Game.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            this.Game.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
            r.DrawRenderable(this);
        }

        public override string getObjectType()
        {
            return "Terrain";
        }

        #region Properties

        public Vector3 Position
        {
            get;
            private set;
        }

        public Quaternion Orientation
        {
            get;
            private set;
        }

        public Vector3 PositionOffset
        {
            get { return Vector3.Zero; }
        }

        public Model Model3D
        {
            get;
            private set;
        }

        #endregion
    }
}
