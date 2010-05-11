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
    /// The display of time on the top of the GameScreen.
    /// </summary>
    public class GameTimeWidget : TWidget
    {
        private string Time;
        private string Old_Time;
        private GameStats g;
        private SpriteFont _timeFont;
        private Color timeColor;
        private IRenderService r;
        private InputPlayer LIP;

        /// <summary>
        /// Initializes the GameTime widget by declaring services and rendering effects
        /// </summary>
        /// <param name="game">The game.</param>
        public GameTimeWidget(Game game, InputPlayer p): base(game)
        {
            //Load InputPlayer
            LIP = p;

            //load render effects
            r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));
            _timeFont = r.LoadFont("timer");
            timeColor = new Color(Color.Red, 200);

            //Load GameStats to get time
            g = (GameStats)this.Game.Services.GetService(typeof(GameStats));

            //update the time
            UpdateTime();
            
            //make an initial graphic
            this.BgImage = r.RenderFont(Time, new Vector2(0.0f, 0.0f), timeColor, Color.TransparentWhite, _timeFont);
        }

        /// <summary>
        /// Updates the time by converting seconds to the string to display
        /// </summary>
        public void UpdateTime()
        {
            //set Old_Time so that changes may be detected
            Old_Time = Time;

            //calculate minutes and seconds
            int minutes = g.TimeLeft / 60;
            int seconds = g.TimeLeft % 60;
            Time = MakeTwoDigits(minutes) + ":" + MakeTwoDigits(seconds);
        }

        /// <summary>
        /// Makes an int a two digit string if the number is one or two digits long
        /// </summary>
        /// <param name="num">The int to be converted</param>
        /// <returns>A two digit string.</returns>
        private string MakeTwoDigits(int num)
        {
            if (num < 10)
                return "0" + num;
            return num + "";
        }

        /// <summary>
        /// Overrides the update and redraws the widget on  any content change.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Update(GameTime gameTime)
        {
            UpdateTime();
            if (!Old_Time.Equals(Time))
            {
                //Load GameTime color
                if (LIP.PlayerStats.YourTeam.ToString() == "Team 1")
                    timeColor = new Color(Color.Red, 200);
                else
                    timeColor = new Color(Color.Blue, 200);

                this.BgImage = r.RenderFont(Time, new Vector2(0.0f, 0.0f), timeColor, Color.TransparentWhite, _timeFont);
            }

        }

        
    }
}
