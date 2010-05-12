using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mammoth.Engine;
using Mammoth.Engine.Graphics;
using Mammoth.Engine.Interface;

namespace Mammoth
{
    public sealed class NVidiaSplashScreen : TWidgetScreen
    {
        public NVidiaSplashScreen(Game game)
            : base(game)
        {
            this.TransitionOnTime = new TimeSpan(0, 0, 2);
            this.TransitionOffTime = new TimeSpan(0, 0, 1);
        }

        public override void Initialize()
        {
            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));

            _baseWidget = new TWidget(this.Game)
            {
                Size = new Vector2(this.Game.Window.ClientBounds.Width, this.Game.Window.ClientBounds.Height),
                Location = Point.Zero,
                BgColor = Color.White
            };

            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));

            Texture2D image = r.LoadTexture("nvidia-physx");

            _baseWidget.Add(new TImage(this.Game, image)
            {
                Size = new Vector2(this.Game.Window.ClientBounds.Width, this.Game.Window.ClientBounds.Height),
                Location = Point.Zero,
                BgColor = Color.White,
                Alignment = TWidget.AlignmentFlags.Center
            });
        }

        public override void Update(GameTime gameTime, bool hasFocus, bool visible)
        {
            if (this.ScreenState == ScreenState.Active)
                this.IsExiting = true;

            base.Update(gameTime, hasFocus, visible);
        }
    }
}
