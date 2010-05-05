using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Mammoth.Engine.Interface
{
    public abstract class TButton : TWidget
    {
        #region Enums

        public enum State
        {
            Normal,
            Hover,
            Down
        }

        #endregion

        #region Events

        public event EventHandler OnClick;

        #endregion

        public TButton(Game game)
            : base(game)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            MouseState mouse = Mouse.GetState();

            if(this.Bounds.Contains(new Point(mouse.X, mouse.Y)))
            {
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    if (this.OnClick != null)
                        this.OnClick(this, new EventArgs());
                    this.CurrentState = State.Down;
                }
                else
                    this.CurrentState = State.Hover;
            }
            else
                this.CurrentState = State.Normal;
        }

        #region Properties

        public State CurrentState
        {
            get;
            protected set;
        }

        #endregion
    }
}
