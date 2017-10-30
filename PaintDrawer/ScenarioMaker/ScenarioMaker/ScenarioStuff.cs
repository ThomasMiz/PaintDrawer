using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ScenarioMaker
{
    class Line
    {
        Vector2 t, f;
        public Shape belongTo;

        public Vector2 from
        {
            get { return f; }
            set
            {
                f = value;
            }
        }
        public Vector2 to
        {
            get { return t; }
            set
            {
                t = value;
            }
        }

        public Line(Vector2 from, Vector2 to, Shape belongTo)
        {
            this.belongTo = belongTo;
            this.f = from;
            this.t = to;
        }

        public Line(float fromX, float fromY, float toX, float toY, Shape belongTo)
        {
            this.belongTo = belongTo;
            f = new Vector2(fromX, fromY);
            t = new Vector2(toX, toY);
        }

        public void ChangeValues(float fromX, float fromY, float toX, float toY)
        {
            f = new Vector2(fromX, fromY);
            t = new Vector2(toX, toY);
            belongTo.RecalcVertex();
        }
    }

    class Shape
    {
        public const byte TypeLinegroup = 0, TypePoly = 1, TypeRect = 2;
        public Line[] lines;
        protected VertexBuffer buffer;
        /// <summary>Type: 0=Shape; 1=Polygon; 2=Rectangle</summary>
        public virtual byte Type { get { return TypeLinegroup; } }
        public virtual bool IsOk { get { return true; } }

        public Shape() { }
		~Shape(){ buffer.Dispose(); }
        public Shape(Vector2[] vertices)
        {
            lines = new Line[vertices.Length - 1];
            for (int i = 0; i < lines.Length; )
                lines[i] = new Line(vertices[i++], vertices[i], this);
            buffer = new VertexBuffer(Game1.game.GraphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
            RecalcVertex();
        }

        public virtual void Draw(GraphicsDevice device)
        {
            device.SetVertexBuffer(buffer);
            device.DrawPrimitives(PrimitiveType.LineStrip, 0, buffer.VertexCount - 1);
        }

        public virtual void RecalcVertex()
        {
            VertexPositionColor[] data = new VertexPositionColor[lines.Length+1];
            for (int i = 0; i < lines.Length; i++)
                data[i] = new VertexPositionColor(new Vector3(lines[i].from, 0), Scenes.Editor.vertexColor);
            data[lines.Length] = new VertexPositionColor(new Vector3(lines[lines.Length - 1].to, 0), Scenes.Editor.vertexColor);
            buffer.SetData(data);
        }

        public virtual Shape Duplicate()
        {
            Vector2[] vert = new Vector2[lines.Length + 1];
            int i;
            for (i = 0; i < lines.Length; i++)
                vert[i] = lines[i].from;
            vert[i] = lines[i-1].to;
            return new Shape(vert);
        }
    }

    class Polygon : Shape
    {
        public override byte Type { get { return TypePoly; } }
        public override bool IsOk { get { return isOk; } }

        bool isOk = true;
        List<LinearMath.Line> mathlines;

        public Polygon(Vector2[] vertices)
        {
            lines = new Line[vertices.Length];
            int max = lines.Length - 1;
            for (int i = 0; i < max; )
                lines[i] = new Line(vertices[i], vertices[++i], this);
            lines[max] = new Line(vertices[max], vertices[0], this);
            buffer = new VertexBuffer(Game1.game.GraphicsDevice, typeof(VertexPositionColor), vertices.Length+2+vertices.Length, BufferUsage.WriteOnly);
            mathlines = new List<LinearMath.Line>(vertices.Length+1);
            RecalcVertex();
        }

        public Polygon(Vector2[] vertices, Vector2 offset)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].X += offset.X;
                vertices[i].Y += offset.Y;
            }
            lines = new Line[vertices.Length];
            int max = lines.Length - 1;
            for (int i = 0; i < max; )
                lines[i] = new Line(vertices[i], vertices[++i], this);
            lines[max] = new Line(vertices[0], vertices[max], this);
            buffer = new VertexBuffer(Game1.game.GraphicsDevice, typeof(VertexPositionColor), vertices.Length + 2 + vertices.Length, BufferUsage.WriteOnly);
            mathlines = new List<LinearMath.Line>(vertices.Length + 1);
            RecalcVertex();
        }

        public override Shape Duplicate()
        {
            Vector2[] vert = new Vector2[lines.Length];
            int i;
            for (i = 0; i < lines.Length; i++)
                vert[i] = lines[i].from;
            return new Polygon(vert);
        }

        private Color[] GetLineColors()
        {
            isOk = true;
            Color[] colors = new Color[lines.Length + 1];

            mathlines.Clear();
            for (int i = 0; i < lines.Length; i++)
                mathlines.Add(new LinearMath.Line(lines[i].from, lines[i].to));
            for (int i = 0; i < lines.Length; i++)
            {
                List<LinearMath.Line> collide = new List<LinearMath.Line>(mathlines);
                collide.Remove(mathlines[i]);
                collide.Remove(mathlines[(i + 1) % mathlines.Count]);
                collide.Remove(mathlines[(mathlines.Count + i - 1) % mathlines.Count]);
                

                LinearMath.Raycast cast = new LinearMath.Raycast(collide, lines[i].from, lines[i].to);

                if (cast.HasNoContacts)
                    colors[i] = Scenes.Editor.vertexColor;
                else
                {
                    isOk = false;
                    colors[i] = Color.Red;
                }
            }

            return colors;
        }

        public override void RecalcVertex()
        {
            VertexPositionColor[] data = new VertexPositionColor[lines.Length + lines.Length + 2];
            Color[] c = GetLineColors();
            int index = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                data[index++] = new VertexPositionColor(new Vector3(lines[i].from, 0), c[i]);
                data[index++] = new VertexPositionColor(new Vector3(lines[i].to, 0), c[i]);
            }
            buffer.SetData(data);
        }

        public override void Draw(GraphicsDevice device)
        {
            device.SetVertexBuffer(buffer);
            device.DrawPrimitives(PrimitiveType.LineList, 0, buffer.VertexCount / 2);
        }
    }

    class Rect : Polygon
    {
        public override byte Type { get { return TypeRect; } }
        public override bool IsOk { get { return true; } }

        float x, y, wid, hei;
        public float Width { get { return wid; } }
        public float Height { get { return hei; } }

        public Rect(float x, float y, float width, float height)
            : base(new Vector2[]{
                new Vector2(x, y), new Vector2(x+width, y),
                new Vector2(x+width, y+height), new Vector2(x, y+height) })
        {
            wid = width;
            hei = height;
            this.x = x;
            this.y = y;
        }

        public void SetSize(float width, float height)
        {
            wid = width;
            hei = height;
            float right = lines[0].from.X + width, down = lines[0].from.Y + height;

            //lines[0].ChangeToX(right);
            lines[0].to = new Vector2(right, lines[0].to.Y);
            lines[1].ChangeValues(right, lines[1].from.Y, right, down);
            lines[2].ChangeValues(right, down, lines[2].to.X, down);
            //lines[3].ChangeFromY(down);
            lines[3].from = new Vector2(lines[0].from.X, down);

            RecalcVertex();
        }

        public Polygon ToPoly()
        {
            return new Polygon(new Vector2[]{lines[0].from, lines[1].from, lines[2].from, lines[3].from});
        }

        public override Shape Duplicate()
        {
            return new Rect(x, y, wid, hei);
        }

        public override void RecalcVertex()
        {
            VertexPositionColor[] data = new VertexPositionColor[lines.Length + 1];
            for (int i = 0; i < lines.Length; i++)
                data[i] = new VertexPositionColor(new Vector3(lines[i].from, 0), Scenes.Editor.vertexColor);
            data[lines.Length] = data[0];
            buffer.SetData(data);
        }

        public virtual void Draw(GraphicsDevice device)
        {
            device.SetVertexBuffer(buffer);
            device.DrawPrimitives(PrimitiveType.LineStrip, 0, buffer.VertexCount - 1);
        }
    }
}
