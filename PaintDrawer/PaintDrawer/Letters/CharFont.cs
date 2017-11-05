using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PaintDrawer.Letters
{
    class CharFont
    {
        /// <summary>The maximum numeric value of characters the font can hold</summary>
        public const int MaxCharNumericValue = 1024;

        private const float MultilineDiffY = 1.4f, WidthMult = 1.1f;

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
                if (Int32.TryParse(f.Substring(0, fi), out res) && res >= 0 && res < MaxCharNumericValue)
                {
                    if (chars[res] == null)
                        chars[res] = new Character(files[i]);
                    else
                    {
                        Console.ForegroundColor = Colors.Message;
                        Console.WriteLine("[CharFont] Warning! Character " + res + " or '" + ((char)res) + "' is defined more than once!");
                    }
                }
                else
                {

                }
            }

            Console.ForegroundColor = Colors.Success;
            Console.WriteLine("Success loading font. Loaded " + files.Length + " characters.");
        }

        /// <summary>
        /// Draws a text wrapped in a certain space. Will not check for text overflow to the bottom of the screen,
        /// use CalculateDrawWrappedSize to check that.
        /// </summary>
        public void DrawWrapped(String text, Vec2 at, float size, float width)
        {
            if (!IsStringOk(ref text))
            {
                Console.ForegroundColor = Colors.Error;
                Console.WriteLine("[CharFont] ERROR: The given string contains characters unknown to the font, drawing aborted.");
                return;
            }

            int nextStartChar = 0, lastSpace = 0, from, to;
            float builWid = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\r')
                    continue;

                if (text[i] == ' ')
                    lastSpace = i;

                if (text[i] == '\n')
                {
                    // flush
                    from = nextStartChar;
                    to = i;
                    if (to > from)
                        _draw(text.Substring(from, to - from), at, size);

                    nextStartChar = Math.Max(to + 1, 0);
                    while (nextStartChar < text.Length && text[nextStartChar] == '\r') nextStartChar++;
                    lastSpace = nextStartChar;
                    at.Y += MultilineDiffY * size;
                    builWid = Measure(size, text, nextStartChar, i - nextStartChar).X;
                }
                else
                {
                    float tmp = builWid + Measure(size, text[i]);
                    if (tmp >= width)
                    {
                        // flush
                        from = nextStartChar;
                        to = lastSpace;
                        if (from >= to)
                        {
                            // no spaces? oh well, cut it in half.
                            to = i;
                        }
                        if (to > from)
                            _draw(text.Substring(from, to - from), at, size);

                        nextStartChar = Math.Max(to + 1, 0);
                        while (nextStartChar < text.Length && text[nextStartChar] == '\r') nextStartChar++;
                        lastSpace = nextStartChar;
                        at.Y += MultilineDiffY * size;
                        builWid = Measure(size, text, nextStartChar, i - nextStartChar + 1).X;
                    }
                    else
                        builWid = tmp;
                }
            }

            // flush
            from = nextStartChar;
            to = text.Length;
            if (to > from)
                _draw(text.Substring(from, to - from), at, size);
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

        /// <summary>
        /// Draws a text without any checking. Just plain fucking does it.
        /// </summary>
        private void _draw(String text, Vec2 at, float size)
        {
            float startX = at.X;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\r') //say the fuck you want, a quick google search said this is ok.
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
                    at.X += chars[index].Width * size * WidthMult;
                }
            }
        }

        /// <summary>
        /// Measures a String's size in pixels based on the given size
        /// <para>This method will throw an exception if any of the string's characters are invalid.</para>
        /// </summary>
        /// <param name="size">The size the text is being drawn at</param>
        /// <param name="text">The text to measure</param>
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
                    s.Y += MultilineDiffY;
                }
                else
                    wid += chars[text[i]].Width;
            }
            if (wid > s.X)
                s.X = wid;

            return new Vec2(s.X * size * WidthMult, s.Y * size);
        }

        /// <summary>
        /// Measures a String's size in pixels based on the given size
        /// <para>This method will throw an exception if any of the string's characters are invalid.</para>
        /// </summary>
        /// <param name="size">The size the text is being drawn at</param>
        /// <param name="text">The text to measure</param>
        /// <param name="index">The index to start from</param>
        /// <param name="length">The amount of characters to measure</param>
        public Vec2 Measure(float size, String text, int index, int length)
        {
            Vec2 s = new Vec2(0);
            float wid = 0;

            for (int indx = 0; indx < length; indx++)
            {
                int i = indx + index;
                if (text[i] == '\r')
                    continue;

                if (text[i] == '\n')
                {
                    if (wid > s.X)
                        s.X = wid;
                    wid = 0;
                    s.Y += MultilineDiffY;
                }
                else
                    wid += chars[text[i]].Width;
            }
            if (wid > s.X)
                s.X = wid;

            return new Vec2(s.X * size * WidthMult, s.Y * size);
        }

        /// <summary>
        /// Measures a chars's size in pixels based on the given size
        /// <para>This method will throw an exception if the character is invalid.</para>
        /// </summary>
        /// <param name="size">The size the text is being drawn at</param>
        /// <param name="c">The character to measure</param>
        public float Measure(float size, char c)
        {
            return chars[(int)c].Width * size * WidthMult;
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
            for (int i = 0; i < s.Length; i++)
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

            int nextStartChar = 0, lastSpace = 0, from, to;
            float builWid = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\r')
                    continue;

                if (text[i] == ' ')
                    lastSpace = i;

                if (text[i] == '\n')
                {
                    // flush
                    from = nextStartChar;
                    to = i;
                    if (to > from)
                        drawSize.X = Math.Max(drawSize.X, Measure(size, text.Substring(from, to - from)).X);

                    nextStartChar = Math.Max(to + 1, 0);
                    while (nextStartChar < text.Length && text[nextStartChar] == '\r') nextStartChar++;
                    lastSpace = nextStartChar;
                    drawSize.Y += MultilineDiffY * size;
                    builWid = Measure(size, text, nextStartChar, i - nextStartChar).X;
                }
                else
                {
                    float tmp = builWid + Measure(size, text[i]);
                    if (tmp >= width)
                    {
                        // flush
                        from = nextStartChar;
                        to = lastSpace;
                        if (from >= to)
                        {
                            // no spaces? oh well, cut it in half.
                            to = i;
                        }
                        if (to > from)
                            drawSize.X = Math.Max(drawSize.X, Measure(size, text.Substring(from, to - from)).X);

                        nextStartChar = Math.Max(to + 1, 0);
                        while (nextStartChar < text.Length && text[nextStartChar] == '\r') nextStartChar++;
                        lastSpace = nextStartChar;
                        drawSize.Y += MultilineDiffY * size;
                        builWid = Measure(size, text, nextStartChar, i - nextStartChar + 1).X;
                    }
                    else
                        builWid = tmp;
                }
            }

            // flush
            from = nextStartChar;
            to = text.Length;
            if (to > from)
                drawSize.X = Math.Max(drawSize.X, Measure(size, text.Substring(from, to - from)).X);
            drawSize.Y += MultilineDiffY * size;

            return true;
        }
    }
}
