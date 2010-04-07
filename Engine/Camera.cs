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
            this.Type = CameraType.THIRD_PERSON;
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
            Vector3 forward = Vector3.Transform(Vector3.Forward, lp.HeadOrient);

            Vector3 position, look;
            switch(this.Type)
            {
                case CameraType.FIRST_PERSON:
                    position = lp.Position + (Vector3.Up * lp.Height / 4.0f);
                    look = position + forward;
                    break;
                // TODO: Make sure that the third-person "works".
                // I don't think it'll look any good, but once we have something drawing for the player, we
                // should fix it up so that it's somewhat useful for debugging.
                case CameraType.THIRD_PERSON:
                    position = lp.Position - forward * 15 + Vector3.Up * 5;
                    look = lp.Position + Vector3.Up * lp.Height * 3 / 4;
                    break;
                default:
                    position = Vector3.Zero;
                    look = Vector3.Zero;
                    break;
            }
            
            this.View = Matrix.CreateLookAt(position, look, Vector3.Up);
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
