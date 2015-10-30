
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        private const int MAX = 256;      // max iterations
        private const double SX = -2.025; // start value real
        private const double SY = -1.125; // start value imaginary
        private const double EX = 0.6;    // end value real
        private const double EY = 1.125;  // end value imaginary
        private static int x1, y1, xs, ys, xe, ye;
        private static double xstart, ystart, xende, yende, xzoom, yzoom;
        private static bool action, rectangle, finished;
        private static float xy;
        private Image picture;
        private Graphics g1;


        private Bitmap myBitmap;

        public static class HSB
        {//djm added, it makes it simpler to have this code in here than in the C#
            public static double rChan = 0, gChan = 0, bChan = 0;

            public static void fromHSB(float h, float s, float b)
            {
                float red = b;
                float green = b;
                float blue = b;
                if (s != 0)
                {
                    float max = b;
                    float dif = b * s / 255f;
                    float min = b - dif;

                    float h2 = h * 360f / 255f;

                    if (h2 < 60f)
                    {
                        red = max;
                        green = h2 * dif / 60f + min;
                        blue = min;
                    }
                    else if (h2 < 120f)
                    {
                        red = -(h2 - 120f) * dif / 60f + min;
                        green = max;
                        blue = min;
                    }
                    else if (h2 < 180f)
                    {
                        red = min;
                        green = max;
                        blue = (h2 - 120f) * dif / 60f + min;
                    }
                    else if (h2 < 240f)
                    {
                        red = min;
                        green = -(h2 - 240f) * dif / 60f + min;
                        blue = max;
                    }
                    else if (h2 < 300f)
                    {
                        red = (h2 - 240f) * dif / 60f + min;
                        green = min;
                        blue = max;
                    }
                    else if (h2 <= 360f)
                    {
                        red = max;
                        green = min;
                        blue = -(h2 - 360f) * dif / 60 + min;
                    }
                    else
                    {
                        red = 0;
                        green = 0;
                        blue = 0;
                    }
                }

                rChan = Math.Round(Math.Min(Math.Max(red, 0f), 255));
                gChan = Math.Round(Math.Min(Math.Max(green, 0), 255));
                bChan = Math.Round(Math.Min(Math.Max(blue, 0), 255));

            }

        }


        public void init() // all instances will be prepared
        {
            //HSBcol = new HSB();
            //finished = false;
            //addMouseListener(this);
            //addMouseMotionListener(this);
            //c1 = new Cursor(Cursor.WAIT_CURSOR);
            //c2 = new Cursor(Cursor.CROSSHAIR_CURSOR);
            x1 = this.Width;
            y1 = this.Height;
            xy = (float)x1 / (float)y1;

            myBitmap = new Bitmap(x1, y1);
            g1 = Graphics.FromImage(myBitmap);
            finished = true;
        }

        public void start()
        {
            action = false;
            rectangle = false;
            initvalues();
            xzoom = (xende - xstart) / (double)x1;
            yzoom = (yende - ystart) / (double)y1;
            mandelbrot();
        }

        private void initvalues() // reset start values
        {
            xstart = SX;
            ystart = SY;
            xende = EX;
            yende = EY;
            if ((float)((xende - xstart) / (yende - ystart)) != xy)
                xstart = xende - (yende - ystart) * (double)xy;
        }

        private void Fractal_Paint(object sender, PaintEventArgs e)
        {
            update(g1);
        }

        public void update(Graphics g1)
        {
            Pen blackPen = new Pen(Color.Black, 3);

            g1.DrawImage(myBitmap, 0, 0);
            if (rectangle)
            {
                //g.setColor(Color.White);
                if (xs < xe)
                {
                    if (ys < ye) g1.DrawRectangle(blackPen, xs, ys, (xe - xs), (ye - ys));
                    else g1.DrawRectangle(blackPen, xs, ye, (xe - xs), (ys - ye));
                }
                else
                {
                    if (ys < ye) g1.DrawRectangle(blackPen, xe, ys, (xs - xe), (ye - ys));
                    else g1.DrawRectangle(blackPen, xe, ye, (xs - xe), (ys - ye));
                }
            }
        }

        private void mandelbrot() // calculate all points
        {
            Pen whitePen = new Pen(Color.White, 3);

            Pen tempPen = null;
            Color col;
            int x, y;
            float h, b, alt = 0.0f;

            action = false;
            Cursor.Current = Cursors.WaitCursor; //setCursor(c1);
            //showStatus("Mandelbrot-Set will be produced - please wait...");
            for (x = 0; x < x1; x += 2)
                for (y = 0; y < y1; y++)
                {
                    h = pointcolour(xstart + xzoom * (double)x, ystart + yzoom * (double)y); // color value
                    if (h != alt)
                    {
                        b = 1.0f - h * h; // brightnes

                        HSB.fromHSB(h, 0.8f, b);

                        col = Color.FromArgb(0, Convert.ToInt32(HSB.rChan), Convert.ToInt32(HSB.gChan), Convert.ToInt32(HSB.bChan));

                        tempPen = new Pen(col, 3);


                        ///djm added
                        ///HSBcol.fromHSB(h,0.8f,b); //convert hsb to rgb then make a Java Color
                        ///Color col = new Color(0,HSBcol.rChan,HSBcol.gChan,HSBcol.bChan);
                        ///g1.setColor(col);
                        //djm end
                        //djm added to convert to RGB from HSB

                        //g1.setColor(HSBtoRGB(h, 0.8f, b));
                        //djm test

                        //Color col = HSBtoRGB(h, 0.8f, b);
                        //int red = col.R; //.getRed();
                        //int green = col.G; //.getGreen();
                        //int blue = col.B; //.getBlue();
                        //djm 
                        alt = h;
                    }
                    g1.DrawLine(tempPen, x, y, x + 1, y);
                }
            //showStatus("Mandelbrot-Set ready - please select zoom area with pressed mouse.");
            Cursor.Current = Cursors.Cross; //setCursor(c2);
            action = true;
        }

        private float pointcolour(double xwert, double ywert) // color value from 0.0 to 1.0 by iterations
        {
            double r = 0.0, i = 0.0, m = 0.0;
            int j = 0;

            while ((j < MAX) && (m < 4.0))
            {
                j++;
                m = r * r - i * i;
                i = 2.0 * r * i + ywert;
                r = m + xwert;
            }
            return (float)j / (float)MAX;
        }

        private void Fractal_MouseDown(object sender, MouseEventArgs e)
        {

            if (action)
            {
                xs = e.X;
                ys = e.Y;
            }
        }

        private void Fractal_MouseUp(object sender, MouseEventArgs e)
        {
            int z, w;


            if (action)
            {
                xe = e.X;
                ye = e.Y;
                if (xs > xe)
                {
                    z = xs;
                    xs = xe;
                    xe = z;
                }
                if (ys > ye)
                {
                    z = ys;
                    ys = ye;
                    ye = z;
                }
                w = (xe - xs);
                z = (ye - ys);
                if ((w < 2) && (z < 2)) initvalues();
                else
                {
                    if (((float)w > (float)z * xy)) ye = (int)((float)ys + (float)w / xy);
                    else xe = (int)((float)xs + (float)z * xy);
                    xende = xstart + xzoom * (double)xe;
                    yende = ystart + yzoom * (double)ye;
                    xstart += xzoom * (double)xs;
                    ystart += yzoom * (double)ys;
                }
                xzoom = (xende - xstart) / (double)x1;
                yzoom = (yende - ystart) / (double)y1;
                mandelbrot();
                rectangle = false;
                Invalidate();
            }
        }



        private void Fractal_MouseMove(object sender, MouseEventArgs e)
        {
            //e.consume();
            if (action)
            {
                xe = e.X;
                ye = e.Y;
                rectangle = true;
                Invalidate();
            }
        }

        public Form1()
        {

            InitializeComponent();
        }
    }
}
