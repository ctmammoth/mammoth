using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

using Mammoth.Engine;
using Mammoth.Engine.Physics;
using Mammoth.Engine.Graphics;
using Mammoth.Engine.ContentProcessors;

namespace Mammoth
{
    public class Terrain : PhysicalObject, IRenderable
    {
        public Terrain(Game game)
            : base(game)
        {
            this.Model3D = this.Game.Content.Load<Model>("textures\\terrain-256");

            InitializePhysX();

            this.Position = new Vector3(0.0f, 0.0f, 0.0f);
            this.PositionOffset = new Vector3(0.0f, 64.0f, 0.0f);
            this.Orientation = Quaternion.Identity;
        }

        private void InitializePhysX()
        {
            IPhysicsManagerService physics = (IPhysicsManagerService)this.Game.Services.GetService(typeof(IPhysicsManagerService));

            Texture2D tex = this.Game.Content.Load<Texture2D>("textures\\heightmap-256");
            int width = tex.Width;
            int height = tex.Height;

            Color[] texData = new Color[width * height];

            tex.GetData<Color>(texData);

            HeightFieldSample[] samples = new HeightFieldSample[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //float h = texData[x * width + y].R / 255.0f * short.MaxValue;
                    //float h = (float) (Math.Sin(y) * Math.Cos(x) * short.MaxValue);
                    float h = texData[x * width + y].R;
                    //Console.WriteLine(texData[x * width + y].R);
                    //Console.WriteLine(h);
                    /*samples[x * width + y] = new HeightFieldSample()
                    {
                        Height = (short)h,
                        MaterialIndex0 = 0,
                        MaterialIndex1 = 1,
                        TessellationFlag = 0
                    };*/
                    samples[y * width + x].Height = (short)h;
                    samples[y * width + x].MaterialIndex0 = 0;
                    samples[y * width + x].MaterialIndex1 = 1;
                    samples[y * width + x].TessellationFlag = 0;
                }
            }

            HeightFieldDescription heightFieldDesc = new HeightFieldDescription()
            {
                NumberOfColumns = width,
                NumberOfRows = height,
                Thickness = -100
            };
            heightFieldDesc.SetSamples(samples);

            HeightField heightField = physics.Core.CreateHeightField(heightFieldDesc);

            HeightFieldShapeDescription heightFieldShapeDesc = new HeightFieldShapeDescription()
            {
                HeightField = heightField,
                HoleMaterial = 2,
                HeightScale = 64.0f / 255.0f,
                RowScale = 3,
                ColumnScale = 3
            };

            heightFieldShapeDesc.LocalPosition = new Vector3(-0.5f * height * 1 * heightFieldShapeDesc.RowScale, 0, -0.5f * width * 1 * heightFieldShapeDesc.ColumnScale);

            ActorDescription terrainActor = new ActorDescription()
            {
                Shapes = { heightFieldShapeDesc },
                GlobalPose = Matrix.CreateTranslation(0.0f, -32.0f, 0.0f)
            };

            this.Actor = physics.CreateActor(terrainActor, this);
        }

        public override void Draw(GameTime gameTime)
        {
            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));
            this.Game.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            this.Game.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
            r.DrawRenderable(this);
        }

        public override string getObjectType()
        {
            return "Terrain";
        }

        #region Properties

        public Vector3 PositionOffset
        {
            get;
            private set;
        }

        public Model Model3D
        {
            get;
            private set;
        }

        #endregion
    }
}
