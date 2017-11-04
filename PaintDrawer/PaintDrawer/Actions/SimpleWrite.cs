using System;
using PaintDrawer.Letters;

namespace PaintDrawer.Actions
{
    /// <summary>
    /// A Simple IAction that writes a wrapped distorted text where indicated and with the specified size.
    /// </summary>
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

        /// <summary>
        /// Calculates whether there's enough screen area to draw the specified text and stuff
        /// </summary>
        /// <param name="text">The text to check</param>
        /// <param name="at">The location the text will be drawed at</param>
        /// <param name="size">The size of the text</param>
        /// <returns>Whether there's enough screen are to draw with the specified data</returns>
        public static bool IsSizeOk(CharFont font, String text, Vec2 at, float size)
        {
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
