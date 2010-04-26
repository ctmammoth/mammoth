using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mammoth.Engine.ExtensionMethods;

namespace Mammoth.Engine.Interface
{
    public class TWidget : DrawableGameComponent
    {
        #region Fields

        private List<TWidget> _children;

        private Rectangle _bounds;

        [Flags]
        public enum AlignmentFlags
        {
            Center = 0x00,
            Top = 0x01,
            Left = 0x02,
            TopLeft = Top | Left,
            Right = 0x04,
            TopRight = Top | Right,
            Bottom = 0x08,
            BottomLeft = Bottom | Left,
            BottomRight = Bottom | Right
        }

        [Flags]
        public enum WidgetOptions
        {
            Stretch = 0x01,
            NoAutoLayout = 0x02
        }

        #endregion

        #region Events

        public event EventHandler OnClick;

        #endregion

        public TWidget(Game game)
            : base(game)
        {
            this.BgColor = null;
            this.BgImage = null;

            _bounds = Rectangle.Empty;
            _children = new List<TWidget>();
        }

        public override void Draw(GameTime gameTime)
        {
            if (!this.Visible)
                return;

            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));

            if (this.BgColor != null)
                r.DrawFilledRectangle(this.Bounds, (Color) this.BgColor);

            if (this.BgImage != null)
            {
                // TODO: Move all of this crap out of the draw method - we only should calculate it once (per change of alignment).

                // Deal with stretching and alignment here.
                if ((this.Options & WidgetOptions.Stretch) == WidgetOptions.Stretch)
                    r.DrawTexturedRectangle(this.Bounds, this.BgImage);
                else
                {
                    // Set initial (centered) values.
                    int width = this.BgImage.Width;
                    int height = this.BgImage.Height;
                    int x = (int)(this.Center.X - (width / 2.0f));
                    int y = (int)(this.Center.Y - (height / 2.0f));

                    // Deal with bottom-aligned images.
                    if (IsAligned(AlignmentFlags.Bottom))
                        y = this.Bounds.Bottom - this.BgImage.Height;

                    // Deal with top-aligned images.
                    if (IsAligned(AlignmentFlags.Top))
                        y = this.Bounds.Top;

                    // Deal with right-aligned images.
                    if (IsAligned(AlignmentFlags.Right))
                        x = this.Bounds.Right - this.BgImage.Width;

                    // Deal with left-aligned images.
                    if (IsAligned(AlignmentFlags.Left))
                        x = this.Bounds.Left;

                    // Deal with stretching.
                    if (IsAligned(AlignmentFlags.Top | AlignmentFlags.Bottom))
                        height *= 2;
                    if (IsAligned(AlignmentFlags.Left | AlignmentFlags.Right))
                        width *= 2;

                    r.DrawTexturedRectangle(new Rectangle(x, y, width, height), this.BgImage);
                }
            }

            Paint(gameTime);

            foreach (TWidget wid in _children)
                wid.Draw(gameTime);
        }

        // TODO: documentation.
        /// <summary>
        /// This method recursively goes through the tree of TWidgets, laying out each one of them if necessary and then 
        /// </summary>
        /// <returns><code>true</code> if the layout of this component has changed, <code>false</code> otherwise.</returns>
        public bool Layout()
        {
            // If we have Update set Enabled to false at the end of updating, we can effectively use
            // Update to handle laying out components.  When the layout for an object needs to change, it
            // can set Enabled to true, and Update will re-layout the component.
            foreach(TWidget child in _children)
                this.Dirty |= child.Layout();

            if ((this.Options & WidgetOptions.NoAutoLayout) != WidgetOptions.NoAutoLayout)
            {
                // TODO: Figure out how Layout is going to work.  Not sure what to do here yet.
                if (this.Dirty == true)
                {
                    // Lay out the component here.
                }
                else
                {
                    // Do something else here.
                }
            }

            bool ret = this.Dirty;
            this.Dirty = false;
            return ret;
        }

        public void Add(TWidget wid)
        {
            if (!_children.Contains(wid))
                _children.Add(wid);
        }

        public bool Remove(TWidget wid)
        {
            if (!_children.Contains(wid))
                return false;
            else
            {
                _children.Remove(wid);
                return true;
            }
        }

        public bool Remove(string name)
        {
            TWidget wid = _children.Find((w) => w.Name == name);
            if (wid != null)
            {
                _children.Remove(wid);
                return true;
            }
            else
                return false;
        }

        public TWidget GetChild(string name)
        {
            return _children.Find((w) => w.Name == name);
        }

        protected virtual void Paint(GameTime gameTime)
        {

        }

        public bool IsAligned(AlignmentFlags flag)
        {
            return ((this.Alignment & flag) == flag);
        }

        #region Properties

        public string Name
        {
            get;
            set;
        }

        public Nullable<Color> BgColor
        {
            get;
            set;
        }

        public Texture2D BgImage
        {
            get;
            protected set;
        }

        public Rectangle Bounds
        {
            get { return _bounds; }
            set { _bounds = value; }
        }

        public Vector2 Center
        {
            get { return new Vector2(_bounds.Center.X, _bounds.Center.Y); }
            set
            {
                _bounds.Location = (value - (new Vector2(this.Width, this.Height) / 2.0f)).ToPoint();
                this.Dirty = true;
            }
        }

        public Point Location
        {
            get { return _bounds.Location; }
            set
            {
                _bounds.Location = value;
                this.Dirty = true;
            }
        }

        public Vector2 Size
        {
            get { return new Vector2(_bounds.Width, _bounds.Height); }
            set
            {
                _bounds.Width = (int)value.X;
                _bounds.Height = (int)value.Y;
                this.Dirty = true;
            }
        }

        public int Height
        {
            get { return _bounds.Height; }
            set
            {
                _bounds.Height = value;
                this.Dirty = true;
            }
        }

        public int Width
        {
            get { return _bounds.Width; }
            set
            {
                _bounds.Width = value;
                this.Dirty = true;
            }
        }

        public bool Dirty
        {
            get;
            protected set;
        }

        public AlignmentFlags Alignment
        {
            get;
            set;
        }

        public WidgetOptions Options
        {
            get;
            set;
        }

        #endregion
    }
}
