using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace Tugas1_Sisrek
{
    public class Histogram
    {
        public int[] HistogramValue(Bitmap b)
        {


            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;

            System.IntPtr Scan0 = bmData.Scan0;

            int[] data = new int[b.Width * b.Height];

            unsafe
            {

                byte* p = (byte*)(void*)Scan0;



                int nOffset = stride - b.Width * 3;

                for (int y = 0; y < b.Height; ++y)
                {

                    for (int x = 0; x < b.Width; ++x)
                    {



                        data[p[0]] += 1;



                        p += 3;

                    }

                    p += nOffset;

                }



            }

            b.UnlockBits(bmData);

            return data;

        }
    }
}
