using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    public class LocalInputPlayer : InputPlayer
    {
        public LocalInputPlayer(Game game)
            : base(game)
        {
            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));

            this.Model3D = r.LoadModel("soldier-low-poly");
        }
    }
}
