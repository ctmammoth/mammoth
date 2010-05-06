using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth.Engine.Interface
{
    public class TImageButton : TButton
    {
        public TImageButton(Game game)
            : base(game)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (this.CurrentState)
            {
                case State.Normal:
                    this.BgImage = this.NormalImage;
                    break;

                case State.Hover:
                    this.BgImage = this.HoverImage;
                    break;

                case State.Down:
                    this.BgImage = this.DownImage;
                    break;

                default:
                    this.BgImage = this.NormalImage;
                    break;
            }
        }

        #region Properties

        public Texture2D NormalImage
        {
            get;
            set;
        }

        public Texture2D HoverImage
        {
            get;
            set;
        }

        public Texture2D DownImage
        {
            get;
            set;
        }

        #endregion
    }
}
