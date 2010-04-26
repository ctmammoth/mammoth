using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Mammoth.Engine
{
    public class Renderer : IRenderService
    {
        #region Fields

        private ContentManager _content;
        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;

        // Used to draw solid colored rectangles to the screen.
        private Texture2D _whitePixel;

        // Default font to use for drawing text.
        private SpriteFont _defaultFont;

        #endregion

        public Renderer(Game game)
        {
            this.Game = game;
            _content = game.Content;
            _graphics = game.GraphicsDevice;
            _spriteBatch = new SpriteBatch(_graphics);
            
            // Create the white pixel.
            _whitePixel = new Texture2D(_graphics, 1, 1, 1, TextureUsage.None, SurfaceFormat.Color);
            _whitePixel.SetData<Color>(new Color[] { Color.White });

            // Load the default font.
            _defaultFont = LoadFont("calibri");
        }

        public Model LoadModel(string path)
        {
            return _content.Load<Model>("models\\" + path);
        }

        public Texture2D LoadTexture(string path)
        {
            return _content.Load<Texture2D>("textures\\" + path);
        }

        public SpriteFont LoadFont(string path)
        {
            return _content.Load<SpriteFont>("fonts\\" + path);
        }

        public void DrawFilledRectangle(Rectangle rect, Color color)
        {
            _spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            _spriteBatch.Draw(_whitePixel, rect, color);
            _spriteBatch.End();
        }

        public void DrawTexturedRectangle(Vector2 pos, Texture2D tex)
        {
            _spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            _spriteBatch.Draw(tex, pos, Color.White);
            _spriteBatch.End();
        }

        public void DrawTexturedRectangle(Rectangle rect, Texture2D tex)
        {
            _spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            _spriteBatch.Draw(tex, rect, Color.White);
            _spriteBatch.End();
        }

        public Texture2D RenderFont(string text, Vector2 pos, Color textColor, Color bgColor)
        {
            return this.RenderFont(text, pos, textColor, bgColor, _defaultFont);
        }

        public Texture2D RenderFont(string text, Vector2 pos, Color textColor, Color bgColor, SpriteFont font)
        {
            Vector2 textSize = font.MeasureString(text);

            RenderTarget2D target = new RenderTarget2D(_graphics, (int) textSize.X, (int) textSize.Y, 0, SurfaceFormat.Color);

            _graphics.SetRenderTarget(0, target);
            {
                _graphics.Clear(bgColor);
                DrawText(text, pos, textColor, bgColor, font);
            }
            _graphics.SetRenderTarget(0, null);

            return target.GetTexture();
        }

        public void DrawText(string text, Vector2 pos, Color textColor, Color bgColor)
        {
            this.DrawText(text, pos, textColor, bgColor, _defaultFont);
        }

        public void DrawText(string text, Vector2 pos, Color textColor, Color bgColor, SpriteFont font)
        {
            _spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            _spriteBatch.DrawString(font, text, pos, textColor, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1);
            _spriteBatch.End();
        }

        public void DrawRenderable(IRenderable obj)
        {
            Camera cam = (Camera) this.Game.Services.GetService(typeof(ICameraService));

            Model m = obj.Model3D;

            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix projection = cam.Projection;
            Matrix view = cam.View;

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.View = view;
                    effect.Projection = projection;
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateFromQuaternion(obj.Orientation) * Matrix.CreateTranslation(obj.Position + obj.PositionOffset);
                }

                mesh.Draw();
            }
        }

        /// <summary>
        /// This method gets the debug geometry from the PhysX scene, and draws it to the screen.  This is useful for
        /// making sure that models are being correctly placed within their PhysX bounding boxes, and for checking to
        /// see if things are physically working without actually drawing them.
        /// </summary>
        /// <param name="scene">The PhysX scene that we want to draw debug geometry for.</param>
        public void DrawPhysXDebug(StillDesign.PhysX.Scene scene)
        {
            Camera cam = (Camera)this.Game.Services.GetService(typeof(ICameraService));

            _graphics.VertexDeclaration = new VertexDeclaration(_graphics, VertexPositionColor.VertexElements);

            BasicEffect debugEffect = new BasicEffect(_graphics, null);
            debugEffect.World = Matrix.Identity;
            debugEffect.View = cam.View;
            debugEffect.Projection = cam.Projection;

            DebugRenderable data = scene.GetDebugRenderable();

            debugEffect.Begin();

            foreach (EffectPass pass in debugEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                if (data.PointCount > 0)
                {
                    DebugPoint[] points = data.GetDebugPoints();

                    _graphics.DrawUserPrimitives<DebugPoint>(PrimitiveType.PointList, points, 0, points.Length);
                }

                if (data.LineCount > 0)
                {
                    DebugLine[] lines = data.GetDebugLines();

                    VertexPositionColor[] vertices = new VertexPositionColor[data.LineCount * 2];
                    for (int x = 0; x < data.LineCount; x++)
                    {
                        DebugLine line = lines[x];

                        vertices[x * 2 + 0] = new VertexPositionColor(line.Point0, Int32ToColor(line.Color));
                        vertices[x * 2 + 1] = new VertexPositionColor(line.Point1, Int32ToColor(line.Color));
                    }

                    _graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, lines.Length);
                }

                if (data.TriangleCount > 0)
                {
                    DebugTriangle[] triangles = data.GetDebugTriangles();

                    VertexPositionColor[] vertices = new VertexPositionColor[data.TriangleCount * 3];
                    for (int x = 0; x < data.TriangleCount; x++)
                    {
                        DebugTriangle triangle = triangles[x];

                        vertices[x * 3 + 0] = new VertexPositionColor(triangle.Point0, Int32ToColor(triangle.Color));
                        vertices[x * 3 + 1] = new VertexPositionColor(triangle.Point1, Int32ToColor(triangle.Color));
                        vertices[x * 3 + 2] = new VertexPositionColor(triangle.Point2, Int32ToColor(triangle.Color));
                    }

                    _graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, vertices, 0, triangles.Length);
                }

                pass.End();
            }

            debugEffect.End();
        }

        // Takes an int and pulls the ARGB color out of it, byte by byte.
        public static Color Int32ToColor(int color)
        {
            byte a = (byte)((color & 0xFF000000) >> 32);
            byte r = (byte)((color & 0x00FF0000) >> 16);
            byte g = (byte)((color & 0x0000FF00) >> 8);
            byte b = (byte)((color & 0x000000FF) >> 0);

            return new Color(r, g, b, a);
        }

        #region Properties

        public Game Game
        {
            get;
            private set;
        }

        #endregion
    }
}
