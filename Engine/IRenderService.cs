using System;
namespace Mammoth.Engine
{
    interface IRenderService
    {
        void DrawObject(IRenderable obj);
        void DrawPhysXDebug(StillDesign.PhysX.Scene scene);
        Microsoft.Xna.Framework.Graphics.SpriteFont LoadFont(string path);
        Microsoft.Xna.Framework.Graphics.Model LoadModel(string path);
        Microsoft.Xna.Framework.Graphics.Texture2D LoadTexture(string path);
    }
}
