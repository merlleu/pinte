using System;
using Xunit;

namespace Tests
{
    public class ParserTests
    {
        [Fact]
        public void Bateau(){
            // we just create the generated folder if it doesn't exist
            Utils.InitOutputFolder();

            var img = new PSILib.MyImage(Utils.GetPath("resources/wl1.bmp"));
            
            var img2 = img.Clone();
            img2.Rotate(27); 
            Utils.CompareWithRef(img2, "Rotate27");

            
            img.Crop(2000, 0, 700, 700);
            Utils.CompareWithRef(img, "Crop");

            img2 = img.Clone();
            img2.GaussianBlur3();
            Utils.CompareWithRef(img2, "GaussianBlur3");

            img2 = img.Clone();
            img2.GaussianBlur5();
            Utils.CompareWithRef(img2, "GaussianBlur5");

            img2 = img.Clone();
            img2.EdgeDetection(gray: false);
            Utils.CompareWithRef(img2, "EdgeDetection");
            
            img2 = img.Clone();
            img2.EdgeDetection();
            Utils.CompareWithRef(img2, "EdgeDetectionGray");

            img2 = img.Clone();
            img2.Rotate(90);
            Utils.CompareWithRef(img2, "Rotate90");
            
            img2 = img.Clone();
            img2.Rotate(27, resize_factor: 1);
            img2.EdgeDetection();
            Utils.CompareWithRef(img2, "Rotate27Resize1EdgeDetection");

            img2 = img.Clone();
            img2.Rotate(27, resize_factor: 2);
            Utils.CompareWithRef(img2, "Rotate27Resize2");

            img2.Rotate(-27, resize_factor: 2);
            Utils.CompareWithRef(img2, "RotateThenRotateBack");

            img2.RemoveBorder();
            Utils.CompareWithRef(img2, "PostRotateCleanedUp");


            img2 = img.Clone();
            img2.Sharpen();
            Utils.CompareWithRef(img2, "Sharpen");

            img2 = img.Clone();
            img2.UnsharpMask5();
            Utils.CompareWithRef(img2, "UnsharpMask5");

            img2 = img.Clone();
            img2.Resize(2);
            Utils.CompareWithRef(img2, "Resize2");

            img2 = img.Clone();
            img2.Resize(1.25);
            Utils.CompareWithRef(img2, "Resize125");

            img2 = img.Clone();
            img2.Resize(0.5);
            Utils.CompareWithRef(img2, "Resize05");

            img2 = img.Clone();
            img2.Resize(0.25);
            Utils.CompareWithRef(img2, "Resize025");
        }
        
        [Fact]
        public void Lac() {
            // we just create the generated folder if it doesn't exist
            Utils.InitOutputFolder();
            
            var img3 = new PSILib.MyImage(Utils.GetPath("resources/lac.BMP"));
            img3.EdgeDetection();
            Utils.CompareWithRef(img3, "EdgeDetectionLac");
        }

        [Fact]
        public void Coco() {
            // we just create the generated folder if it doesn't exist
            Utils.InitOutputFolder();
            
            var img = new PSILib.MyImage(Utils.GetPath("resources/coco.bmp"));
            var img3 = img.Clone();
            img3.Sharpen();
            Utils.CompareWithRef(img3, "Sharpencoco");

            img3 = img.Clone();
            img3.EdgeDetection(gray: false);
            Utils.CompareWithRef(img3, "EdgeDetectioncoco");
        }

        [Fact]
        public void Compression() {
            // we just create the generated folder if it doesn't exist
            Utils.InitOutputFolder();
            
            var img = new PSILib.MyImage(Utils.GetPath("resources/wl1.bmp"));
            img.Save("test.beermp", compression: PSILib.Constants.BI_HUFFMAN1D);
        }
    }
}
