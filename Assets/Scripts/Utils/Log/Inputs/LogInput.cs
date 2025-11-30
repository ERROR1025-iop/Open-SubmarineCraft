using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft
{
	public abstract class LogInput 
	{
        protected LogOutput mLogOutput;

        public LogInput(LogOutput logOutput)
        {
            mLogOutput = logOutput;
        }

        public void debug(string message)
        {
            write(0, message);
        }
        public void info(string message)
        {
            write(1, message);
        }
        public void warn(string message)
        {
            write(2, message);
        }

        protected abstract void write(int level, string message);
    }
}
