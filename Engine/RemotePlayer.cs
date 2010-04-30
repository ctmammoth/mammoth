using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using StillDesign.PhysX;

using Mammoth.Engine.Physics;

namespace Mammoth.Engine
{
    class RemotePlayer : Player
    {
        public RemotePlayer(Game game) : base(game)
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();

            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));

            this.Model3D = r.LoadModel("soldier-low-poly");
            this.Height = 6.0f;

            InitializePhysX();

            this.Spawn(Vector3.Zero, Quaternion.Identity);
        }

        // TODO: There has to be a better way to do this.
        public void InitializePhysX()
        {
            ControllerDescription desc = new CapsuleControllerDescription(1, this.Height - 2.0f)
            {
                UpDirection = Axis.Y,
                Position = Vector3.UnitY * (this.Height - 1.0f) / 2.0f
            };
            this.PositionOffset = -1.0f * desc.Position;

            IPhysicsManagerService physics = (IPhysicsManagerService) this.Game.Services.GetService(typeof(IPhysicsManagerService));
            this.Controller = physics.CreateController(desc);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));
            ICameraService cam = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));

            // If you're using the first-person camera, don't draw your own geometry.
            if (cam.Type != Camera.CameraType.FIRST_PERSON)
                r.DrawRenderable(this);
        }
    }
}
