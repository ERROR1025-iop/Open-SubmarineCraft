using Scraft.BlockSpace;

namespace Scraft
{
    public class DFSGraph
    {

        int w, h;
        int vertexNum;
        int[,] a;
        bool[,] isVisit;
        IPoint sp;


        public bool start(Block[,] map, IRect rect)
        {
            vertexNum = 0;
            w = rect.with + 1;
            h = rect.height + 1;
            a = new int[w, h];
            isVisit = new bool[w, h];
            bool isSetSp = false;
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    Block block = map[x + rect.x, y + rect.y];
                    if (block == null )
                    {
                        a[x, y] = 1;
                        if (!isSetSp)
                        {
                            sp = new IPoint(x, y);
                            isSetSp = true;
                        }
                    }
                    else
                    {
                        a[x, y] = 0;
                    }
                }
            }

            if (!isSetSp)
            {
                return true;
            }

            return startDFSGraph();
        }

        public bool startDFSGraph()
        {
            preDeal();
            bool set = true;
            int loop = 0;
            int judgeDFSCount = 0;
            a[sp.x, sp.y] = 2;

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    isVisit[x, y] = false;
                }
            }

            while (set)
            {
                set = false;
                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        if (isVisit[x, y] == false && a[x, y] == 2)
                        {
                            isVisit[x, y] = true;
                            bool r = DFS(x, y, 1, 2);
                            if (!set)
                            {
                                set = r;
                            }
                        }
                    }
                }
                loop++;
                if (loop > 50)
                {
                    break;
                }
            }

            bool isSetSp = false;
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (a[x, y] == 2)
                    {
                        judgeDFSCount++;
                    }
                }
            }
            //Debug.Log("loop:" + loop);
            //Debug.Log("judgeDFSCount:" + judgeDFSCount);
            //Debug.Log("vertexNum:" + vertexNum);
            return judgeDFSCount == vertexNum;
        }

        void preDeal()
        {
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    isVisit[x, y] = false;
                }
            }

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (a[x, y] == 0)
                    {
                        continue;
                    }
                    if (x == 0 || y == 0 || x == w - 1 || y == h - 1)
                    {
                        if (a[x, y] == 1)
                        {
                            a[x, y] = 2;
                        }
                    }
                    else
                    {
                        a[x, y] = 1;
                    }
                }
            }

            int forStack = 0;
            bool set = true;
            while (set)
            {
                set = false;
                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        if (isVisit[x, y] == false && a[x, y] == 2)
                        {
                            isVisit[x, y] = true;
                            bool r = DFS(x, y, 1, 2);
                            if (!set)
                            {
                                set = r;
                            }
                        }
                    }
                }
                forStack++;
                if (forStack > 50)
                {
                    //Debug.Log("forStack:" + forStack);
                    break;
                }
            }

            bool isSetSp = false;
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (a[x, y] != 2)
                    {
                        vertexNum++;
                        a[x, y] = 1;
                        if (!isSetSp)
                        {
                            sp = new IPoint(x, y);
                            isSetSp = true;
                        }
                    }
                    else
                    {
                        a[x, y] = 0;
                    }
                }
            }
        }

        //DFS递归
        bool DFS(int x, int y, int eq, int setto)
        {
            bool set = false;

            if (y + 1 < h && a[x, y + 1] == eq)
            {
                a[x, y + 1] = setto;
                set = true;
            }
            if (y - 1 >= 0 && a[x, y - 1] == eq)
            {
                a[x, y - 1] = setto;
                set = true;
            }
            if (x + 1 < w && a[x + 1, y] == eq)
            {
                a[x + 1, y] = setto;
                set = true;
            }
            if (x - 1 >= 0 && a[x - 1, y] == eq)
            {
                a[x - 1, y] = setto;
                set = true;
            }

            if (x + 1 < w && y + 1 < h && a[x + 1, y + 1] == eq)
            {
                a[x + 1, y + 1] = setto;
                set = true;
            }
            if (x - 1 >= 0 && y + 1 < h && a[x - 1, y + 1] == eq)
            {
                a[x - 1, y + 1] = setto;
                set = true;
            }
            if (x + 1 < w && y - 1 >= 0 && a[x + 1, y - 1] == eq)
            {
                a[x + 1, y - 1] = setto;
                set = true;
            }
            if (x - 1 >= 0 && y - 1 >= 0 && a[x - 1, y - 1] == eq)
            {
                a[x - 1, y - 1] = setto;
                set = true;
            }
            return set;
        }
    }
}