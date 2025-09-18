using Scraft.BlockSpace;
using UnityEngine;

namespace Scraft
{
    public class IPoint
    {

        public static IPoint zero = new IPoint(0, 0);
        public static IPoint up = new IPoint(0, 1);
        public static IPoint left = new IPoint(-1, 0);
        public static IPoint down = new IPoint(0, -1);
        public static IPoint right = new IPoint(1, 0);

        public int x;
        public int y;

        public IPoint()
        {

        }

        public IPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public IPoint(float x, float y)
        {
            this.x = Mathf.RoundToInt(x);
            this.y = Mathf.RoundToInt(y);
        }

        public string toString()
        {
            return "(" + x + "," + y + ")";
        }

        public bool equal(IPoint p)
        {
            return (x == p.x && y == p.y) ? true : false;
        }

        public float getStraightDistance(IPoint otherPoint)
        {
            float dx = x - otherPoint.x;
            float dy = y - otherPoint.y;
            return Mathf.Pow(dx * dx + dy * dy, 0.5f);
        }

        public int getFrameDistance(IPoint otherPoint)
        {
            int dx = Mathf.Abs(x - otherPoint.x);
            int dy = Mathf.Abs(y - otherPoint.y);
            return dx + dy;
        }

        public bool isNeighborBlock(IPoint otherPoint)
        {
            return (Mathf.Abs(otherPoint.x - x) <= 1) && (Mathf.Abs(otherPoint.y - y) <= 1);
        }

        public IPoint getDirPoint(int dir)
        {
            if (dir > 3)
            {
                dir -= 4;
            }
            else if (dir < 0)
            {
                dir += 4;
            }

            switch (dir)
            {
                case Dir.up:
                    return new IPoint(x, y + 1);
                case Dir.right:
                    return new IPoint(x + 1, y);
                case Dir.down:
                    return new IPoint(x, y - 1);
                case Dir.left:
                    return new IPoint(x - 1, y);
                default:
                    return new IPoint(0, 0);

            }
        }

        public bool isZeroPoint()
        {
            return (x == 0 && y == 0);
        }

        public bool isErrPoint()
        {
            return (x == 9999 && y == 9999);
        }

        public int relativeDir(IPoint p)
        {
            if (p.x > x)
                return Dir.right;
            if (p.x < x)
                return Dir.left;
            if (p.y > y)
                return Dir.up;
            if (p.y < y)
                return Dir.down;
            return -1;
        }

        public Vector3 mapIPoint2WordVector(IPoint mapSize, float z = 0)
        {
            return new Vector3((x - mapSize.x * 0.5f) * 0.16f, (y - mapSize.y * 0.5f) * 0.16f, z);
        }

        public Vector3 mapIPoint2WordVector(float z = 0)
        {
            return new Vector3((x - World.mapSize.x * 0.5f) * 0.16f, (y - World.mapSize.y * 0.5f) * 0.16f, z);
        }

        static public IPoint createMapIPointByWordVector(Vector3 v, IPoint mapRect)
        {
            return new IPoint(Mathf.RoundToInt(v.x * 6.25f), Mathf.RoundToInt(v.y * 6.25f)) + mapRect * 0.5f;
        }

        static public IPoint createMapIPointByWordVector(Vector3 v)
        {
            return createMapIPointByWordVector(v, World.mapSize);
        }

        public static IPoint operator +(IPoint a, IPoint b)
        {
            return new IPoint(a.x + b.x, a.y + b.y);
        }

        public static IPoint operator -(IPoint a, IPoint b)
        {
            return new IPoint(a.x - b.x, a.y - b.y);
        }

        public static IPoint operator *(IPoint a, float f)
        {
            return new IPoint(a.x * f, a.y * f);
        }

        public static bool operator >(IPoint a, IPoint b)
        {
            return (a.x > b.x && a.y > b.y) ? true : false;
        }

        public static bool operator <(IPoint a, IPoint b)
        {
            return (a.x < b.x && a.y < b.y) ? true : false;
        }
    }
}