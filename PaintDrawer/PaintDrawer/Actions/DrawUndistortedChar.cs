using PaintDrawer.Letters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintDrawer.Actions
{
    /// <summary>
    /// An IAction for drawing a single undistorted character as scaled up as allowed by the screen dimentions.
    /// </summary>
    class DrawUndistortedChar : IAction
    {
        Vec2 at;
        float scale;
        Character character;
        char c;

        public DrawUndistortedChar(CharFont font, Vec2 min, Vec2 max, char c)
        {
            this.c = c;
            character = font.chars[c];
            Vec2 size = new Vec2(max.X - min.X, max.Y - min.Y);
            scale = Math.Min(size.X / character.Width, size.Y / character.Height) * 0.925f;

            Vec2 center = new Vec2(min.X + size.X * 0.5f, min.Y + size.Y * 0.5f);
            at = new Vec2(center.X - character.Width * scale * 0.5f, center.Y - character.Height * scale * 0.5f);
        }

        public DrawUndistortedChar(CharFont font, char c)
            : this(font, SimpleWrite.DefaultAt, new Vec2(Stuff.ScreenWidth - 30, Stuff.ScreenHeight - 127), c)
        {

        }

        public void Act()
        {
            character.DrawUndistorted(at, scale);
        }

        public override string ToString()
        {
            return String.Concat("DrawUndistortedChar (int)c=", (int)c, " (", character.Name, ")");
        }
    }
}
