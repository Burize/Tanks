using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Tank
{
   public class Block: GameObject
    {
      public  Block(Coordinates coordinates)
            : base(coordinates)
        { }

       public override void Draw(object sender, PaintEventArgs e)
       { }
    }
}
