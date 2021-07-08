using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        bool[,] PointData;  //坐标轴上的“点”数据
        int md, mn, mx, my; //鼠标位置d、n、x、y
        int w, h;       //pictureBox宽高
        int fx = 1;     //极坐标的方向  1:顺时针方向；-1:逆时针方向
        Point center;   //中心原点坐标Point(w/2,h/2)，是相对pictureBox的中心
        int fd = 0;     //缩放等级-3~3
        double rateD;   //单位距离与pictureBox实际大小的放大率
        zbx myzbx;      //坐标系对象
        private string pathname = string.Empty;   //定义图片路径名变量
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            int minsize = Math.Min(h, w);
            center = new Point(minsize / 2, minsize / 2);

            comboBoxSize.Text = "16";
            comboBoxSc.Text = "8";
            pictureBox1.BackColor = Color.White;
            int size, isize, n;
            size = txtToNum.toInt(comboBoxSize.Text);
            isize = txtToNum.toInt(textInSize.Text);
            n = txtToNum.toInt(textfbl.Text);
            myzbx = new zbx(pictureBox1, isize + size, isize, n, 1);
            createCoordinates();

            comboBoxFx.Text = "顺时针";//comboBoxFx包含SelectedIndexChanged事件，必须放在最后
            comboBoxzbz.Text = "极坐标";//comboBoxzbz包含SelectedIndexChanged事件，必须放在最后
            showAppInfo();
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)//在当前坐标系上画个一坐标点
        {
            int sub_d;
            if (md < myzbx.OutSize && md >= myzbx.InSize)
            {
                sub_d = md - myzbx.InSize;
                if (e.Button == MouseButtons.Left)
                {
                    myzbx.DrawPoint(Color.Red, md, mn);
                    PointData[sub_d, mn] = true;
                }
                if (e.Button == MouseButtons.Right)
                {
                    myzbx.DrawPoint(Color.White, md, mn);
                    PointData[sub_d, mn] = false;
                }
            }



        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)//鼠标在pictureBox1移动时 显示实时坐标信息 若按下则在坐标系上画点
        {

            double temp;
            md = (int)(Math.Sqrt((e.X - center.X) * (e.X - center.X) + (e.Y - center.Y) * (e.Y - center.Y)) / rateD + 0.5);
            temp = Math.Atan2((e.Y - center.Y), (e.X - center.X)) / myzbx.rateA;

            mn = (int)(temp > 0 ? temp + 0.5 : temp - 0.5);
            if (mn < 0) mn = myzbx.N + mn;

            temp = (e.X - center.X) / rateD;
            mx = (int)((temp > 0 ? temp + 0.5 : temp - 0.5));
            temp = (center.Y - e.Y) / rateD;
            my = (int)((temp > 0 ? temp + 0.5 : temp - 0.5));
            textBox1.Text = md.ToString();
            textBox2.Text = mn.ToString();
            textBox5.Text = mx.ToString();
            textBox6.Text = my.ToString();

            int sub_d;
            if (md < myzbx.OutSize && md >= myzbx.InSize)
            {
                sub_d = md - myzbx.InSize;
                if (e.Button == MouseButtons.Left)
                {
                    myzbx.DrawPoint(Color.Red, md, mn);
                    PointData[sub_d, mn] = true;
                }
                if (e.Button == MouseButtons.Right)
                {
                    myzbx.DrawPoint(Color.White, md, mn);
                    PointData[sub_d, mn] = false;
                }
            }

        }
        private void zoom()
        {
            
            pictureBox1.Width = (int)(myzbx.w * Math.Pow(2, fd));
            pictureBox1.Height = (int)(myzbx.h * Math.Pow(2, fd));
            w = pictureBox1.Width;
            h = pictureBox1.Height;
            center = new Point(w / 2, h / 2);
            rateD = myzbx.rateD * Math.Pow(2, fd);
        }
        private void button1_Click(object sender, EventArgs e)//放大
        {
            button4.Enabled = true;
            ++fd;
            zoom();
            if (fd == 3)button1.Enabled = false;
            
        }
        private void button4_Click(object sender, EventArgs e)//缩小
        {
            button1.Enabled = true;
            --fd;
            zoom();
            if ( fd== -3) button4.Enabled = false;
      
            



        }
        private void button3_Click(object sender, EventArgs e)//将数据输出到文本框
        {
            string t="";
            int a = txtToNum.toInt(comboBoxSc.Text)/2;
            byte data = 0;
            for (int count=0,i = myzbx.Fx==1? 0: (myzbx.N-1);  myzbx.Fx==1? (i<myzbx.N):(i>=0);  i=i+ myzbx.Fx)
            {
                if (count++ % a == 0) t += "\r\n";
                for (int j = 0; j < myzbx.Size/8; j++)
                {
                    data = 0;
                    for (int k = 0; k < 8; k++)
                    {
                        if (PointData[j*8 + k,i]) data |= (byte)(0x01 << k);
                    }
                    t += "0x" + data.ToString("X2")+",";
                }
                
            }
            if (t.Length != 0) t = t.Substring(0, t.Length - 1);//去除最后一个逗号
            textBox3.Text = "//方向：" + (fx == 1 ? "顺时针" : "逆时针") + "旋转\r\n";
            textBox3.Text += "//分辨率：" +myzbx.Size+"点*"+myzbx.N+ "帧\r\n";
            textBox3.Text += "//内径：" + myzbx.InSize +"点\r\n";
            textBox3.Text += "//共" + myzbx.N * myzbx.Size / 8 + "字节\r\n";
            textBox3.Text += t;
            
             
        }
        private void button5_Click(object sender, EventArgs e)//打开图片
        {
            OpenFileDialog file = new OpenFileDialog();
            file.InitialDirectory = ".";
            file.Filter = "图片|*.jpg;*.png;*.jpeg;*.bmp";;
            file.ShowDialog();
            if (file.FileName != string.Empty)
            {
                try
                {
                    fd = 0;
                    zoom();
                    pathname = file.FileName;   //获得文件的绝对路径
                    this.pictureBox1.Load(pathname);
                    myzbx.setPictureBox(pictureBox1);   
                    rePaintCoordinates();
                    rePaintPoint();
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    pathname = "";
                }
            }

        }
        private void rePaintPoint() //重新画点
        {
            for (int i = 0; i < myzbx.Size; i++) 
            {
                for (int j = 0; j < myzbx.N; j++)
                    if (PointData[i, j])
                        myzbx.DrawPoint(Color.Red, i + myzbx.InSize, j);
            }
        }
        private void button7_Click(object sender, EventArgs e)//清除图像
        {
            pathname = "";
            myzbx.Clear();
            rePaintCoordinates();
            rePaintPoint();
        }
        private void rePaintCoordinates() //重新画坐标系
        {
            if (comboBoxzbz.SelectedIndex == 0)
            {
                myzbx.CreatePolarCoordinates();
            }
            else if (comboBoxzbz.SelectedIndex == 1)
            {
                myzbx.CreateRectangularCoordinates();
            }
            else
            {
                myzbx.CreatePolarCoordinates();
                myzbx.CreateRectangularCoordinates();
            }
        }
        private void button6_Click(object sender, EventArgs e)//取模
        {
            if (pathname == "")
            {
                MessageBox.Show("请先打开图片!", "提示");
                return;
            }
            fd = 0;
            zoom();
            myzbx.Clear();
            this.pictureBox1.Load(pathname);
            myzbx.setPictureBox(pictureBox1);
            Color c;
            int gray;
            for (int i=0;i<myzbx.N;i++)
            { 
                for (int j = 0; j < myzbx.Size; j++)
                {
                    c = myzbx.GetPointColor(j + myzbx.InSize, i);
                    gray = (c.R + c.G + c.B)/3;
                    if (gray<128)
                    {
                        Console.WriteLine("d=" + j + ",n=" + i + ",gray=" + gray);
                        
                        PointData[j, i] = true;
                    }else
                        PointData[j, i] = false;
                }
            }
            rePaintCoordinates();
            rePaintPoint();
            MessageBox.Show("完成!", "提示");
        }
        private void comboBoxFx_SelectedIndexChanged(object sender, EventArgs e)//顺时针 或 逆时针 改变状态触发
        {
            if (comboBoxFx.Text == "顺时针")
            {
                myzbx.setFx(1);
                fx = 1;
            }
            else
            {
                myzbx.setFx(-1);
                fx = -1;
            }
        }
        private void comboBoxzbz_SelectedIndexChanged(object sender, EventArgs e)//坐标系的选择 重新创建坐标系和画点
        {
            
            myzbx.Clear();
            if (pathname != "")
            {
                
                fd = 0;
                zoom();
                this.pictureBox1.Load(pathname);
                myzbx.setPictureBox(pictureBox1);
            }
            if (comboBoxzbz.SelectedIndex == 0)
            {
                myzbx.CreatePolarCoordinates();
            }
            else if (comboBoxzbz.SelectedIndex == 1)
            {
                myzbx.CreateRectangularCoordinates();
            }
            else
            {
                myzbx.CreatePolarCoordinates();
                myzbx.CreateRectangularCoordinates();
            }
            rePaintPoint();

        }
        private void createCoordinates()//根据设置的参数创建坐标系
        {
            int size, isize, n;
            size = txtToNum.toInt(comboBoxSize.Text);
            isize = txtToNum.toInt(textInSize.Text);
            textInSize.Text = isize.ToString();
            n = txtToNum.toInt(textfbl.Text);
            if (n < 10 || n>400)
            {
                textfbl.Text = "100"; n = 100;
                MessageBox.Show("分辨率为10-400以内的数字");
            }
            if (isize < 0 || isize>20)
            {
                textInSize.Text = "0";
                isize = 0;
                MessageBox.Show("内半径为0-20以内的数字");
            }
            
            if (comboBoxFx.Text == "逆时针") fx = -1;
            else fx = 1;
            pictureBox1.Width = (size + isize) * 50;
            pictureBox1.Height = (size + isize) * 50;

            myzbx.setSize(pictureBox1, size, isize, n, fx);

            w = pictureBox1.Width;
            h = pictureBox1.Height;

            center = new Point(w / 2, h / 2);
            fd = 0;
            rateD = myzbx.rateD * Math.Pow(2, fd);
            
            PointData = new bool[size, n];

            myzbx.Clear();
            if (comboBoxzbz.SelectedIndex == 0)
            {
                myzbx.CreatePolarCoordinates();
            }
            else if (comboBoxzbz.SelectedIndex == 1)
            {
                myzbx.CreateRectangularCoordinates();
            }
            else
            {
                myzbx.CreatePolarCoordinates();
                myzbx.CreateRectangularCoordinates();
            }
        }
        private void button2_Click(object sender, EventArgs e)//重新设置参数的确定按钮
        {
            if (MessageBox.Show("改变参数所有数据会被清空", "提示", MessageBoxButtons.OKCancel) != DialogResult.OK) return;
            else createCoordinates();
            showAppInfo();
        }
        private void showAppInfo()//显示app信息
        {
            textBox3.Text = "一、使用方法\r\n" +
                            "  1.设置参数。\r\n" +
                            "  2.打开图片点击取模或直接在左侧区域绘点。\r\n" +
                            "  3.生成数据。\r\n\r\n"+
                            "二、说明\r\n" +
                            "  1.【设置】设置LED数量、圈内半径、分辨率，按下确定后将清空左侧原有的数据。\r\n" +
                            "  2.【视图】放大和缩小区域大小、改变要显示的坐标轴样式、改变极坐标轴的方向。\r\n" +
                            "  3.【图像】打开图片、移除图片、对图片取模，按RGB平均亮度大于128取亮点。\r\n" +
                            "  4.【输出】可设置每行要显示的字节数、导出所有数据。从圈内到圈外，高字节在外侧。\r\n\r\n" +
                            "作者：张李浩\r\n邮箱：LiHooo@qq.com\r\n日期：2018年8月";
        }
    }
}
