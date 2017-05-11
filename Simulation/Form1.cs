using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulation
{
    public partial class Form1 : Form
    {
        class Food
        {
            int x;
            int y;
            int energy;
            public Food(int X, int Y, int Energy)
            {
                x = X;
                y = Y;
                energy = Energy;
            }
            public int X
            {
                get { return x; }
                set { x = value; }
            }
            public int Y
            {
                get { return y; }
                set { y = value; }
            }
            public int Energy
            {
                get { return energy; }
            }
        }

        class Bug
        {
            int x;
            int y;
            int e;
            int a;
            public Bug(int X, int Y, int Energy)
            {
                x = X;
                y = Y;
                e = Energy;
                a = 0;
            }
            public int X
            {
                get { return x; }
                set { x = value; }
            }
            public int Y
            {
                get { return y; }
                set { y = value; }
            }
            public int Energy
            {
                get { return e; }
                set { e = value; }
            }
            public int Age
            {
                get { return a; }
                set { a = value; }
            }
        }

        System.Collections.ArrayList Foods;
        System.Collections.ArrayList Bugs;
        Random R;
        Form2 ChartForm;
        System.Collections.ArrayList Census;
        System.Collections.ArrayList FoodCensus;
        System.Collections.ArrayList Deaths;
        System.Collections.ArrayList FoodProduction;
        int Clock;
        int DeathCount;
        int FoodProduced;
        double SolarEnergy;
        int DaysToHarvest;

        public Form1()
        {
            InitializeComponent();
            this.Text = "Ecosystem";
            R = new Random();
            Foods = new System.Collections.ArrayList();
            Bugs = new System.Collections.ArrayList();
            for (int i = 0; i < 3000; i++)
                Foods.Add(new Food(R.Next(pictureBox1.Width), R.Next(pictureBox1.Height), 20));
            for (int i=0; i<100; i++)
            Bugs.Add(new Bug(R.Next(pictureBox1.Width), R.Next(pictureBox1.Height), 500));
            DrawEcosystem();
            ChartForm = new Form2();
            ChartForm.Show();
            Census = new System.Collections.ArrayList();
            FoodCensus = new System.Collections.ArrayList();
            Deaths = new System.Collections.ArrayList();
            FoodProduction = new System.Collections.ArrayList();
            FoodProduced = 0;
            DeathCount = 0;
            Clock = 0;
            SolarEnergy = 0;
            DaysToHarvest = 9000;
            timer1.Enabled = true;
        }
        private void DrawEcosystem()
        {
            Bitmap View = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(View);
            g.FillRectangle(new SolidBrush(Color.Black), 0, 0, View.Width, View.Height);
            foreach (Food f in Foods)
            {
                View.SetPixel(f.X, f.Y, Color.Green);
            }
            foreach (Bug b in Bugs)
            {
                g.FillRectangle(new SolidBrush(Color.White), b.X - 1, b.Y - 1, 3, 3);
            }
            pictureBox1.Image = View;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Clock++;
            // Add Solar energy
            SolarEnergy += 100 * Math.Sin(Clock * Math.PI / 7500) + 100;
            //Add more Food
            while (SolarEnergy >= 20)
            {
                Foods.Add(new Food(R.Next(pictureBox1.Width), R.Next(pictureBox1.Height), 20));
                SolarEnergy -= 20;
                ++FoodProduced;
            }
            System.Collections.ArrayList DeadBugs = new System.Collections.ArrayList();
            System.Collections.ArrayList NewBugs = new System.Collections.ArrayList();
            foreach (Bug b in Bugs)
            {
                //Move bug
                int m = R.Next(8);
                switch (m)
                {
                    case 0:
                        if ((b.X < pictureBox1.Width - 1) && (b.Y > 2))
                        {
                            b.X += 1;
                            b.Y -= 2;
                        }
                        break;
                    case 1:
                        if ((b.X < pictureBox1.Width - 3) && (b.Y > 1))
                        {
                            b.X = b.X + 2;
                            b.Y = b.Y - 1;
                        }
                        break;
                    case 2:
                        if ((b.X < pictureBox1.Width - 3) && (b.Y < pictureBox1.Height - 2))
                        {
                            b.X = b.X + 2;
                            b.Y = b.Y + 1;
                        }
                        break;
                    case 3:
                        if ((b.X < pictureBox1.Width - 2) && (b.Y < pictureBox1.Height - 3))
                        {
                            b.X = b.X + 1;
                            b.Y = b.Y + 2;
                        }
                        break;
                    case 4:
                        if ((b.X > 1) && (b.Y < pictureBox1.Height - 3))
                        {
                            b.X = b.X - 1;
                            b.Y = b.Y + 2;
                        }
                        break;
                    case 5:
                        if ((b.X > 2) && (b.Y < pictureBox1.Height - 2))
                        {
                            b.X = b.X - 2;
                            b.Y = b.Y + 1;
                        }
                        break;
                    case 6:
                        if ((b.X > 2) && (b.Y > 1))
                        {
                            b.X = b.X - 2;
                            b.Y = b.Y - 1;
                        }
                        break;
                    case 7:
                        if ((b.X > 1) && (b.Y > 2))
                        {
                            b.X = b.X - 1;
                            b.Y = b.Y - 2;
                        }
                        break;
                }
                //Eat
                System.Collections.ArrayList Eaten = new System.Collections.ArrayList();
                foreach (Food f in Foods)
                {
                    if ((f.X >= b.X - 1) && (f.X <= b.X + 1) && (f.Y >= b.Y - 1) && (f.Y <= b.Y + 1))
                    {
                        b.Energy += f.Energy;
                        Eaten.Add(f);
                    }
                }
                foreach (Food f in Eaten)
                    Foods.Remove(f);
                //Expend Energy
                --b.Energy;
                //Increment Age
                ++b.Age;
                //Die?
                if (b.Energy == 0)
                {
                    DeadBugs.Add(b);
                    ++DeathCount;
                }
                //Split?
                if ((b.Age >= 500) && (b.Energy >= 1000))
                {
                    DeadBugs.Add(b);
                    NewBugs.Add(new Bug(b.X, b.Y, b.Energy / 2));
                    NewBugs.Add(new Bug(b.X, b.Y, b.Energy / 2));
                }
            }
            //Delete Dead Bugs
            foreach (Bug b in DeadBugs)
                Bugs.Remove(b);
            //Add New Bugs
            foreach (Bug b in NewBugs)
                Bugs.Add(b);
            //Draw
          DrawEcosystem();
            // Harvest Bugs
            --DaysToHarvest;
            //DeadBugs = new System.Collections.ArrayList();
            if (DaysToHarvest<=0)
            {
                int n = Bugs.Count - 20;
                while (n > 0)
                {
                    Bugs.RemoveAt(R.Next(n+20));
                    --n;
                }
                DaysToHarvest = 15000;
            }
            //Delete Dead Bugs
           // foreach (Bug b in DeadBugs)
            //    Bugs.Remove(b);
            //Take Census
            if (Clock % 100 == 0)
            {
                Census.Add(Bugs.Count);
                FoodCensus.Add(Foods.Count);
                Deaths.Add(DeathCount);
                FoodProduction.Add(FoodProduced);
                DeathCount = 0;
                FoodProduced = 0;
                if (Census.Count > 530)
                {
                    Census.RemoveAt(0);
                    FoodCensus.RemoveAt(0);
                    Deaths.RemoveAt(0);
                    FoodProduction.RemoveAt(0);
                }
                DrawChart();
            }
        }
        void DrawChart()
        {
            Bitmap Chart = new Bitmap(530, 200);
            Graphics g = Graphics.FromImage(Chart);
            g.FillRectangle(new SolidBrush(Color.Black), 0, 0, Chart.Width, Chart.Height);
            // Draw population line
            int x = 1;

            // Draw food production
            PointF P0p = new PointF(x, (200 - (float)Convert.ToDecimal(FoodProduction[0]) / 50));
            foreach (int p in FoodProduction)
            {
                PointF P1 = new PointF(x, (float)(200 - p / 100));
                g.DrawLine(new Pen(Color.Gold), P0p, P1);
                P0p = P1;
                x++;
            }
            
            // Draw Bug deaths
            x = 1;
            PointF P0d = new PointF(x, 200 - (float)Convert.ToDecimal(Deaths[0]));
            foreach (int p in Deaths)
            {
                PointF P1 = new PointF(x, (float)(200 - p));
                g.DrawLine(new Pen(Color.DarkGray), P0d, P1);
                P0d = P1;
                x++;
            }
                // Draw food population
            x = 1;
                PointF P0f = new PointF(x, (200 - (float)Convert.ToDecimal(FoodCensus[0]) / 100));
                foreach (int p in FoodCensus)
                {
                    PointF P1 = new PointF(x, (float)(200 - p / 50));
                    g.DrawLine(new Pen(Color.Green), P0f, P1);
                    P0f = P1;
                    x++;
                }
                // Draw Bug Population
                x = 1;
                PointF P0 = new PointF(x, 200 - (float)Convert.ToDecimal(Census[0]));
                foreach (int p in Census)
                {
                    PointF P1 = new PointF(x, (float)(200 - p));
                    g.DrawLine(new Pen(Color.White), P0, P1);
                    P0 = P1;
                    x++;
                }
                //Display Chart
                ChartForm.ShowChart(Chart);
            }
        }
    }
    

