using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mammoth.Engine.Networking;
using Mammoth.Engine.Audio;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
        public LocalInputPlayer(Game game, int clientID): base(game)
        {
            //Load the Model
            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));

            this.ID = clientID << 25;
            this.Model3D = r.LoadModel("soldier-low-poly");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Health <= 20)
            {
                IAudioService audio = (IAudioService)this.Game.Services.GetService(typeof(IAudioService));
                audio.loopSound("Heartbeat");
            }
        }

        public override void Die()
        {
            base.Die();

            IAudioService audio = (IAudioService)this.Game.Services.GetService(typeof(IAudioService));
            audio.stopSound("Heartbeat");
            audio.playSound("Scream");
        }
    }
}
