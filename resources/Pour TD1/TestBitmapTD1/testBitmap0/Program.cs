using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
//using System.ComponentModel;
//using System.Media;
//using System.Diagnostics;





namespace testBitmap0
{
    class Program
    {


   
        static void Main(string[] args)
        {
            Bitmap b = new Bitmap("./Images/lena.bmp");
            //b.RotateFlip(RotateFlipType.Rotate180FlipX);
            //b.Save("./Images/lenasortie1.bmp");

            //ColorPalette pal = b.Palette;
            //for (int i=0;i<pal.Entries.Length;i++)
            //Console.WriteLine(pal.Entries[i]);

            RectangleF rec = new RectangleF(10.0f, 10.0f, 100.0f, 100.0f);
            Bitmap c = b.Clone(rec, PixelFormat.DontCare);
            c.Save("./Images/lenasortie2.bmp");

            for (int i = 0; i < b.Height; i++)
                for (int j = 0; j < b.Width; j++)
                {
                    Color mycolor = b.GetPixel(i, j);
                    b.SetPixel(i, j, Color.FromArgb(255 - mycolor.R, 255 - mycolor.R, 255 - mycolor.R));

                    //   c.SetPixel(i, j, Color.Coral);
                }

            //c.MakeTransparent(Color.Gray);
            //c.MakeTransparent();
            b.Save("./Images/lenasortie2.bmp");

            //Marche si l'image est sous le même répertoire que l'exécutable
            //Process.Start("lenasortie1.bmp");
            Console.ReadKey();


            Console.ReadKey();

        }
    }
}
