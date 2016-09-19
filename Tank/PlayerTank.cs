using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Tank
{
  public  class PlayerTank: Tank
    {

        public int bullets_count = 0;   
       
        public Coordinates startposition;
        public Boolean shield = false;
        public Boolean second = false;
        public Timer timer_for_reload = new Timer();
        public Timer Timer_Slipping = new Timer();

        
    //    public event DelPlayerMove TankMove;
    
        public PlayerTank(Coordinates coordinates, int lvl, Boolean second = false)
            :base(coordinates)
        {
            this.lvl = lvl;
            HP = 3;
            drive_speed = 1;
            bullet_speed = 1;

            Timer_Move.Interval = 60;

            this.coordinates = coordinates;
            startposition = coordinates;
            this.second = second;

            timer_for_reload.Interval = 2000;
            timer_for_reload.Tick += Reload;
            timer_for_reload.Enabled = true;

            Timer_Slipping.Interval = 20;
            Timer_Slipping.Tick += Timer_slipping_tick;
        }

        public void Lvl_Up()
        {
            lvl++;
            if (lvl == 2)
                bullet_speed++;
        }

        public override void Draw(object sender, PaintEventArgs e)
        {
            SolidBrush brush = new SolidBrush(Color.Goldenrod);
            if (second)
                brush = new SolidBrush(Color.DarkOliveGreen);
            if (shield)
                brush = new SolidBrush(Color.DarkGray);

            switch (direction)
            {
                case Direction.top:
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y + 4, 8, 26));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 22, coordinates.y + 4, 8, 26));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y + 12, 30, 14));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 13, coordinates.y, 4, 15));
                    break;
                case Direction.right:
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y, 26, 8));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y + 22, 26, 8));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 4, coordinates.y, 14, 30));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 4, coordinates.y + 13, 26, 4));
                    break;
                case Direction.bottom:
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y, 8, 26));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 22, coordinates.y, 8, 26));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y + 4, 30, 14));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 13, coordinates.y + 4, 4, 26));
                    break;
                case Direction.left:
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 4, coordinates.y, 26, 8));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 4, coordinates.y + 22, 26, 8));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 12, coordinates.y, 14, 30));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y + 13, 26, 4));
                    break;  
            }

        }

       void Reload(object sender, EventArgs e)
        {
            if (lvl >= 3)
                bullets_count = 3;
            else
                bullets_count = 1;
        }
       
        public override void Fire()
        {
            if (bullets_count != 0 && ((DateTime.Now.Subtract(last_shot).TotalMilliseconds >= 700 && lvl >= 3)
                || (DateTime.Now.Subtract(last_shot).TotalMilliseconds >= 2000 && lvl < 3)))
            {
                bullets_count--;
                last_shot = DateTime.Now;
                base.Fire();
            }
        }

        public Boolean Can_It_Go_to()
        {
            int x;
            int y;

            int block_size = 40;
            int tank_size = 30;

            switch (direction)
            {
                case Direction.left:
                    x = (coordinates.x + block_size - 5) / block_size;
                    y = coordinates.y;
                    if (field[x - 1, y / block_size] == -1 || field[x - 1, (y + tank_size) / block_size] == -1)
                        return false;
                    break;
                case Direction.right:
                    x = (coordinates.x + tank_size - block_size + 5) / block_size;
                    y = coordinates.y;
                    if (field[x + 1, y / block_size] == -1 || field[x + 1, (y + tank_size) / block_size] == -1)
                        return false;
                    break;
                case Direction.top:
                    x = coordinates.x;
                    y = (coordinates.y + block_size - 5) / block_size;
                    if (field[x / block_size, y - 1] == -1 || field[(x + tank_size) / block_size, y - 1] == -1)
                        return false;
                    break;
                case Direction.bottom:

                    x = coordinates.x;
                    y = (coordinates.y + tank_size - block_size + 5) / block_size;
                    if (field[x / block_size, y + 1] == -1 || field[(x + tank_size) / block_size, y + 1] == -1)
                        return false;
                    break;
            }


            return true;
        }

      

      public override void Timer_move_tick(object sender, EventArgs e)
        {
            
            if (Can_It_Go_to())
            {
              
                switch (direction)
                {
                    case Direction.top:
                        coordinates.y -= 4 * drive_speed;
                        break;
                    case Direction.right:
                        coordinates.x += 4 * drive_speed;
                        break;
                    case Direction.bottom:
                        coordinates.y += 4 * drive_speed;
                        break;
                    case Direction.left:
                        coordinates.x -= 4 * drive_speed;
                        break;
                }

                base.Timer_move_tick(sender,e); 
            }
        }

        public void Timer_slipping_tick(object sender, EventArgs e)
        {

            if (Can_It_Go_to() && Timer_Move.Enabled==false)
            {
               
                switch (direction)
                {
                    case Direction.top:
                        coordinates.y -= 1;
                        break;
                    case Direction.right:
                        coordinates.x += 1;
                        break;
                    case Direction.bottom:
                        coordinates.y += 1;
                        break;
                    case Direction.left:
                        coordinates.x -= 1;
                        break;
                }
            }
            Timer_Slipping.Stop();

            base.Timer_move_tick(sender,e); 

        }
  }

}
