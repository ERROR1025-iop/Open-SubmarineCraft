using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft
{
    public class Log
    {
        public static volatile int sOutputLevel = 1;

        private static LogInput sInput = new SimpleLogInput(new MultipleLogOutput(new SystemLogOutput()));

        public static LogInput input()
        {
            return sInput;
        }

        public static void setOutputLevel(int level)
        {
            sOutputLevel = level;
        }
    }
}
