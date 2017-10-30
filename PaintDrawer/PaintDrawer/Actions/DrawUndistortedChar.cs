using PaintDrawer.Letters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintDrawer.Actions
{
    class DrawUndistortedChar : IAction
    {
        Vec2 at;
        float scale;
        Character character;

        public DrawUndistortedChar(CharFont font, Vec2 at, char c)
        {
            this.at = at;
            character = font.chars[c];
            scale = Math.Min((Stuff.ScreenWidth - at.X - 100) / character.Width, (Stuff.ScreenHeight - at.Y - 150) / character.Height);
        }

        public DrawUndistortedChar(CharFont font, char c)
            : this(font, SimpleWrite.DefaultAt, c)
        {

        }

        public void Act()
        {
            character.DrawUndistorted(at, scale);
        }
    }
}
