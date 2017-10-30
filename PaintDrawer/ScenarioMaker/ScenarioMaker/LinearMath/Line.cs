using System;
using Microsoft.Xna.Framework;

namespace ScenarioMaker.LinearMath
{
    class Line
    {
        public Function funct;
        Vector2 _from, _to;
        public Vector2 from
        {
            get { return _from; }
        }
        public Vector2 to
        {
            get { return _to; }
        }

        public Line(float fromX, float fromY, float toX, float toY)
        {
            _from = new Vector2(fromX, fromY);
            _to = new Vector2(toX, toY);
            if (_from.X == _to.X)
                funct = new VerticalFunc(fromX, fromY, toY);
            else
                funct = new LineFunction(_from, _to);
        }

        public Line(Vector2 From, Vector2 To)
        {
            _from = From;
            _to = To;
            if (_from.X == _to.X)
                funct = new VerticalFunc(_from.X, _from.Y, _to.Y);
            else
                funct = new LineFunction(_from, _to);
        }

        public void ChangeValues(float fromX, float fromY, float toX, float toY)
        {
            _from.X = fromX;
            _from.Y = fromY;
            _to.X = toX;
            _to.Y = toY;
            if (_from.X == _to.X)
                funct = new VerticalFunc(_from.X, _from.Y, _to.Y);
            else
                funct = new LineFunction(_from, _to);
        }

        public void ChangeToX(float toX)
        {
            _to.X = toX;
            if (_from.X == _to.X)
                funct = new VerticalFunc(_from.X, _from.Y, _to.Y);
            else
                funct = new LineFunction(_from, _to);
        }

        public void ChangeFromY(float fromY)
        {
            _from.Y = fromY;
            if (_from.X == _to.X)
                funct = new VerticalFunc(_from.X, _from.Y, _to.Y);
            else
                funct = new LineFunction(_from, _to);
        }

        public static bool AreOrdered(float x, float y, float z)
        {
            return (x >= y && y >= z) || (x <= y && y <= z);
        }
    }
}
