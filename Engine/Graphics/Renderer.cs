using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StillDesign.PhysX;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Mammoth.Engine.Graphics
{
    /// <summary>
    /// A class for rendering graphics.  Allows for rendering IRenderable objects, rendering text to textures
    /// or to the screen, drawing textures to the screen, etc.
    /// </summary>
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

        /// <summary>
        /// Create a new instance of the renderer.
        /// </summary>
        /// <param name="game">The Game that owns this renderer.</param>
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
        
        /// <summary>
        /// Load a 3D model from a content file.
        /// </summary>
        /// <param name="path">The name of the model to load.</param>
        /// <returns>A Model object containing the data from the model file.</returns>
        public Model LoadModel(string path)
        {
            return _content.Load<Model>("models\\" + path);
        }

        /// <summary>
        /// Load an image (texture) from a content file.
        /// </summary>
        /// <param name="path">The name of the texture to load.</param>
        /// <returns>A Texture2D object containing the image from the chosen file.</returns>
        public Texture2D LoadTexture(string path)
        {
            return _content.Load<Texture2D>("textures\\" + path);
        }

        /// <summary>
        /// Loads a font from a spritefont content file.
        /// </summary>
        /// <param name="path">The name of the font to load.</param>
        /// <returns>A SpriteFont object representing the chosen font.</returns>
        public SpriteFont LoadFont(string path)
        {
            return _content.Load<SpriteFont>("fonts\\" + path);
        }

        /// <summary>
        /// Draw a solid-colored rectangle to the screen.
        /// </summary>
        /// <param name="rect">The rectangle to draw.</param>
        /// <param name="color">The color to fill the rectangle with.</param>
        public void DrawFilledRectangle(Rectangle rect, Color color)
        {
            _spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            _spriteBatch.Draw(_whitePixel, rect, color);
            _spriteBatch.End();
        }

        /// <summary>
        /// Overloaded.  Draws a textured rectangle to the screen.
        /// </summary>
        /// <param name="pos">The position of the top left corner of the rectangle.</param>
        /// <param name="tex">The texture to draw.</param>
        public void DrawTexturedRectangle(Vector2 pos, Texture2D tex)
        {
            this.DrawTexturedRectangle(pos, tex, Color.White);
        }

        /// <summary>
        /// Overloaded.  Draws a textured rectangle to the screen.
        /// </summary>
        /// <param name="pos">The position of the top left corner of the rectangle.</param>
        /// <param name="tex">The texture to draw.</param>
        /// <param name="tint">A color to tint the texture with.</param>
        public void DrawTexturedRectangle(Vector2 pos, Texture2D tex, Color tint)
        {
            _spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            _spriteBatch.Draw(tex, pos, tint);
            _spriteBatch.End();
        }

        /// <summary>
        /// Overloaded.  Draws a textured rectangle to the screen.
        /// </summary>
        /// <param name="rect">The rectangle into which the texture will be drawn.</param>
        /// <param name="tex">The texture to draw.</param>
        public void DrawTexturedRectangle(Rectangle rect, Texture2D tex)
        {
            this.DrawTexturedRectangle(rect, tex, Color.White);
        }

        /// <summary>
        /// Overloaded.  Draws a textured rectangle to the screen.
        /// </summary>
        /// <param name="rect">The rectangle into which the texture will be drawn.</param>
        /// <param name="tex">The texture to draw.</param>
        /// <param name="tint">A color to tint the texture with.</param>
        public void DrawTexturedRectangle(Rectangle rect, Texture2D tex, Color tint)
        {
            _spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            _spriteBatch.Draw(tex, rect, tint);
            _spriteBatch.End();
        }

        /// <summary>
        /// Draw a textured quad that always points towards you.
        /// </summary>
        /// <param name="pos">The position of the quad (in 3-space).</param>
        /// <param name="size">The size of the quad.</param>
        /// <param name="tex">The texture to draw on the quad.</param>
        public void DrawTexturedBillboard(Vector3 pos, Vector2 size, Texture2D tex)
        {
            ICameraService cam = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));

            Matrix billboardRot = Matrix.CreateBillboard(pos, cam.Position, Vector3.Up, null);

            using (BasicEffect effect = new BasicEffect(_graphics, null))
            {
                // Set the matrices for the effect.
                // The world matrix is the local transform for the quad.
                effect.World = billboardRot;
                // This is the camera's viewing matrix.
                effect.View = cam.View;
                // This is the camera's projection matrix.
                effect.Projection = cam.Projection;

                // Enable texturing and set the texture.
                effect.TextureEnabled = true;
                effect.Texture = tex;

                size /= 2.0f;

                // Create the vertices for the quad.
                VertexPositionTexture[] vertices = new VertexPositionTexture[4];
                int[] triangleIndices = new int[6];

                Vector3 topCenter = (Vector3.Up * size.Y / 2.0f);
                // Top-left
                vertices[1] = new VertexPositionTexture(topCenter + (Vector3.Left * size.X / 2.0f), Vector2.Zero);
                // Top-right
                vertices[3] = new VertexPositionTexture(topCenter + (Vector3.Right * size.X / 2.0f), new Vector2(1.0f, 0.0f));
                // Bottom-left
                vertices[0] = new VertexPositionTexture(vertices[1].Position + (Vector3.Down * size.Y), new Vector2(0.0f, 1.0f));
                // Bottom-right
                vertices[2] = new VertexPositionTexture(vertices[3].Position + (Vector3.Down * size.Y), new Vector2(1.0f, 1.0f));

                // Set the indices for drawing.
                triangleIndices[0] = 0;
                triangleIndices[1] = 1;
                triangleIndices[2] = 2;

                triangleIndices[3] = 2;
                triangleIndices[4] = 1;
                triangleIndices[5] = 3;

                // Turn off triangle culling (this seems to be necessary due to Matrix.CreateBillboard creating the wrong rotation matrix).
                CullMode prev = _graphics.RenderState.CullMode;
                _graphics.RenderState.CullMode = CullMode.None;

                // Set important graphics states (that can be set incorrectly when drawing with SpriteBatches).
                _graphics.RenderState.DepthBufferEnable = true;
                _graphics.RenderState.AlphaBlendEnable = false;

                // Turn on simple transparency.
                _graphics.RenderState.AlphaTestEnable = true;

                // Set the vertex type used for rendering.
                _graphics.VertexDeclaration = new VertexDeclaration(_graphics, VertexPositionTexture.VertexElements);

                // Now draw the quad!
                effect.Begin();
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Begin();

                        _graphics.DrawUserIndexedPrimitives<VertexPositionTexture>(
                            PrimitiveType.TriangleList, vertices, 0, 4, triangleIndices, 0, 2);                                                                                 

                    pass.End();
                }
                effect.End();

                // Turn alpha testing off.
                _graphics.RenderState.AlphaTestEnable = false;

                // Reset the culling mode (to speed up rendering).
                _graphics.RenderState.CullMode = prev;
            }
        }

        public Texture2D RenderFont(string text, Vector2 pos, Color textColor, Color bgColor)
        {
            return this.RenderFont(text, pos, textColor, bgColor, _defaultFont, SpriteEffects.None);
        }

        public Texture2D RenderFont(string text, Vector2 pos, Color textColor, Color bgColor, SpriteFont font)
        {
            return this.RenderFont(text, pos, textColor, bgColor, font, SpriteEffects.None);
        }

        // TODO: Change this so that pos is a margin, not a position.
        /// <summary>
        /// Render a string to an image.  This image can later be used to draw the text to the screen.
        /// </summary>
        /// <param name="text">The string to render.</param>
        /// <param name="pos">The position of the string relative to the top-left hand corner of the produced texture.</param>
        /// <param name="textColor">The text color.</param>
        /// <param name="bgColor">The background color.</param>
        /// <param name="font">The font to use to render the text.</param>
        /// <param name="effects">The SpriteEffects that should be used to render the text.</param>
        /// <returns>A Texture2D with the text rendered to it.</returns>
        public Texture2D RenderFont(string text, Vector2 pos, Color textColor, Color bgColor, SpriteFont font, SpriteEffects effects)
        {
            Vector2 textSize = font.MeasureString(text) + Vector2.One;

            RenderTarget2D target = new RenderTarget2D(_graphics, (int)textSize.X, (int)textSize.Y, 1, SurfaceFormat.Color);

            _graphics.SetRenderTarget(0, target);
            {
                _graphics.Clear(bgColor);
                DrawText(text, pos, textColor, bgColor, font, effects);
            }
            _graphics.SetRenderTarget(0, null);

            return target.GetTexture();

        }

        public void DrawText(string text, Vector2 pos, Color textColor, Color bgColor)
        {
            this.DrawText(text, pos, textColor, bgColor, _defaultFont, SpriteEffects.None);
        }

        /// <summary>
        /// Draw some text directly to the screen.
        /// </summary>
        /// <param name="text">The string to render.</param>
        /// <param name="pos">The position of the string relative to the top-left hand corner of the produced texture.</param>
        /// <param name="textColor">The text color.</param>
        /// <param name="bgColor">The background color.</param>
        /// <param name="font">The font to use to render the text.</param>
        /// <param name="effects">The SpriteEffects that should be used to render the text.</param>
        public void DrawText(string text, Vector2 pos, Color textColor, Color bgColor, SpriteFont font, SpriteEffects effects)
        {
            _spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            _spriteBatch.DrawString(font, text, pos, textColor, 0.0f, Vector2.Zero, 1.0f, effects, 1);
            _spriteBatch.End();
        }

        /// <summary>
        /// Draw an IRenderable to the screen.  This method is used to draw an object whose graphical representation is stored
        /// in a Model object.  It gets positional and orientation information from the IRenderable and uses them to draw the
        /// Model.
        /// </summary>
        /// <param name="obj"></param>
        public void DrawRenderable(IRenderable obj)
        {
            _graphics.RenderState.DepthBufferEnable = true;
            _graphics.RenderState.AlphaBlendEnable = false;
            _graphics.RenderState.AlphaTestEnable = false;

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
                    effect.PreferPerPixelLighting = true;
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

        public SpriteFont DefaultFont
        {
            get { return _defaultFont; }
        }

        #endregion
    }
}
