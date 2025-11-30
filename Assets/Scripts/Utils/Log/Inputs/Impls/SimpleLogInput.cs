using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft
{
	public class SimpleLogInput : LogInput
	{
        public SimpleLogInput(LogOutput logOutput) : base(logOutput)
        {

        }

        protected override void write(int level, string message)
        {
            mLogOutput.write(level, message);
        }
    }
}
