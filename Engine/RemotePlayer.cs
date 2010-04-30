using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Mammoth.Engine
{
    class RemotePlayer : Player
    {
        public RemotePlayer(Engine game) : base(game)
        {
            game.Components.Add(this);
            IModelDBService mdb = (IModelDBService)this.Game.Services.GetService(typeof(IModelDBService));
            mdb.registerObject(this);
        }

        public override void Initialize()
        {
            base.Initialize();

            Renderer r = (Renderer)this.Game.Services.GetService(typeof(IRenderService));

            this.Model3D = r.LoadModel("soldier-low-poly");
            this.Height = 6.0f;

            InitializePhysX();

            /// TODO: Change LocalPlayer so that it can be given starting values.
            /// It's probably best to put a specialized factory in this file and give it things
            /// like position and orientation values.  It could then be part of an umbrella
            /// factory that allows the game to create objects of various types.

            this.Position = new Vector3(-3.0f, 10.0f, 0.0f);
            this.Orientation = Quaternion.Identity;
            this.HeadOrient = Quaternion.Identity;
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
            this.Controller = Player.ControllerManager.CreateController(desc);
            this.Controller.SetCollisionEnabled(true);
            this.Controller.Name = "Local Player Controller";
            this.Controller.Actor.Name = "Local Player Actor";
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
