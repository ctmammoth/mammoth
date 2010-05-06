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

        #region Fields

        public enum CameraType
        {
            FIRST_PERSON,
            THIRD_PERSON,
            MENU
        }

        #endregion

        public Camera(Game game, Player target) : base(game)
        {
            this.Target = target;

            this.Game.Services.AddService(typeof(ICameraService), this);
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
                                                                  this.Game.GraphicsDevice.Viewport.AspectRatio,
                                                                  0.1f, 10000.0f);
        }

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

        // We might want to change this from Player to some other class, as we might want
        // the camera to target something other than the player.  Sadly, C# 3.0 doesn't support
        // variant return types, so there's no clean way to do this.
        public Player Target
        {
            get;
            private set;
        }

        #endregion
    }

    public class FirstPersonCamera : Camera
    {
        #region Fields

        private CameraType _type;

        #endregion

        public FirstPersonCamera(Game game, Player target)
            : base(game, target)
        {
            this.Type = CameraType.FIRST_PERSON;
        }

        public override void Update(GameTime gameTime)
        {
            Vector3 forward = Vector3.Transform(Vector3.Forward, this.Target.HeadOrient) * 1000.0f;

            Vector3 position = this.Target.Position + (Vector3.Up * this.Target.Height / 4.0f);
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
        #region Fields

        private CameraType _type;

        #endregion

        public ThirdPersonCamera(Game game, Player target)
            : base(game, target)
        {
            this.Type = CameraType.THIRD_PERSON;
        }

        public override void Update(GameTime gameTime)
        {
            Vector3 forward = Vector3.Transform(Vector3.Forward, this.Target.HeadOrient) * 1000.0f;

            Vector3 position = this.Target.Position - forward * 15 + Vector3.Up * 5;
            Vector3 look = this.Target.Position + Vector3.Up * this.Target.Height * 3 / 4;

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
