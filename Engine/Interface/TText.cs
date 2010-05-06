using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mammoth.Engine.ExtensionMethods;

namespace Mammoth.Engine.Interface
{
    public class TText : TWidget
    {
        #region Fields

        private string _text;
        private Color _textColor;

        #endregion

        public TText(Game game, string text)
            : this(game, text, Color.Black)
        {
            
        }

        public TText(Game game, string text, Color color)
            : base(game)
        {
            // Get an instance of the renderer and use it to render the font to this.BgImage.
            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));

            _text = text;
            _textColor = color;

            this.BgImage = r.RenderFont(_text, Vector2.Zero, _textColor, Color.TransparentBlack);
        }
    }
}
