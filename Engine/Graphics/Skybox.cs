using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth.Engine.Graphics
{
    public class Skybox : DrawableGameComponent
    {
        #region Fields

        Texture2D up, down, north, east, south, west;

        #endregion

        public Skybox(Game game)
            : base(game)
        {
            LoadContent();
        }

        protected override void LoadContent()
        {
            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));

            // Load skybox textures.
            up = r.LoadTexture("skybox\\up");
            down = r.LoadTexture("skybox\\down");
            north = r.LoadTexture("skybox\\north");
            east = r.LoadTexture("skybox\\east");
            south = r.LoadTexture("skybox\\south");
            west = r.LoadTexture("skybox\\west");
        }

        public override void Draw(GameTime gameTime)
        {
            IRenderService r = (IRenderService)this.Game.Services.GetService(typeof(IRenderService));
            GraphicsDevice g = this.Game.GraphicsDevice;

            // Set up the texture samplers.
            g.SamplerStates[0].AddressU = TextureAddressMode.Clamp;
            g.SamplerStates[0].AddressV = TextureAddressMode.Clamp;

            // Turn off the depth buffer.
            g.RenderState.DepthBufferWriteEnable = false;

            // Get the camera.
            ICameraService cam = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));
            Vector3 scale, trans;
            Quaternion rot;
            cam.View.Decompose(out scale, out rot, out trans);

            // Create a quad to use to draw the skybox.
            VertexPositionTexture[] vertices = new VertexPositionTexture[4];
            vertices[0] = new VertexPositionTexture(new Vector3(-1.0f, 1.0f, 1.0f), new Vector2(0.0f, 0.0f));
            vertices[1] = new VertexPositionTexture(new Vector3(-1.0f, -1.0f, 1.0f), new Vector2(0.0f, 1.0f));
            vertices[2] = new VertexPositionTexture(new Vector3(1.0f, 1.0f, 1.0f), new Vector2(1.0f, 0.0f));
            vertices[3] = new VertexPositionTexture(new Vector3(1.0f, -1.0f, 1.0f), new Vector2(1.0f, 1.0f));

            VertexBuffer vertexBuffer = new VertexBuffer(g, VertexPositionTexture.SizeInBytes * vertices.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionTexture>(vertices);

            VertexDeclaration vertexDecl = new VertexDeclaration(g, VertexPositionTexture.VertexElements);

            // Set up the effect.
            BasicEffect effect = new BasicEffect(g, new EffectPool());
            effect.World = Matrix.Identity;
            effect.View = Matrix.CreateFromQuaternion(rot);
            effect.Projection = cam.Projection;
            effect.TextureEnabled = true;

            // Set the vertex declaration for the graphics device.
            g.VertexDeclaration = vertexDecl;
            g.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionTexture.SizeInBytes);

            // Begin the effect.
            effect.Begin();
            
            // Draw the north skybox texture.
            effect.Texture = north;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                g.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                pass.End();
            }

            // Draw the east skybox texture.
            effect.Texture = east;
            effect.View = Matrix.CreateFromQuaternion(rot * Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver2));
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                g.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                pass.End();
            }

            // Draw the south skybox texture.
            effect.Texture = south;
            effect.View = Matrix.CreateFromQuaternion(rot * Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi));
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                g.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                pass.End();
            }

            // Draw the west skybox texture.
            effect.Texture = west;
            effect.View = Matrix.CreateFromQuaternion(rot * Quaternion.CreateFromAxisAngle(Vector3.Up, -MathHelper.PiOver2));
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                g.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                pass.End();
            }

            // Draw the up skybox texture.
            effect.Texture = up;
            effect.View = Matrix.CreateFromQuaternion(rot *
                                Quaternion.CreateFromAxisAngle(Vector3.Right, MathHelper.PiOver2) *
                                Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi));
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                g.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                pass.End();
            }

            // Draw the down skybox texture.
            effect.Texture = down;
            effect.View = Matrix.CreateFromQuaternion(rot *
                                Quaternion.CreateFromAxisAngle(Vector3.Right, -MathHelper.PiOver2) *
                                Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi));
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Begin();
                g.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                pass.End();
            }

            // End the effect.
            effect.End();

            // Turn the depth buffer back on.
            g.RenderState.DepthBufferWriteEnable = true;
        }
    }
}
