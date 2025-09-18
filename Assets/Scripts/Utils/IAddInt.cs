using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scraft
{
    public class IAddInt
    {

        int stack;
        int max;

        public IAddInt(int start, int max)
        {
            stack = start;
            this.max = max;
        }

        public IAddInt(int max)
        {
            stack = 0;
            this.max = max;
        }

        public int add()
        {
            stack++;
            if (stack > max)
            {
                stack = 0;
            }
            return stack;
        }

        public int get()
        {
            return stack;
        }

        public void set(int value)
        {
            stack = value;
        }
    }
}
