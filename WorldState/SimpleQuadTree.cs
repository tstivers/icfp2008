using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace ICFP08
{
    public class SimpleQuadTree
    {
        static readonly int MAX_DEPTH = 5;
        static readonly int SPLIT_ITEMS = 10;

        public RectangleF rect;
        public SimpleQuadTree[] children = null;
        public List<MarsObject> objects = new List<MarsObject>();
        public int depth;

        public SimpleQuadTree(RectangleF rect)
        {
            this.rect = rect;
            this.depth = 0;
        }

        public SimpleQuadTree(RectangleF rect, SimpleQuadTree parent)
        {
            this.rect = rect;
            this.depth = parent.depth + 1;

            foreach(MarsObject o in parent.objects)
            {
                if (this.rect.IntersectsWith(o.GetRect()))
                    AddObject(o);
            }
        }

        public void AddObject(MarsObject o)
        {
            if (children == null)
            {
                objects.Add(o);
                if(objects.Count > SPLIT_ITEMS && depth < MAX_DEPTH)
                    Split();
            }
            else
                foreach(SimpleQuadTree t in children)
                    if(t.rect.IntersectsWith(o.GetRect()))
                        t.AddObject(o);
        }

        private void Split()
        {
            children = new SimpleQuadTree[4];
            children[0] = new SimpleQuadTree(
                new RectangleF(rect.Left, rect.Top, rect.Width / 2.0f, rect.Height / 2.0f),
                this);
            children[1] = new SimpleQuadTree(
                new RectangleF(children[0].rect.Right, rect.Top, this.rect.Width / 2.0f, this.rect.Height / 2.0f),
                this);
            children[2] = new SimpleQuadTree(
                new RectangleF(rect.Left, children[0].rect.Bottom, this.rect.Width / 2.0f, this.rect.Height / 2.0f),
                this);
            children[3] = new SimpleQuadTree(
                new RectangleF(children[0].rect.Right, children[0].rect.Bottom, this.rect.Width / 2.0f, this.rect.Height / 2.0f),
                this);            
            objects = null;
        }

        public bool FindClosestCollision(Vector2d origin, float direction, float length, float padding, ref MarsObject obj, ref float distance, ref int tests)
        {
            Vector2d end = origin + (Vector2d.FromAngle(direction) * -length);
            
            // exit if line doesn't intersect
            if (!IntersectsLine(origin, end))
                return false;

            // test children
            if (children != null)
            {
                return FindClosestCollisionChildren(origin, direction, length, padding, ref obj, ref distance, ref tests);
            }

            // test objects
            float closest_range = float.MaxValue;
            MarsObject closest_object = null;
            foreach (MarsObject o in objects)
            {
                tests++;
                if (o.IntersectsLine(origin, end, padding))
                {
                    float range = (origin - o.Position).length() - (o.Radius + padding);
                    if (range < closest_range && range > 0.0f)
                    {
                        closest_range = range;
                        closest_object = o;
                    }
                }
            }

            if (closest_range != float.MaxValue)
            {
                obj = closest_object;
                distance = closest_range;
                return true;
            }

            return false;
        }

        // adapted from forum post at http://www.programmers-corner.com/forums/about25.html
        private bool IntersectSegments(Vector2d A1, Vector2d A2, Vector2d B1, Vector2d B2)
        {
            float fA1, fA2, fB1, fB2;
            
            fA1 = (B2 - B1) ^ (B1 - A1);
            fB1 = (A1 - B1) ^ (A2 - B1);
            fA2 = (B1 - A2) ^ (B2 - A2);
            fB2 = (A2 - B2) ^ (A1 - B2);

            return (fA1 <= 0 && fA2 <= 0 && fB1 <= 0 && fB2 <= 0) ||
                (fA1 >= 0 && fA2 >= 0 && fB1 >= 0 && fB2 >= 0);
        }

        private bool IntersectsLine(Vector2d origin, Vector2d end)
        {
            if (rect.Contains(origin) || rect.Contains(end))
                return true;
            Vector2d tl = new Vector2d(rect.X, rect.Y);
            Vector2d tr = new Vector2d(rect.Right, rect.Y);
            Vector2d bl = new Vector2d(rect.X, rect.Bottom);
            Vector2d br = new Vector2d(rect.Right, rect.Bottom);
            if (IntersectSegments(origin, end, tl, tr))
                return true;
            if (IntersectSegments(origin, end, tr, br))
                return true;
            if (IntersectSegments(origin, end, br, bl))
                return true;
            if (IntersectSegments(origin, end, bl, tl))
                return true;
            return false;
        }

        private bool FindClosestCollisionChildren(Vector2d origin, float direction, float length, float padding, ref MarsObject obj, ref float distance, ref int tests)
        {
            bool found_closer = false;

            foreach (SimpleQuadTree t in children)
            {
                float temp_distance = float.MaxValue;
                MarsObject temp_obj = null;

                if (t.FindClosestCollision(origin, direction, length, padding, ref temp_obj, ref temp_distance, ref tests))
                {
                    if (temp_distance < distance)
                    {
                        distance = temp_distance;
                        obj = temp_obj;
                        found_closer = true;
                    }
                }
            }
            return found_closer;
        }

        private bool FindFirstCollision(Vector2d origin, float direction, float length, float padding, ref MarsObject obj, ref float distance)
        {
            return false;
        }
    }

}
