using System;
using System.Collections.Generic;
using System.Text;



namespace PIC_Sim
{ 
   class Stack
	{
       int pointer;
       int[] returnPCL;
       Log Log = new Log();

       public stack 
       {
        returnPCL = new int[8];
        reset();
       }

        public int[] getStack()
        {
         return returnPCL;
        }

        public void push(int returnPCL)
        {
         if (pointer < 6 || pointer > -1)
         {
            pointer++;
            returnPCL[pointer] = returnPCL;   
         }
         else
         {
            //Log.Msg(8, "Stack overflow");
         }
        }

       public int pop() 
       {
          if (pointer > 0 || poinnter < 7)
          {
                int result = returnPCL[pointer];
                returnPCL[pointer] = 0;
                pointer--;
                return result;
          }
           else
           {
                Log.Msg(8, "Stack underflow");
                return 0;
           }
        }

        public void reset()
        {
           pointer = -1;
           for (int x = 0; x < returnPCL.Length; x++)
            returnPCL[x];
        }
	}
}
