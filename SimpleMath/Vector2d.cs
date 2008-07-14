using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ICFP08
{
    public struct Vector2d
    {
        public static Vector2d FromAngle(float angle)
        {
            return new Vector2d(angle);
        }

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

        public Vector2d(float angle)
        {
            this.x = (float)Math.Cos(((-1.0f * angle) - 90) * (Math.PI / 180.0f));
            this.y = (float)Math.Sin(((-1.0f * angle) - 90) * (Math.PI / 180.0f));
        }

        public float angle(Vector2d other)
        {
            return (float)(Math.Atan2(other.y, other.x) - Math.Atan2(this.y, this.x));
        }

        public float angle()
        {
            return angle(new Vector2d(0, 1));
        }

        public float dot(Vector2d other)
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

        public Vector2d lerp(Vector2d end, float distance)
        {
            return ((end - this) * distance) + this;
        }

        public static Vector2d operator +(Vector2d v1, Vector2d v2)
        {
            return new Vector2d(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector2d operator -(Vector2d v1, Vector2d v2)
        {
            return new Vector2d(v1.x - v2.x, v1.y - v2.y);
        }

        public static Vector2d operator *(float f, Vector2d v)
        {
            return new Vector2d(v.x * f, v.y * f);
        }

        public static Vector2d operator *(Vector2d v, float f)
        {
            return new Vector2d(v.x * f, v.y * f);
        }

        public static float operator *(Vector2d v1, Vector2d v2)
        {
            return v1.dot(v2);
        }

        public static float operator ^(Vector2d v1, Vector2d v2)
        {
            return v1.cross(v2);
        }

        private float cross(Vector2d other)
        {
            return (this.x * other.y) - (this.y * other.x);
        }

        public static implicit operator PointF(Vector2d v)
        {
            return new PointF(v.x, v.y);
        }

        public float x;
        public float y;
    }
}
