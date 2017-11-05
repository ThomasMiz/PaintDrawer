
//#define PRINT_MOVES

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using In = System.Windows.Input;

namespace PaintDrawer
{
    class Input
    {
        const float MoveDistance = 12*5, MoreDistance = MoveDistance + 3; //usual speed is 12
        const float MoveDistanceSquared = MoveDistance * MoveDistance, MoreDistanceSquared = MoreDistance * MoreDistance;
        const int SLPTime = 16;


        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        // Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;

        public const byte KEY_DELETE = 0x2E;
        public const byte KEY_LWIN = 0x5B;
        public const byte KEY_ENTER = 0x0D;
        public const byte KEY_SHIFT = 0x10;
        public const byte KEY_CONTROL = 0x11;

        #region MouseUpDown
        public static void MouseDown(uint x, uint y)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
        }

        public static void MouseUp(uint x, uint y)
        {
            mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
        }

        public static void MouseDown(int x, int y)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)x, (uint)y, 0, 0);
        }

        public static void MouseUp(int x, int y)
        {
            mouse_event(MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
        }

        public static void MouseDown()
        {
            Point p = Cursor.Position;
            MouseDown((uint)p.X, (uint)p.Y);
        }

        public static void MouseUp()
        {
            Point p = Cursor.Position;
            MouseUp(p.X, p.Y);
        }

        public static void RegisterClick()
        {
            MouseDown();
            MouseUp();
        }
        #endregion

        /// <summary>
        /// Moves the mouse by the specified values
        /// </summary>
        public static void MoveBy(int x, int y)
        {
            Point p = Cursor.Position;
            Cursor.Position = new Point(p.X + x, p.Y + y);
        }

        public static void MoveBy(float rotation, int distance)
        {
            Point p = Cursor.Position;
            Cursor.Position = new Point(p.X + (int)(Math.Cos(rotation) * distance), p.Y + (int)(Math.Sin(rotation) * distance));
        }

        /// <summary>
        /// Moves the mouse slightly towards a certain direction
        /// </summary>
        public static void MoveBy(Point to, float distance)
        {
            Point p = Cursor.Position;
            float rotation = (float)Math.Atan2(to.Y - p.Y, to.X - p.X);
            Cursor.Position = new Point(p.X + (int)(Math.Cos(rotation) * distance), p.Y + (int)(Math.Sin(rotation) * distance));
        }

        public static void MoveTo(Point to)
        {
            while (Stuff.DistanceSquared(Cursor.Position, to) > MoreDistanceSquared)
            {
                Point p = Cursor.Position;

                double rot = Math.Atan2(to.Y - p.Y, to.X - p.X);
                MoveBy((int)(Math.Cos(rot) * MoveDistance), (int)(Math.Sin(rot) * MoveDistance));

                Thread.Sleep(16);
            }
            Cursor.Position = to;
            Thread.Sleep(16);
        }

        public static void MakeLine(Point a, Point b)
        {
            MoveTo(a);
            MouseDown((uint)a.X, (uint)a.Y);
            MoveTo(b);
            MouseUp((uint)b.X, (uint)b.Y);
        }

        public static void MakeLineGroup(Point[] vertices)
        {
            if (vertices.Length == 0)
                return;

            float distToVertex = 0;
            float distLeft = MoveDistance;
            int index = 0;

            while (true)
            {
#if PRINT_MOVES
                Console.ForegroundColor = Colors.Normal;
#endif

                distLeft = MoveDistance;
                distToVertex = Stuff.Distance(Cursor.Position, vertices[index]);
                while (distLeft > 0)
                {
                    if (distLeft < distToVertex)
                    {
#if PRINT_MOVES
                        Console.WriteLine("[Input] (" + index + ") Moved by " + distLeft);
#endif
                        MoveBy(vertices[index], distLeft);
                        distLeft = 0;
                    }
                    else
                    {
                        while (distLeft >= distToVertex)
                        {
#if PRINT_MOVES
                            Console.WriteLine("[Input] (" + index + ") Closed at " + distToVertex);
#endif
                            distLeft -= distToVertex;
                            Cursor.Position = vertices[index];
                            index++;
                            if (index == vertices.Length)
                            {
#if PRINT_MOVES
                                Console.WriteLine("[Input] Linegroup size " + vertices.Length + " done.");
#endif
                                MouseUp();
                                return;
                            }
                            else if (index == 1)
                                MouseDown();
                            distToVertex = Stuff.Distance(Cursor.Position, vertices[index]);
                        }
                    }
                }

#if PRINT_MOVES
                Console.ForegroundColor = Colors.Message;
                Console.WriteLine("[Input] slept.");
#endif
                Thread.Sleep(SLPTime);
            }
        }

        /// <summary>
        /// Registers the windows key down event for a key
        /// </summary>
        public static void RegisterKeyDown(byte key)
        {
            keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY, 0);
        }

        /// <summary>
        /// Registers the windows key up event for a key
        /// </summary>
        public static void RegisterKeyUp(byte key)
        {
            keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }

        /// <summary>
        /// Registers the winows key down event for a key, followed by the key up event.
        /// </summary>
        public static void PressKey(byte key)
        {
            RegisterKeyDown(key);
            RegisterKeyUp(key);
        }

        /// <summary>
        /// Registers the key presses required to write the specified text
        /// </summary>
        /// <param name="text">The Text to type, supports caps and non-caps.</param>
        /// <param name="slp">The tile to sleep in milliseconds in between key presses</param>
        public static void KeyboardWrite(String text, int slp)
        {
            bool shiftDown = false;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (Char.IsUpper(c))
                {
                    if (!shiftDown)
                    {
                        RegisterKeyDown(KEY_SHIFT);
                        shiftDown = true;
                    }
                }
                else
                {
                    if(shiftDown)
                    {
                        RegisterKeyUp(KEY_SHIFT);
                        shiftDown = false;
                    }
                }
                PressKey((byte)Char.ToUpper(text[i]));
                Thread.Sleep(slp);
            }

            if (shiftDown)
                RegisterKeyUp(KEY_SHIFT);
        }

        /// <summary>
        /// Opens Paint by openin the Windows Start Menu, typing "Paint", waiting and pressing enter.
        /// <para>It is possible that it doesn't wait enough for the Paint search result to appear, in this case we're fucked.</para>
        /// </summary>
        public static void OpenPaint()
        {
            Console.ForegroundColor = Colors.Message;
            Console.WriteLine("[Input] Openning Paint... (hopefully)");
            PressKey(KEY_LWIN);
            Thread.Sleep(200);

            KeyboardWrite("Paint", 100);

            Thread.Sleep(1000);
            PressKey(KEY_ENTER);

        }

        /// <summary>
        /// Selects the brush tool in Paint by moving the mouse to it's location and registering a click.
        /// </summary>
        public static void PaintSelectBrush()
        {
            Console.ForegroundColor = Colors.Message;
            Console.WriteLine("[Input] Selecting Brush tool... (hopefully)");
            if (Stuff.IsWin10())
                MoveTo(new Point(336, 70)); //hopefully win10
            else
                MoveTo(new Point(420, 70)); //hopefully other
            RegisterClick();
        }

        /// <summary>
        /// Clicks the selection tool, selects the whole canvas and presses the delete key.
        /// </summary>
        public static void PaintClearImage()
        {
            Console.ForegroundColor = Colors.Message;
            Console.WriteLine("[Input] Clearing Paint... (hopefully)");

            
            MoveTo(new Point(134, 75));
            RegisterClick();

            MoveTo(new Point(Stuff.ScreenWidth - 50, Stuff.ScreenHeight - 85));
            MouseDown();
            MoveTo(new Point(0, 0));
            MouseUp();

            PressKey(KEY_DELETE);
        }
    }
}
