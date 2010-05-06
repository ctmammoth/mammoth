using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth.Engine.Interface
{
    public class TImage : TWidget
    {
        public TImage(Game game, Texture2D image)
            : base(game)
        {
            this.BgImage = image;
        }
    }
}
