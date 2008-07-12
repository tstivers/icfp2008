using System;
using System.Collections.Generic;
using System.Text;

namespace ICFP08
{
    public struct Vector2d
    {
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return ((Vector2d)obj).x == this.x &&
                ((Vector2d)obj).y == this.y;
        }

        public override string ToString()
        {
            return "<" + x.ToString() + ", " + y.ToString() + ">";
        }

        public Vector2d(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public float angle(Vector2d other)
        {
            return (float)(Math.Atan2(other.y, other.x) - Math.Atan2(this.y, this.x));
        }

        public float angle()
        {
            return angle(new Vector2d(0, 1));
        }

        private double dot(Vector2d other)
        {
            return (this.x * other.x) + (this.y * other.y);
        }

        public Vector2d normalize()
        {
            float length = this.length();
            return new Vector2d(x / length, y / length);
        }

        public float length()
        {
 	        return (float)(Math.Sqrt((x * x) + (y * y)));
        }

        public float x;
        public float y;
    }
}
