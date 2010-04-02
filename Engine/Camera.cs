using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Mammoth.Engine
{
    /**
     * This is a simple and fairly inefficient camera class taken from the camera class in the PhysX.Net samples.  It
     * could be improved so that it stores more information and therefore has to do less calculation each update cycle,
     * but that shouldn't be worried about unless it is deemed to be an issue (by way of profiling).
     */
    public class Camera : GameComponent
    {

        #region Variables

        public enum CameraType
        {
            FIRST_PERSON,
            THIRD_PERSON,
            MENU
        }

        #endregion

        public Camera(Game game) : base(game)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdateView();
        }

        /// <summary>
        /// This updates the projection matrix based on the game's aspect ratio.  This should be called whenever
        /// the resolution is changed to ensure that the projection matrix is correct.
        /// </summary>
        public void UpdateProjection()
        {
            this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                  Engine.Instance.GraphicsDevice.Viewport.AspectRatio,
                                                                  0.1f, 10000.0f);
        }

        public void UpdateView()
        {
            LocalPlayer lp = Engine.Instance.LocalPlayer;

            Vector3 position = lp.Position + (Vector3.Up * lp.Height / 4.0f);
            Vector3 forward = Vector3.Transform(Vector3.Forward, lp.Orientation);

            //Console.WriteLine("position: " + position + "\nforward: " + forward);

            this.View = Matrix.CreateLookAt(position, position + forward, Vector3.Up);
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
