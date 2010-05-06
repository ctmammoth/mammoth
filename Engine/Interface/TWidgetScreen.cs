﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine.Interface
{
    public class TWidgetScreen : TScreen
    {
        #region Fields

        protected TWidget _baseWidget;

        #endregion

        public TWidgetScreen(Game game)
            : base(game)
        {

        }

        public override void Initialize()
        {
            base.Initialize();

            _baseWidget.Initialize();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool hasFocus, bool visible)
        {
            base.Update(gameTime, hasFocus, visible);

            // TODO: Change TWidget's update so that it can update and deal with input events separately.
            // This involves making TWidgets NOT DrawableGameComponents, but emulating some of that functionality.
            if(visible && hasFocus)
                _baseWidget.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            _baseWidget.Draw(gameTime);
        }

    }
}
