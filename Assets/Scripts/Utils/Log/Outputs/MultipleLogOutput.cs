using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft
{
    public class MultipleLogOutput : LogOutput
    {
        private LogOutput[] mLogOutputs;

        public MultipleLogOutput(params LogOutput[] args)
        {
            mLogOutputs = args;
        }


        public override void write(int level, string message)
        {
            if (mLogOutputs != null)
            {
                foreach (LogOutput output in mLogOutputs)
                {
                    output.write(level, message);
                }
            }
        }
    }
}
