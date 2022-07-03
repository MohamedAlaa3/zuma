using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace Zuma
{
    class CMyLine
    {
        public float xs, ys, xe, ye, dx, dy, m, invM, currX, currY;
        int Speed = 20;

        public void SetVals(float a, float b, float c, float d)
        {
            xs = a;
            ys = b;
            xe = c;
            ye = d;
            //////////////////
            dx = xe - xs;
            dy = ye - ys;
            m = dy / dx;
            invM = dx / dy;
            /////////////////
            currX = xs;
            currY = ys;
        }


        public void MoveStep()
        {
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                if (xs < xe)
                {
                    currX += Speed;
                    currY += m * Speed;
                    if (currX >= xe)
                    {
                        //SetVals(xe, ye, xs, ys);
                    }
                }
                else
                {
                    currX -= Speed;
                    currY -= m * Speed;
                    if (currX <= xe)
                    {
                        //SetVals(xe, ye, xs, ys);
                    }
                }
            }
            else
            {
                if (ys < ye)
                {
                    currY += Speed;
                    currX += invM * Speed;
                    if (currY >= ye)
                    {
                        //SetVals(xe, ye, xs, ys);
                    }
                }
                else
                {
                    currY -= Speed;
                    currX -= invM * Speed;
                    if (currY <= ye)
                    {
                        //SetVals(xe, ye, xs, ys);
                    }
                }
            }
        }

        public void DrawYourCurrPos(Graphics g)
        {
            g.FillEllipse(Brushes.Yellow, currX - 5, currY - 5, 10, 10);
            g.DrawLine(Pens.Yellow, xs, ys, xe, ye);
        }
    }

    public class ball
    {
        public PointF mypoint;
        public Bitmap img;
        public int myimg,col;
        public float curve;

    }
    class frog
    {
        public float X, Y;
        public Bitmap img;
        public ball myball;
    }
    public partial class Form1 : Form
    {
        Bitmap off;
        Timer t = new Timer();
        Bitmap background = new Bitmap("background.jpg");
        BezierCurve obj = new BezierCurve();
        int tick = 0;
        Random r=new Random();
        List<ball> allballs = new List<ball>();
        List<ball> shooted = new List<ball>();
        List<CMyLine> lines = new List<CMyLine>();
        frog myfrog = new frog();
        double mouseX, mouseY;
        float moveball = .008f;
        int add = -1;
        bool added = false;
        public Form1()
        {
            this.WindowState = FormWindowState.Maximized;
            this.Paint += Form1_Paint;
            this.Load += Form1_Load;
            this.MouseDown += Form1_MouseDown;
            this.MouseMove += Form1_MouseMove;
            t.Tick += T_Tick;
            t.Interval = 150;
            t.Start();

        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            CMyLine line = new CMyLine();
            line.SetVals((myfrog.X+myfrog.img.Width/2)-25, myfrog.Y, e.X, e.Y);
            
      ;

            lines.Add(line);
            myfrog.myball.mypoint.X = line.currX;
            myfrog.myball.mypoint.Y = line.currY;
            shooted.Add(myfrog.myball);

            ball pnn = new ball();
            int a = r.Next(1, 7);
            pnn.img = new Bitmap(a + "ball.gif");
            pnn.img.MakeTransparent();
            pnn.curve = 0f;
            pnn.mypoint = obj.CalcCurvePointAtTime(pnn.curve);
            pnn.myimg = 0;
            pnn.col = a;
            myfrog.myball = pnn;
            pnn.mypoint = new Point(150, 320);

        }
        int ct22 = 0;
        void moveballs(int min)
        {
            
            for (int i = min; i < allballs.Count; i++)
            {
                
                    //move
                    if (allballs[i].curve == 0 && i != 24)
                    {
                        allballs[i].curve += .008f;
                    }
                    else
                    {
                        if (allballs.Count > 24||ct22>0)
                        {
                             ct22++;
                            allballs[i].curve += .001f;
                        }
                        else
                        {
                            allballs[i].curve += moveball;

                        }
                    }
                    allballs[i].mypoint = obj.CalcCurvePointAtTime(allballs[i].curve);
                    // allballs[i].myimg++;
                    if (allballs[i].myimg == 47)
                    {
                        allballs[i].myimg = 0;
                    }
                
            }
        }
        private void T_Tick(object sender, EventArgs e)
        {
           
            if (tick<25)
            {
                moveballs(0);
                ball pnn = new ball();
                int a = r.Next(1, 7);
                pnn.img = new Bitmap(a+"ball.gif");
                pnn.img.MakeTransparent();
                pnn.curve = 0f;
                pnn.mypoint = obj.CalcCurvePointAtTime(pnn.curve);
                pnn.myimg = 0;
                pnn.col = a;

                allballs.Add(pnn);


            }
            else
            {
               
                    moveballs(0);
                
            }
            //move
            for (int i = 0; i < shooted.Count(); i++)
            {
                lines[i].MoveStep();
                shooted[i].mypoint.X = lines[i].currX;
                shooted[i].mypoint.Y = lines[i].currY;
                for (int j = 0; j < allballs.Count(); j++)
                {
                    added = false;
                    if (i < shooted.Count())
                    {
                        if (shooted[i].mypoint.X >= allballs[j].mypoint.X - 25 &&
                            shooted[i].mypoint.X <= allballs[j].mypoint.X + 25 &&
                            shooted[i].mypoint.Y >= allballs[j].mypoint.Y - 25 &&
                            shooted[i].mypoint.Y <= allballs[j].mypoint.Y + 25)
                        {
                            added = true;
                            shooted[i].curve = allballs[j].curve;
                            allballs.Insert(j, shooted[i]);
                            shooted.Remove(shooted[i]);
                            lines.Remove(lines[i]);

                            for (int k = j ; k >=0; k--)
                            {
                                allballs[k].curve += moveball;
                                allballs[k].mypoint = obj.CalcCurvePointAtTime(allballs[k].curve);

                            }
                            add = j;
                            break;
                            //MessageBox.Show("1");
                        }
                    }
                }
            }
         

            DrawDubb(CreateGraphics());
            DrawDubb(CreateGraphics());

            tick++;
            if (added == true)
            {
                int min = 999999999;
                

                List<ball> rballs = new List<ball>();
                int ct = 1;
                for (int i = add+1; i < allballs.Count; i++)
                {
                    if (allballs[i].col == allballs[add].col)
                    {
                        if (min > i)
                        {
                            min = i;
                            rballs.Add(allballs[i]);
                            ct++;
                        }
                        
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = add -1; i > -1; i--)
                {
                    if (allballs[i].col == allballs[add].col)
                    {
                        if (min > i)
                        {
                            min = i;
                            rballs.Add(allballs[i]);
                            ct++;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                rballs.Add(allballs[add]);
                if(ct>=3)
                {
                    if (min > add)
                    {
                        min = add;
                      
                    }
                    for (int m = 0; m < 3; m++)
                    {


                        for (int k = min; k <allballs.Count(); k++)
                        {
                            allballs[k].curve += moveball;
                            allballs[k].mypoint = obj.CalcCurvePointAtTime(allballs[k].curve);

                        }
                    }


                    //MessageBox.Show("d");
                    for (int i = 0; i < rballs.Count(); i++)
                    {
                        allballs.Remove(rballs[i]);
                        
                    }

                    DrawDubb(CreateGraphics());



                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            off = new Bitmap(ClientSize.Width, ClientSize.Height);
            MakePath();
            myfrog.X = 625;
            myfrog.Y = 310;
            myfrog.img = new Bitmap("frog.png");
            myfrog.img.MakeTransparent(myfrog.img.GetPixel(0, 0));

            ball pnn = new ball();
            int a = r.Next(1, 7);
            pnn.img = new Bitmap(a + "ball.gif");
            pnn.img.MakeTransparent();
            pnn.curve = 0f;
            pnn.mypoint = obj.CalcCurvePointAtTime(pnn.curve);
            pnn.myimg = 0;
            pnn.col = a;
            myfrog.myball = pnn;
            pnn.mypoint=new Point(150,320);


        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDubb(e.Graphics);
        }
        List<Point> ReadPoints()
        {


            List<Point> points = new List<Point>();


            string[] readText = File.ReadAllLines("points.txt");
            int[] values = new int[readText.Length / 2];
            int ct = 0;
            foreach (string s in readText)
            {
                if (s != ",")
                {
                    values[ct] = Int32.Parse(s);
                    ct++;
                }


            }
            int j = 1;
            for (int i = 0; i < ct - 1; i += 2)
            {
                Point pn = new Point(values[i], values[j]);

                j += 2;

                points.Add(pn);

            }

            return points;

        }

        void MakePath()
        {
            List<Point> points = ReadPoints();

            for (int i = 0; i < points.Count; i++)
            {
                obj.SetControlPoint(points[i]);


            }


        }
        public Bitmap Rotateimg(Bitmap b, float angle)
        {
            int maxside = (int)(Math.Sqrt(b.Width * b.Width + b.Height * b.Height));

            //create a new empty bitmap to hold rotated image

            Bitmap returnBitmap = new Bitmap(maxside, maxside);

            //make a graphics object from the empty bitmap

            Graphics g = Graphics.FromImage(returnBitmap);





            //move rotation point to center of image

            g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);

            //rotate

            g.RotateTransform(angle);

            //move image back

            g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);

            //draw passed in image onto graphics object

            g.DrawImage(b, new Point(0, 0));



            return returnBitmap;
        }

        void DrawScene(Graphics g)
        {
            g.Clear(Color.Black);

            Rectangle rcDest = new Rectangle(0, 0, 1392,790);
            Rectangle rcSrc = new Rectangle(0, 0,background.Width, background.Height);
            g.DrawImage(background,rcDest, rcSrc,GraphicsUnit.Pixel);
            for (int i = 0; i < allballs.Count; i++)
            {
                Rectangle rcSrc1 = new Rectangle(0, allballs[i].myimg*(allballs[i].img.Height / 56), allballs[i].img.Width, allballs[i].img.Height/49);
                Rectangle rcDest1 = new Rectangle(((int)allballs[i].mypoint.X-25), ((int)allballs[i].mypoint.Y-25), 50, 50);
                g.DrawImage(allballs[i].img, rcDest1, rcSrc1, GraphicsUnit.Pixel);
            }
            for (int i = 0; i < shooted.Count(); i++)
            {
                Rectangle rcSrc1 = new Rectangle(0, shooted[i].myimg * (shooted[i].img.Height / 56), shooted[i].img.Width, shooted[i].img.Height / 49);
                Rectangle rcDest1 = new Rectangle(((int)shooted[i].mypoint.X - 25), ((int)shooted[i].mypoint.Y - 25), 50, 50);
                g.DrawImage(shooted[i].img, rcDest1, rcSrc1, GraphicsUnit.Pixel);
            }
            // obj.DrawCurve(g);
            // g.DrawImage(myfrog.img, myfrog.X, myfrog.Y);
            int dx = (int) (mouseX - myfrog.X);
            int dy = (int)(mouseY - myfrog.Y);

            double rad = Math.Atan2(dy, dx);
           double deg = (rad * 180) / Math.PI;

             Bitmap rfrog = Rotateimg(myfrog.img, (float)deg - 90);
            rfrog.MakeTransparent(rfrog.GetPixel(0, 0));




            //Bitmap rfrog = Rotateimg(myfrog.img, (float)(((Math.Atan2(mouseY - (myfrog.Y + myfrog.img.Height), mouseX - ((myfrog.X + myfrog.img.Width) / 1))) * 180) / Math.PI));
            Rectangle rcSrc2 = new Rectangle(0, myfrog.myball.myimg * (myfrog.myball.img.Height / 56), myfrog.myball.img.Width, myfrog.myball.img.Height / 49);
            Rectangle rcDest2 = new Rectangle(((int)myfrog.myball.mypoint.X - 25), ((int)myfrog.myball.mypoint.Y - 25), 50, 50);
            g.DrawImage(myfrog.myball.img, rcDest2, rcSrc2, GraphicsUnit.Pixel);
            g.DrawImage(rfrog, myfrog.X, myfrog.Y);





        }
        void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
    }
}
