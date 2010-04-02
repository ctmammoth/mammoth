using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth.Engine
{
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

            Renderer.Instance.DrawObject(this);
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

        #endregion
    }

    class SoldierObject : StaticObject
    {
        public SoldierObject(Game game) : base(game)
        {
            this.Model3D = Renderer.Instance.LoadModel("soldier-low-poly");
            this.Position = Vector3.Zero;
            this.Orientation = Quaternion.Identity;
        }
    }
}
