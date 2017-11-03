using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PaintDrawer.Letters
{
    class CharFont
    {
        public const int MaxCharNumericValue = 1024;
        private const float MultilineDiffY = 1.4f;

        public Character[] chars;

        //Creates a CharFont from the given directory
        public CharFont(String folder)
        {
            Console.ForegroundColor = Colors.Message;
            Console.WriteLine("[CharFont] Loading font from: " + folder);
            if (!Directory.Exists(folder))
            {
                Console.ForegroundColor = Colors.Error;
                Console.WriteLine("[CharFont] ERROR: Directory not found: " + folder);
                Console.WriteLine("[CharFont] Throwing Exception...");
                throw new DirectoryNotFoundException(folder);
            }

            chars = new Character[MaxCharNumericValue];
            String[] files = Directory.GetFiles(folder);

            for (int i = 0; i < files.Length; i++)
            {
                String f = files[i].Substring(files[i].LastIndexOf('/') + 1);
                int fi = f.IndexOf(';');
                int res;
                if (Int32.TryParse(f.Substring(0, fi), out res) && res > 0 && res < MaxCharNumericValue)
                {
                    if (chars[res] == null)
                        chars[res] = new Character(files[i]);
                    else
                    {
                        Console.ForegroundColor = Colors.Message;
                        Console.WriteLine("[CharFont] Warning! Character " + res + " or '" + ((char)res) + "' is defined more than once!");
                    }
                }
            }

            Console.ForegroundColor = Colors.Success;
            Console.WriteLine("Success loading font. Loaded " + files.Length + " characters.");
        }

        /// <summary>
        /// Draws a text wrapped in a certain space. Doesn't support splits with measure larget than width.
        /// </summary>
        public void DrawWrapped(String text, Vec2 at, float size, float width)
        {
            if (!IsStringOk(ref text))
            {
                Console.ForegroundColor = Colors.Error;
                Console.WriteLine("[CharFont] ERROR: The given string contains characters unknown to the font, drawing aborted.");
                return;
            }

            String[] enterSplit = text.Split('\n');
            float spaceWidth = Measure(size, " ").X;
            width -= spaceWidth;

            for (int c = 0; c < enterSplit.Length; c++)
            {
                String[] split = enterSplit[c].Split(' ');
                Vec2[] measures = Measure(size, split);

                float buildWid = 0;
                StringBuilder builder = new StringBuilder(128);
                for (int i = 0; i < split.Length; i++)
                {
                    float tmp = buildWid + measures[i].X;
                    float xd = Measure(size, builder.ToString()).X;
                    if (tmp >= width)
                    {
                        _draw(builder.ToString(), at, size);
                        at.Y += MultilineDiffY * size;
                        builder.Clear();
                        builder.Append(split[i]);
                        builder.Append(' ');
                        buildWid = measures[i].X + spaceWidth;
                    }
                    else
                    {
                        builder.Append(split[i]);
                        builder.Append(' ');
                        buildWid = tmp + spaceWidth;
                    }
                }
                _draw(builder.ToString(), at, size);
                at.Y += MultilineDiffY * size;
            }
        }

        /// <summary>
        /// Draws a String unwrapped on the screen. Doesn't check for out of screen bounds drawing.
        /// </summary>
        /// <param name="text">The String to draw</param>
        /// <param name="at">The screen location in pixels to start drawing at (the top-left)</param>
        /// <param name="size">The size in pixels of the font</param>
        public void Draw(String text, Vec2 at, float size)
        {
            if (IsStringOk(ref text))
                _draw(text, at, size);
            else
            {
                Console.ForegroundColor = Colors.Error;
                Console.WriteLine("[CharFont] ERROR: The given string contains characters unknown to the font, drawing aborted.");
            }
        }

        private void _draw(String text, Vec2 at, float size)
        {
            float startX = at.X;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\r')
                    continue;

                if (text[i] == '\n')
                {
                    at.X = startX;
                    at.Y += MultilineDiffY * size;
                }
                else
                {
                    int index = (int)text[i];
                    chars[index].DrawDistorted(at, size);
                    at.X += chars[index].Width * size * 1.1f;
                }
            }
        }

        /// <summary>
        /// Measures a String's size in pixels based on the given size
        /// </summary>
        /// <param name="size">The size the text is being drawn at</param>
        /// <param name="text">If I have to explain this parameter, something's wrong with you</param>
        public Vec2 Measure(float size, String text)
        {
            Vec2 s = new Vec2(0);
            float wid = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\r')
                    continue;

                if (text[i] == '\n')
                {
                    if (wid > s.X)
                        s.X = wid;
                    wid = 0;
                    s.Y += MultilineDiffY * size;
                }
                else
                    wid += chars[text[i]].Width * size;
            }
            if (wid > s.X)
                s.X = wid;
            return s;
        }

        /// <summary>
        /// Literally the same as Measure(float, String) but now String is an array and it calls Measure() for all the elements in it.
        /// </summary>
        public Vec2[] Measure(float size, String[] texts)
        {
            Vec2[] m = new Vec2[texts.Length];
            for (int i = 0; i < m.Length; i++)
                m[i] = Measure(size, texts[i]);
            return m;
        }

        /// <summary>
        /// Returns whether a char is loaded in the fond and is drawable
        /// </summary>
        /// <param name="c">The char to check if exists, duh</param>
        public bool DoesCharExist(char c)
        {
            int i = (int)c;
            return (i < MaxCharNumericValue && chars[i] != null) || c == '\n' || c == '\r';
        }

        /// <summary>
        /// Returns whether a String's chars are loaded in the fond and drawable
        /// </summary>
        /// <param name="s">The string to check</param>
        /// <returns></returns>
        public bool IsStringOk(ref String s)
        {
            for(int i=0; i<s.Length; i++)
                if (!DoesCharExist(s[i]))
                    return false;
            return true;
        }


        /// <summary>
        /// Calculates the area required for DrawWrapped to work inside the screen bounds. Returns whether the text is valid
        /// </summary>
        public bool CalculateDrawWrappedSize(String text, Vec2 at, float size, float width, out Vec2 drawSize)
        {
            drawSize = new Vec2(0);

            if (!IsStringOk(ref text))
                return false;

            String[] enterSplit = text.Split('\n');
            for (int c = 0; c < enterSplit.Length; c++)
            {
                String[] split = enterSplit[c].Split(' ');
                Vec2[] measures = Measure(size, split);
                float spaceWidth = Measure(size, " ").X;

                float buildWid = 0;
                StringBuilder builder = new StringBuilder(128);
                for (int i = 0; i < split.Length; i++)
                {
                    if (measures[i].X > width)
                        return false;

                    float tmp = buildWid + measures[i].X;
                    if (tmp > width)
                    {
                        at.Y += MultilineDiffY * size;
                        builder.Clear();
                        builder.Append(split[i]);
                        builder.Append(' ');
                        buildWid = measures[i].X + spaceWidth;
                        at.X = Math.Max(at.X, buildWid);
                    }
                    else
                    {
                        builder.Append(split[i]);
                        builder.Append(' ');
                        buildWid = tmp + spaceWidth;
                        at.X = Math.Max(at.X, buildWid);
                    }
                }
                
                at.Y += MultilineDiffY * size;
            }

            drawSize.Y = at.Y;
            return true;
        }
    }
}
