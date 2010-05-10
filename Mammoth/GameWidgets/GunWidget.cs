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
    /// The display of gun on the bottom-right of the GameScreen.
    /// </summary>
    public class GunWidget : TWidget
    {
        private string Gun;
        private string Old_Gun;
        private InputPlayer LIP;
        private SpriteFont _gunFont;
        private Color timeColor;
        private IRenderService r;

        /// <summary>
        /// Initializes the Gun widget by declaring services and rendering effects
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="p">The current LocalInputPlayer.</param>
        public GunWidget(Game game, InputPlayer p)
            : base(game)
        {
            //load render effects
            r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));
            _gunFont = r.LoadFont("hud");
            timeColor = new Color(230, 230, 230, 230);

            //load player
            LIP = p;

            //update gun count
            UpdateGun();
            this.BgImage = r.RenderFont(Gun, new Vector2(0.0f, 0.0f), timeColor, Color.TransparentWhite, _gunFont);
        }


        /// <summary>
        /// Updates the gun as long as the local player is available.
        /// </summary>
        public void UpdateGun()
        {
            //store old ammo value
            Old_Gun = Gun;

            //get new ammo value
            if (LIP != null)
            {
                //get ammo information
                Gun = "Gun: " + LIP.CurWeapon.getObjectType();
            }
        }

        /// <summary>
        /// Overrides the update and redraws the widget on  any content change.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Update(GameTime gameTime)
        {
            UpdateGun();
            if (Old_Gun != Gun)
            {
                this.BgImage = r.RenderFont(Gun, new Vector2(0.0f, 0.0f), timeColor, Color.TransparentWhite, _gunFont);
            }
        }


    }
}
