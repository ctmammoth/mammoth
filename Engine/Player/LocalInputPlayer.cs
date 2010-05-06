using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mammoth.Engine.Networking;
using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    /// <summary>
    /// An InputPlayer that draws itself on the client-side, acting as the client's player.
    /// </summary>
    public class LocalInputPlayer : InputPlayer
    {      
        /// <summary>
        /// Initializes the player and loads its model.
        /// </summary>
        /// <param name="game">The game</param>
        public LocalInputPlayer(Game game): base(game)
        {
            //Load the Model
            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));

            this.ID = clientID << 25;
            this.Model3D = r.LoadModel("soldier-low-poly");
        }
    }
}
