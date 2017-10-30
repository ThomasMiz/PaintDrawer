using System;
using PaintDrawer.Letters;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using WindowScrape.Types;
using System.Windows.Input;
using PaintDrawer.Actions;
using System.Collections.Generic;

namespace PaintDrawer
{
    static class Program
    {
        // Windows comlains if this isnt decorated with a STAThread
        [STAThread] 
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Paint Drawer V0.1!");
            //The drawing and stuff is in another thread
            Thread t = new Thread(ProcessStuff);
            t.Start();

            // The most cancer way to kill the program; literally kill it from another thread.
            while (true)
            {
                // Just press ASD at the same time and boom, you just fucked everything up :D
                if (Keyboard.IsKeyDown(Key.A) && Keyboard.IsKeyDown(Key.S) && Keyboard.IsKeyDown(Key.D))
                {
                    t.Abort();
                    Console.ForegroundColor = Colors.Error;
                    Console.WriteLine("[Main] Program has been killed.");
                    Input.MouseUp();
                    Console.ReadLine();
                    return;
                }
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// All the application runs in here while the main thread waits to maybe kill it.
        /// </summary>
        static void ProcessStuff()
        {
            Stuff.Init();

            CharFont font = new CharFont("CharFiles/MizConsole/");
            Actions.Actions.Init(font);

            Thread.Sleep(1000);

            HwndObject obj;
            if (GetPaintHwnd(out obj))
            {
                Console.ForegroundColor = Colors.Success;
                Console.WriteLine("[Program] Paint Hwnd found! using: " + obj.Text);
                obj.Activate();
                WindowScrape.Static.HwndInterface.ShowWindow(obj.Hwnd, 3);
            }
            else
            {
                Console.ForegroundColor = Colors.Message;
                Console.WriteLine("[Program] Paint Hwnd not found.");
                Input.OpenPaint();
                if (!ForceGetPaintHwnd(out obj))
                {
                    Console.ForegroundColor = Colors.Error;
                    Console.WriteLine("[Program] ERROR: Can't find Paint Hwnd. Process aborted.");
                    return;
                }
            }

            Thread.Sleep(1000);

            if (obj.Location.X > 10 && obj.Location.Y > 10 && obj.Size.Width < Stuff.ScreenWidth - 50 && obj.Size.Height < Stuff.ScreenHeight - 50)
            {
                Console.ForegroundColor = Colors.Message;
                Console.WriteLine("Paint window not maximized, maximizing...");
                Input.MoveTo(new Point(obj.Location.X + obj.Size.Width - 69, obj.Location.Y + 3));
                Input.RegisterClick();
                Thread.Sleep(100);
            }

            Input.PaintSelectBrush();

            while (true)
            {
                new DrawUndistortedChar(font, new Vec2(50, 170), (char)4).Act();
                return;

                Actions.Actions.RandomAction.Act();

                Thread.Sleep(2500);
                Input.PaintClearImage();
                Input.PaintSelectBrush();
            }
        }

        /// <summary>
        /// Tries 5 times to get a HwndObject corresponding to Paint's window. Returns true if one was found, false (and null) otherwise.
        /// </summary>
        /// <param name="obj">The HwndObject found</param>
        static bool ForceGetPaintHwnd(out HwndObject obj)
        {
            const int MaxTries = 5;

            List<HwndObject> list = HwndObject.GetWindows();
            int i = 0;
            while (true)
            {
                if (GetPaintHwnd(out obj))
                {
                    Console.ForegroundColor = Colors.Success;
                    Console.WriteLine("[Program] Found (hopefully) paint window: " + obj.Text);
                    return true;
                }

                if (++i >= MaxTries)
                {
                    Console.WriteLine(String.Concat("[Program] (", i, ')', " No Paint window found. Not retrying #yolo"));
                    obj = null;
                    return false;
                }
                Console.ForegroundColor = Colors.Error;
                Console.WriteLine(String.Concat("[Program] (", i, ")", "No Paint window found. Retrying in 1s..."));
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Tries once to get a HwndObject with the string "Paint". Returns whether any was found
        /// </summary>
        /// <param name="obj">The Hwnd object found</param>
        /// <returns></returns>
        static bool GetPaintHwnd(out HwndObject obj)
        {
            List<HwndObject> list = HwndObject.GetWindows();
            foreach (HwndObject o in list)
            {
                if (o.Text.Contains("Paint") && !o.Text.ToLower().Contains("drawer"))
                {
                    obj = o;
                    return true;
                }
            }
            obj = null;
            return false;
        }
    }
}