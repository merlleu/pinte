namespace PSILib {
    public class Pixel {
        #region Properties
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }

        /// <summary>
        /// Alpha channel (optional) is the opacity of the pixel

        // public byte Alpha { get; set; }
        #endregion

        #region Constructors
        public Pixel(byte blue, byte green, byte red)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public Pixel(byte color)
        {
            Red = color;
            Green = color;
            Blue = color;
        }

        public Pixel()
        {
            Red = 0;
            Green = 0;
            Blue = 0;
        }

        #endregion


        #region filters
        public void Invert()
        {
            Red = (byte)(255 - Red);
            Green = (byte)(255 - Green);
            Blue = (byte)(255 - Blue);
        }
        
        public void Grayscale()
        {
            byte avg = (byte)((Red + Green + Blue) / 3);
            Red = avg;
            Green = avg;
            Blue = avg;
        }
        #endregion

        #region Utils
        public Pixel Clone() {
            return new Pixel(Blue, Green, Red);
        }
        #endregion
    }
}