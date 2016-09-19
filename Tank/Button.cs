using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
namespace Tank
{
    public class Button: IComparable<Button>
    {
        public Coordinates cordinates;
        public string title;
        public Boolean focus = false;

        public DelButton_Press method;

        public Button(Coordinates cordinates, string title, DelButton_Press method)
        {
            this.cordinates = cordinates;
            this.title = title;
            this.method += method;
        }

        public int CompareTo(Button button)
        {
            if (this.cordinates.y < button.cordinates.y)
                return -1;
            if (this.cordinates.y > button.cordinates.y)
                return 1;
            else
                return 0;
        }
        public void press_handler()
        {
            if (focus)
                method();
        }

        public void Draw(object sender, PaintEventArgs e)
        {
            Rectangle rec = new Rectangle(cordinates.x + 70, cordinates.y, 300, 50);

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;

            e.Graphics.DrawString(title, new Font("Times", 25), Brushes.White, rec, sf);

            if (focus)
            {
                e.Graphics.FillRectangle(Brushes.Yellow, new Rectangle(cordinates.x , cordinates.y, 26, 8));
                e.Graphics.FillRectangle(Brushes.Yellow, new Rectangle(cordinates.x , cordinates.y + 22, 26, 8));
                e.Graphics.FillRectangle(Brushes.Yellow, new Rectangle(cordinates.x + 4 , cordinates.y, 14, 30));
                e.Graphics.FillRectangle(Brushes.Yellow, new Rectangle(cordinates.x + 4 , cordinates.y + 13, 26, 4));
            }
        }

    }
}
