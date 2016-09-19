using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Tank
{
    public class Ice:Block
    {
        public Ice(Coordinates coordinates)
            :base(coordinates)
        { }

        public override void Draw(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Gainsboro, new Rectangle(coordinates.x, coordinates.y, 40, 40));
        }
    }
}
