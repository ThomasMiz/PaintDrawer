using System;
using PaintDrawer.Letters;

namespace PaintDrawer.Actions
{
    class SimpleWrite : IAction
    {
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
            this.at = new Vec2(30, 200);
            this.size = size;
        }

        public SimpleWrite(CharFont font, String text)
        {
            this.font = font;
            this.text = text;
            this.at = new Vec2(30, 200);
            this.size = 90;
        }

        public void Act()
        {
            font.DrawWrapped(text, at, size, Stuff.ScreenWidth - at.X - 500);
        }
    }
}
