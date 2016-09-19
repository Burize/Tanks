using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
namespace Tank
{
    
   public class Bullet:GameObject
    {
        public int speed;
        public int lvl;
        Direction direction;
        public Timer timer = new Timer();
        public Tank tank;

        public event DelBulletMove BulletMove;

        public Bullet(Coordinates coordinates,int speed, int lvl,Direction direction, Tank tank)
            :base(coordinates)
        {
            this.speed = speed*10;

            this.coordinates.x += 6;
            this.coordinates.y += 6;
            this.lvl = lvl;
            this.direction = direction;
            this.tank = tank;

            timer.Interval = 40;
            timer.Tick += Timer_tick;
            timer.Enabled = true;
        }

        public override void Draw (object sender, PaintEventArgs e)
        {  
            SolidBrush brush = new SolidBrush(Color.White);
           
            e.Graphics.FillEllipse(brush, coordinates.x, coordinates.y, 10, 10);
        }

        void Timer_tick(object sender, EventArgs e)
        {
            switch (direction)
            {
                case Direction.top:
                    coordinates.y -= speed;
                    break;
                case Direction.right:
                    coordinates.x += speed;
                    break;
                case Direction.bottom:
                    coordinates.y += speed;
                    break;
                case Direction.left:
                    coordinates.x -= speed;
                    break;
            }
            if(BulletMove!=null)
            BulletMove(this);
        }
    }
}
