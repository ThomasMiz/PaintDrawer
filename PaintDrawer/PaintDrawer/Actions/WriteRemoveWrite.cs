using PaintDrawer.Letters;
using System;
using System.Drawing;
using System.Threading;

namespace PaintDrawer.Actions
{
    /// <summary>
    /// A simple IAction that writes something and instantly deletes it and writes something else
    /// </summary>
    class WriteRemoveWrite : IAction
    {
        SimpleWrite first, second;

        public WriteRemoveWrite(CharFont font, String txt1, Vec2 off1, float size1, String txt2, Vec2 off2, float size2)
        {
            first = new SimpleWrite(font, txt1, off1, size1);
            second = new SimpleWrite(font, txt2, off2, size2);
        }

        public WriteRemoveWrite(CharFont font, String txt1, float size1, String txt2, float size2)
        {
            first = new SimpleWrite(font, txt1, size1);
            second = new SimpleWrite(font, txt2, size2);
        }

        public void Act()
        {
            first.Act();
            Input.MoveTo(new Point(420, 70));
            
            Input.RegisterKeyDown(Input.KEY_CONTROL);
            Input.PressKey((byte)'E');
            Input.RegisterKeyUp(Input.KEY_CONTROL);
            Input.PressKey(Input.KEY_DELETE);
            Input.PaintSelectBrush();
            second.Act();
        }
    }
}
