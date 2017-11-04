using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace PaintDrawer
{
    /// <summary>
    /// Contains random bits and pieces that are pretty self explanatory
    /// </summary>
    static class Stuff
    {
        public static int ScreenWidth, ScreenHeight;
        public static Random r;
        public static OperatingSystem OperatingSystem;

        public static float Random(float max) { return (float)r.NextDouble() * max; }
        public static float Random(float min, float max) { return (float)r.NextDouble() * (max - min) + min; }

        public static void Init()
        {
            r = new Random();
            ScreenWidth = Screen.PrimaryScreen.Bounds.Width;
            ScreenHeight = Screen.PrimaryScreen.Bounds.Height;
            OperatingSystem = Environment.OSVersion;

            Console.ForegroundColor = Colors.Message;
            Console.WriteLine("WinMajor=" + OperatingSystem.Version.Major);
            Console.WriteLine("WinMinor=" + OperatingSystem.Version.Minor);
            Console.WriteLine("Stuff Initiated.");
        }
        
        public static int DistanceSquared(Point a, Point b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;
            return (dx * dx + dy * dy);
        }

        public static float Distance(Point a, Point b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        public static Point Lerp(Point a, Point b, float lerp)
        {
            return new Point((int)(a.X + (b.X - a.X) * lerp), (int)(a.Y + (b.Y - a.Y) * lerp));
        }

        public static bool IsWin10()
        {
            return OperatingSystem.Version.Major == 6 && OperatingSystem.Version.Minor >= 2;
        }
    }

    static class Colors
    {
        public const ConsoleColor Error = ConsoleColor.Red;
        public const ConsoleColor Success = ConsoleColor.Green;
        public const ConsoleColor Message = ConsoleColor.Yellow;
        public const ConsoleColor Normal = ConsoleColor.Gray;
        
    }
}
