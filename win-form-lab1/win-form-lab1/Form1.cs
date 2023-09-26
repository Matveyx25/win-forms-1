using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace win_form_lab1
{
    public partial class Form1 : Form
    {
        //----Constructor----//
        public Form1()
        {
            InitializeComponent();
            g = pictureBox1.CreateGraphics();
        }
        //-------------------//

        Graphics g;



        Pen DrawPen = new Pen(Color.Black, 1);
        int SplineType;
        const int np = 20; // Размер массива
        Point[] ArPoints = new Point[np]; // Массив точек
        int CountPoints = 0; // Счетчик точек

        //------------------------------
        


        //------------------------------
        //..............METHODS...............//
        public void DrawCubeSpline(Pen DrPen, Point[] P)
        {
            PointF[] L = new PointF[4]; // Матрица вещественных коэффициентов
            Point Pv1 = P[0];
            Point Pv2 = P[0];
            const double dt = 0.04;
            double t = 0;
            double xt, yt;
            Point Ppred = P[0], Pt = P[0];
            // Касательные векторы
            Pv1.X = 4 * (P[1].X - P[0].X);
            Pv1.Y = 4 * (P[1].Y - P[0].Y);
            Pv2.X = 4 * (P[3].X - P[2].X);
            Pv2.Y = 4 * (P[3].Y - P[2].Y);
            // Коэффициенты полинома
            L[0].X = 2 * P[0].X - 2 * P[2].X + Pv1.X + Pv2.X; // Ax
            L[0].Y = 2 * P[0].Y - 2 * P[2].Y + Pv1.Y + Pv2.Y; // Ay
            L[1].X = -3 * P[0].X + 3 * P[2].X - 2 * Pv1.X - Pv2.X; // Bx
            L[1].Y = -3 * P[0].Y + 3 * P[2].Y - 2 * Pv1.Y - Pv2.Y; // By
            L[2].X = Pv1.X; // Cx
            L[2].Y = Pv1.Y; // Cy
            L[3].X = P[0].X; // Dx
            L[3].Y = P[0].Y; // Dy
            while (t < 1 + dt / 2)
            {
                xt = ((L[0].X * t + L[1].X) * t + L[2].X) * t + L[3].X;
                yt = ((L[0].Y * t + L[1].Y) * t + L[2].Y) * t + L[3].Y;
                Pt.X = (int)Math.Round(xt);
                Pt.Y = (int)Math.Round(yt);
            g.DrawLine(DrPen, Ppred, Pt);
                Ppred = Pt;
                t = t + dt;
            }
        }
        //------------------------------
        // Curve Bezie
        public void DrawBezie(Pen DrPen, Point[] P, int n)
        {
            float step = 0.01f; // Шаг табуляции
            //PointF[] result = new PointF[100]; // Массив точек кривой
            //Point Pv1 = P[0];
            //Point Pv2 = P[0];

            int yPred = P[0].Y;
            int xPred = P[0].X;
           
            //int j = 0;
            for (float t = step; t < 1 + step / 2; t += step)
            {
                double ytmp = 0;
                double xtmp = 0;

                for (int i = 0; i < n; i++)
                {
                    double b = GetBernsteinPolynomial(i, n - 1, t);
                    xtmp += P[i].X * b;
                    ytmp += P[i].Y * b;
                }
                g.DrawLine(DrPen, xPred, yPred, (float)Math.Round(xtmp), (float)Math.Round(ytmp));
                xPred = (int)Math.Round(xtmp);
                yPred = (int)Math.Round(ytmp);
            }

            //Graphics g = pictureBox1.CreateGraphics();
           
        }

        //------------------------------
        //Factorial
        static double Factorial(int n)
        {
            double x = 1;
            for (int i = 1; i <= n; i++)
                x *= i;
            return x;
        }


        //----------------------

        private double GetBernsteinPolynomial(int i, int n, float t)
        {
            return (Factorial(n) / (Factorial(i) * Factorial(n - i))) * (float)Math.Pow(t, i) * (float)Math.Pow(1 - t, n - i);
        }

        //-----------------------------------------------------------------------------//




        //...........SLOTS...........//

        // Обработчик события выбора цвета
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.SelectedIndex) // выбор цвета
            {
                case 0:
                    DrawPen.Color = Color.Black;
                    break;
                case 1:
                    DrawPen.Color = Color.Red;
                    break;
                case 2:
                    DrawPen.Color = Color.Green;
                    break;
                case 3:
                    DrawPen.Color = Color.Blue;
                    break;
            }

        }
        //------------------------------------------//
        // Обработчик события выбора типа сплайна
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SplineType = comboBox1.SelectedIndex;
        }


        //-----Slot button1 is clicked---//
        private void button1_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            CountPoints = 0;
        }

        //----Slot pictureBox1 is MouseDown----//
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (CountPoints >= np) return;
            ArPoints[CountPoints].X = e.X; ArPoints[CountPoints].Y = e.Y;
           
            if (SplineType == 0) // Кубический сплайн
            {
                g.DrawEllipse(DrawPen, e.X - 2, e.Y - 2, 5, 5);
                switch (CountPoints)
                {
                    case 1: // первый вектор
                        {
                            g.DrawLine(new Pen(Color.Magenta, 1), ArPoints[0], ArPoints[1]);
                            CountPoints++;
                        }
                        break;
                    case 3: // второй вектор
                        {
                            g.DrawLine(new Pen(Color.Magenta, 1), ArPoints[2], ArPoints[3]);
                            DrawCubeSpline(new Pen(DrawPen.Color, 3), ArPoints);
                            CountPoints = 0;
                        }
                        break;
                    default:
                        CountPoints++; // иначе
                        break;
                }
            }
            else // Безье
            {
                if (e.Button == MouseButtons.Right) // Конец ввода
                {
                    if (CountPoints > 1)
                    {
                        //g.DrawLine(new Pen(Color.Magenta, 1), ArPoints[CountPoints - 1], ArPoints[CountPoints]);
                        DrawBezie(new Pen(DrawPen.Color, 1), ArPoints, CountPoints);
                        CountPoints = 0;
                    }
                       
                       
                    
                }
                else
                {
                    g.DrawEllipse(DrawPen, e.X - 2, e.Y - 2, 5, 5);
                    if (CountPoints > 0)
                        g.DrawLine(new Pen(Color.Magenta, 1),
                       ArPoints[CountPoints - 1], ArPoints[CountPoints]);
                    CountPoints++;
                }
            }

        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
           
        }
    }
}
