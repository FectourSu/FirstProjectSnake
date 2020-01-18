using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class Food
    {
        private Point location;
        public Point Location
        {
            get {return location;}
        }

        public Food()
        {
            location = new Point();
        }
        public void CreateFood()
        {
          Random rnd = new Random();
          location = new Point(rnd.Next(0, 25), rnd.Next(0, 25));//генерация рандомных точек (max, min)
        }
    }
}
