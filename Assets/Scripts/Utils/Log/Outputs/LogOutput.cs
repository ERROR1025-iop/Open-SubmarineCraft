using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft
{
	public abstract class LogOutput 
	{
		public abstract void write(int level, string message);
	}
}
