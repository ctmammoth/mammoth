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

        public EndScreen(Game game, GameStats g) : base(game)
        {
            GameStats = g;
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

            // Add a button to use to join a game.
            TButton mscreen = new TSimpleButton(this.Game, "Back to Main Menu")
            {
                Size = new Vector2(120, 50),
                Center = new Vector2(this.Game.Window.ClientBounds.Width / 2, 250)
            };
            mscreen.OnClick += new EventHandler(ToMMenu);
            baseWid.Add(mscreen);


            // Set the base widget for this screen.
            _baseWidget = baseWid;

            // Initialize the base widget and children.
            base.Initialize();
        }

        /// <summary>
        /// Returns to main menu
        /// </summary>
        public void ToMMenu(object sender, EventArgs e)
        {
            this.ScreenManager.AddScreen(new PrettyMenuScreen(this.Game));
            this.IsExiting = true;
        }
    }
}
