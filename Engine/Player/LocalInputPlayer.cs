using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    public class LocalInputPlayer : InputPlayer
    {
        #region Properties
        private float Health
        {
            get;
            set;
        }
        #endregion

        private void TakeDamage(float damage)
        {
            Health -= damage;
        }

        public LocalInputPlayer(Game game)
            : base(game)
        {
            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));

            this.Model3D = r.LoadModel("soldier-low-poly");
        }
    }
}
