using System;
using Xunit;

namespace Tests
{
    public class FractaleTests
    {
        [Fact]
        public void Test_Fractales() {
            // we just create the generated folder if it doesn't exist
            Utils.InitOutputFolder();
            
            var fractale = new PSILib.FractaleBuilder(1000, 1000)
                .Basic();
            Utils.CompareWithRef(fractale, "fractale_1");

            fractale = new PSILib.FractaleBuilder(1000, 1000)
                .Basic(fill: false);
            Utils.CompareWithRef(fractale, "fractale_2");
        }
    }
}