using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft
{
    public class IRect
    {

        public int x;
        public int y;
        public int with;
        public int height;

        IPoint min_p;
        IPoint max_p;

        public static IRect ZeroRect = new IRect(0, 0, 0, 0);

        public IRect()
        {

        }

        public IRect(int x, int y, int with, int height)
        {
            this.x = x;
            this.y = y;
            this.with = with;
            this.height = height;

            min_p = new IPoint(x, y);
            max_p = new IPoint(x + with, y + height);
        }

        static public IRect createByCenter(IPoint range, IPoint center)
        {
            IPoint p = range * 0.5f;
            return new IRect(center.x - p.x, center.y - p.y, range.x, range.y);
        }

        public void setWith(int w)
        {
            with = w;
            max_p = new IPoint(x + with, y + height);
        }

        public void setHeight(int h)
        {
            height = h;
            max_p = new IPoint(x + with, y + height);
        }

        public void setMinX(int x)
        {
            this.x = x;
            min_p = new IPoint(x, y);
            max_p = new IPoint(x + with, y + height);
        }

        public void setMinY(int y)
        {
            this.y = y;
            min_p = new IPoint(x, y);
            max_p = new IPoint(x + with, y + height);
        }

        public void setMaxX(int MaxX)
        {
            this.with = MaxX - x;
            min_p = new IPoint(x, y);
            max_p = new IPoint(MaxX, y + height);
        }

        public void setMaxY(int MaxY)
        {
            this.height = MaxY - y;
            min_p = new IPoint(x, y);
            max_p = new IPoint(x + with, MaxY);
        }

        public int getMaxX()
        {
            return max_p.x;
        }

        public int getMaxY()
        {
            return max_p.y;
        }

        public int getMinX()
        {
            return min_p.x;
        }

        public int getMinY()
        {
            return min_p.y;
        }

        public bool containsPoint(IPoint point)
        {
            if (point.x >= getMinX() && point.x <= getMaxX() && point.y >= getMinY() && point.y <= getMaxY())
            {
                return true;
            }

            return false;
        }
    }
}