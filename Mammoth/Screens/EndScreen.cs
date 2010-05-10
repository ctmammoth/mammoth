using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mammoth.Engine.Input;
using Mammoth.Engine.Interface;
using Mammoth.Engine;

namespace Mammoth.Screens
{
    public class EndScreen : TWidgetScreen
    {
        private GameStats GameStats;

        public EndScreen(Game game) : base(game)
        {
            GameStats = (GameStats)this.Game.Services.GetService(typeof(GameStats));
        }

        public override void Initialize()
        {
            // Create a base widget (kinda like a JFrame or a JPanel that contains everything else).
            TWidget baseWid = new TWidget(this.Game)
            {
                Bounds = this.Game.Window.ClientBounds,
                //BgColor = new Color(0.0f, 0.0f, 0.0f, 0.5f)
            };

            // Add winning team
            baseWid.Add(new TText(this.Game, "Winning Team: " + GameStats.LeadingTeam)
            {
                Center = new Vector2(this.Game.Window.ClientBounds.Width / 2, 50)
            });

            // Add losing team
            baseWid.Add(new TText(this.Game, "Losing Team: " + GameStats.TrailingTeam)
            {
                Center = new Vector2(this.Game.Window.ClientBounds.Width / 2, 100)
            });

            // Set the base widget for this screen.
            _baseWidget = baseWid;

            // Initialize the base widget and children.
            base.Initialize();
        }

        public override void Update(GameTime gameTime, bool hasFocus, bool visible)
        {
            if (hasFocus)
            {
                    //this.IsExiting = true;
            }

            base.Update(gameTime, hasFocus, visible);
        }
    }
}
