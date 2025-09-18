using Scraft.BlockSpace;
using UnityEngine;

namespace Scraft
{
    public class LargeBlockTap
    {

        static int MAX_Childs_Count = 21;
        Transform parent;
        Transform[] childs;

        IPoint placeCoor;
        bool m_isShowTap;

        public LargeBlockTap()
        {
            childs = new Transform[MAX_Childs_Count];
            parent = GameObject.Find("large block tap").transform;

            for (int i = 0; i < MAX_Childs_Count; i++)
            {
                childs[i] = parent.GetChild(i);
            }

            removeTapChilds();
            m_isShowTap = false;
        }

        public void placeLargeBlockTap(IPoint coor, LargeBlock largeBlockStatic)
        {
            IPoint size = largeBlockStatic.getSize();

            placeCoor = coor - (size * 0.5f - new IPoint(0, 1));
            int count = 0;
            for (int offsetx = 0; offsetx < size.x; offsetx++)
            {
                for (int offsety = 0; offsety < size.y; offsety++)
                {
                    IPoint obOffset = new IPoint(offsetx, offsety);
                    IPoint tapCoor = placeCoor + obOffset;
                    childs[count].localPosition = tapCoor.mapIPoint2WordVector();
                    count++;
                }
            }

            m_isShowTap = true;
        }

        public void removeTapChilds()
        {
            for (int i = 0; i < MAX_Childs_Count; i++)
            {
                childs[i].localPosition = new Vector3(9999, 0);
            }
        }

        public bool isShowTap()
        {
            return m_isShowTap;
        }

        public void onLargeBlockPlace()
        {
            m_isShowTap = false;
            removeTapChilds();
        }

        public IPoint getPlaceCoor()
        {
            return placeCoor;
        }
    }
}