using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mammoth.Engine.Interface;
using Mammoth.Engine.Graphics;
using Mammoth.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth.GameWidgets
{
    /// <summary>
    /// The display of health on the bottom-right of the GameScreen.
    /// </summary>
    public class HealthWidget : TWidget
    {
        private string Health;
        private InputPlayer LIP;
        private SpriteFont _healthFont;
        private Color timeColor;
        private string Old_Health;
        private IRenderService r;


        /// <summary>
        /// Initializes the Ammo widget by declaring services and rendering effects
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="p">The current LocalInputPlayer.</param>
        public HealthWidget(Game game, InputPlayer p)
            : base(game)
        {
            //load render effects
            r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));
            _healthFont = r.LoadFont("hud");
            timeColor = new Color(230, 230, 230, 230);

            //load player
            LIP = p;

            //update health
            UpdateHealth();
            this.BgImage = r.RenderFont(Health, new Vector2(0.0f, 0.0f), timeColor, Color.TransparentWhite, _healthFont);
        }

        /// <summary>
        /// Updates health is the LocalInputPlayer is available.
        /// </summary>
        public void UpdateHealth()
        {
            //store old health value
            Old_Health = Health;

            //get new health value
            if (LIP != null)
            {
                //get health information
                Health = "Health: " + LIP.Health;
            }
        }


        /// <summary>
        /// Overrides the update and redraws the widget on  any content change.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Update(GameTime gameTime)
        {
            UpdateHealth();
            if (Old_Health != Health)
            {
                this.BgImage = r.RenderFont(Health, new Vector2(0.0f, 0.0f), timeColor, Color.TransparentWhite, _healthFont);
            }
        }


    }
}
