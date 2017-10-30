using System;
using Microsoft.Xna.Framework;

namespace ScenarioMaker.LinearMath
{
    class LineFunction : Function
    {
        public Vector2 from, to;
        float m, b;
        public LineFunction(Vector2 from, Vector2 to)
        {
            this.to = to;
            this.from = from;
            if (from.X > to.X)
            {
                Vector2 tmp = to;
                to = from;
                from = tmp;
            }

            m = (to.Y - from.Y) / (to.X - from.X);
            b = to.Y - m * to.X;
        }

        public float ValueAt(float x)
        {
            return m * x + b;
        }

        public override bool CollidesWith(LineFunction line, out Vector2 crossPoint)
        {
            if (m == line.m)
            {
                crossPoint = Vector2.Zero;
                return false;
            }
            float x = (line.b - b) / (m - line.m);
            float y = line.ValueAt(x);
            crossPoint = new Vector2(x, y);
            return Line.AreOrdered(from.X, x, to.X) && Line.AreOrdered(line.from.X, x, line.to.X);//(x >= from.X && x <= to.X) && (x >= line.from.X && x <= line.to.X);
            //x = (b2 - b1) / (m1 - m2)
        }

        public override bool CollidesWith(VerticalFunc line, out Vector2 crossPoint)
        {
            float y = ValueAt(line.x);
            crossPoint = new Vector2(line.x, y);
            bool ret = Line.AreOrdered(line.from, y, line.to) && Line.AreOrdered(from.X, line.x, to.X);//(y <= line.to && y >= line.from);
            return ret;
        }
    }
}
