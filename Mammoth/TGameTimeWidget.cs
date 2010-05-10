using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mammoth.Engine.Interface;
using Mammoth.Engine.Graphics;
using Mammoth.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth
{
    public class TGameTimeWidget : TWidget
    {
        public string Time;

        public TGameTimeWidget(Game game): base(game)
        {
            UpdateTime();
        }

        public void UpdateTime()
        {
            //get time in string form
            IGameStats gstatus = (IGameStats)this.Game.Services.GetService(typeof(IGameStats));
            int minutes = gstatus.TimeLeft / 60;
            int seconds = gstatus.TimeLeft % 60;
            Time = minutes + ":" + seconds;
        }

        public override void Update(GameTime gameTime)
        {
            UpdateTime();
            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));
            SpriteFont _timeFont = r.LoadFont("timer");
            Color timeColor = new Color(230,230,230, 230);
            this.BgImage = r.RenderFont(Time, new Vector2(0.0f, 0.0f), timeColor , Color.TransparentWhite, _timeFont);
        }

        
    }
}
