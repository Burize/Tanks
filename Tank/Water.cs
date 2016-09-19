using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Tank
{
    class Water: Block
    {
        public Water(Coordinates coordinates)
            :base(coordinates)
         {  }

        public override void Draw(object sender, PaintEventArgs e)
        {
             SolidBrush brush= new SolidBrush(Color.DarkBlue);
             e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y, 40, 40));
        }
    }
}
