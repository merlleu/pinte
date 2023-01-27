using System.Drawing;

namespace ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region basic tests
            PSILib.MyImage img = new PSILib.MyImage("../resources/WL1.BMP");
            // img.Invert();
            img.Rotate(); 
            img.Crop(0, 2000, 700, 700);
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
            img2.EdgeDetection(gray: false);
            img2.Save("../resources/EdgeDetection.BMP");
            
            img2 = img.Clone();
            img2.EdgeDetection();
            img2.Save("../resources/EdgeDetectionGray.BMP");

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
            img2.Resize(1.25);
            img2.Save("../resources/Resize125.BMP");

            img2 = img.Clone();
            img2.Resize(0.5);
            img2.Save("../resources/Resize05.BMP");

            img2 = img.Clone();
            img2.Resize(0.25);
            img2.Save("../resources/Resize025.BMP");

            
            var img3 = new PSILib.MyImage("../resources/lac.BMP");
            img3.EdgeDetection();
            img3.Save("../resources/lacEdge.BMP");
            #endregion

            #region fractales

            var fractale = new PSILib.FractaleBuilder()
                .SetSize(1000, 1000)
                .Build();
            fractale.Save("../resources/fractale_1.BMP");

            #endregion
        }
    }
}