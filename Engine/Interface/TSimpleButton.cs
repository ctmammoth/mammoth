using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mammoth.Engine.ExtensionMethods;

namespace Mammoth.Engine.Interface
{
    public class TSimpleButton : TButton
    {
        #region Variables

        private string _text;

        #endregion

        public TSimpleButton(Game game, string text)
            : base(game)
        {
            _text = text;

            this.TextColor = Color.Black;

            this.NormalColor = Color.SlateBlue;
            this.HoverColor = Color.MediumSlateBlue;
            this.DownColor = Color.DarkKhaki;
            this.DisabledColor = Color.LightSlateGray;
        }

        public override void Initialize()
        {
            base.Initialize();

            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));
            this.BgImage = r.RenderFont(_text, Vector2.Zero, this.TextColor, Color.TransparentBlack);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!this.Enabled)
                this.BgColor = this.DisabledColor;
            else
            {
                switch (this.CurrentState)
                {
                    case State.Normal:
                        this.BgColor = this.NormalColor;
                        break;

                    case State.Hover:
                        this.BgColor = this.HoverColor;
                        break;

                    case State.Down:
                        this.BgColor = this.DownColor;
                        break;
                }
            }
        }

        #region Properties

        public Color TextColor
        {
            get;
            set;
        }

        public Color NormalColor
        {
            get;
            set;
        }

        public Color HoverColor
        {
            get;
            set;
        }

        public Color DownColor
        {
            get;
            set;
        }

        public Color DisabledColor
        {
            get;
            set;
        }

        #endregion
    }
}
