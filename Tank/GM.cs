using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;


namespace Tank
{
    public delegate void DelTankMove (Tank tank);
    public delegate void DelNewBullet(Bullet bullet);
    public delegate void DelBulletMove(Bullet b);
    public delegate void DelCollide(GameObject one, GameObject two);
    public delegate void DelPlayerInArea (object _object, PlayerTank tank);
    public delegate Coordinates DelForPLace ();
    public delegate Boolean DelOrderToMove(Tank tank,Direction direction);
    
   public static class GameManager
    {
       public static Form1 form;

 #region Lists
      
       static List<EnemyTank> tanks = new List<EnemyTank>();
       static List<Wall> walls = new List<Wall>();
       static List<Wall> fence = new List<Wall>();
       static List<GameObject> landscapes = new List<GameObject>();
       static List<Ice> ice = new List<Ice>();
       static List<Bullet> bullets = new List<Bullet>();
       static Wall headquarters;

   #endregion

 #region Timers
        
         static System.Threading.Timer timer_for_beton;
         static System.Threading.Timer timer_for_shield;
         static System.Threading.Timer timer_for_ffire;

         static Timer global_timer = new Timer();
         static Timer timer_for_pause = new Timer();
         static Timer timer_for_stoptime = new Timer();

#endregion 

 #region Fields
        
        static int interval;
        static int tank_for_lvl;
        static int[,] field  = new int[15, 15];

        static int CurrentLevel;
        static Bonus currentbonus;
      
        static PlayerTank player1 = null;
        static PlayerTank player2 = null;
        static int player_count;

        static int player1_total_score = 0;
        static int player2_total_score = 0;

        static int player1_lvl_score = 0;
        static int player2_lvl_score = 0;
        static string playername="";
       
        static DateTime delay_for_tank = DateTime.Now;
        static Random random = new Random();
#endregion

 #region Game

       public static void ON(Form1 _form)
        {
            form = _form;
            Menu.form = form;
            MainMenu();

            global_timer.Tick += Global_Timer_Tick;
            timer_for_stoptime.Tick += StopTime_Tick;
            timer_for_pause.Tick += pause_tick;

            global_timer.Interval = 80;
            timer_for_stoptime.Interval = 5000;
            timer_for_pause.Interval = 50;

            GameObject.collide = Collide;
          //  GameObject.OnArea = OnArea;
            Tank.NewBullet = NewBullet;
            EnemyTank.Place = Place;
        }

       static void StartGame(Boolean two = false)
        {
            form.KeyDown += Press_Interrupt;
            form.Paint -= Draw_Mainmenu;
            form.Paint += Draw_Game;
            Menu.Remove();
  
            headquarters = new Wall(new Coordinates(7 * 40, 13 * 40), 1);
            field[7, 13] = -1;

            global_timer.Start();
           
            player1_total_score = 0;
            player2_total_score = 0;
            player1_lvl_score = 0;
            player2_lvl_score = 0;
            Tank.field = field;
           
            player_count = 1;
            player1 = new PlayerTank(new Coordinates(202, 525),1);
            form.KeyDown += Player1Move_keydown;
            form.KeyUp += Player1Move_keyup;
            EnemyTank.player1 = player1;
                if (two)
                {
                    player_count = 2;
                    player2 = new PlayerTank(new Coordinates(369, 525), 1, true);
                    form.KeyDown += Player2Move_keydown;
                    form.KeyUp += Player2Move_keyup;
                    EnemyTank.player2 = player2;
                }
            
            Wall block;
            for (int i = 0; i < 15; i++)
            {
                field[0, i] = -1;
                block = new Wall(new Coordinates( 0, i * 40 ), -1);
                fence.Add(block);
                field[14, i] = -1;
                block = new Wall(new Coordinates(14*40, i * 40), -1);
                fence.Add(block);
                field[i, 0] = -1;
                block = new Wall(new Coordinates(i * 40 , 0), -1);
                fence.Add(block);
                field[i, 14] = -1;
                block = new Wall(new Coordinates(i * 40 , 14 * 40 ), -1);
                fence.Add(block);
            }
          
            CurrentLevel = 8;
            Nextlvl();
        }

       static void Nextlvl()
       {
           CurrentLevel++;

           player1_lvl_score = 0;
           player2_lvl_score = 0;
           tank_for_lvl = 2;


           interval = (int)(2 * ((double)(190 - CurrentLevel * 4 - (player_count - 1) * 20) / (double)60) * 1000);
           if (player1 != null)
           {
               player1.coordinates = player1.startposition;
               player1.direction = Direction.top;
               form.Paint += player1.Draw;
           }
           if (player2 != null)
           {
               player2.coordinates = player2.startposition;
               player2.direction = Direction.top;
               form.Paint += player2.Draw;
           }

           String way = "..\\..\\levels\\" + CurrentLevel.ToString() + ".txt";
           String[] lvl = File.ReadAllLines(@way, System.Text.Encoding.Default);
           Wall block;
           GameObject landscape;
           Ice _ice;
           for (int i = 0; i < 26; i += 2)
               for (int j = 0; j < 26; j += 2)
               {
                   switch (lvl[i][j])
                   {
                       case '#'://стена);
                           block = new Wall(new Coordinates(j * 20 + 40, i * 20 + 40), 2);
                           walls.Add(block);
                           form.Paint += block.Draw;
                           field[j / 2 + 1, i / 2 + 1] = -1;
                           break;
                       case '@'://бетон);
                           block = new Wall(new Coordinates(j * 20 + 40, i * 20 + 40), 3);
                           walls.Add(block);
                           form.Paint += block.Draw;
                           field[j / 2 + 1, i / 2 + 1] = -1;
                           break;
                       case '%'://кусты);
                           landscape = new Shrub(new Coordinates(j * 20 + 40, i * 20 + 40));
                           landscapes.Add(landscape);
                           form.Paint += landscape.Draw;
                           break;
                       case '~'://вода);
                           landscape = new Water(new Coordinates(j * 20 + 40, i * 20 + 40));
                           landscapes.Add(landscape);
                           form.Paint += landscape.Draw;
                           field[j / 2 + 1, i / 2 + 1] = -1;
                           break;
                       case '-': // "лед);
                           _ice = new Ice(new Coordinates(j * 20 + 40, i * 20 + 40));
                           ice.Add(_ice);
                           if (player1 != null)
                               player1.TankMove += _ice.SearchGameObject;
                           if (player2 != null)
                               player2.TankMove += _ice.SearchGameObject;
                           form.Paint += _ice.Draw;
                           break;
                   }
               }
           if (ice.Count != 0)
           {
               if (player1 != null)
               {
                   form.Paint -= player1.Draw;
                   form.Paint += player1.Draw;
               }
               if (player2 != null)
               {
                   form.Paint -= player2.Draw;
                   form.Paint += player2.Draw;
               }
           } 
           block = new Wall(new Coordinates(6 * 40, 40 * 13), 2);
           walls.Add(block);
           form.Paint += block.Draw;
           field[6, 13] = -1;
           block = new Wall(new Coordinates(6 * 40, 40 * 12), 2);;
           walls.Add(block);
           form.Paint += block.Draw;
           field[6, 12] = -1;
           block = new Wall(new Coordinates(7 * 40, 40 * 12), 2);
           walls.Add(block);
           form.Paint += block.Draw;
           field[7, 12] = -1;
           block = new Wall(new Coordinates(8 * 40, 40 * 12), 2);
           walls.Add(block);
           form.Paint += block.Draw;
           field[8, 12] = -1;

           global_timer.Start();
       }

       static void Clearlvl(bool fully = false)
       {
           while (walls.Count != 0)
               RemoveWall(walls[0]);
           while (bullets.Count != 0)
               RemoveBullet(bullets[0]);
           while (landscapes.Count != 0)
               RemoveLandScape(landscapes[0]);
           while (ice.Count != 0)
               RemoveIce(ice[0]);

           if (currentbonus != null)
               RemoveBonus();
           form.Paint -= Draw_Game;
           form.KeyDown -= Press_Interrupt;
           form.KeyDown -= Player1Move_keydown;
           form.KeyUp -= Player1Move_keyup;

           form.KeyDown -= Player2Move_keydown;
           form.KeyUp -= Player2Move_keyup;

           if (player1 != null)
           {
               form.Paint -= player1.Draw;
               player1.Timer_Move.Stop();
           }
           if (player2 != null)
           {
               form.Paint -= player2.Draw;
               player2.Timer_Move.Stop();
           }

           if (fully)
           {
               while (tanks.Count != 0)
                   Remove_EnemyTank(tanks[0], null);

               while (fence.Count != 0)
               {
                   form.Paint -= fence[0].Draw;
                   fence.Remove(fence[0]);
               }
               if (player1 != null) RemovePlayer(player1);
               if (player2 != null) RemovePlayer(player2);
           }
           form.Invalidate();
       }
     
       static void Changelvl()
        {
            global_timer.Stop();
            form.Paint -= Draw_Game;
                if (CurrentLevel != 35)
                {
                    Clearlvl();
                    form.Paint += Draw_Between_lvl;
                    form.KeyUp += Press_Nextlvl;
                }
                else
                {
                    Clearlvl(true);
                    if (player1 != null) RemovePlayer(player1);
                    if (player2 != null) RemovePlayer(player2);
                    player1_total_score += player1_lvl_score;
                    player2_total_score += player2_lvl_score;
                    Go_To_TR();
                }
        }

       static void GameOver()
       {
           global_timer.Stop();
           player1_total_score += player1_lvl_score;
           player2_total_score += player2_lvl_score;
           Clearlvl(true);    
           form.Paint -= Draw_Game;
           Go_To_TR();

       }

       static void Interrupt()
       {
           Clearlvl(true);
           global_timer.Stop();
           form.Paint -= Draw_Game;
           form.Paint -= Draw_Pause;
           MainMenu();
       }

       static void Go_To_TR()
       {         
           playername = "";
           if (TableRecords.Order_To_Add(player1_total_score) || TableRecords.Order_To_Add(player2_total_score))
           {
               form.KeyPress += Press_WriteName;
               form.Paint += Draw_Writename;
               form.Paint += Draw_Records;
           }
           else
               Menu_Records();

       }

#endregion

 #region Menu

       static void MainMenu()
       {
           form.Paint -= Draw_Records;
           form.Paint -= Draw_Exit;
           form.Paint += Draw_Mainmenu;
           Menu.Remove();
           Menu.Add(new Button(new Coordinates(250, 200), "1 Игрок", Menu_1Player));
           Menu.Add(new Button(new Coordinates(250, 250), "2 Игрока", Menu_2Players));
           Menu.Add(new Button(new Coordinates(250, 300), "Рекорды", Menu_Records));
           Menu.Add(new Button(new Coordinates(250, 350), "Выйти", Menu_Exit));

       }

       static void Menu_1Player()
       {
           StartGame(); ;
       }

       static void Menu_2Players()
       {
           StartGame(true);
       }

       static void Menu_Records()
       {

           form.Paint -= Draw_Mainmenu;
           form.Paint += Draw_Records;

           Menu.Remove();
           Menu.Add(new Button(new Coordinates(250, 450), "Очистить", TableRecords.Clear));
           Menu.Add(new Button(new Coordinates(250, 500), "Главное меню", MainMenu));
       }

       static void Menu_Exit()
       {
           form.Paint -= Draw_Mainmenu;
           form.Paint += Draw_Exit;
           Menu.Remove();
           Menu.Add(new Button(new Coordinates(275, 200), "Нет", MainMenu));
           Menu.Add(new Button(new Coordinates(275, 250), "Да", () => { System.Environment.Exit(0); }));
       }

       #endregion
     
 #region Newobjects

       static void NewTank()
       {

           Boolean bonustank = false;

           if (tank_for_lvl == 2 || tank_for_lvl == 9 || tank_for_lvl == 16)
               bonustank = true;
           int choice = random.Next(8);

           int tanklevel = 0;

           if (choice >= 0 && choice <= 2)
               tanklevel = 1;
           if (choice >= 3 && choice <= 4)
               tanklevel = 2;
           if (choice >= 5 && choice <= 6)
               tanklevel = 3;
           if (choice == 7)
               tanklevel = 4;

           EnemyTank tank = new EnemyTank(new Coordinates(45 + 240 * random.Next(3), 45), tanklevel,3*interval ,bonustank);
           form.Paint += tank.Draw;
           foreach (var bullet in bullets)
               bullet.BulletMove += tank.SearchGameObject;
           tanks.Add(tank);
           tank_for_lvl--;
           Under_Bush();

       }

       static void NewBullet(Bullet bullet)
      {

       form.Paint += bullet.Draw;

          bullet.BulletMove += headquarters.SearchGameObject;

          if (player1 != null && bullet.tank != player1)
                bullet.BulletMove += player1.SearchGameObject;

          if (player2 != null && player2 != bullet.tank)
                bullet.BulletMove += player2.SearchGameObject;
          if (bullet.tank is PlayerTank)
                foreach (var tnk in tanks)
                    if (tnk != bullet.tank)
                        bullet.BulletMove += tnk.SearchGameObject;

            
            foreach(var block  in walls)
                bullet.BulletMove += block.SearchGameObject;
            
            foreach (var block in fence)
                bullet.BulletMove += block.SearchGameObject;

            foreach (var _bullet in bullets)
                if (_bullet != bullet)
                    bullet.BulletMove += _bullet.SearchGameObject;

            bullets.Add(bullet);
          Under_Bush();

      }
      
       static void NewBonus()
        {
            if (currentbonus != null)
                RemoveBonus();

           
            BonusType bonustype;
            switch (random.Next(6))
            {
                case 0:
                    bonustype = BonusType.beton;
                break;
                    case 1:
                    bonustype = BonusType.bomb;
                break;
                    case 2:
                    bonustype = BonusType.live;
                break;
                    case 3:
                    bonustype = BonusType.lvl;
                break;
                    case 4:
                    bonustype = BonusType.shield;
                break;
                    case 5:
                    bonustype = BonusType.timestop;
                break;
                default:
                bonustype = BonusType.beton;
                break;
            }

            Bonus bonus = new Bonus(Place(), bonustype);
            form.Paint += bonus.Draw;
            if(player1!=null)
             player1.TankMove+=bonus.SearchGameObject;
            if(player2!=null)
                player2.TankMove += bonus.SearchGameObject;
            currentbonus = bonus;
        }
 
#endregion     

 #region Remove

       static void Collide(GameObject one, GameObject two)
       {
           if (one is Bullet)
           {

               RemoveBullet(one as Bullet);

               if (two is Bullet)
                   RemoveBullet(two as Bullet);

               if (two is EnemyTank)
                   Hit_EnemyTank(two as EnemyTank, one as Bullet);

               if (two is PlayerTank)
                   Hit_Player(two as PlayerTank, one as Bullet);

               if (two is Wall)
                   Hit_Wall(two as Wall, one as Bullet);
               if (two == headquarters)
               {
                   RemoveWall(two as Wall);
                   GameOver();
               }
           }
           else
           {
               OnArea( two, one as Tank);
           }

       }

       static void Hit_EnemyTank(EnemyTank tank, Bullet bullet)
       {
           if (tank.bonustank)
           {
               tank.timer_for_color.Stop();
               tank.brush.Color = tank.color;
               NewBonus();
               tank.bonustank = false;
           }

           tank.HP--;

           if (tank.HP == 0)
               Remove_EnemyTank(tank, bullet);
           else
               tank.ChangeColor();

       }

       static void Hit_Player(PlayerTank tank, Bullet bullet)
       {
           if (!tank.shield)
           {
               if (bullet.tank is PlayerTank)
               {
                   if (tank == player1)
                   {
                       form.KeyDown -= Player1Move_keydown;
                       form.KeyUp -= Player1Move_keyup;
                       player2.Timer_Move.Stop();
                       timer_for_ffire = new System.Threading.Timer(Friendly_Fire, player1 as object, 3000, -1);
                   }
                   else
                   {
                       form.KeyDown -= Player2Move_keydown;
                       form.KeyUp -= Player2Move_keyup;
                       player2.Timer_Move.Stop();
                       timer_for_ffire = new System.Threading.Timer(Friendly_Fire, player2 as object, 3000, -1);
                   }

               }
               else
               {
                   tank.HP--;
                   tank.lvl = 1;
                   tank.coordinates = tank.startposition;
                   if (tank.HP == 0)
                       RemovePlayer(tank);
               }
           }
       }

       static void Hit_Wall(Wall block, Bullet bullet)
       {
           if (block.coordinates.x >= 40 && block.coordinates.x < 560 && block.coordinates.y >= 40 && block.coordinates.y < 560)
           {
               if (block.HP != 3)
                   block.HP--;
               if (block.HP == 0 || bullet.lvl == 4)
                   RemoveWall(block);
           }
       }
       

       static void RemoveBullet(Bullet bullet)
        {
            bullets.Remove(bullet);
            bullet.timer.Enabled = false;
            
            form.Paint -= bullet.Draw;
           
           if (player1 != null)
                bullet.BulletMove -= player1.SearchGameObject;
           if (player2 != null)
                bullet.BulletMove -= player2.SearchGameObject;

            foreach (var tank in tanks)
                   bullet.BulletMove -= tank.SearchGameObject;
               
            foreach (var block in walls)
                    bullet.BulletMove -= block.SearchGameObject;
            foreach (var _bullet in bullets)
                bullet.BulletMove -= _bullet.SearchGameObject;
       }
        
       static void RemoveWall(Wall block)
       {
           walls.Remove(block);
           form.Paint -= block.Draw;
           field[block.coordinates.x / 40, block.coordinates.y / 40] = 0;
           foreach (var bullet in bullets)
           {
               bullet.BulletMove -= block.SearchGameObject;
           }
       }

       static void Remove_EnemyTank(EnemyTank tank, Bullet bullet)
       {
           int score = 0;
           if (bullet != null)
           {
               switch (tank.lvl)
               {
                   case 1:
                       score = 100;
                       break;
                   case 2:
                       score = 200;
                       break;
                   case 3:
                       score = 300;
                       break;
                   case 4:
                       score = 400;
                       break;
               }

               if (bullet.tank == player1)
                   player1_lvl_score += score;
               else
                   player2_lvl_score += score;
           }

           form.Paint -= tank.Draw;
           tank.timer_for_stage.Stop();
           tank.timer_for_fire.Stop();
           tank.timer_for_handler.Stop();
           foreach (var _bullet in bullets)
           {
               _bullet.BulletMove -= tank.SearchGameObject;
           }
           tanks.Remove(tank);
       }
       
       static void RemoveIce(Ice _ice)
       {
           form.Paint -= _ice.Draw;
           if (player1 != null)
               player1.TankMove -= _ice.SearchGameObject;
           if (player2 != null)
               player2.TankMove -= _ice.SearchGameObject;
           ice.Remove(_ice);
       
       }
       
       static void RemoveLandScape(GameObject landscape)
       {
           landscapes.Remove(landscape);
           form.Paint -= landscape.Draw;
           field[landscape.coordinates.x / 40, landscape.coordinates.y / 40] = 0;
       }
       
       static void RemoveBonus()
       {
           form.Paint -= currentbonus.Draw;
           if (player1 != null)
               player1.TankMove -= currentbonus.SearchGameObject;
           if (player2 != null)
               player2.TankMove -= currentbonus.SearchGameObject;
           currentbonus = null;

       }

       static void RemovePlayer(PlayerTank tank)
       {

           if (tank == player1)
           {
               form.KeyDown -= Player1Move_keydown;
               form.KeyUp -= Player1Move_keyup;
               form.Paint -= player1.Draw;
               player1.Timer_Move.Stop();            
               player1 = null;
           }
           else
           {
               form.KeyDown -= Player2Move_keydown;
               form.KeyUp -= Player2Move_keyup;
               form.Paint -= player2.Draw;
               player2.Timer_Move.Stop();              
               player2 = null;
           }
           foreach (var bullet in bullets)
           {
               bullet.BulletMove -= tank.SearchGameObject;
           }
           
       }      
       
#endregion
       
 #region Bonus

       static void UseBonus(Bonus bonus, Tank tank)
       {

           if (tank == player1)
               player1_lvl_score += 500;
           else
               player2_lvl_score += 500;
           switch (bonus.type)
           {
               case BonusType.beton:
                   Bonus_Beton(3);
                   break;
               case BonusType.bomb:
                   Bonus_Bomb();
                   break;
               case BonusType.live:
                   Bonus_Live(tank);
                   break;
               case BonusType.lvl:
                   Bonus_lvl(tank as PlayerTank);
                   break;
               case BonusType.shield:
                   Bonus_Set_Shield(tank as PlayerTank);
                   break;
               case BonusType.timestop:
                   Bonus_TimeStop();
                   break;
           }
           RemoveBonus();
       }

       static void Bonus_Beton(object HP)
       {
           List<Wall> blocks_buffer = new List<Wall>();
           foreach (var block in walls)
               if (block.coordinates.x / 40 >= 6 && block.coordinates.x / 40 <= 8
                   && block.coordinates.y / 40 >= 12 && block.coordinates.y / 40 <= 13)
                   blocks_buffer.Add(block);

           foreach (var block in blocks_buffer)
           {
               walls.Remove(block);
               form.Paint -=block.Draw;
           }
           Wall _block;

           for (int i = 12; i <= 13; i++)
           {
               field[6, i] = -1;
               field[8, i] = -1;
               _block = new Wall(new Coordinates(6 * 40 , i * 40), (int)HP);      
               walls.Add(_block);
               form.Paint += _block.Draw;
               _block = new Wall(new Coordinates(8 * 40 , i * 40), (int)HP);
               walls.Add(_block);
               form.Paint += _block.Draw;
           }
           field[7, 12] = -1;
           _block = new Wall(new Coordinates(7 * 40 , 12 * 40), (int)HP);
           walls.Add(_block);
           form.Paint += _block.Draw;

           timer_for_beton = new System.Threading.Timer(Bonus_Beton, 2, 20000, -1);   
       }

       static void Bonus_Set_Shield( PlayerTank tank)
       {
           tank.shield = true;
           timer_for_shield = new System.Threading.Timer(Bonus_Destroy_Shield, null, 10000, -1);
       }
       static void Bonus_Destroy_Shield( object empty)
       {
           if (player1 != null)
           player1.shield = false;
           if (player2 != null)
               player2.shield = false;
       }

       static void Bonus_Bomb()
       {
          while( tanks.Count > 0)
               Remove_EnemyTank(tanks[0],null);
       }

       static void Bonus_lvl(PlayerTank tank)
       {
           tank.Lvl_Up();
       }

       static void Bonus_Live(Tank tank)
       {
           tank.HP++;
       }

       static void Bonus_TimeStop()
       {
           timer_for_stoptime.Start();
           foreach (var tank in tanks)
           {
               tank.Timer_Move.Stop();
               tank.timer_for_stage.Stop();
               tank.timer_for_fire.Stop();
               tank.timer_for_handler.Stop();
           }        
       }
           
#endregion

 #region Logic


       static Coordinates Place()
       {
           int x;
           int y;

           Random random = new Random();
           while (true)
           {
               x = random.Next(14) + 1;
               y = random.Next(14) + 1;
               if (field[x, y] != -1)
                   break;
           }
           return new Coordinates(40 * x + 1, 40 * y + 1);
       }

       static void OnArea(Object _object, Tank tank)
       {
           if (_object is Ice)
               OnIce(_object, tank as PlayerTank);
           if (_object is Bonus)
               UseBonus(_object as Bonus, tank);
       }
       
       static void Friendly_Fire(object tank)
       {
           if (tank == player1)
           {
               form.KeyDown += Player1Move_keydown;
               form.KeyUp += Player1Move_keyup;
           }
           else
           {
               form.KeyDown += Player2Move_keydown;
               form.KeyUp += Player2Move_keyup;
           }

       }

       static void OnPause()
       {
           if (global_timer.Enabled == true)
           {
               global_timer.Stop();
               form.KeyDown -= Player1Move_keydown;
               form.KeyDown -= Player2Move_keydown;
               foreach (var tank in tanks)
               {
                   tank.Timer_Move.Stop();
                   tank.timer_for_stage.Stop();
                   tank.timer_for_fire.Stop();
                   tank.timer_for_handler.Stop();
               }
               timer_for_pause.Start();
               form.Paint += Draw_Pause;
               form.Invalidate();
           }
           else
           {
               global_timer.Start();
               form.KeyDown += Player1Move_keydown;
               form.KeyDown += Player2Move_keydown;
               foreach (var tank in tanks)
               {
                   tank.Timer_Move.Start();
                   tank.timer_for_stage.Start();
                   tank.timer_for_fire.Start();
                   tank.timer_for_handler.Start();
               }
               timer_for_pause.Stop();
               form.Paint -= Draw_Pause;
           }
       }

       static void OnIce(object _object, PlayerTank tank)
       {
           if (global_timer.Enabled == true) 
               tank.Timer_Slipping.Start();    
       }

       static void Under_Bush()
       {
           foreach (var bush in landscapes)
           {
               if (bush is Shrub)
               {
                   form.Paint -= bush.Draw;
                   form.Paint += bush.Draw;
               }
           }
       }

 #endregion

 #region Timers_Ticks

       static void Global_Timer_Tick(object sender, EventArgs e)
        {
            form.Invalidate();

            if (tank_for_lvl <= 0)
            {
                delay_for_tank = DateTime.Now;
                if (tanks.Count == 0)
                {
                    Changelvl();
                    return;
                }
            }
           
          if (tanks.Count >= 4 || tanks.Count >= 6 && player2 != null)
                  delay_for_tank = DateTime.Now;

           if (DateTime.Now.Subtract(delay_for_tank).TotalMilliseconds > interval )                  
                    {
                        delay_for_tank = DateTime.Now;
                        NewTank();  
                    }
 
           if (player1 == null && player2 == null)
               GameOver();
        }

       static void StopTime_Tick(object sender, EventArgs e)
       {

           foreach (var tank in tanks)
           {
               tank.Timer_Move.Start();
               tank.timer_for_handler.Start();
               tank.timer_for_stage.Start();
               tank.timer_for_fire.Start(); 
           }
           timer_for_stoptime.Stop();
       }

       static void pause_tick(object sender, EventArgs e)
       {
           foreach (var tank in tanks)
           {
               tank.last_shot = tank.last_shot.AddMilliseconds(timer_for_pause.Interval);
           }
           if (player1 != null)
               player1.last_shot.AddMilliseconds(timer_for_pause.Interval);
           if (player2 != null)
               player2.last_shot.AddMilliseconds(timer_for_pause.Interval);
           delay_for_tank = delay_for_tank.AddMilliseconds(timer_for_pause.Interval);
       }

#endregion

 #region Handlers

       static void Player1Move_keydown(object sender, KeyEventArgs e)
       {
               switch (e.KeyCode)
               {
                   case Keys.D:
                       player1.direction = Direction.right;
                       player1.Timer_Move.Enabled = true;
                       break;
                   case Keys.S:
                       player1.direction = Direction.bottom;
                       player1.Timer_Move.Enabled = true;
                       break;
                   case Keys.A:
                       player1.direction = Direction.left;
                       player1.Timer_Move.Enabled = true;
                       break;
                   case Keys.W:
                       player1.direction = Direction.top;
                       player1.Timer_Move.Enabled = true;
                       break;
                   case Keys.G:
                       player1.Fire();
                       break;
               }

       }
       
       static void Player1Move_keyup(object sender, KeyEventArgs e)
       {

           if (e.KeyCode == Keys.D || e.KeyCode == Keys.S || e.KeyCode == Keys.A || e.KeyCode == Keys.W)
           {
               player1.Timer_Move.Enabled = false;
           }

       }
       
       static void Player2Move_keydown(object sender, KeyEventArgs e)
       {


           switch (e.KeyCode)
           {
               case Keys.Right:
                   player2.direction = Direction.right;
                   player2.Timer_Move.Start();
                   break;
               case Keys.Down:
                   player2.direction = Direction.bottom;
                   player2.Timer_Move.Start();
                   break;
               case Keys.Left:
                   player2.direction = Direction.left;
                   player2.Timer_Move.Start();
                   break;
               case Keys.Up:
                   player2.direction = Direction.top;
                   player2.Timer_Move.Start();
                   break;
               case Keys.Enter:
                       player2.Fire();
                   break;
           }
       }
     
       static void Player2Move_keyup(object sender, KeyEventArgs e)
       {
           if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Up)
               player2.Timer_Move.Stop();

       }

       
       static void Press_Interrupt(object sender, KeyEventArgs e)
       {
           if (e.KeyCode == Keys.Escape)
               Interrupt();

           if (e.KeyCode == Keys.P)
               OnPause();
       }      
      
       static void Press_Nextlvl(object sender, KeyEventArgs e)
       {
           if(e.KeyCode == Keys.Enter)
           {

               player1_total_score += player1_lvl_score;
               player2_total_score += player2_lvl_score;
               player1_lvl_score = 0;
               player2_lvl_score = 0;
               form.Paint -= Draw_Between_lvl;
               form.Paint += Draw_Game;
               form.KeyUp -= Press_Nextlvl;
               form.KeyDown += Press_Interrupt;
              if (player1 != null)
              {
                  form.KeyDown += Player1Move_keydown;
                  form.KeyUp += Player1Move_keyup;
              }
              if (player2 != null)
              {
                  form.KeyDown += Player2Move_keydown;
                  form.KeyUp += Player2Move_keyup;
              }
               Nextlvl();
           }
       }

       static void Press_WriteName(object sender, KeyPressEventArgs e)
       {
           if ((e.KeyChar >= 97 && e.KeyChar <= 122 && playername.Length<11) ||
               (e.KeyChar >= 48 && e.KeyChar <= 57 && playername.Length < 11))
           {         
               playername += e.KeyChar.ToString();
           }

           if (playername.Length > 0 && e.KeyChar == (Char)Keys.Back)
           {
               playername = playername.Substring(0, playername.Length - 1);
           }

           if (e.KeyChar == (Char)Keys.Enter)
           {
               if (TableRecords.Order_To_Add(player1_total_score))
               {
                   TableRecords.Add(playername, player1_total_score);
                   player1_total_score = -1;
                   playername = "";
                   if (TableRecords.Order_To_Add(player2_total_score))
                   goto Exit;
               }
               if (TableRecords.Order_To_Add(player2_total_score))
                   TableRecords.Add(playername, player2_total_score);

               form.Paint -= Draw_Writename;
               form.KeyPress -= Press_WriteName;
               form.Paint -= Draw_Records;
               Menu_Records();
           }
           Exit:
           form.Invalidate();
       }

#endregion

 #region Drawing

        static void Draw_Mainmenu(object sender, PaintEventArgs e)
       {
           Rectangle rec = new Rectangle(75, 50, 500, 100);

           StringFormat sf = new StringFormat();
           sf.Alignment = StringAlignment.Center;
           sf.LineAlignment = StringAlignment.Center;

           e.Graphics.DrawString("Танчики", new Font("Arial Black", 70), Brushes.DarkRed, rec, sf);
       }
       
        static void Draw_Game(object sender, PaintEventArgs e)
       {
           Point[] flag = { new Point(7 * 40 + 10, 13 * 40 + 5), new Point(7 * 40 + 35, 13 * 40 + 13), new Point(7 * 40 + 10, 13 * 40 + 21) }; 
           e.Graphics.FillPolygon(Brushes.Orange,flag);
           e.Graphics.FillRectangle(Brushes.White, new Rectangle(7*40+7,13*40+5,3,35));


            SolidBrush brush = new SolidBrush(Color.DarkGray);
           e.Graphics.FillRectangle(brush, new Rectangle(0, 40, 40, 520));
           e.Graphics.FillRectangle(brush, new Rectangle(0, 0, 560, 40));
           e.Graphics.FillRectangle(brush, new Rectangle(0, 560, 560, 40));
           e.Graphics.FillRectangle(brush, new Rectangle(560, 0, 100, 600));

           StringFormat sf = new StringFormat();
           sf.Alignment = StringAlignment.Center;
           sf.LineAlignment = StringAlignment.Center;
     
           e.Graphics.FillRectangle(Brushes.White, new Rectangle(580, 100 + 4, 8, 26));
           e.Graphics.FillRectangle(Brushes.White, new Rectangle(580 + 22, 100 + 4, 8, 26));
           e.Graphics.FillRectangle(Brushes.White, new Rectangle(580, 100 + 12, 30, 14));
           e.Graphics.FillRectangle(Brushes.White, new Rectangle(580 + 13, 100, 4, 15));
           e.Graphics.DrawString(tank_for_lvl.ToString(), new Font("Arial Black", 25), Brushes.White, new Rectangle(605, 93, 70, 50), sf);

           e.Graphics.DrawString( "Уровень " + CurrentLevel.ToString() , new Font("Arial Black", 14), Brushes.White, new Rectangle(560, 193, 100, 50), sf);

           e.Graphics.FillRectangle(Brushes.Goldenrod, new Rectangle(580, 300 + 4, 8, 26));
           e.Graphics.FillRectangle(Brushes.Goldenrod, new Rectangle(580 + 22, 300 + 4, 8, 26));
           e.Graphics.FillRectangle(Brushes.Goldenrod, new Rectangle(580, 300 + 12, 30, 14));
           e.Graphics.FillRectangle(Brushes.Goldenrod, new Rectangle(580 + 13, 300, 4, 15));
           e.Graphics.DrawString(player1.HP.ToString(), new Font("Arial Black", 25), Brushes.White, new Rectangle(600, 293, 70, 50), sf);

           if (player2 != null)
           {
               e.Graphics.FillRectangle(Brushes.DarkOliveGreen, new Rectangle(580, 400 + 4, 8, 26));
               e.Graphics.FillRectangle(Brushes.DarkOliveGreen, new Rectangle(580 + 22, 400 + 4, 8, 26));
               e.Graphics.FillRectangle(Brushes.DarkOliveGreen, new Rectangle(580, 400 + 12, 30, 14));
               e.Graphics.FillRectangle(Brushes.DarkOliveGreen, new Rectangle(580 + 13, 400, 4, 15));
               e.Graphics.DrawString(player2.HP.ToString(), new Font("Arial Black", 25), Brushes.White, new Rectangle(600, 393, 70, 50), sf);
           }
       }

        static void Draw_Between_lvl(object sender, PaintEventArgs e)
        {
            int sum1 = player1_lvl_score + player1_total_score;
            int sum2 = player2_lvl_score + player2_total_score;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            e.Graphics.DrawString("Уровень "+CurrentLevel.ToString()+" пройден", new Font("Arial Black", 40), Brushes.DarkRed, new Rectangle(60, 25, 500, 150), sf);

            e.Graphics.DrawString("Заработано ранее", new Font("Arial Black", 15), Brushes.DarkRed, new Rectangle(85, 150, 250, 100), sf);
            e.Graphics.DrawString(" За уровень", new Font("Arial Black", 15), Brushes.DarkRed, new Rectangle(310, 150, 150, 100), sf);
            e.Graphics.DrawString("Всего", new Font("Arial Black", 15), Brushes.DarkRed, new Rectangle(460, 150, 100, 100), sf);
           
            e.Graphics.DrawString("Игрок 1:", new Font("Arial Black", 15), Brushes.White, new Rectangle(0, 200, 110, 100), sf);
            e.Graphics.DrawString(player1_total_score.ToString(), new Font("Arial Black", 15), Brushes.White, new Rectangle(85, 200, 250, 100), sf);
            e.Graphics.DrawString(player1_lvl_score.ToString(), new Font("Arial Black", 15), Brushes.White, new Rectangle(310, 200, 150, 100), sf);
            e.Graphics.DrawString(sum1.ToString(), new Font("Arial Black", 15), Brushes.White, new Rectangle(460, 200, 100, 100), sf);

            if (player2_total_score != 0 || player2!=null)
            {
                e.Graphics.DrawString("Игрок 2:", new Font("Arial Black", 15), Brushes.White, new Rectangle(0, 300, 110, 100), sf);
                e.Graphics.DrawString(player2_total_score.ToString(), new Font("Arial Black", 15), Brushes.White, new Rectangle(85, 300, 250, 100), sf);
                e.Graphics.DrawString(player2_lvl_score.ToString(), new Font("Arial Black", 15), Brushes.White, new Rectangle(310, 300, 150, 100), sf);
                e.Graphics.DrawString(sum2.ToString(), new Font("Arial Black", 15), Brushes.White, new Rectangle(460, 300, 100, 100), sf);
            }
            e.Graphics.DrawString("Нажмите Enter для перехода на следующий уровень", new Font("Arial Black", 15), Brushes.DarkRed, new Rectangle(115, 400, 400, 200), sf);
        }

        static void Draw_Records(object sender, PaintEventArgs e)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Far;
            sf.LineAlignment = StringAlignment.Center;

            e.Graphics.DrawString("Рекорды", new Font("Arial Black", 35), Brushes.DarkRed, new Rectangle(150, 20, 300, 50), sf);
            e.Graphics.DrawString("Игрок", new Font("Arial Black", 14), Brushes.DarkRed, new Rectangle(160, 60, 100, 50), sf);
            e.Graphics.DrawString("Счет", new Font("Arial Black", 14), Brushes.DarkRed, new Rectangle(350, 60, 100, 50), sf);

            for (int i = 0; i < TableRecords.records.Count; i++)
            {
                if (TableRecords.records[i].score != 1)
                {
                    e.Graphics.DrawString(TableRecords.records[i].name.ToString(), new Font("Arial Black", 12), Brushes.White, new Rectangle(100, 100 + 30 * i, 150, 30), sf);
                    e.Graphics.DrawString(TableRecords.records[i].score.ToString(), new Font("Arial Black", 12), Brushes.White, new Rectangle(350, 100 + 30 * i, 100, 30), sf);
                }
            }
        }

        static void Draw_Writename(object sender, PaintEventArgs e)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            if (TableRecords.Order_To_Add(player1_total_score))
                e.Graphics.DrawString("Игрок 1 набрал "+ player1_total_score.ToString()+ " очков.",new Font("Arial Black",12), Brushes.DarkRed,new Rectangle( 125,400,400,50),sf);
            else
                e.Graphics.DrawString("Игрок 2 набрал " + player2_total_score.ToString() + " очков.", new Font("Arial Black", 12), Brushes.DarkRed, new Rectangle(125, 400, 400, 50), sf);
            e.Graphics.DrawString("Введите имя для записи в таблицу.", new Font("Arial Black", 12), Brushes.DarkRed, new Rectangle(125, 425, 400, 50), sf);
            e.Graphics.DrawString("(Имя должно содержать только символы латинского алфавита и цифры.)", new Font("Arial Black", 8), Brushes.DarkRed, new Rectangle(80, 450, 470, 50), sf);

            e.Graphics.DrawString(playername, new Font("Arial Black", 15), Brushes.DarkRed, new Rectangle(80, 475, 470, 50), sf);
        }

        static void Draw_Exit(object sender, PaintEventArgs e)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            e.Graphics.DrawString("Выйти из игры?", new Font("Arial Black", 35), Brushes.DarkRed, new Rectangle(75, 40, 500, 50), sf);
        }

        static void Draw_Pause(object sender, PaintEventArgs e)
        {
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            e.Graphics.DrawString("Пауза", new Font("Arial Black", 30), Brushes.White,new Rectangle(150,300,300,50), sf);
        }

#endregion



    }
}
