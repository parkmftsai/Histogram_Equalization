using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Histogram_Equalization
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Bitmap show_picture;
            button1.Enabled = false;
            button2.Enabled = false;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Bitmap show_picture;

        }

        private void button1_Browse(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            button2.Enabled = false;

            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png; *.tif)|*.jpg; *.jpeg; *.gif; *.bmp; *.png; *.tif";


            if (open.ShowDialog() == DialogResult.OK)
            {
                /*相關參數設定*/
                Bitmap tempbitmap = new Bitmap(open.FileName);
                Bitmap input = new Bitmap(tempbitmap.Width / 2, tempbitmap.Height / 2);
                Bitmap output = new Bitmap(250, 250);

                int max = 0;
                int[] number = new int[256];

                double[] p = new double[256];

                int i, j, x, y;
                //縮小
                int w = (int)(tempbitmap.Width / 2);
                int h = (int)(tempbitmap.Height / 2);
                if (tempbitmap.Height > 256 && tempbitmap.Width > 256)
                {
                    for (j = 0; j < tempbitmap.Height / 2; j++)
                        for (i = 0; i < tempbitmap.Width / 2; i++)
                        {

                            y = (int)(j * 2 + 0.5); x = (int)(i * 2 + 0.5); //四捨五入

                            if (x >= tempbitmap.Width)
                                x = tempbitmap.Width + (tempbitmap.Width - x - 1);
                            if (y >= tempbitmap.Height)
                                y = tempbitmap.Height + (tempbitmap.Height - y - 1);
                            Color getpixel = tempbitmap.GetPixel(x, y);
                            if (i >= w)
                                i = w + (w - i - 1);
                            if (j >= h)
                                j = h + (h - j - 1);
                            input.SetPixel(i, j, Color.FromArgb(getpixel.R, getpixel.G, getpixel.B));

                        }
                    pictureBox1.Image = input;
                }
                else
                {
                    pictureBox1.Image = tempbitmap;
                }

                /*繪製input直方圖*/
                for (i = 0; i < 256; i++)
                    number[i] = 0;

                for (j = 0; j < tempbitmap.Height; j++)
                    for (i = 0; i < tempbitmap.Width; i++)
                    {
                        Color getanpixel = tempbitmap.GetPixel(i, j);
                        number[getanpixel.R]++;
                        if (number[getanpixel.R] > max)
                            max = number[getanpixel.R];
                    }


                Graphics g2 = Graphics.FromImage((Image)output);
                pictureBox2.BackColor = Color.Black;
                Pen myPen = new Pen(new SolidBrush(Color.White), 1);
                int x1, y1, x2, y2;
                double ratey = (pictureBox2.Height * 1.0) / (max * 1.0);
                double ratex = (pictureBox2.Width * 1.0) / (255 * 1.0);
                int y_height;

                for (i = 0; i <= pictureBox2.Width; i++)
                {
                    x1 = i;
                    x2 = x1;
                    y1 = pictureBox2.Height;
                    y_height = Convert.ToInt32(number[Convert.ToInt32(i / ratex)] * ratey);
                    y2 = pictureBox2.Height - y_height;
                    g2.DrawLine(myPen, x1, y1, x2, y2);

                }
                pictureBox2.Image = (Image)output;
                pictureBox2.Refresh();
                button1.Enabled = true;
                Browse.Enabled = false;

            }
        }

        private void button1_trancefer(object sender, EventArgs e)
        {
            /*相關參數設定*/
            Bitmap an = (Bitmap)pictureBox1.Image;
            Bitmap input = new Bitmap(250, 250);
            Bitmap output = new Bitmap(250, 250);
            Bitmap his = new Bitmap(an.Width, an.Height);
            int i, j, x, y, th, k, max = 0;
            int[] number = new int[256];
            double[] p = new double[256];
            Graphics g2 = Graphics.FromImage((Image)input);
            Pen myPen = new Pen(new SolidBrush(Color.White), 1);
            int x1, y1, x2, y2;
            double ratey = (pictureBox2.Height * 1.0) / (max * 1.0);
            double ratex = (pictureBox2.Width * 1.0) / (255 * 1.0);
            int y_height;

            button1.Enabled = false;

            /*以下程式使用Histogram Equalization Algorithm*/
            for (i = 0; i < 256; i++)
                p[i] = number[i] = 0;

            //統計
            for (j = 0; j < an.Height; j++)
                for (i = 0; i < an.Width; i++)
                {
                    Color getanpixel = an.GetPixel(i, j);
                    number[getanpixel.R]++;

                    if (number[getanpixel.R] > max)
                        max = number[getanpixel.R];
                }

            //算機率(平均值means)
            for (i = 0; i < 256; i++)
            {
                p[i] = (double)number[i] / (double)(an.Height * an.Width);
            }
            //累進機率(每個人都會需要彼此)    
            for (i = 1; i < 256; i++)
                p[i] = p[i] + p[i - 1];


            //重新取代   
            for (i = 0; i < 256; i++)
            {
                number[i] = (int)((double)(p[i]) * 255 + 0.5);
            }


            //根據對應的顏色，用新的顏色取代
            for (j = 0; j < an.Height; j++)
                for (i = 0; i < an.Width; i++)
                {
                    Color getanpixel = an.GetPixel(i, j);
                    int n = getanpixel.R;

                    his.SetPixel(i, j, Color.FromArgb(number[n], number[n], number[n]));
                }
            //接圖
            pictureBox4.Image = his;


            /*繪製output直方圖*/
            max = 0;
            for (i = 0; i < 256; i++)
                number[i] = 0;
            for (j = 0; j < his.Height; j++)
                for (i = 0; i < his.Width; i++)
                {
                    Color getanpixel = his.GetPixel(i, j);
                    number[getanpixel.R]++;
                    if (number[getanpixel.R] > max)
                        max = number[getanpixel.R];
                }

            g2 = Graphics.FromImage((Image)output);
            pictureBox3.BackColor = Color.Black;
            ratey = (pictureBox3.Height * 1.0) / (max * 1.0);
            ratex = (pictureBox3.Width * 1.0) / (255 * 1.0);

            for (i = 0; i <= pictureBox3.Width; i++)
            {
                x1 = i;
                x2 = x1;
                y1 = pictureBox3.Height;
                y_height = Convert.ToInt32(number[Convert.ToInt32(i / ratex)] * ratey);
                y2 = pictureBox3.Height - y_height;
                g2.DrawLine(myPen, x1, y1, x2, y2);
            }

            pictureBox3.Image = (Image)output;
            pictureBox3.Refresh();
            button2.Enabled = true;
        }


        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.Transparent;
            pictureBox2.BackColor = Color.Transparent;
            pictureBox3.BackColor = Color.Transparent;
            pictureBox4.BackColor = Color.Transparent;
            pictureBox2.Image = null;
            pictureBox3.Image = null;
            pictureBox1.Image = null;
            pictureBox4.Image = null;
            button2.Enabled = false;
            Browse.Enabled = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
