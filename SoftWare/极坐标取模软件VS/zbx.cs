using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class zbx
    {
        public int w, h;
        public double rateD;//距离缩放,坐标系每格的大小
        public double rateA;//角度缩放,每块扇形的角度(弧度制)
        public int Fx;
        public Point center;
        Graphics g;
        PictureBox pictureBox;
        public Bitmap bmp;
        
        public int N;   //分辨率，一圈的扇形的数量
        public int OutSize;
        public int InSize;
        public int Size;
        public zbx(PictureBox pictureBox,int Size,int InSize,int N,int Fx)//OutSize,InSize单位:格
        {
            this.pictureBox = pictureBox;
            bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
            g = Graphics.FromImage(bmp);
            w = pictureBox.Width;
            h = pictureBox.Height;
            this.Fx = Fx;
            this.Size = Size;
            this.OutSize= Size+ InSize;
            this.InSize= InSize;
            this.N = N;
            center = new Point(OutSize, OutSize);
            rateD = Math.Min(w, h) / 2.0 / OutSize;
            rateA = 2 * Math.PI / N * Fx;  // (弧度制)
            g.Clear(Color.White);

        }
        public void setSize(PictureBox pictureBox, int Size, int InSize, int N, int Fx)
        {
            this.pictureBox = pictureBox;
            bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
            g = Graphics.FromImage(bmp);
            w = pictureBox.Width;
            h = pictureBox.Height;
            this.Fx = Fx;
            this.Size = Size;
            this.OutSize = Size + InSize;
            this.InSize = InSize;
            this.N = N;

            center = new Point(OutSize, OutSize);
            rateD = Math.Min(w, h) / 2.0 / OutSize;
            rateA = 2 * Math.PI / N * Fx;  // (弧度制)
            g.Clear(Color.White);

        }
        public void setPictureBox(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
            pictureBox.Width = w;
            pictureBox.Height = h;
            bmp = new Bitmap(pictureBox.Image,w,h);
            g = Graphics.FromImage(bmp);
        }
        public void setFx(int f)//反正极坐标方向
        {
            Fx = f;
            rateA = 2 * Math.PI / N * Fx;  // (弧度制)
        }
        public void Clear()//清除图像
        {
            g.Clear(Color.White);
        }   
        void DrawCircle( Color c, Point o, int r) //画圆，颜色，坐标，半径
        {
            int x = (int)(o.X * rateD);
            int y = (int)(o.Y * rateD);
            r = (int)(r * rateD);
            g.DrawEllipse(new Pen(c), x - r, y - r, 2 * r, 2 * r);
            
            pictureBox.Image = bmp;
        }
        public void DrawPoint(Color c,int d, int n)//d是到极点的距离，n是角度
        {

            double r = (d * rateD);
            double a = n%N * rateA;
            double centerX = center.X* rateD;
            double centerY = center.Y* rateD;
            
            float x = (float)(r * Math.Cos(a)+centerX);
            float y = (float)(r * Math.Sin(a)+centerY);

            float p = (float)(Math.Abs( 2*r * Math.Sin(rateA / 2.0)));

            float cr = (float)(Math.Min(p/2.0, rateD/2.0));
            g.DrawEllipse(new Pen(c), x-cr, y-cr, cr*2, cr*2);
  
            pictureBox.Image = bmp;
        }
        public Color GetPointColor(int d, int n)//获取坐标上的颜色，d是到极点的距离，n是角度
        {

            double r = (d * rateD);
            double a = n % N * rateA;
            double centerX = center.X * rateD;
            double centerY = center.Y * rateD;

            int x = (int)(r * Math.Cos(a) + centerX);
            int y = (int)(r * Math.Sin(a) + centerY);
           
            return bmp.GetPixel(x, y);
            
        }
        public void CreateRectangularCoordinates()//创建直角坐标系
        {
           
            float b = (float)rateD;
            for (float i = 0; i <= w; i+=b)
            {
                g.DrawLine(new Pen(Color.Green,1), 0,i, (int) (rateD* OutSize*2), i);
            }
            for (float i = 0; i <= h; i += b)
            {
                g.DrawLine(new Pen(Color.Green,1), i, 0, i, (int) (rateD* OutSize*2));
            }

            g.DrawLine(new Pen(Color.Blue, 1), (int)(rateD * OutSize), 0, (int)(rateD * OutSize), (int)(rateD * OutSize * 2));
            g.DrawLine(new Pen(Color.Blue, 1), 0, (int)(rateD * OutSize), (int)(rateD * OutSize * 2), (int)(rateD * OutSize));
            pictureBox.Image = bmp;
        }
        public void CreatePolarCoordinates()//创建极坐标系
        {
           
            double centerX = center.X * rateD;
            double centerY = center.Y * rateD;
            double x1, y1,x2,y2;
            for (double i = 0; i < N; i++)
            {
                x1 = (rateD * OutSize) * Math.Cos(i * rateA);
                y1 = (rateD * OutSize) * Math.Sin(i * rateA);

                x2 = (rateD * InSize) * Math.Cos(i * rateA);
                y2 = (rateD * InSize) * Math.Sin(i * rateA);
                if(i==0)
                    g.DrawLine(new Pen(Color.Red, 1), (int)(x1 + centerX), (int)(y1 + centerY), (int)(centerX + x2), (int)(+centerY+ y2));
                else
                    g.DrawLine(new Pen(Color.Blue, 1), (int)(x1 + centerX), (int)(y1 + centerY), (int)(centerX + x2), (int)(+centerY + y2));
            }

            for (int i = 1; i <=OutSize ; i++)
            {
                DrawCircle( i<InSize? Color.Yellow:Color.Blue ,center, i);
            }
            pictureBox.Image = bmp;
        }

    }
}
