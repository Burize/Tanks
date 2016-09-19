using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Tank
{
    public enum Stage { random, to_Player, to_Headquarters }

   public class EnemyTank:Tank
    {
       public SolidBrush brush;
       public Color color;
       public Timer timer_for_color = new Timer();
       public Timer timer_for_stage = new Timer();
       public Timer timer_for_fire = new Timer();
       public Timer timer_for_handler = new Timer();
       public Boolean bonustank;
       public List<Coordinates> way = new List<Coordinates>();
       public Stage stage = Stage.random;
       public static PlayerTank player1;
       public static PlayerTank player2;
       public static DelForPLace Place;
       int tick;

         public EnemyTank(Coordinates cordinates, int lvl, int interval,Boolean bonustank =false)
            :base(cordinates)
        {
            this.lvl = lvl;
            this.bonustank = bonustank;
            switch (lvl)
            {
                case 1:
                    HP = 1;
                    drive_speed = 1;
                    bullet_speed = 1;
                    break;
                case 2:
                    HP = 1;
                    drive_speed = 2;
                    bullet_speed = 1;
                    break;
                case 3:
                    HP = 1;
                    drive_speed = 1;
                    bullet_speed = 2;
                    break;
                case 4:
                    HP = 4;
                    drive_speed = 1;
                    bullet_speed = 1;
                    break;
            }
             tick=10/drive_speed;

           
                       
             if (lvl == 4)
                color = Color.Gray;
            else
                color = Color.White;

            brush = new SolidBrush(color);

             if (bonustank)
            {
                timer_for_color.Interval = 500;
                timer_for_color.Tick += Change_color;
                timer_for_color.Enabled = true;
            }

             timer_for_stage.Interval = interval;
             timer_for_stage.Tick += Timer_stage_tick;
             timer_for_stage.Start();

             timer_for_fire.Interval = 2000;
             timer_for_fire.Tick += Timer_fire_tick;
             timer_for_fire.Start();

             timer_for_handler.Interval = 200;
             timer_for_handler.Tick += Timer_handler_tick;
             timer_for_handler.Start();

             Timer_Move.Interval = 60;
             Timer_Move.Start();
         }

         public void EnemyMove()
         {           
             try
            {
                if (((coordinates.x + 15) / 40) == way[0].x && ((coordinates.y + 15) / 40) == way[0].y)
                {
                    way.RemoveAt(0);                   
                }
             
                if ((coordinates.x + 15) / 40 < way[0].x)
                    direction = Direction.right;
                if ((coordinates.x + 15) / 40 > way[0].x)
                    direction = Direction.left;
                if ((coordinates.y + 15) / 40 < way[0].y)
                    direction = Direction.bottom;
                if ((coordinates.y + 15) / 40 > way[0].y)
                    direction = Direction.top;
                 }
                catch(ArgumentOutOfRangeException)
                 {                  
                     Timer_Move.Stop();
                     tick = 10/drive_speed;
                 }  
         }

       

         public override void Draw(object sender, PaintEventArgs e)
         {                  
             switch (lvl)
             {
                 case 1:
                     Drawlvl1(sender, e);
                     break;
                 case 2:
                     Drawlvl2(sender, e);
                     break;
                 case 3:
                     Drawlvl3(sender, e);
                     break;
                 case 4:
                     Drawlvl4(sender, e);
                     break;
             }
         }
         public void ChangeColor()
         {
             switch (HP)
             {
                 case 1:
                     brush.Color = Color.White;
                     break;
                 case 2:
                     brush.Color = Color.Gainsboro;
                     break;
                 case 3:
                     brush.Color = Color.DarkGray;
                     break;
                 case 4:
                     brush.Color = Color.Gray;
                     break;
             }
         }
        
         public override void Fire()
       {
           if (DateTime.Now.Subtract(last_shot).TotalMilliseconds >= 2000)
               base.Fire();
       }
    
         public Boolean Order_To_Fire()
       {
           int x = (coordinates.x + 15) / 40;
           int y = (coordinates.y + 15) / 40;
           int _x = 0;
           int _y = 0;

           switch (direction)
           {
               case Direction.top:
                   _x = 0;
                   _y = -1;
                   break;
               case Direction.right:
                   _x = 1;
                   _y = 0;
                   break;
               case Direction.bottom:
                   _x = 0;
                   _y = 1;
                   break;
               case Direction.left:
                   _x = -1;
                   _y = 0;
                   break;
           }

           while (true)
           {
               if (x == 7 && y == 13)
                   return true;
               if (field[x, y] == -1)
                   return false;
               if (player1 != null && x == (player1.coordinates.x + 15) / 40 && y == (player1.coordinates.y + 15) / 40)
                   return true;
               if (player2 != null && x == (player2.coordinates.x + 15) / 40 && y == (player2.coordinates.y + 15) / 40)
                   return true;
               x += _x;
               y += _y;
           }
       }

         private void Drawlvl1(object sender, PaintEventArgs e)
         {
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
         private void Drawlvl2(object sender, PaintEventArgs e)
         {
             switch (direction)
             {
                 case Direction.top:
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y + 4, 8, 26));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 22, coordinates.y + 4, 8, 26)); //faster
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y + 8, 30, 14));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 13, coordinates.y, 4, 15));
                     break;
                 case Direction.right:
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y, 26, 8));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y + 22, 26, 8));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 8, coordinates.y, 14, 30));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 15, coordinates.y + 13, 15, 4));
                     break;
                 case Direction.bottom:
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y, 8, 26));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 22, coordinates.y, 8, 26));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y + 8, 30, 14));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 13, coordinates.y + 16, 4, 14));
                     break;
                 case Direction.left:
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 4, coordinates.y, 26, 8));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 4, coordinates.y + 22, 26, 8));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 8, coordinates.y, 14, 30));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y + 13, 15, 4));
                     break;
             }
         }
         private void Drawlvl3(object sender, PaintEventArgs e)
         {
             switch (direction)
             {
                 case Direction.top:
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y + 4, 8, 26));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 22, coordinates.y + 4, 8, 26));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y + 12, 30, 14));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 12, coordinates.y, 6, 15)); 
                    break;
                case Direction.right:
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y, 26, 8));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y + 22, 26, 8));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 4, coordinates.y, 14, 30));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 4, coordinates.y + 12, 26, 6));
                    break;
                case Direction.bottom:
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y, 8, 26));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 22, coordinates.y, 8, 26));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y + 4, 30, 14));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 12, coordinates.y + 4, 6, 26));
                    break;
                case Direction.left:
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 4, coordinates.y, 26, 8));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 4, coordinates.y + 22, 26, 8));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 12, coordinates.y, 14, 30));
                    e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y + 12, 26, 6));
                    break;
             }
         }
         private void Drawlvl4(object sender, PaintEventArgs e)
         {
             
             
             switch (direction)
             {
                 case Direction.top:
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y + 4, 9, 26));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 21, coordinates.y + 4, 9, 26));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 5, coordinates.y + 12, 20, 20));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 12, coordinates.y, 6, 15));
                     break;
                 case Direction.right:
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y, 26, 9));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y + 21, 26, 9));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x - 2, coordinates.y + 5, 20, 20));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 4, coordinates.y + 12, 26, 6));
                     break;
                 case Direction.bottom:
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y, 9, 26));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 21, coordinates.y, 9, 26));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 5, coordinates.y - 2, 20, 20));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 12, coordinates.y + 4, 6, 26));
                     break;
                 case Direction.left:
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 4, coordinates.y, 26, 9));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 4, coordinates.y + 21, 26, 9));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x + 12, coordinates.y + 5, 20, 20));
                     e.Graphics.FillRectangle(brush, new Rectangle(coordinates.x, coordinates.y + 12, 26, 6));
                     break;
             }
         }
         private void Change_color(object sender, EventArgs e)
         {
             if (brush.Color == Color.Red)
                 brush.Color = color;
             else
                 brush.Color = Color.Red;
         }


         public List<Coordinates> Tracker( Coordinates target_coordinates)
         {
             Coordinates tank_cordinates = new Coordinates((coordinates.x + 15) / 40, (coordinates.y + 15) / 40);

             Coordinates target = new Coordinates((target_coordinates.x + 15) / 40, (target_coordinates.y + 15) / 40);

             if (tank_cordinates.x == target.x && tank_cordinates.y == target.y)
                 return new List<Coordinates>();
             var _field = (int[,])field.Clone();

             int step = 1;
             _field[tank_cordinates.x, tank_cordinates.y] = 1;
             while (true)
             {

                 for (int x = 1; x < 14; x++)
                     for (int y = 1; y < 14; y++)
                         if (_field[x, y] == step)
                         {
                             if (_field[x, y - 1] == 0)
                                 _field[x, y - 1] = step + 1;
                             if (_field[x - 1, y] == 0)
                                 _field[x - 1, y] = step + 1;
                             if (_field[x, y + 1] == 0)
                                 _field[x, y + 1] = step + 1;
                             if (_field[x + 1, y] == 0)
                                 _field[x + 1, y] = step + 1;
                         }

                 step++;

                 if (_field[target.x, target.y] > 0)
                     break;
                 if (step > 13 * 13)
                     return new List<Coordinates>();

             }
             int _x = target.x;
             int _y = target.y;

             int[] borders = new int[4];
             List<Coordinates> way = new List<Coordinates>();
             way.Add(new Coordinates(_x, _y));
             while (true)
             {
                 if (_field[_x - 1, _y] > 0)
                     borders[0] = _field[_x - 1, _y];
                 else
                     borders[0] = 100000;
                 if (_field[_x + 1, _y] > 0)
                     borders[1] = _field[_x + 1, _y];
                 else
                     borders[1] = 100000;
                 if (_field[_x, _y - 1] > 0)
                     borders[2] = _field[_x, _y - 1];
                 else
                     borders[2] = 100000;
                 if (_field[_x, _y + 1] > 0)
                     borders[3] = _field[_x, _y + 1];
                 else
                     borders[3] = 100000;

                 Array.Sort(borders);


                 if (borders[0] == _field[_x - 1, _y])
                 {
                     way.Add(new Coordinates(_x - 1, _y));
                     _x--;
                 }
                 else if (borders[0] == _field[_x + 1, _y])
                 {
                     way.Add(new Coordinates(_x + 1, _y));
                     _x++;
                 }
                 else if (borders[0] == _field[_x, _y - 1])
                 {
                     way.Add(new Coordinates(_x, _y - 1));
                     _y--;
                 }
                 else if (borders[0] == _field[_x, _y + 1])
                 {
                     way.Add(new Coordinates(_x, _y + 1));
                     _y++;
                 }
                 if (tank_cordinates.x == _x && tank_cordinates.y == _y)
                     break;
             }


             way.Reverse(0, way.Count);
             way.RemoveAt(0);
             return way;

         }
      
         public void Find_Players()
         {

             List<Coordinates> way1 = new List<Coordinates>();
             List<Coordinates> way2 = new List<Coordinates>();

            
                 if (way.Count == 0 && stage == Stage.random)
                 {
                     way = Tracker( Place());
                     Timer_Move.Start();
                     return;
                 }

                 if (stage == Stage.to_Player)
                 {
                     if (player1 != null)
                         way1 = Tracker( player1.coordinates);

                     if (player2 != null)
                         way2 = Tracker( player2.coordinates);


                     if (player1 != null && player2 != null)
                     {
                         if (way1.Count > way2.Count)
                             way = way2;
                         else
                             way = way1;
                     }
                     else
                     {
                         if (player1 != null)
                             way = way1;
                         if (player2 != null)
                             way = way2;
                     }
                      Timer_Move.Start();
                     return;
                 }

                 if (stage == Stage.to_Headquarters)
                 {
                     way1 = Tracker( new Coordinates(5 * 40, 13 * 40));
                     way2 = Tracker( new Coordinates(9 * 40, 13 * 40));
                     if (way1.Count > way2.Count)
                         way = way2;
                     else
                         way = way1;

                     if (way.Count == 0 && coordinates.y > 40 * 13)
                         if (coordinates.x < 40 * 7)
                             direction = Direction.right;
                         else
                             direction = Direction.left;
                     Fire();

                 }


             
         }

         public override void Timer_move_tick(object sender, EventArgs e)
         {
    
            if (tick == 10/drive_speed)
            {
                tick = 0;
                EnemyMove();    
            }
            if (Timer_Move.Enabled != false)
            {
                tick++;
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
            }
          }

         public void Timer_stage_tick(object sender, EventArgs e)
         {
             if (stage != Stage.to_Headquarters)
                 stage++;
         }

         public void Timer_fire_tick(object sender, EventArgs e)
         {
             Random rnd= new Random();
             if (rnd.Next(2) == 0)
                 Fire();
         }

         public void Timer_handler_tick(object sender, EventArgs e)
         {
             Find_Players();
             if (Order_To_Fire())
                 Fire(); 
         }
   }
}
