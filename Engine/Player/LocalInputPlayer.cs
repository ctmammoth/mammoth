using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mammoth.Engine.Networking;
using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    public class LocalInputPlayer : InputPlayer
    {       
        public LocalInputPlayer(Game game, int clientID)
            : base(game)
        {
            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));

            this.ID = clientID << 25;

            this.Model3D = r.LoadModel("soldier-low-poly");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //Console.WriteLine("Position: " + this.Position);
        }
    }
}
