using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace PaintDrawer.Letters
{
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
            List<Vec2[]> list = new List<Vec2[]>(5);

            byte[] data = File.ReadAllBytes(file);
            ByteStream stream = new ByteStream(data);

            for (int i = 0; i < 5; i++)
                stream.ReadByte(); //preamble;

            stream.ReadByte(); //V1
            stream.ReadByte(); //V2

            name = stream.ReadString(); //name

            stream.ReadByte(); //bg

            width = stream.ReadFloat();
            height = stream.ReadFloat();

            stream.ReadVector2(); //powerpos

            int pp = stream.ReadByte(); //playercount
            for (int i = 0; i < pp; i++)
                stream.ReadVector2();

            while (stream.HasNext())
            {
                switch(stream.ReadByte())
                {
                    case 1:
                        int c = stream.ReadInt32();
                        Vec2[] arr = new Vec2[c + 1];
                        for (int i = 0; i < c; i++)
                            arr[i] = stream.ReadVector2();
                        arr[c] = arr[0];
                        list.Add(arr);
                        break;
                    case 2:
                        Vec2 tl = stream.ReadVector2(), br = stream.ReadVector2();
                        list.Add(new Vec2[] { tl, new Vec2(br.X, tl.Y), br, new Vec2(tl.X, br.Y), tl });
                        break;

                    default:
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

        public void DrawDistorted(Vec2 at, float size)
        {
            float multX = Stuff.Random(0.05f, 0.1f);
            float multY = Stuff.Random(0.05f, 0.1f);

            float defX = Stuff.Random(6.29f);
            float defY = Stuff.Random(6.29f);

            float bulX = Stuff.Random(0.5f, 1.5f) * 6.29f;
            float bulY = Stuff.Random(0.5f, 1.5f) * 6.29f;

            for (int i = 0; i < lines.Length; i++)
            {
                Vec2[] line = lines[i];
                Point[] points = new Point[line.Length];

                for (int c = 0; c < points.Length; c++)
                {
                    float mx = (float)Math.Sin(line[c].X * bulX + defX) * multX;
                    float my = (float)Math.Sin(line[c].Y * bulY + defY) * multY;

                    float x = at.X + (line[c].X + mx) * size;
                    float y = at.Y + (line[c].Y + my) * size;

                    points[c] = new Point((int)x, (int)y);
                }

                Input.MakeLineGroup(points);
            }
        }

        public void DrawUndistorted(Vec2 at, float size)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                Vec2[] line = lines[i];
                Point[] points = new Point[line.Length];

                for (int c = 0; c < points.Length; c++)
                {
                    float x = at.X + line[c].X * size;
                    float y = at.Y + line[c].Y * size;

                    points[c] = new Point((int)x, (int)y);
                }

                Input.MakeLineGroup(points);
            }
        }
    }
}
