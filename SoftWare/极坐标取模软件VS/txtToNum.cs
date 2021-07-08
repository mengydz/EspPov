using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class txtToNum
    {
        public static int toInt(string s)
        {
            try
            {
                int r=int.Parse(s);
                return r;
            }
            catch
            {
                MessageBox.Show("请输入数字","提示");
            }
            return -1;
        }
        static double toDouble(string s)
        {
            try
            {
                double r = double.Parse(s);
                return r;
            }
            catch
            {
                MessageBox.Show("请输入数字", "提示");
            } 
            return -1;
        }
    }
}
