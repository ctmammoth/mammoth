using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Mammoth.Engine.Graphics;

namespace Mammoth.Engine
{
    /// <summary>
    /// Deprecated, used in the A-level stuff
    /// </summary>
    class StaticObject : DrawableGameComponent, IRenderable
    {
        public StaticObject(Game game)
            : base(game)
        {
            this.Enabled = false;
            this.Visible = true;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Renderer r = (Renderer) this.Game.Services.GetService(typeof(IRenderService));

            r.DrawRenderable(this);
        }

        #region Properties

        public Vector3 Position
        {
            get;
            internal set;
        }

        public Vector3 PositionOffset
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

        #endregion
    }

    class SoldierObject : StaticObject
    {
        public SoldierObject(Game game) : base(game)
        {
            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));

            this.Model3D = r.LoadModel("soldier-low-poly");
            this.Position = Vector3.Zero;
            this.PositionOffset = Vector3.Zero;
            this.Orientation = Quaternion.Identity;
        }
    }
}
