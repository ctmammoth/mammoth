using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Mammoth.Core;

namespace Mammoth.Engine
{
    /**
     * This is a simple and fairly inefficient camera class taken from the camera class in the PhysX.Net samples.  It
     * could be improved so that it stores more information and therefore has to do less calculation each update cycle,
     * but that shouldn't be worried about unless it is deemed to be an issue (by way of profiling).
     */
    class Camera : DynamicObject<Camera>
    {

        #region Variables

        private Game _game;

        public enum CameraType
        {
            FIRST_PERSON,
            THIRD_PERSON,
            MENU
        }

        #endregion

        internal Camera()
        {
            // We need to set the initial projection matrix.
            UpdateProjection();

            // Need to have this here at the end to create the camera's controller.
            CreateController();
        }

        /// <summary>
        /// This method creates the camera's controller, and sets the camera's Controller property to point to
        /// that new controller.  Finally, it registers the controller with the Kernel so that the camera receives
        /// updates.
        /// </summary>
        protected override void CreateController()
        {
            base.Controller = new CameraController(this);
            base.Controller.Register();
        }

        /// <summary>
        /// This updates the projection matrix based on the game's aspect ratio.  This should be called whenever
        /// the resolution is changed to ensure that the projection matrix is correct.
        /// </summary>
        internal void UpdateProjection()
        {
            this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                  Engine.Instance.Game.GraphicsDevice.Viewport.AspectRatio,
                                                                  0.1f, 10000.0f);
        }

        // TODO: Create a method by which something can change the view matrix.
        internal void UpdateView()
        {

        }
        /**
         * Warp the cursor to the center of the window.
         */
        public void CenterCursor()
        {
            GameWindow window = Engine.Instance.Game.Window;

            Mouse.SetPosition(window.ClientBounds.Width / 2, window.ClientBounds.Height / 2);
        }

        #region Properties

        public Matrix View
        {
            get;
            private set;
        }

        public Matrix Projection
        {
            get;
            private set;
        }

        public CameraType Type
        {
            get;
            internal set;
        }

        #endregion
    }
}
