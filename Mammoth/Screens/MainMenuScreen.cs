using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Interface;

namespace Mammoth
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

            // Add a text widget to the menu.
            baseWid.Add(new TText(this.Game, "MAIN MENU!!!!")
            {
                Center = new Vector2(this.Game.Window.ClientBounds.Width / 2, 150)
            });

            // Add a button to use to join a game.
            TButton joinButton = new TSimpleButton(this.Game, "Join Game")
            {
                Size = new Vector2(120, 50),
                Center = new Vector2(this.Game.Window.ClientBounds.Width / 2, 250)
            };
            joinButton.OnClick += new EventHandler(StartGame);
            baseWid.Add(joinButton);

            // Add a button to use to quit the game.
            TButton quitButton = new TSimpleButton(this.Game, "Quit Game")
            {
                Size = new Vector2(120, 50),
                Center = new Vector2(this.Game.Window.ClientBounds.Width / 2, 310)
            };
            quitButton.OnClick += new EventHandler(Quit);
            baseWid.Add(quitButton);

            // Set the base widget for this screen.
            _baseWidget = baseWid;

            // Initialize the base widget and children.
            base.Initialize();
        }

        /// <summary>
        /// Quit the game.
        /// </summary>
        public void Quit(object sender, EventArgs e)
        {
            // TODO: There's probably a "safer"/cleaner way of doing this.
            // One possibility is leaving this call but writing a few handlers that tie into
            // the Game.Exiting event.
            this.Game.Exit();
        }

        /// <summary>
        /// Creates a new GameScreen and adds it to the screen manager.
        /// </summary>
        public void StartGame(object sender, EventArgs e)
        {
            this.ScreenManager.AddScreen(new GameScreen(this.Game));
        }
    }
}
