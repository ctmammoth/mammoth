using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mammoth.Engine.Input;
using Mammoth.Engine.Interface;
using Mammoth.Engine;

namespace Mammoth.Engine
{
    public class StatsScreen : TWidgetScreen
    {

        private IGameStats GameStats;


        public StatsScreen(Game game): base(game)
        {
            GameStats = (IGameStats)game.Services.GetService(typeof(IGameStats));
        }

        public override void Initialize()
        {
            // Create a base widget (kinda like a JFrame or a JPanel that contains everything else).
            TWidget baseWid = new TWidget(this.Game)
            {
                Bounds = this.Game.Window.ClientBounds,
                //BgColor = new Color(0.0f, 0.0f, 0.0f, 0.5f)
            };



            try
            {

                // Add a Leading team header
                baseWid.Add(new TText(this.Game, "Leading Team: " + GameStats.LeadingTeam)
                {
                    Center = new Vector2(this.Game.Window.ClientBounds.Width / 4, 50)
                });

                baseWid.Add(new TText(this.Game, "Team Kills: " + GameStats.LeadingTeam_NumKills)
                {
                    Center = new Vector2(this.Game.Window.ClientBounds.Width / 4, 100)
                });

                baseWid.Add(new TText(this.Game, "Team Captures: " + GameStats.LeadingTeam_NumCaptures)
                {
                    Center = new Vector2(this.Game.Window.ClientBounds.Width / 4, 150)
                });


                baseWid.Add(new TText(this.Game, "Team Total: " + GameStats.LeadingTeam_NumPoints)
                {
                    Center = new Vector2(this.Game.Window.ClientBounds.Width / 4, 200)
                });








                // Add a Trailing team header
                baseWid.Add(new TText(this.Game, "Trailing Team: " + GameStats.TrailingTeam)
                {
                    Center = new Vector2((3 * this.Game.Window.ClientBounds.Width) / 4, 50)
                });

                baseWid.Add(new TText(this.Game, "Team Kills: " + GameStats.TrailingTeam_NumKills)
                {
                    Center = new Vector2((3 * this.Game.Window.ClientBounds.Width) / 4, 100)
                });

                baseWid.Add(new TText(this.Game, "Team Captures: " + GameStats.TrailingTeam_NumCaptures)
                {
                    Center = new Vector2((3 * this.Game.Window.ClientBounds.Width) / 4, 150)
                });

                baseWid.Add(new TText(this.Game, "Team Total: " + GameStats.TrailingTeam_NumPoints)
                {
                    Center = new Vector2((3 * this.Game.Window.ClientBounds.Width) / 4, 200)
                });


                //Personal stats
                baseWid.Add(new TText(this.Game, "Personal Stats")
                {
                    Center = new Vector2((this.Game.Window.ClientBounds.Width) / 2, 300)
                });

                baseWid.Add(new TText(this.Game, "Personal Kills: " + GameStats.NumKills)
                {
                    Center = new Vector2((1 * this.Game.Window.ClientBounds.Width) / 6, 350)
                });

                baseWid.Add(new TText(this.Game, "Personal Captures: " + GameStats.NumCaptures)
                {
                    Center = new Vector2((3 * this.Game.Window.ClientBounds.Width) / 6, 350)
                });

                baseWid.Add(new TText(this.Game, "Personal Deaths: " + GameStats.NumDeaths)
                {
                    Center = new Vector2((5 * this.Game.Window.ClientBounds.Width) / 6, 350)
                });


                //Your Team
                baseWid.Add(new TText(this.Game, "Your Team: " + GameStats.YourTeam)
                {
                    Center = new Vector2((this.Game.Window.ClientBounds.Width) / 2, 400)
                });

            }
            catch (Exception e)
            {
                //Default
                baseWid.Add(new TText(this.Game, "NOT CONNECTED TO SERVER")
                {
                    Center = new Vector2((this.Game.Window.ClientBounds.Width) / 2, 150)
                });
            }

            // Set the base widget for this screen.
            _baseWidget = baseWid;

            // Initialize the base widget and children.
            base.Initialize();
        }

        public override void Update(GameTime gameTime, bool hasFocus, bool visible)
        {
            if (hasFocus)
            {
                IInputService input = (IInputService)this.Game.Services.GetService(typeof(IInputService));
                InputState iState = input.States.Peek();
                if (iState.KeyReleased(InputType.Stats))
                    this.IsExiting = true;
            }

            base.Update(gameTime, hasFocus, visible);
        }

        public bool IsPopup
        {
            get
            {
                return true;
            }
        }
    }
}
