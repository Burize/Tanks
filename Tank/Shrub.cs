using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Tank
{
    class Shrub: Block
    {

        public Shrub(Coordinates coordinates)
        :base(coordinates)
        { }

        public override void Draw(object sender, PaintEventArgs e)
        {
            SolidBrush brush = new SolidBrush(Color.DarkGreen);
            e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y + 4, 20, 12));
            e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 4, coordinates.y, 12, 20));
            e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 20, coordinates.y + 4, 20, 12));
            e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 20 + 4, coordinates.y, 12, 20));
            e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y + 4 + 20, 20, 12));
            e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 4, coordinates.y + 20, 12, 20));
            e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 20, coordinates.y + 4 + 20, 20, 12));
            e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 20 + 4, coordinates.y + 20, 12, 20));
        }
    
    }
}
