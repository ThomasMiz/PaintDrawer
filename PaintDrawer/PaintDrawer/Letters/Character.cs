using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace PaintDrawer.Letters
{
    /// <summary>
    /// Represents a single Character in a CharFont. Stores its Width, Height and line groups that make it.
    /// </summary>
    class Character
    {
        String name;
        float width, height;
        Vec2[][] lines;

        public float Width { get { return width; } }
        public float Height { get { return height; } }
        public String Name { get { return name; } }

        public Character(String file)
        {
            List<Vec2[]> list = new List<Vec2[]>(16);

            byte[] data = File.ReadAllBytes(file);
            ByteStream stream = new ByteStream(data);

            for (int i = 0; i < 5; i++)
                stream.ReadByte(); //preamble; 5 bytes i'm ignoring because I dont want to implement checking these

            stream.ReadByte(); //V1; ignoring these too, they are for "version control" but there arent different versions really...
            stream.ReadByte(); //V2

            name = stream.ReadString(); //name;

            stream.ReadByte(); //bg; a single byte indicating the background type of the ClusterWave scenario. Well, this ain't ClusterWave, m8

            width = stream.ReadFloat();
            height = stream.ReadFloat();

            stream.ReadVector2(); //powerup pos; once agian; dis ain't ClusterWave. dis mor slav dan that, no mor western

            int pp = stream.ReadByte(); //playercount; ve no need no player cont
            for (int i = 0; i < pp; i++) //ve beat thos 'murikans capitalist pigs
                stream.ReadVector2(); //show then who slav then report bak to great liader, to stalin

            while (stream.HasNext()) //i'm deeply sorry for the USSR references, my crippling depression takes over sometimes
            {
                switch(stream.ReadByte()) //so i'm just gonna read a byte to decide which shape to use
                {
                    case 1: //and hope my crippling depression is cured when I read a closed line loop (actually a polygon but oh well)
                        int c = stream.ReadInt32(); //western int tells slav how many adidas r coming
                        Vec2[] arr = new Vec2[c + 1]; //adidas array with extra stripes
                        for (int i = 0; i < c; i++) //for each adidas
                            arr[i] = stream.ReadVector2(); //store the stripes
                        arr[c] = arr[0]; //last stripe is same as first, make comunism reach back where it all started
                        list.Add(arr); //i'm terribly sorry if you read this. Contact me immediately to let me know you read it.
                        break;
                    case 2: //reads a rectangle: 4 floats containing Left, Top, Right, Bottom
                        Vec2 tl = stream.ReadVector2(), br = stream.ReadVector2();
                        list.Add(new Vec2[] { tl, new Vec2(br.X, tl.Y), br, new Vec2(tl.X, br.Y), tl });
                        break;

                    default: //reads a simple line group.
                    list.Add(stream.ReadVector2Array());
                        break;
                }
            }

            lines = list.ToArray();
        }

        public void Draw(Vec2 at, float size, bool distort)
        {
            if (distort)
                DrawDistorted(at, size);
            else
                DrawUndistorted(at, size);
        }

        /// <summary>
        /// Draws the character slightly distorted to give it a hand-written effect, varying each time it's drawn.
        /// </summary>
        /// <param name="at">The location in pixels in the screen the text should be drawn at</param>
        /// <param name="size">The height in pixels characters should have</param>
        public void DrawDistorted(Vec2 at, float size)
        {
            float multX = Stuff.Random(0.033f, 0.05f); // these values are used for distortion
            float multY = Stuff.Random(0.033f, 0.05f);

            float defX = Stuff.Random(6.29f);
            float defY = Stuff.Random(6.29f);

            float bulX = Stuff.Random(0.75f, 1.15f) * 6.29f;
            float bulY = Stuff.Random(0.75f, 1.15f) * 6.29f;

            // for each line group
            for (int i = 0; i < lines.Length; i++)
            {
                Vec2[] line = lines[i];
                Point[] points = new Point[line.Length];

                for (int c = 0; c < points.Length; c++)
                {
                    // turns the raw line group data into an array of points on the screen, applying some distortion.
                    // the distortion works by using wave functions, which is better than just adding random values
                    // per vertex position because lines with a lot of vertices will remain being lines and dont get
                    // just completely fucked up. This distortion is uniform across the character.
                    float mx = (float)Math.Sin(line[c].X * bulX + defX) * multX;
                    float my = (float)Math.Sin(line[c].Y * bulY + defY) * multY;

                    float x = at.X + (line[c].X + mx) * size;
                    float y = at.Y + (line[c].Y + my) * size;

                    points[c] = new Point((int)x, (int)y);
                }

                // and now Input handles the mouse moving for us.
                Input.MakeLineGroup(points);
            }
        }

        /// <summary>
        /// Draws the character without distortion, just plain ol' linegroup as it is.
        /// </summary>
        /// <param name="at">The location in pixels in the screen the text should be drawn at</param>
        /// <param name="size">The height in pixels characters should have</param>
        public void DrawUndistorted(Vec2 at, float size)
        {
            // for each line group
            for (int i = 0; i < lines.Length; i++)
            {
                // turns the raw line group data into an array of points on the screen
                Vec2[] line = lines[i];
                Point[] points = new Point[line.Length];

                for (int c = 0; c < points.Length; c++)
                {
                    float x = at.X + line[c].X * size;
                    float y = at.Y + line[c].Y * size;

                    points[c] = new Point((int)x, (int)y);
                }

                // and now Input handles the mouse moving for us.
                Input.MakeLineGroup(points);
            }
        }
    }
}
