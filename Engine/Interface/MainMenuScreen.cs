using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine.Interface
{
    public class MainMenuScreen : TWidgetScreen
    {
        public MainMenuScreen(Game game)
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

            baseWid.Add(new TText(this.Game, "MAIN MENU!!!!")
            {
               Center = new Vector2(this.Game.Window.ClientBounds.Width / 2, 150)
            });

            TButton joinButton = new TSimpleButton(this.Game, "Join Game")
            {
                Size = new Vector2(120, 50),
                Center = new Vector2(this.Game.Window.ClientBounds.Width / 2, 250)
            };
            baseWid.Add(joinButton);

            TButton quitButton = new TSimpleButton(this.Game, "Quit Game")
            {
                Size = new Vector2(120, 50),
                Center = new Vector2(this.Game.Window.ClientBounds.Width / 2, 310)
            };
            quitButton.OnClick += new EventHandler(Quit);
            baseWid.Add(quitButton);

            _baseWidget = baseWid;

            // Initialize the base widget and children.
            base.Initialize();
        }
        
        public void Quit(object sender, EventArgs e)
        {
            this.Game.Exit();
        }
    }
}
