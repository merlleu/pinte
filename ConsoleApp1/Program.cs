using System.Drawing;

namespace ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region basic tests
            // we just create the generated folder if it doesn't exist
            if (!System.IO.Directory.Exists("../out"))
                System.IO.Directory.CreateDirectory("../out");
            
            var img = new PSILib.MyImage("../resources/WL1.BMP");
            
            var img2 = img.Clone();
            // Console.WriteLine(img);
            img2.Rotate(27); 
            img2.Save("../out/Rotate27.BMP");

            
            img.Crop(2000, 0, 700, 700);
            img.Save("../out/Crop.BMP");

            img2 = img.Clone();
            img2.GaussianBlur3();
            img2.Save("../out/GaussianBlur3.BMP");

            img2 = img.Clone();
            img2.GaussianBlur5();
            img2.Save("../out/GaussianBlur5.BMP");

            img2 = img.Clone();
            img2.EdgeDetection(gray: false);
            img2.Save("../out/EdgeDetection.BMP");
            
            img2 = img.Clone();
            img2.EdgeDetection();
            img2.Save("../out/EdgeDetectionGray.BMP");

            img2 = img.Clone();
            img2.Rotate(90);
            img2.Save("../out/Rotate90.BMP");
            
            img2 = img.Clone();
            img2.Rotate(27, resize_factor: 1);
            img2.EdgeDetection();
            img2.Save("../out/EdgeDetectionGrayCropRotate.BMP");

            img2 = img.Clone();
            img2.Rotate(27, resize_factor: 2);
            // img2.EdgeDetection();
            img2.Save("../out/GrayCropRotateResize.BMP");
            // Console.WriteLine(img2);
            img2.Rotate(-27, resize_factor: 2);
            img2.Save("../out/GrayCropRotateResizeRotate.BMP");
            // Console.WriteLine(img2);
            img2.RemoveBorder();
            img2.Save("../out/GrayCropRotateResizeRotateCrop.BMP");

            img2 = img.Clone();
            img2.Sharpen();
            img2.Save("../out/Sharpen.BMP");

            img2 = img.Clone();
            img2.UnsharpMask5();
            img2.Save("../out/UnsharpMask5.BMP");

            img2 = img.Clone();
            img2.Resize(2);
            img2.Save("../out/Resize2.BMP");

            img2 = img.Clone();
            img2.Resize(1.25);
            img2.Save("../out/Resize125.BMP");

            img2 = img.Clone();
            img2.Resize(0.5);
            img2.Save("../out/Resize05.BMP");

            img2 = img.Clone();
            img2.Resize(0.25);
            img2.Save("../out/Resize025.BMP");

            
            var img3 = new PSILib.MyImage("../resources/lac.BMP");
            img3.EdgeDetection();
            img3.Save("../out/lacEdge.BMP");
            #endregion

            #region fractales

            var fractale = new PSILib.FractaleBuilder(1000, 1000)
                .Basic();
            fractale.Save("../out/fractale_1.BMP");

            fractale = new PSILib.FractaleBuilder(1000, 1000)
                .Basic(fill: false);
            fractale.Save("../out/fractale_2.BMP");

            #endregion

            #region steganography
            img2 = img.Clone();
            img3 = new PSILib.MyImage("../resources/lac.BMP");
            img3.Crop(0, 0, 600, 600);
            img2.HideImage(img3, 0, 0, bits: 4);
            img2.Save("../out/lacHide.BMP");
            img2.ExtractImage(0, 0, img3.Width, img3.Height, bits: 4);
            img2.Save("../out/lacExtract.BMP");
            #endregion
        }
    }
}