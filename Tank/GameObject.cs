using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Tank
{
  

    public abstract class GameObject
    {
       public Coordinates coordinates;
       static public DelCollide collide;

        public GameObject(Coordinates cordinates)
        {
            this.coordinates = cordinates;
        }

        public abstract void Draw(object sender, PaintEventArgs e);
       
        public void SearchGameObject(GameObject obj)
        {
            if (obj.coordinates.x + 15 >= coordinates.x && obj.coordinates.x + 15 <= coordinates.x + 40
              && obj.coordinates.y + 15 >= coordinates.y && obj.coordinates.y + 15 <= coordinates.y + 40)
                collide(obj, this);
        }
    }
}
