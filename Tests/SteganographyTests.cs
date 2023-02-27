using System;
using Xunit;

namespace Tests
{
    public class SteganographyTests
    {
        [Fact]
        public void Test_Steganography() {
            // we just create the generated folder if it doesn't exist
            Utils.InitOutputFolder();
            var img = new PSILib.MyImage(Utils.GetPath("resources/wl1.bmp"));
            img.Crop(2000, 0, 700, 700);

            var img3 = new PSILib.MyImage(Utils.GetPath("resources/lac.BMP"));
            img3.Crop(0, 0, 600, 600);
            img.HideImage(img3, 0, 0, bits: 4);
            Utils.CompareWithRef(img, "lacHide");
            img.ExtractImage(0, 0, img3.Width, img3.Height, bits: 4);
            Utils.CompareWithRef(img, "lacExtract");
        }
    }
}