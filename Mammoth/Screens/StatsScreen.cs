using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Interface;

namespace Mammoth.Screens
{
    public class StatsScreen : TWidgetScreen
    {
        public StatsScreen(Game game)
            : base(game)
        {

        }

        public override void Initialize()
        {
            // Create a base widget (kinda like a JFrame or a JPanel that contains everything else).
            TWidget baseWid = new TWidget(this.Game)
            {
                Bounds = this.Game.Window.ClientBounds
            };





            // Add a Leading team header
            baseWid.Add(new TText(this.Game, "Leading Team: " + "Team 2")
            {
                Center = new Vector2(this.Game.Window.ClientBounds.Width / 4, 50)
            });

            baseWid.Add(new TText(this.Game, "Team Kills: " + "0")
            {
                Center = new Vector2(this.Game.Window.ClientBounds.Width / 4, 100)
            });

            baseWid.Add(new TText(this.Game, "Team Captures: " + "0")
            {
                Center = new Vector2(this.Game.Window.ClientBounds.Width / 4, 150)
            });








            // Add a Trailing team header
            baseWid.Add(new TText(this.Game, "Trailing Team: " + "Team 1")
            {
                Center = new Vector2((3 * this.Game.Window.ClientBounds.Width) / 4, 50)
            });

            baseWid.Add(new TText(this.Game, "Team Kills: " + "0")
            {
                Center = new Vector2((3 * this.Game.Window.ClientBounds.Width) / 4, 100)
            });

            baseWid.Add(new TText(this.Game, "Team Captures: " + "0")
            {
                Center = new Vector2((3 * this.Game.Window.ClientBounds.Width) / 4, 150)
            });




            //Personal stats
            baseWid.Add(new TText(this.Game, "Personal Stats")
            {
                Center = new Vector2((this.Game.Window.ClientBounds.Width) / 2, 300)
            });

            baseWid.Add(new TText(this.Game, "Personal Captures: " + "0")
            {
                Center = new Vector2((this.Game.Window.ClientBounds.Width) / 4, 350)
            });

            baseWid.Add(new TText(this.Game, "Personal Deaths: " + "0")
            {
                Center = new Vector2((3 * this.Game.Window.ClientBounds.Width) / 4, 350)
            });




            // Set the base widget for this screen.
            _baseWidget = baseWid;

            // Initialize the base widget and children.
            base.Initialize();
        }

    }
}
