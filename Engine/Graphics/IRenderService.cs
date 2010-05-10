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
        void DrawTexturedBillboard(Vector3 pos, Vector2 size, Texture2D tex);
        Texture2D RenderFont(string text, Vector2 pos, Color textColor, Color bgColor);
        Texture2D RenderFont(string text, Vector2 pos, Color textColor, Color bgColor, SpriteFont font);
        Texture2D RenderFont(string text, Vector2 pos, Color textColor, Color bgColor, SpriteFont font, SpriteEffects effects);
        void DrawText(string text, Vector2 pos, Color textColor, Color bgColor);
        void DrawText(string text, Vector2 pos, Color textColor, Color bgColor, SpriteFont font, SpriteEffects effects);
        void DrawRenderable(IRenderable obj);
        void DrawPhysXDebug(StillDesign.PhysX.Scene scene);

        SpriteFont LoadFont(string path);
        Model LoadModel(string path);
        Texture2D LoadTexture(string path);

        #region Properties

        SpriteFont DefaultFont
        {
            get;
        }

        #endregion
    }
}
