using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class Dir
    {

        public const int up = 0;
        public const int right = 1;
        public const int down = 2;
        public const int left = 3;
        public const int centre = 4;

        public static int addDir(int dir1, int dir2)
        {
            dir1 = dir1 + dir2;
            if (dir1 > Dir.left)
            {
                dir1 -= 4;
            }
            return dir1;
        }
    }
}
