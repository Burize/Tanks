using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
namespace Tank
{
    public class Wall:Block
    {
        public int HP;

        public Wall(Coordinates coordinates, int HP)
            :base(coordinates)
        {
            this.HP = HP;
        }

        public override void Draw(object sender, PaintEventArgs e)
        {
            SolidBrush brush = null;
            switch (HP)
            {
                case 2:
                    brush = new SolidBrush(Color.DarkRed);
                    break;
                case 1:
                    brush = new SolidBrush(Color.Red);
                    break;
                case 3:
                    brush = new SolidBrush(Color.Gray);
                    break;
            }
            e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y, 40, 40));
        }
    }
}
