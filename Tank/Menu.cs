using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Tank
{
    public delegate void DelButton_Press();

    public static class Menu
    {
        public static List<Button> buttons = new List<Button>();

        public static Form1 form;

        public static event DelButton_Press button_press;

        public static void Add(Button button)
        {
            if (buttons.Count == 0)
                form.KeyDown += OnPress;
            form.Paint += button.Draw;
            button_press += button.press_handler;

            buttons.Add(button);

            if (buttons.Count > 1)
                buttons.Sort();
            else
                buttons[0].focus = true;

        }

        public static void Remove(string title)
        {
            foreach (var button in buttons)
            {
                if (button.title == title)
                {
                    form.Paint -= button.Draw;
                    buttons.Remove(button);
                    break;
                }
            }

            if (buttons.Count == 0)
                form.KeyDown -= OnPress;
        }

        public static void Remove()
        {
            while (buttons.Count > 0)
            {
                form.Paint -= buttons[0].Draw;
                button_press -= buttons[0].press_handler;
                buttons.RemoveAt(0);
            }
            form.KeyDown -= OnPress;
        }
       

        static int Get_focus()
        { 
            foreach (var button in buttons)
                if (button.focus == true)
                    return buttons.IndexOf(button);
    
            return -1;
        }


        public static void OnPress(object sender, KeyEventArgs e)
        {          
                int index = Get_focus();
            if(index!=-1)
                switch (e.KeyCode)
                {
                    case Keys.Up:
                        if (buttons[0].focus != true)
                        {
                            buttons[index].focus = false;
                            buttons[index - 1].focus = true;
                            form.Invalidate(new Rectangle(buttons[index].cordinates.x, buttons[index].cordinates.y,370,50));
                            form.Invalidate(new Rectangle(buttons[index - 1].cordinates.x, buttons[index - 1].cordinates.y, 370, 50));
                        }
                        break;
                    case Keys.Down:
                        if (buttons[buttons.Count - 1].focus != true)
                        {
                            buttons[index].focus = false;
                            buttons[index + 1].focus = true;
                            form.Invalidate(new Rectangle(buttons[index].cordinates.x, buttons[index].cordinates.y, 370, 50));
                            form.Invalidate(new Rectangle(buttons[index + 1].cordinates.x, buttons[index + 1].cordinates.y, 370, 50));
                        }
                        break;
                    case Keys.Enter:
                        {
                            button_press();
                            form.Invalidate();
                        }
                        break;
                }
            }
        }
    
        
   


}
