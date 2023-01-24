namespace PSILib {
    public class Pixel {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }

        public Pixel(int blue, int green, int red)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public void Invert()
        {
            Red = 255 - Red;
            Green = 255 - Green;
            Blue = 255 - Blue;
        }
    }
}