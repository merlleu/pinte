namespace PSILib {
    public class Pixel {
        #region Properties
        // The color of the pixel
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        
        // public byte Alpha { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new pixel.
        /// </summary>
        /// <param name="blue">The blue color</param>
        /// <param name="green">The green color</param>
        /// <param name="red">The red color</param>
        public Pixel(byte blue, byte green, byte red) {
            Red = red;
            Green = green;
            Blue = blue;
        }

        /// <summary>
        /// Create a new pixel
        /// </summary>
        /// <param name="color">The color of the pixel (as grayscale).</param>
        public Pixel(byte color) {
            Red = color;
            Green = color;
            Blue = color;
        }

        /// <summary>
        /// Create a new blank pixel.
        /// </summary>
        public Pixel() {
            Red = 0;
            Green = 0;
            Blue = 0;
        }

        #endregion


        #region filters
        /// <summary>
        /// Invert the colors of the pixel.
        /// </summary>
        public void Invert() {
            Red = (byte)(255 - Red);
            Green = (byte)(255 - Green);
            Blue = (byte)(255 - Blue);
        }
        
        /// <summary>
        /// Apply a grayscale filter to the pixel.
        /// </summary>
        public void Grayscale() {
            byte avg = (byte)((Red + Green + Blue) / 3);
            Red = avg;
            Green = avg;
            Blue = avg;
        }

        /// <summary>
        /// Apply a sepia filter to the pixel.
        /// </summary>
        public void Sepia() {
            byte avg = (byte)((Red + Green + Blue) / 3);
            Red = (byte)(avg + 2 * 20);
            Green = (byte)(avg + 20);
            Blue = (byte)(avg - 20);
        }

        /// <summary>
        /// Replaces 'bits' lsb of each colors by 'bits' average of each 'pixel'.
        /// </summary>
        /// <param name="pixel">The pixel to use</param>
        /// <param name="bits">The number of bits to replace</param>
        public void InsertLSB(Pixel pixel, int bits) {
            // We only keep the len(integer) - bits msb of each colors from the original image,
            // then store the 'bits' lsb of each colors of the image to hide.
            Red = (byte)((Red & ~((1 << bits) - 1)) | (pixel.Red >> (8 - bits)));
            Green = (byte)((Green & ~((1 << bits) - 1)) | (pixel.Green >> (8 - bits)));
            Blue = (byte)((Blue & ~((1 << bits) - 1)) | (pixel.Blue >> (8 - bits)));
        }

        /// <summary>
        /// Extract 'bits' lsb of each colors and shift them to the msb.
        /// </summary>
        /// <param name="bits">The number of bits to extract</param>
        public void ExtractLSB(int bits) {
            // We only keep the 'bits' lsb of each colors,
            // To do so, we shift the colors to the right by len(integer) - bits.
            Red = (byte)(Red << (8 - bits));
            Green = (byte)(Green << (8 - bits));
            Blue = (byte)(Blue << (8 - bits));
        }

        /// <summary>
        /// Turn the pixel into a the absolute distance between the pixel and the given pixel.
        /// </summary>
        /// <param name="pixel">The pixel to use</param>
        public void DistanceAbs(Pixel pixel) {
            Red = (byte)Math.Abs((int)Red - (int)pixel.Red);
            Green = (byte)Math.Abs((int)Green - (int)pixel.Green);
            Blue = (byte)Math.Abs((int)Blue - (int)pixel.Blue);
        }
        #endregion

        #region Utils
        /// <summary>
        /// Deep copy the pixel.
        /// Allows to modify the pixel without modifying the original pixel.
        /// </summary>
        /// <returns>An identical pixel</returns>
        public Pixel Clone() {
            return new Pixel(Blue, Green, Red);
        }

        /// <summary>
        /// Check if the pixel is empty (black).
        /// </summary>
        public bool IsEmpty() {
            return Red == 0 && Green == 0 && Blue == 0;
        }

        /// <summary>
        /// Update the pixel if it is empty.
        /// </summary>
        /// <param name="pixel">The pixel to use</param>
        public void UpdateIfEmpty(Pixel pixel)
        {
            if (IsEmpty())
            {
                Red = pixel.Red;
                Green = pixel.Green;
                Blue = pixel.Blue;
            }
        }

        /// <summary>
        /// Display the pixel as a string.
        /// </summary>
        public override string ToString() {
            return $"Pixel(blue: {Blue}, green: {Green}, red: {Red})";
        }

        #endregion

        #region Operators
        /// <summary>
        /// Check if two pixels are equal.
        /// </summary>
        /// <param name="pixel">The pixel to compare</param>
        /// <returns>True if the pixels are equal, false otherwise</returns>
        public bool Equals(Pixel pixel) {
            return Red == pixel.Red && Green == pixel.Green && Blue == pixel.Blue;
        }
        #endregion
    }
}