using PaintDrawer.Letters;
using System;
using System.Drawing;
using System.Threading;

namespace PaintDrawer.Actions
{
    class WriteRemoveWrite : IAction
    {
        CharFont font;
        String txt1, txt2;
        float size1, size2;
        Vec2 off1, off2;

        public WriteRemoveWrite(CharFont font, String txt1, float size1, Vec2 off1, String txt2, float size2, Vec2 off2)
        {
            this.font = font;
            this.txt1 = txt1;
            this.txt2 = txt2;
            this.size1 = size1;
            this.size2 = size2;
            this.off1 = off1;
            this.off2 = off2;
        }

        public WriteRemoveWrite(CharFont font, String txt1, float size1, String txt2, float size2)
        {
            this.font = font;
            this.txt1 = txt1;
            this.txt2 = txt2;
            this.size1 = size1;
            this.size2 = size2;
            this.off1 = new Vec2(30, 200);
            this.off2 = new Vec2(30, 200);
        }

        public void Act()
        {
            font.Draw(txt1, off1, size1);
            Input.MoveTo(new Point(420, 70));
            
            Input.RegisterKeyDown(Input.KEY_CONTROL);
            Input.PressKey((byte)'E');
            Input.RegisterKeyUp(Input.KEY_CONTROL);
            Input.PressKey(Input.KEY_DELETE);
            Input.PaintSelectBrush();
            font.Draw(txt2, off2, size2);
        }
    }
}
