using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace WindowsFormsApp1
{
    [Serializable]
    public partial class SnakeView : Form
    {
        private string datalFileName = AppDomain.CurrentDomain.BaseDirectory + @"\data.xml";
        private int score = 0;
        private int recorde;
        private Keys direction;
        private Keys arrow;
        private Point lastsegment;
        private Food food;
        private Snake snake;
        private Bitmap offscreenbitmap;
        private Graphics BitmapGraph;
        private Graphics ScreenGraph;
        public SnakeView()
        {
            InitializeComponent();
            direction = Keys.Left;
            arrow = direction;//направление
            offscreenbitmap = new Bitmap(250, 250);//наше поле
            snake = new Snake();//наша змейка
            food = new Food();//наша еда
            lastsegment = snake.Location[snake.Lenght - 1];
            
        }
      
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;
        private const int WM_LBUTTONDBLCLK = 0x00A3;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDBLCLK)
            {
                return;
            }
            switch (m.Msg)
            {

                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    if ((int)m.Result == HTCLIENT)
                        m.Result = (IntPtr)HTCAPTION;
                    return;
            }
            base.WndProc(ref m);
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            timer2.Start();
            var image1 = (Bitmap)Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\1.jpg");
            Snack(image1);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            timer1.Start();
            var image1 = (Bitmap)Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\2.png");
            Snack(image1);
        }
        private  void Snack(Bitmap image1)
        {
            lastsegment = snake.Location[snake.Lenght - 1];
            if (((((arrow == Keys.Left) && (direction != Keys.Right)) || (arrow == Keys.Right) && (direction != Keys.Left)) || (arrow == Keys.Up) && (direction != Keys.Down)) || (arrow == Keys.Down) && (direction != Keys.Up)) direction = arrow;//настройки направления змейки
            switch (direction)
            {
                case Keys.Left:
                    snake.Left();
                    break;
                case Keys.Right:
                    snake.Right();
                    break;
                case Keys.Up:
                    snake.Up();
                    break;
                case Keys.Down:
                    snake.Down();
                    break;
            }
            BitmapGraph.Clear(Color.White);
            Bitmap image2 = (Bitmap)Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\3.png");
            BitmapGraph.FillEllipse(new TextureBrush(bitmap: image2), (food.Location.X * 10), (food.Location.Y * 10), 10, 10);

            bool gameover = false;
            for (int i = 0; i < snake.Lenght; i++)//увеличивает вашу змейку(ну не ту что в штанах, ей даже цикл не поможет)
            {
                BitmapGraph.FillRectangle(new TextureBrush(bitmap: image1), (snake.Location[i].X * 10), (snake.Location[i].Y * 10), 10, 10);
                if ((snake.Location[i] == snake.Location[0]) && (i > 0))
                    gameover = true; //игра окончена если змейка ударились в себя
            }
            ScreenGraph.DrawImage(offscreenbitmap, 0, 0);

            Check();//проверить, не побеждает ли змея
            if (gameover == true)
            {
                GameOver();
            }
        }
        private void паузаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BitmapGraph != null)
                BitmapGraph.Dispose();
            if (ScreenGraph != null)
                ScreenGraph.Dispose();
            Application.Exit();

        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Left) || (e.KeyCode == Keys.Right) || (e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down)) arrow = e.KeyCode;
        }
        protected override bool IsInputKey(Keys keyData)
        {
            return true;
        }
        private void Check()
        {
            if (snake.Location[0] == food.Location)//счётчик очков
            {
                snake.IncLength();
                snake.Location[snake.Lenght - 1] = lastsegment;
                score += 10;
                ScoreLabel.Text = score.ToString();
                CreateFood();
            }
            if (score > recorde)//рекорд
            {
              recorde = score;
              RecordeLabel.Text = Convert.ToString(Deserialize() + score);
            }
        }
        private void CreateFood()
        {
            bool occupied;//оккупированный
            do
            {
                food.CreateFood();
                occupied = false;
                for (int i = 0; i < snake.Lenght; i++)
                {
                    if (food.Location == snake.Location[i])
                    {
                        occupied = true;
                        break;
                    }
                }
            }
            while (occupied == true);
        }


            private void StartGame()
            {
                snake.Reset();
                CreateFood();
                direction = Keys.Left;
                timer1.Interval = 100;
                timer2.Interval = 100;
                Serialize();
                ScoreLabel.Text = "0";
                score = 0;
                recorde = score;
                RecordeLabel.Text = Convert.ToString(Deserialize() + score);
                BitmapGraph = Graphics.FromImage(offscreenbitmap);
                ScreenGraph = playArea.CreateGraphics();
                timer1.Enabled = true;
                timer2.Enabled = false;
                this.BackColor = Color.White;
                menuStrip2.BackColor = Color.White;
                RecordeLabel.ForeColor = Color.DarkGreen;
                ScoreLabel.ForeColor = Color.FromArgb(0, 192, 0);
                label1.ForeColor = Color.Black;
                Record.ForeColor = Color.Black;
                Deserialize();
        }

            public void GameOver()
            {
                Serialize();
                timer1.Enabled = false;
                timer2.Enabled = false;
                BitmapGraph.Dispose();
                ScreenGraph.Dispose();
                string text1 = "Вы набрали " + score + " поинтов!" +"\nВаш рекорд: " + RecordeLabel.Text + "\nНачать новую игру?";
            if (MessageBox.Show(text1, "Игра окончена!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)//выбор действия
            {
                StartGame();
            }
            else
            {
                playArea.BackgroundImage = (Bitmap)Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\313.png");
            }
            Deserialize();
        }

        private void тяжелыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Interval = 150;
            timer2.Interval = 150;
            this.BackColor = Color.LightSeaGreen;
            menuStrip2.BackColor = Color.LightSeaGreen;
            RecordeLabel.ForeColor = Color.White;
            ScoreLabel.ForeColor = Color.White;
            Record.ForeColor = Color.White;
            label1.ForeColor = Color.White;
        }

        private void легкийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Interval = 100;
            timer2.Interval = 100;
            this.BackColor = Color.LightSkyBlue;
            menuStrip2.BackColor = Color.LightSkyBlue;
            RecordeLabel.ForeColor = Color.White;
            ScoreLabel.ForeColor = Color.White;
            Record.ForeColor = Color.White;
            label1.ForeColor = Color.White;
        }

        private void тяжелыйToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            timer1.Interval = 50;
            timer2.Interval = 50;
            this.BackColor = Color.Tomato;
            menuStrip2.BackColor = Color.Tomato;
            RecordeLabel.ForeColor = Color.White;
            ScoreLabel.ForeColor = Color.White;
            Record.ForeColor = Color.White;
            label1.ForeColor = Color.White;
        }

        private void игратьToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            StartGame();
            timer1_Tick(sender, e);
        }


        private void зеленыйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1_Tick(sender,e);
        }

        private void синийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer2_Tick(sender, e);
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Serialize();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void SnakeView_Load(object sender, EventArgs e)
        {
            if (File.Exists(datalFileName))
                RecordeLabel.Text = Convert.ToString(Deserialize());
                
        }
        private void Serialize()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(Convert.ToInt32(RecordeLabel.Text).GetType());

            using (StreamWriter writer = File.CreateText("data.xml"))
            {
                try
                {
                    xmlSerializer.Serialize(writer, Convert.ToInt32(RecordeLabel.Text));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private int Deserialize()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(recorde.GetType());
            using (FileStream fs = new FileStream(datalFileName, FileMode.Open))
            {
                var list = new int();

                try
                {
                    list = (int)xmlSerializer.Deserialize(fs);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                return list;
            }
        }
    }
}
