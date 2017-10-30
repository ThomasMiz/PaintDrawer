using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ScenarioMaker.LinearMath
{
    class Raycast
    {
        List<Line> lines;
        List<Vector2> contacts;
        Vector2 to, from, nearest;
        float length = -1;
        bool nearestNotCalculated = true;

        public Vector2 To { get { return to; } }
        public Vector2 From { get { return from; } }

        public bool HasNoContacts { get { return contacts.Count == 0; } }

        public float Lenght
        {
            get
            {
                if (length == -1)
                    length = Vector2.Distance(from, NearestContact);
                return length;
            }
        }

        public Vector2 NearestContact
        {
            get
            {
                if (nearestNotCalculated && contacts.Count != 0)
                {
                    nearestNotCalculated = false;
                    nearest = contacts[0];
                    float dis = Vector2.DistanceSquared(from, nearest);
                    for (int i = 1; i < contacts.Count; i++)
                    {
                        float d = Vector2.DistanceSquared(from, contacts[i]);
                        if (d < dis)
                        {
                            dis = d;
                            nearest = contacts[i];
                        }
                    }
                }
                return nearest;
            }
        }

        public Raycast(List<Line> lines, Vector2 from, Vector2 to)
        {
            this.to = to;
            this.from = from;
            this.lines = lines;
            Cast();
        }

        public Raycast(List<Line> lines, Vector2 from, float castRotation)
        {
            this.from = from;
            this.to = new Vector2(from.X + (float)Math.Cos(castRotation) * 9999, from.Y + (float)Math.Sin(castRotation) * 9999);
            this.lines = lines;
            Cast();
        }

        private void Cast()
        {
            //if (lines.Count == 0)
            //    contacts = new List<Vector2>(1);
            //else
            {
                contacts = new List<Vector2>(lines.Count);
                if (to.X == from.X)
                    CastVertical();
                else
                    CastHorizontal();
            }
        }

        private void CastVertical()
        {
            VerticalFunc func = new VerticalFunc(to.X, from.Y, to.Y);
            for (int i = 0; i < lines.Count; i++)
            {
                Vector2 vecOut;
                if (lines[i].funct.CollidesWith(func, out vecOut))
                    contacts.Add(vecOut);
            }
        }

        private void CastHorizontal()
        {
            LineFunction func = new LineFunction(from, to);
            for(int i=0; i<lines.Count; i++)
            {
                Vector2 vecOut;
                if (lines[i].funct.CollidesWith(func, out vecOut))
                    contacts.Add(vecOut);
            }
        }
    }
}
