using System.Drawing;

namespace ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            PSILib.MyImage img = new PSILib.MyImage("../resources/WL1.BMP");
            // img.Invert();
            img.Rotate(); 
            img.Crop(0, 500, 300, 800);
            img.Rotate(-1);

            // img.Resize(2);
            // img.Rotate();
            // img.Rotate();
            img.Save("../resources/Crop.BMP");

            var img2 = img.Clone();
            img2.GaussianBlur3();
            img2.Save("../resources/GaussianBlur3.BMP");

            img2 = img.Clone();
            img2.GaussianBlur5();
            img2.Save("../resources/GaussianBlur5.BMP");

            img2 = img.Clone();
            img2.EdgeDetection();
            img2.Save("../resources/EdgeDetection.BMP");

            img2 = img.Clone();
            img2.Sharpen();
            img2.Save("../resources/Sharpen.BMP");

            img2 = img.Clone();
            img2.UnsharpMask5();
            img2.Save("../resources/UnsharpMask5.BMP");

            img2 = img.Clone();
            img2.Resize(2);
            img2.Save("../resources/Resize2.BMP");

            img2 = img.Clone();
            img2.Resize(0.5);
            img2.Save("../resources/Resize05.BMP");

            img2 = img.Clone();
            img2.Resize(1.25);
            img2.Save("../resources/Resize125.BMP");

            img2 = img.Clone();
            img2.Resize(0.25);
            img2.Save("../resources/Resize025.BMP");

            
            

            // var gray = img.Clone();
            // gray.Grayscale();
            // gray.Save("../resources/gray.BMP");

            // var rot = img.Clone();
            // File.WriteAllText("../resources/rot.txt", rot.Repr());
            // rot.Crop(0, 0, 5, 5);
            // File.WriteAllText("../resources/rot2.txt", rot.Repr());
            // rot.Save("../resources/rot.BMP");

            // read bitmap ../resources/rot.BMP
            // Bitmap bmp = new Bitmap("../resources/OUT2.BMP");
            // bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            // bmp.Save("../resources/OUT3.BMP");
        }
    }
}