using System;
using Microsoft.Xna.Framework;

namespace ScenarioMaker.LinearMath
{
    abstract class Function
    {
        public abstract bool CollidesWith(LineFunction line, out Vector2 crossPoint);
        public abstract bool CollidesWith(VerticalFunc line, out Vector2 crossPoint);
    }
}
