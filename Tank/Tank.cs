using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Data;

namespace Tank
{
    public abstract class Tank:GameObject
    {

        public static int[,] field;

        public int HP;
        public int drive_speed;
        public  int bullet_speed;
        public int lvl;
        public DateTime last_shot = DateTime.Now;
        public Timer Timer_Move = new Timer();
        public static DelNewBullet NewBullet;
       
        public Direction direction = Direction.top;
        
        
        public Tank(Coordinates coordinates)
        :base(coordinates)
        {
            this.coordinates = coordinates;
   
            Timer_Move.Tick += Timer_move_tick;
           
        }

        public event DelTankMove TankMove;
        

        public virtual void Fire()
        {
            Bullet bullet = new Bullet(new Coordinates(this.coordinates.x, this.coordinates.y), bullet_speed, lvl, direction, this);
            NewBullet(bullet);
            last_shot = DateTime.Now;
        }

        public virtual void Timer_move_tick(object sender, EventArgs e)
        {
            if (TankMove != null) TankMove(this);
        }
       
    }
}
