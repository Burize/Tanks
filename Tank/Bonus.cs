using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Tank
{
    public enum BonusType { bomb, shield, live, lvl, timestop, beton }

    public class Bonus:GameObject
    {
        public BonusType type;

        public Bonus(Coordinates coordinates, BonusType type)
        :base(coordinates)
        {
            this.type = type;
        }
       
        public override void Draw(object sender, PaintEventArgs e)
        {
            String name=null;
            switch (type)
            {
                case BonusType.beton:
                    name = "Л";
                    break;
                case BonusType.bomb:
                    name = "Б";
                    break;
                case BonusType.live:
                    name = "З";
                    break;
                case BonusType.lvl:
                    name = "У";
                    break;
                case BonusType.shield:
                    name = "Щ";
                    break;
                case BonusType.timestop:
                    name = "Ч";
                    break;
            }

            Rectangle rec = new Rectangle(coordinates.x,coordinates.y, 40,40);
            e.Graphics.FillEllipse(Brushes.DarkBlue, rec);
            
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            e.Graphics.DrawString(name, new Font("Times", 25),Brushes.White,rec,sf);
        }

    }


}

