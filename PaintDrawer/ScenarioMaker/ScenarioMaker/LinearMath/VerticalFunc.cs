using System;
using Microsoft.Xna.Framework;

namespace ScenarioMaker.LinearMath
{
    class VerticalFunc : Function
    {
        public float x, to, from;
        public VerticalFunc(float x, float yFrom, float yTo)
        {
            this.x = x;
            if (yFrom > yTo)
            {
                from = yTo;
                to = yFrom;
            }
            else
            {
                from = yFrom;
                to = yTo;
            }
        }

        public override bool CollidesWith(VerticalFunc line, out Vector2 crossPoint)
        {
            crossPoint = Vector2.Zero;
            return false;
        }

        public override bool CollidesWith(LineFunction line, out Vector2 crossPoint)
        {
            return line.CollidesWith(this, out crossPoint);
        }
    }
}
