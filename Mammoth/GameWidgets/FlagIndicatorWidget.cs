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
        public class FlagIndicatorWidget : TWidget
        {
            private bool HasFlag;
            private bool Old_HasFlag;
            private InputPlayer LIP;
            private IRenderService r;

            public FlagIndicatorWidget(Game game, InputPlayer p)
                : base(game)
            {
                //load render effects
                r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));

                //load player
                LIP = p;

                //update flag indicator
                UpdateFlagIndicator();
            }

            public void UpdateFlagIndicator()
            {
                //store old health value
                Old_HasFlag = HasFlag;

                //get new health value
                if (LIP != null)
                {
                    if (LIP.Flag != null)
                        HasFlag = true;
                    else
                        HasFlag = false;
                }
            }

            public override void Update(GameTime gameTime)
            {
                UpdateFlagIndicator();
                if (Old_HasFlag != HasFlag)
                {
                    this.BgImage = r.LoadTexture("flag");
                }
            }


        }
    }
