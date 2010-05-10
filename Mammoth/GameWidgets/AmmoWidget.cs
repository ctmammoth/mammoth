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
    /// The display of ammo on the bottom-right of the GameScreen.
    /// </summary>
    public class AmmoWidget : TWidget
    {
        private string Ammo;
        private string Old_Ammo;
        private InputPlayer LIP;
        private SpriteFont _ammoFont;
        private Color timeColor;
        private IRenderService r;

        /// <summary>
        /// Initializes the Ammo widget by declaring services and rendering effects
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="p">The current LocalInputPlayer.</param>
        public AmmoWidget(Game game, InputPlayer p): base(game)
        {
            //load render effects
            r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));
            _ammoFont = r.LoadFont("hud");
            timeColor = new Color(230, 230, 230, 230);

            //load player
            LIP = p;

            //update ammo count
            UpdateAmmo();
            this.BgImage = r.RenderFont(Ammo, new Vector2(0.0f, 0.0f), timeColor, Color.TransparentWhite, _ammoFont);
        }


        /// <summary>
        /// Updates the amo as long as the local player is available.
        /// </summary>
        public void UpdateAmmo()
        {
            //store old ammo value
            Old_Ammo = Ammo;

            //get new ammo value
            if (LIP != null)
            {
                //get ammo information
                Ammo = "Ammo: " + LIP.CurWeapon.MagCount;
            }
        }

        /// <summary>
        /// Overrides the update and redraws the widget on  any content change.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Update(GameTime gameTime)
        {
            UpdateAmmo();
            if (Old_Ammo != Ammo)
            {
                this.BgImage = r.RenderFont(Ammo, new Vector2(0.0f, 0.0f), timeColor, Color.TransparentWhite, _ammoFont);
            }
        }


    }
}
