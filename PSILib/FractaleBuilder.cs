namespace PSILib;

public class FractaleBuilder {
    #region Properties
    // The size of the fractale
    public int Width { get; set; }
    public int Height { get; set; }

    #endregion

    #region Constructors & Builder

    /// <summary>
    /// Create a new FractaleBuilder.
    /// </summary>
    public FractaleBuilder() {}

    /// <summary>
    /// Set the size of the fractale.
    /// </summary>
    /// <param name="width">The width of the fractale</param>
    /// <param name="height">The height of the fractale</param>
    public FractaleBuilder SetSize(int width, int height) {
        Width = width;
        Height = height;
        return this;
    }

    /// <summary>
    /// Build the fractale.
    /// </summary>
    /// <returns>The fractale as a MyImage</returns>
    public MyImage Build() {
        if (Width * Height > 1_000_000_000) {
            throw new System.Exception("Image too big");
        }
        Pixel[,] pixelMatrix = new Pixel[Width, Height];
        GenerateFractale(pixelMatrix);
        MyImage image = new MyImage(Width, Height, 24, pixelMatrix);
        return image;
    }
    #endregion

    #region Fractale generation

    /// <summary>
    /// Generate a fractale using the Mandelbrot set.
    /// </summary>
    /// <param name="pixelMatrix">The matrix of pixels to fill</param>
    private void GenerateFractale(Pixel[,] pixelMatrix, int max_depth = 1000, double zoom = 1, double moveX = -0.5, double moveY = 0) {
        double x0, y0, x, y, xtemp;
        int depth = 0;
        

        for (int i = 0; i < Width; i++) {
            for (int j = 0; j < Height; j++) {
                x0 = (i - Width / 2) / (0.5 * zoom * Width) + moveX;
                y0 = (j - Height / 2) / (0.5 * zoom * Height) + moveY;
                x = 0;
                y = 0;
                depth = 0;
                while (x * x + y * y < 2 * 2 && depth < max_depth) {
                    xtemp = x * x - y * y + x0;
                    y = 2 * x * y + y0;
                    x = xtemp;
                    depth++;
                }
                if (depth == max_depth) {
                    pixelMatrix[i, j] = new Pixel(0, 0, 0);
                } else {
                    pixelMatrix[i, j] = new Pixel(255, 255, 255);
                }
            }
        }
    }
    #endregion
}