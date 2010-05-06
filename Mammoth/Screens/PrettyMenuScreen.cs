using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Interface;
using Mammoth.Engine.Audio;
using Mammoth.Engine;

namespace Mammoth
{
    public class PrettyMenuScreen : TWidgetScreen
    {
        public PrettyMenuScreen(Game game)
            : base(game)
        {

        }

        public override void Initialize()
        {
            // Get the renderer so we can load images.
            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));

            // Get the width and height of the window.
            int width = this.Game.Window.ClientBounds.Width;
            int height = this.Game.Window.ClientBounds.Height;

            // Calculate vector for scaling images to fit screen.
            Vector2 scaling = new Vector2(width / 1600.0f, height / 1066.0f);

            // Create a base widget (kinda like a JFrame or a JPanel that contains everything else).
            TWidget baseWid = new TImage(this.Game, r.LoadTexture("menu\\background"))
            {
                Bounds = new Rectangle(0, 0, width, height),
                Options = TWidget.WidgetOptions.Stretch
            };

            // Add a button to use to join a game.
            TButton playButton = new TImageButton(this.Game)
             {
                 NormalImage = r.LoadTexture("menu\\play_normal"),
                 HoverImage = r.LoadTexture("menu\\play_hover"),
                 DownImage = r.LoadTexture("menu\\play_down"),
                 Size = (new Vector2(200, 110) * scaling),
                 Center = new Vector2((int)(width * 0.2159f), (int)(height * 0.502f)),
                 Options = TWidget.WidgetOptions.Stretch
             };
            playButton.OnClick += new EventHandler(StartGame);
            baseWid.Add(playButton);

            // Add a button to use to quit the game.
            TButton exitButton = new TImageButton(this.Game)
            {
                NormalImage = r.LoadTexture("menu\\exit_normal"),
                HoverImage = r.LoadTexture("menu\\exit_hover"),
                DownImage = r.LoadTexture("menu\\exit_down"),
                Size = (new Vector2(200, 110) * scaling),
                Center = new Vector2((int)(width * 0.2159f), (int)(height * 0.621f)),
                Options = TWidget.WidgetOptions.Stretch
            };
            exitButton.OnClick += new EventHandler(Quit);
            baseWid.Add(exitButton);

            // Set the base widget for this screen.
            _baseWidget = baseWid;

            // Play the menu music
            IAudioService audio = (IAudioService)Game.Services.GetService(typeof(IAudioService));
            audio.playMusic("Main_Menu");

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
