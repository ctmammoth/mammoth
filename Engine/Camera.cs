using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// TODO: Documentation for Camera code.

namespace Mammoth.Engine
{
    /// <summary>
    /// This interface encapsulates the idea of a camera service.  This can be used with Game.GetService()
    /// to get the current camera.
    /// </summary>
    public interface ICameraService
    {
        Matrix View
        {
            get;
        }

        Matrix Projection
        {
            get;
        }

        Camera.CameraType Type
        {
            get;
        }
    }

    public abstract class Camera : GameComponent, ICameraService
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

        public abstract override void Update(GameTime gameTime);

        public override void Initialize()
        {
            base.Initialize();

            UpdateProjection();
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

        /*public void UpdateView()
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
        }*/

        #region Properties

        public Matrix View
        {
            get;
            protected set;
        }

        public Matrix Projection
        {
            get;
            protected set;
        }

        public abstract CameraType Type
        {
            get;
            protected set;
        }

        #endregion
    }

    public class FirstPersonCamera : Camera
    {
        #region Variables

        private CameraType _type;

        #endregion

        public FirstPersonCamera(Game game)
            : base(game)
        {
            this.Type = CameraType.FIRST_PERSON;
        }

        public override void Update(GameTime gameTime)
        {
            LocalPlayer lp = Engine.Instance.LocalPlayer;
            Vector3 forward = Vector3.Transform(Vector3.Forward, lp.HeadOrient);

            Vector3 position = lp.Position + (Vector3.Up * lp.Height / 4.0f);
            Vector3 look = position + forward;

            this.View = Matrix.CreateLookAt(position, look, Vector3.Up);
        }

        #region Properties

        public override CameraType Type
        {
            get { return _type; }
            protected set { _type = value; }
        }

        #endregion
    }

    public class ThirdPersonCamera : Camera
    {
        #region Variables

        private CameraType _type;

        #endregion

        public ThirdPersonCamera(Game game)
            : base(game)
        {
            this.Type = CameraType.THIRD_PERSON;
        }

        public override void Update(GameTime gameTime)
        {
            LocalPlayer lp = Engine.Instance.LocalPlayer;
            Vector3 forward = Vector3.Transform(Vector3.Forward, lp.HeadOrient);

            Vector3 position = lp.Position - forward * 15 + Vector3.Up * 5;
            Vector3 look = lp.Position + Vector3.Up * lp.Height * 3 / 4;

            this.View = Matrix.CreateLookAt(position, look, Vector3.Up);
        }

        #region Properties

        public override CameraType Type
        {
            get { return _type; }
            protected set { _type = value; }
        }

        #endregion
    }
}
