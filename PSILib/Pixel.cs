namespace PSILib {
    public class Pixel {
        #region Properties
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
        #endregion

        #region Constructors
        public Pixel(int blue, int green, int red)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public Pixel(int color)
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
            Red = 255 - Red;
            Green = 255 - Green;
            Blue = 255 - Blue;
        }
        
        public void Grayscale()
        {
            int avg = (Red + Green + Blue) / 3;
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