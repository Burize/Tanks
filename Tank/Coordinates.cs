using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tank
{
   public struct Coordinates
    {
       public int x;
       public int y;
       public Coordinates(int x, int y)
       {
           this.x = x;
           this.y = y;
       }
    }

   public enum Direction { top, right, bottom, left }

   
}
