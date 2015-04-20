using System;
using System.Collections.Generic;
using System.Text;

namespace PIC_Simulator_deluxe
{
    class Stack
    {
        int pointer;
        int[] returnPCLs;
        Log Log = new Log();

        public Stack()
        {
            returnPCLs = new int[8];
            reset();
        }

        public int[] getStack() 
        {
            return returnPCLs;
        }

        public void reset()
        {
            pointer = -1;
            for (int i = 0; i < returnPCLs.Length; i++)
                returnPCLs[i] = 0;
        }

        public void push(int returnPCL)
        {
            if (pointer > 6 || pointer < -1)
            {
                //Log.msg(8, "Stack overflow");
                
                //throw new CPURuntimeError();
            }
            else
            {
                pointer++;
                returnPCLs[pointer] = returnPCL;
            }
        }

        public int pop()
        {
            if (pointer < 0 || pointer > 7)
            {
                Log.msg(8, "Stack underflow");
                return 0;
                //throw new CPURuntimeError();
            }
            else
            {

                int result = returnPCLs[pointer];
                returnPCLs[pointer] = 0;
                pointer--;
                return result;
            }
        }
    }
}
