using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintDrawer.Letters
{
    struct Vec2
    {
        public float X, Y;

        public Vec2(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public Vec2(float XY)
        {
            X = XY;
            Y = XY;
        }

        public override string ToString()
        {
            return "{" + X.ToString() + ", " + Y.ToString() + "}";
        }
    }
}
