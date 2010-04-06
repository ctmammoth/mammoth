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
    public class Renderer
    {
        #region Variables

        private ContentManager _content;
        private GraphicsDevice _graphics;

        private static Renderer _instance = null;

        #endregion

        private Renderer()
        {
            _content = Engine.Instance.Content;
            _graphics = Engine.Instance.GraphicsDevice;

            this.Camera = Engine.Instance.Camera;
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

        public void DrawObject(IRenderable obj)
        {
            Model m = obj.Model3D;

            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix projection = this.Camera.Projection;
            Matrix view = this.Camera.View;

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.View = view;
                    effect.Projection = projection;
                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateFromQuaternion(obj.Orientation);
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
            _graphics.VertexDeclaration = new VertexDeclaration(_graphics, VertexPositionColor.VertexElements);

            BasicEffect debugEffect = new BasicEffect(_graphics, null);
            debugEffect.World = Matrix.Identity;
            debugEffect.View = this.Camera.View;
            debugEffect.Projection = this.Camera.Projection;

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

        public static Renderer Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Renderer();
                return _instance;
            }
        }

        public Camera Camera
        {
            get;
            private set;
        }

        #endregion
    }
}
