using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft.BlockSpace
{
    public class PState
    {

        static public int special = 0;
        static public int solid = 1;
        static public int liquild = 2;
        static public int gas = 3;
        static public int mushy = 4;
        static public int particle = 5;
        static public int tool = 6;

        static public string getPStateLangName(int pstate)
        {
            return ILang.get("PState." + pstate);
        }
    }
}
