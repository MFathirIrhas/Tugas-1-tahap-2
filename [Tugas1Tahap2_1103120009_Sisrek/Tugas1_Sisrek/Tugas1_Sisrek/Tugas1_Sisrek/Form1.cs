using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

namespace Tugas1_Sisrek
{
    public partial class Form1 : Form
    {
        public Image OriginalImage;
        public Image FrameImage;
        public Image[] ImageArray= new Image[100];
        public Image[,] ImageArray2d = new Image[100, 100];
        private bool sama,sama2 ;
        //int 
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.AutoScroll = true;
            panel3.AutoScroll = true;
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            //pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox2.Size = new Size(600, 600);
            
        }

        #region | LOAD IMAGE |
        private void open_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Select Image";
            open.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp";

            if (open.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(open.FileName);
                filename.Text = open.FileName.ToString();
                OriginalImage = pictureBox1.Image;
            }
        }
        #endregion

        #region | GRAYSCALE |
        private void grayscaleBTN_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Image imgReset = pictureBox1.Image;
                Bitmap bmp = new Bitmap(pictureBox1.Image);
                try
                {
                    Grayscale(bmp);
                    pictureBox1.Image = bmp;
                }
                catch { }
            }
            else
            {
                MessageBox.Show("Masukkan gambar terlebih dahulu!", "Error");
            }
        }

        public static bool Grayscale(Bitmap b)
        {
             
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = bmData.Stride; 
            System.IntPtr Scan0 = bmData.Scan0;
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte red, green, blue;
                int nOffset = stride - b.Width * 3;
                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        blue = p[0];
                        green = p[1];
                        red = p[2];
                        p[0] = p[1] = p[2] = (byte)(.299 * red
                            + .587 * green + .114 * blue);
                        p += 3;
                    }
                    p += nOffset;
                }
            }
            b.UnlockBits(bmData);
            return true;
        }
#endregion

        #region | INVERSE |
        private void inverseBTN_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Image img = pictureBox1.Image;
                Bitmap bmp = new Bitmap(img);

                pictureBox1.Image = Inverse(bmp);
            }
            else
            {
                MessageBox.Show("Masukkan gambar terlebih dahulu!", "Error");
            }
        }

        public Bitmap Inverse(Bitmap bmp)
        {
            
            Color c;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    c = bmp.GetPixel(i, j);
                    bmp.SetPixel(i, j, Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                }
            }
            return bmp;
        }
        #endregion

        #region | RESET |
        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                
                foreach (var control in pictureBox1.Controls.Cast<Control>().ToArray())
                {
                    control.Dispose();
                }

                foreach (var control in pictureBox2.Controls.Cast<Control>().ToArray())
                {
                    control.Dispose();
                }
                pictureBox1.Image = OriginalImage;
                pictureBox2.Image = null;
            }
            else
            {
                MessageBox.Show("Error", "No Image Detected");
            }
        }
        #endregion

        #region | SPLIT |
        private void Split(Bitmap bmp, int y)
        {
            //Image potonganArr;
            if (pictureBox1.Image != null)
            {
                var imgarray = new Image[y, y];
                var img = (Image)bmp;
                int imgHeight = img.Height;
                int imgWidth = img.Width;
                int s_imgHeight = imgHeight / y;
                int s_imgWidth = imgWidth / y;
                
                for (int i = 0; i < y; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        imgarray[i, j] = new Bitmap(s_imgWidth, s_imgHeight);
                        var graphics = Graphics.FromImage(imgarray[i, j]);
                        graphics.DrawImage(img, new Rectangle(0, 0, s_imgWidth, s_imgHeight), new Rectangle(j * s_imgWidth, i * s_imgHeight, s_imgWidth, s_imgHeight), GraphicsUnit.Pixel);
                        graphics.Dispose();
                    }
                        
                }

                for (int k = 0; k < y; k++)
                {
                    for (int l = 0; l < y; l++)
                    {
                        ImageArray[k*y+l] = imgarray[k, l];
                        ImageArray2d[k, l] = imgarray[k, l];
                        //ImageArray2[k * y + l] = imgarray[k, l];
                    }
                }

                List<PictureBox> listpb = new List<PictureBox>();

                for (int k = 0; k < y; k++)
                {
                    for (int l = 0; l < y; l++)
                    {
                        PictureBox pb = new PictureBox
                        {
                            Name = "potongan" + k + 1,
                            Size = new Size(s_imgWidth, s_imgHeight),
                            Location = new Point(l * (s_imgWidth + 5), k * (s_imgHeight + 5)),
                            BorderStyle = BorderStyle.None,
                            SizeMode = PictureBoxSizeMode.Zoom,
                            Image = imgarray[k, l]
                        };
                        listpb.Add(pb);
                    }

                }

                foreach (PictureBox p in listpb)
                {
                    pictureBox2.Controls.Add(p);

                }
                //potonganArr = pictureBox2.Image;
                //return potonganArr;
            }
            else
            {
                MessageBox.Show("Masukkan gambar terlebih dahulu", "Error");
                //return null;
            }
            
        }
        #endregion

        #region | SAVE IMAGE |
        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Bitmap Image (.bmp)|*.bmp|Jpg Image (.jpg)|*.jpg |JPEG Image (.jpeg)|*.jpeg |Png Image (.png)|*.png ";
            sf.AddExtension = true;
            if (sf.ShowDialog() == DialogResult.OK)
            {
                switch (Path.GetExtension(sf.FileName).ToUpper())
                {
                    case ".BMP":
                        pictureBox1.Image.Save(sf.FileName, ImageFormat.Bmp);
                        break;

                    case ".JPG":
                        pictureBox1.Image.Save(sf.FileName, ImageFormat.Jpeg);
                        break;

                    case ".JPEG":
                        pictureBox1.Image.Save(sf.FileName, ImageFormat.Jpeg);
                        break;

                    case ".PNG":
                        pictureBox1.Image.Save(sf.FileName, ImageFormat.Png);
                        break;

                    default:
                        break;
                }
            }
        }
        #endregion

        #region | PIGURA |

        private Bitmap Framing(Bitmap bmp , int L)
        {
            int w = bmp.Width - 2*(L-44);
            int h = bmp.Height - 2*(L-44);
            var b = new SolidBrush(Color.Black);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(b, L-44, L-44,w, h);
                g.Dispose();
            }

            FrameImage = bmp;
            return bmp;
        }
        #endregion

        private void button4_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                //Pigura
                Image img = OriginalImage;
                Bitmap bmp = new Bitmap(img);
                int L = Convert.ToInt32(inputFrameSize.Text);
                

                //Split
                Bitmap bmp2 = new Bitmap(img);
                int y = Convert.ToInt32(inputY.Text);

  
                //Output
                Split(bmp2, y);

                
                pictureBox1.Image = Framing(bmp,L);
            }
            else
            {
                MessageBox.Show("Input gambar Terlebih dahulu", "error");
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int inputy = Convert.ToInt32(inputY.Text);
            int Y = inputy * inputy;
            for (int x = 0; x < Y; x++)
            {
                SaveFileDialog sf = new SaveFileDialog();
                sf.Filter = "Bitmap Image (.bmp)|*.bmp|Jpg Image (.jpg)|*.jpg |JPEG Image (.jpeg)|*.jpeg |Png Image (.png)|*.png ";
                sf.AddExtension = true;
                if (sf.ShowDialog() == DialogResult.OK)
                {
                    switch (Path.GetExtension(sf.FileName).ToUpper())
                    {
                        case ".BMP":
                            ImageArray[x].Save(sf.FileName, ImageFormat.Bmp);
                            break;

                        case ".JPG":
                            ImageArray[x].Save(sf.FileName, ImageFormat.Jpeg);
                            break;

                        case ".JPEG":
                            ImageArray[x].Save(sf.FileName, ImageFormat.Jpeg);
                            break;

                        case ".PNG":
                            ImageArray[x].Save(sf.FileName, ImageFormat.Png);
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        //Match Button
        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            int L = Convert.ToInt32(inputFrameSize.Text)-44;
            int inputy = Convert.ToInt32(inputY.Text);
            int inputy2 = inputy*inputy;
            
            //Atas
            for (int x = 0; x < inputy; x++)
            {
                
                for (int y = 0; y < inputy2; y++)
                {
                    Bitmap bmp2 = new Bitmap(ImageArray[y]);
                    int xx = x * bmp2.Width;
                    bool sama = MatchH(bmp1, bmp2, L, xx,inputy);
                    if (sama == true)
                    {
                        using (Graphics g = Graphics.FromImage(bmp1))
                        {
                            g.DrawImage(ImageArray[y], x * ImageArray[y].Width, 0, ImageArray[y].Width, ImageArray[y].Height);
                            g.Dispose();
                        }
                        //ImageArray = ImageArray.Except(new Image[] { ImageArray[y] }).ToArray();
                        break;
                    }
                    
                }
            }

            //bawah
            for (int y = 0; y < inputy; y++)
            {
                for (int x = 0; x < inputy; x++)
                {
                    Bitmap bmp2 = new Bitmap(ImageArray2d[y, x]);
                    int xx = x * bmp2.Width;
                    //int yy = bmp1.Height - L;
                    bool sama = MatchH2(bmp1, bmp2, L, xx, inputy);
                    if (sama == true)
                    {
                        using (Graphics g = Graphics.FromImage(bmp1))
                        {
                            g.DrawImage(ImageArray2d[y, x], x * ImageArray2d[0, 1].Width, ImageArray2d[y, x].Height * (inputy-1), ImageArray2d[y, x].Width, ImageArray2d[y, x].Height);
                            g.Dispose();
                        }
                        //break;
                    }
                }

            }

            //kiri
            for (int x = 1; x < inputy; x++)
            {
                for (int y = 0; y < inputy; y++)
                {
                    Bitmap bmp2 = new Bitmap(ImageArray2d[x,y]);
                    //int yy = 1;
                    bool sama = MatchV2(bmp1, bmp2, L, x);
                    if (sama == true)
                    {
                        using (Graphics g = Graphics.FromImage(bmp1))
                        {
                            g.DrawImage(ImageArray2d[x,y], 0, x*bmp2.Height, bmp2.Width, bmp2.Height);
                            g.Dispose();
                        }
                        //break;
                    }
                }
            }

            //kanan
            for (int x = 1; x < inputy; x++)
            {
                for (int y = inputy-1; y < inputy; y++)
                {
                    Bitmap bmp2 = new Bitmap(ImageArray2d[x, y]);
                    bool sama = MatchV(bmp1, bmp2, L, x,inputy);
                    if (sama == true)
                    {
                        using (Graphics g = Graphics.FromImage(bmp1))
                        {
                            g.DrawImage(ImageArray2d[x, y], bmp2.Width*(inputy-1), x * bmp2.Height, bmp2.Width, bmp2.Height);
                            g.Dispose();
                        }
                        //break;
                    }
                }
            }

            //tengah 
            for (int x = 1; x < inputy-1; x++)
            {
                for (int y = 1; y < inputy-1; y++)
                {
                    Bitmap bmp2 = new Bitmap(ImageArray2d[x,y]);

                    using (Graphics g = Graphics.FromImage(bmp1))
                    {
                        g.DrawImage(bmp2, y*bmp2.Width, x*bmp2.Height,bmp2.Width,bmp2.Height);
                        g.Dispose();
                    }
                }
            }


            pictureBox1.Image = bmp1;

        }

        //Match Procedure
        #region |MATCH HORIZONTAL|
        public bool MatchH(Bitmap bmp1, Bitmap bmp2,int L,  int xx, int yy)
        {
            
            Rectangle rect1 = new Rectangle(xx, 0, bmp1.Width/yy, bmp1.Height/yy);
            Rectangle rect2 = new Rectangle(0, 0, bmp2.Width, bmp2.Height);
            BitmapData bmpData1 = bmp1.LockBits(rect1, ImageLockMode.ReadOnly, bmp1.PixelFormat);
            BitmapData bmpData2 = bmp2.LockBits(rect2, ImageLockMode.ReadOnly, bmp2.PixelFormat);
            int width;
            Rectangle rect;
            unsafe
            {
                byte* ptr1 = (byte*)bmpData1.Scan0.ToPointer();
                byte* ptr2 = (byte*)bmpData2.Scan0.ToPointer();
                int width1 = rect1.Width * 3;
                int width2 = rect2.Width * 3;
                if (width1 > width2)
                {
                    width = width2;
                    rect = rect2;
                }
                else
                {
                    width = width1;
                    rect = rect1;
                }

                for (int y = 0; y < L; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (*ptr1 != *ptr2)
                        {
                            sama = false;
                            
                            break;
                        }
                        else if (*ptr1 == *ptr2)
                        {
                            sama = true;
                            
                            //break;
                        }
                        ptr1++;
                        ptr2++;
                    }
                    ptr1 += bmpData1.Stride - width;
                    ptr2 += bmpData2.Stride - width;
                }
            }
            bmp1.UnlockBits(bmpData1);
            bmp2.UnlockBits(bmpData2);

            return sama;
        }


        public bool MatchH2(Bitmap bmp1, Bitmap bmp2, int L, int xx, int yy)
        {

            Rectangle rect1 = new Rectangle(xx, /*bmp2.Height*3*/0, bmp1.Width / yy, bmp1.Height / yy);
            Rectangle rect2 = new Rectangle(0, 0, bmp2.Width, bmp2.Height);
            BitmapData bmpData1 = bmp1.LockBits(rect1, ImageLockMode.ReadOnly, bmp1.PixelFormat);
            BitmapData bmpData2 = bmp2.LockBits(rect2, ImageLockMode.ReadOnly, bmp2.PixelFormat);
            int width;
            Rectangle rect;
            unsafe
            {
                byte* ptr1 = (byte*)bmpData1.Scan0.ToPointer();
                byte* ptr2 = (byte*)bmpData2.Scan0.ToPointer();
                int width1 = rect1.Width * 3;
                int width2 = rect2.Width * 3;
                if (width1 > width2)
                {
                    width = width2;
                    rect = rect2;
                }
                else
                {
                    width = width1;
                    rect = rect1;
                }

                for (int y = 0; y < L; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (*ptr1 != *ptr2)
                        {
                            sama = false;

                            //break;
                        }
                        else if (*ptr1 == *ptr2)
                        {
                            sama = true;

                            //break;
                        }
                        ptr1++;
                        ptr2++;
                    }
                    ptr1 += bmpData1.Stride - width;
                    ptr2 += bmpData2.Stride - width;
                }
            }
            bmp1.UnlockBits(bmpData1);
            bmp2.UnlockBits(bmpData2);

            return sama;
        }
        #endregion

        #region |MATCH VERTICAL|
        public bool MatchV(Bitmap bmp1, Bitmap bmp2, int L,int xx,int yy)
        {

            Rectangle part = new Rectangle(bmp2.Width*(yy-1), xx * bmp2.Height, bmp2.Width, bmp2.Height);
            Bitmap bmp = bmp1.Clone(part, bmp1.PixelFormat);
            string jmlPixel1, jmlPixel2;
            using (Graphics g = Graphics.FromImage(bmp2))
            {
                g.FillRectangle(new SolidBrush(Color.Black), 0, 0, bmp2.Width-L, bmp2.Height);
                g.Dispose();
            }
            for (int i = 0; i < bmp2.Height; i++)
            {
                for (int j = 0; j < bmp2.Width; j++)
                {
                    jmlPixel1 = bmp.GetPixel(i, j).ToString();
                    jmlPixel2 = bmp2.GetPixel(i, j).ToString();
                    if (jmlPixel1 == jmlPixel2)
                    {
                        sama2 = true;
                    }
                    else
                    {
                        sama2 = false;
                        break;
                    }
                }
            }

            return sama2;
        }

        public bool MatchV2(Bitmap bmp1, Bitmap bmp2 ,int L/*, int xx*/,int yy)
        {
            Rectangle part = new Rectangle(0,yy*bmp2.Height, bmp2.Width, bmp2.Height);
            Bitmap bmp = bmp1.Clone(part, bmp1.PixelFormat);
            string jmlPixel1, jmlPixel2;
            using (Graphics g = Graphics.FromImage(bmp2))
            {
                g.FillRectangle(new SolidBrush(Color.Black), L, 0, bmp2.Width,bmp2.Height );
                g.Dispose();
            }
            for (int i = 0; i < bmp2.Height; i++)
            {
                for (int j = 0; j < bmp2.Width; j++)
                {
                    jmlPixel1 = bmp.GetPixel(i, j).ToString();
                    jmlPixel2 = bmp2.GetPixel(i, j).ToString();
                    if (jmlPixel1 == jmlPixel2)
                    {
                        sama2 = true;
                    }
                    else
                    {
                        sama2 = false;
                        break;
                    }
                }
            }

            return sama2;
        }
        #endregion

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Select Image";
            open.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp";

            if (open.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.Image = new Bitmap(open.FileName);
                filename.Text = open.FileName.ToString();
                //OriginalImage = pictureBox1.Image;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Bitmap bmp1 = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(pictureBox2.Image);
            int L = Convert.ToInt32(inputFrameSize.Text) - 44;
            int inputy = Convert.ToInt32(inputY.Text);
            sama = MatchV2(bmp1, bmp2, L,inputy/*, 0, ImageArray2d[0, 0].Height*/);
            if (sama == true)
            {
                //using (Graphics g = Graphics.FromImage(bmp1))
                //{
                //    g.DrawImage(ImageArray2d[1, 0], 0, bmp2.Height, ImageArray2d[1, 0].Width, ImageArray2d[1, 0].Height);
                //    g.Dispose();
                //}
                MessageBox.Show("Sama", "Sama");

            }
            else
            {
                MessageBox.Show("Tidak Sama", "Tidak Sama");
            }
            //Bitmap bmp = new Bitmap(pictureBox1.Image);
            //string jml ;

            //for (int i = 0; i < bmp.Width; i++)
            //{
            //    for (int j = 0; j < bmp.Height; j++)
            //    {
            //        jml = bmp.GetPixel(1, j).ToString();
            //        total.Text = jml;
            //    }
                
            //}
            
        }

        
    }
}
