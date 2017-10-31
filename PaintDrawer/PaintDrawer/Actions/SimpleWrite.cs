using System;
using PaintDrawer.Letters;

namespace PaintDrawer.Actions
{
    class SimpleWrite : IAction
    {
        public const float DefaultSize = 90;
        public static Vec2 DefaultAt = new Vec2(30, 200);

        CharFont font;
        String text;
        Vec2 at;
        float size;

        public SimpleWrite(CharFont font, String text, Vec2 at, float size)
        {
            this.font = font;
            this.text = text;
            this.at = at;
            this.size = size;
        }

        public SimpleWrite(CharFont font, String text, float size)
        {
            this.font = font;
            this.text = text;
            this.at = DefaultAt;
            this.size = size;
        }

        public SimpleWrite(CharFont font, String text)
        {
            this.font = font;
            this.text = text;
            this.at = DefaultAt;
            this.size = DefaultSize;
        }

        public void Act()
        {
            font.DrawWrapped(text, at, size, Stuff.ScreenWidth - at.X - 120);
        }

        public static bool IsSizeOk(CharFont font, String text, Vec2 at, float size)
        {
            if (text.Length > 200)
                return false;

            Vec2 s;
            if (font.CalculateDrawWrappedSize(text, at, size, Stuff.ScreenWidth - at.X - 120, out s))
                return at.X + s.X < Stuff.ScreenWidth - 120 && at.Y + s.Y < Stuff.ScreenHeight - 120;
            return false;
        }

        public override string ToString()
        {
            return String.Concat("SimpleWrite text=", text, "; at=", at.ToString(), "; size=", size);
        }
    }
}
