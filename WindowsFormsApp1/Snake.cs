using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class Snake
    {
        private int length;
        public int Lenght
        {
            get { return length; }
            set { length = value; }//длинна по умолчанию
        }
        private Point[] location;
        public Point[] Location
        {
            get { return location; }
        }
        public Snake()
        {
            location = new Point[25 * 25];
            Reset(); //Обновление точек
        }
        public void Reset()
        {
            length = 2;
            for (int i = 0; i < length; i++)
            {
                location[i].X = 12;
                location[i].Y = 12;
            }
        }
        public void Relocate()//перемещение
        {
            for (int i = length - 1; i > 0; i--)
            {
                location[i] = location[i - 1];
            }
        }
        public void Up()//вверх
        {
            Relocate();
            location[0].Y--;
            if (location[0].Y < 0)
            {
                location[0].Y += 25;
            }
        }
        public void Down()//вниз
        {
            Relocate();
            location[0].Y++;
            if (location[0].Y > 24)
            {
                location[0].Y -= 25;
            }
        }
        public void Left()//влево
        {
            Relocate();
            location[0].X--;
            if (location[0].X < 0)
            {
                location[0].X += 25;
            }
        }
        public void Right()//вправо
        {
            Relocate();
            location[0].X++;
            if (location[0].X > 24)
            {
                location[0].X -= 25;
            }
        }
        public void IncLength()//удлиняем
        {
            length++;
        }
    }
}