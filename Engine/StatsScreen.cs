using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Mammoth.Engine.Input;
using Mammoth.Engine.Interface;
using Mammoth.Engine;

namespace Mammoth.Engine
{
    public class StatsScreen : TWidgetScreen
    {

        private int numKills;
        private int numCaptures;
        private int numDeaths;
        private IGameLogic gl;
        private int playerID;


        public StatsScreen(Game game, int nk, int nc, int nd, int id, IGameLogic g)
            : base(game)
        {
            numKills = nk; numCaptures = nc; numDeaths = nd; playerID = id;  gl = g;
        }

        public override void Initialize()
        {
            // Create a base widget (kinda like a JFrame or a JPanel that contains everything else).
            TWidget baseWid = new TWidget(this.Game)
            {
                Bounds = this.Game.Window.ClientBounds
            };



            try
            {
                // Add a Leading team header
                baseWid.Add(new TText(this.Game, "Leading Team: " + gl.GetLeadingTeam().ToString())
                {
                    Center = new Vector2(this.Game.Window.ClientBounds.Width / 4, 50)
                });

                baseWid.Add(new TText(this.Game, "Team Kills: " + gl.GetLeadingTeam().GetKills())
                {
                    Center = new Vector2(this.Game.Window.ClientBounds.Width / 4, 100)
                });

                baseWid.Add(new TText(this.Game, "Team Captures: " + gl.GetLeadingTeam().GetCaptures())
                {
                    Center = new Vector2(this.Game.Window.ClientBounds.Width / 4, 150)
                });


                baseWid.Add(new TText(this.Game, "Team Total: " + gl.GetLeadingTeam().GetTeamPoints())
                {
                    Center = new Vector2(this.Game.Window.ClientBounds.Width / 4, 200)
                });



                // Add a Trailing team header
                baseWid.Add(new TText(this.Game, "Trailing Team: " + gl.GetTrailingTeam().ToString())
                {
                    Center = new Vector2((3 * this.Game.Window.ClientBounds.Width) / 4, 50)
                });

                baseWid.Add(new TText(this.Game, "Team Kills: " + gl.GetTrailingTeam().GetKills())
                {
                    Center = new Vector2((3 * this.Game.Window.ClientBounds.Width) / 4, 100)
                });

                baseWid.Add(new TText(this.Game, "Team Captures: " + gl.GetTrailingTeam().GetCaptures())
                {
                    Center = new Vector2((3 * this.Game.Window.ClientBounds.Width) / 4, 150)
                });

                baseWid.Add(new TText(this.Game, "Team Total: " + gl.GetTrailingTeam().GetTeamPoints())
                {
                    Center = new Vector2((3 * this.Game.Window.ClientBounds.Width) / 4, 200)
                });


                //Personal stats
                baseWid.Add(new TText(this.Game, "Personal Stats")
                {
                    Center = new Vector2((this.Game.Window.ClientBounds.Width) / 2, 300)
                });

                baseWid.Add(new TText(this.Game, "Personal Kills: " + numKills)
                {
                    Center = new Vector2((2 * this.Game.Window.ClientBounds.Width) / 5, 350)
                });

                baseWid.Add(new TText(this.Game, "Personal Captures: " + numCaptures)
                {
                    Center = new Vector2((3 * this.Game.Window.ClientBounds.Width) / 5, 350)
                });

                baseWid.Add(new TText(this.Game, "Personal Deaths: " + numDeaths)
                {
                    Center = new Vector2((4 * this.Game.Window.ClientBounds.Width) / 5, 350)
                });


                //Your Team
                baseWid.Add(new TText(this.Game, "Your Team: " + gl.GetTeamOf(playerID))
                {
                    Center = new Vector2((4 * this.Game.Window.ClientBounds.Width) / 5, 350)
                });
            }
            catch (Exception e)
            {
                baseWid.Add(new TText(this.Game, "NOT CONNECTED TO SERVER")
                {
                    Center = new Vector2((this.Game.Window.ClientBounds.Width) / 2, 200)
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
