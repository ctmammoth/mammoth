using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    class RemotePlayer : Player
    {
        public RemotePlayer(Engine game) : base(game)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));
            ICameraService cam = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));

            // If you're using the first-person camera, don't draw your own geometry.
            if (cam.Type != Camera.CameraType.FIRST_PERSON)
                r.DrawRenderable(this);
        }
    }
}
