using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mammoth.Engine
{
    public interface IRenderService
    {
        void DrawFilledRectangle(Rectangle rect, Color color);
        void DrawTexturedRectangle(Vector2 pos, Texture2D tex);
        void DrawTexturedRectangle(Vector2 pos, Texture2D tex, Color tint);
        void DrawTexturedRectangle(Rectangle rect, Texture2D tex);
        void DrawTexturedRectangle(Rectangle rect, Texture2D tex, Color tint);
        Texture2D RenderFont(string text, Vector2 pos, Color textColor, Color bgColor);
        Texture2D RenderFont(string text, Vector2 pos, Color textColor, Color bgColor, SpriteFont font);
        void DrawText(string text, Vector2 pos, Color textColor, Color bgColor);
        void DrawText(string text, Vector2 pos, Color textColor, Color bgColor, SpriteFont font);
        void DrawRenderable(IRenderable obj);
        void DrawPhysXDebug(StillDesign.PhysX.Scene scene);

        SpriteFont LoadFont(string path);
        Model LoadModel(string path);
        Texture2D LoadTexture(string path);
    }
}
