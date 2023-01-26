namespace PSILib
{
    
    public class MyImage
    {

        #region Properties
        public byte[] Type { get; set; }
        public int Size { get; set; }
        public int Offset { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int DIBHeaderSize { get; set; }

        public int BitsPerPixel { get; set; }
        private int BytesPerPixel => BitsPerPixel / 8;
        private int BytesPerColor => BytesPerPixel / 3;

        public Pixel[,] Pixels { get; set; }

        #endregion

        #region Constructors

        public MyImage(string path) {
            var bytes = File.ReadAllBytes(path);
            if (bytes.Length < 54) {
                throw new Exception("File is too small.");
            }

            // headers
            Type = new byte[2];
            Array.Copy(bytes, 0, Type, 0, 2);
            // check if it's a BMP file
            if (Type[0] != 66 || Type[1] != 77) {
                throw new Exception("Not a BMP file");
            }

            Size = Convertir_Endian_To_Int(bytes, 4, 2);
            Offset = Convertir_Endian_To_Int(bytes, 4, 10);

            // DIB Header
            DIBHeaderSize = Convertir_Endian_To_Int(bytes, 4, 14);
            if (DIBHeaderSize != 40) {
                throw new Exception("DIB Header size must be 40.");
            }

            Width = Convertir_Endian_To_Int(bytes, 4, 18);
            Height = Convertir_Endian_To_Int(bytes, 4, 22);
            BitsPerPixel = Convertir_Endian_To_Int(bytes, 2, 28);

            // we don't handle less than 1 byte per pixel
            if (BitsPerPixel != 24) {
                throw new Exception("Bits per pixel must be 24.");
            }
            
            // check file length
            if (Offset + BitsPerPixel*Width*Height/8 > bytes.Length) {
                throw new Exception("File length is not correct");
            }
            Pixels = ParsePixels(bytes);
        }

        /// <summary>
        /// Create a new image from a 2D array of pixels
        /// </summary>
        private MyImage(MyImage image, Pixel[,] pixels) {
            Type = image.Type;
            Size = image.Size;
            Offset = image.Offset;
            Width = image.Width;
            Height = image.Height;
            BitsPerPixel = image.BitsPerPixel;
            DIBHeaderSize = image.DIBHeaderSize;
            Pixels = pixels;
        }

        #endregion

        #region Serialization
        /// <summary>
        /// Deserialize a BMP grid from a byte array.
        /// </summary>
        /// <param name="bytes">The byte array to deserialize</param>
        /// <returns>A 2D array of pixels</returns>
        private Pixel[,] ParsePixels(byte[] bytes) {
            int RowSize = (int) Math.Ceiling((double)Width * BitsPerPixel / 32) * 4;
            Size = Offset + RowSize * Height;
            var px = new Pixel[Height, Width];
            int pos = Offset;

            for (int i = 0; i < Height; i++) {
                pos = Offset + RowSize * i;
                for (int j = 0; j < Width; j++) {
                    px[i, j] = new Pixel(
                        Convertir_Endian_To_Int(bytes, BytesPerColor, pos),
                        Convertir_Endian_To_Int(bytes, BytesPerColor, pos + BytesPerColor),
                        Convertir_Endian_To_Int(bytes, BytesPerColor, pos + 2 * BytesPerColor)
                    );
                    pos += BytesPerPixel;
                }
            }
            return px;
        }

        /// <summary>
        /// Convert n bits from tab[pos] to a little endian int
        /// </summary>
        /// <param name="tab">The array</param>
        /// <param name="bits">The size of the integer in bytes</param>
        /// <param name="pos">The position in the array</param>
        /// <returns>The little endian int</returns>
        private int Convertir_Endian_To_Int(byte[] tab, int bytes, int pos) {
            // convert n bits from tab[pos] to a little endian int
            int res = 0;
            for (int i = 0; i < bytes; i++) {
                res += tab[pos + i] << (i * 8);
            }
            return res;
        }


        /// <summary>
        /// Convert n to a little endian byte array of size bits
        /// We prefer the next method because it avoids allocating a new array.
        /// </summary>
        /// <param name="n">The number to convert</param>
        /// <param name="bits">The size of the integer in bits</param>
        /// <returns>The little endian byte array</returns>
        private byte[] Convertir_Int_To_Endian(int n, int bytes) {
            // convert n to a little endian byte array of size bits
            byte[] res = new byte[bytes];
            for (int i = 0; i < bytes; i++) {
                res[i] = (byte)(n >> (i * 8));
            }
            return res;
        }
        
        /// <summary>
        /// Convert n to a little endian byte array of size bits
        /// This method avoids allocating a new array.
        /// </summary>
        /// <param name="n">The number to convert</param>
        /// <param name="bits">The size of the integer in bits</param>
        /// <param name="pos">The position in the array</param>
        /// <param name="tab">The array</param>
        private void Convertir_Int_To_Endian(int val, byte[] tab, int bytes, int pos) {
            // convert n bits from tab[pos] to a little endian int
            for (int i = 0; i < bytes; i++) {
                tab[pos + i] = (byte) (val >> (i * 8));
            }
        }

        public void Save(string path) {
            int RowSize = (int) Math.Ceiling((double)Width * BitsPerPixel / 32) * 4;
            Size = Offset + RowSize * Height;

            // Console.WriteLine("RowSize: " + RowSize);

            var bytes = new byte[Size];
            // headers
            Type.CopyTo(bytes, 0);
            Convertir_Int_To_Endian(Size, bytes, 4, 2);
            Convertir_Int_To_Endian(Offset, bytes, 4, 10);
            Convertir_Int_To_Endian(DIBHeaderSize, bytes, 4, 14);
            Convertir_Int_To_Endian(Width, bytes, 4, 18);
            Convertir_Int_To_Endian(Height, bytes, 4, 22);
            Convertir_Int_To_Endian(1, bytes, 2, 26);
            Convertir_Int_To_Endian(BitsPerPixel, bytes, 2, 28);

            int pos = Offset;

            for (int i = 0; i < Height; i++) {
                pos = i*RowSize + Offset;
                for (int j = 0; j < Width; j++) {
                    Convertir_Int_To_Endian(Pixels[i, j].Blue, bytes, BytesPerColor, pos);
                    pos += BytesPerColor;
                    Convertir_Int_To_Endian(Pixels[i, j].Green, bytes, BytesPerColor, pos);
                    pos += BytesPerColor;
                    Convertir_Int_To_Endian(Pixels[i, j].Red, bytes, BytesPerColor, pos);
                    pos += BytesPerColor;
                }
            }

            File.WriteAllBytes(path, bytes);
        }

        #endregion

        #region Operations
        public void Invert() {
            for (int i = 0; i < Height; i++) {
                for (int j = 0; j < Width; j++) {
                    Pixels[i, j].Invert();
                }
            }
        }

        /// <summary>
        /// Rotate the image by 90 degrees.
        /// </summary>
        private void Rotate() {
            Pixel[,] newPixels = new Pixel[Width, Height];
            for (int i = 0; i < Height; i++) {
                for (int j = 0; j < Width; j++) {
                    newPixels[j, Height - i - 1] = Pixels[i, j];
                }
            }
            Height = newPixels.GetLength(0);
            Width = newPixels.GetLength(1);
            Pixels = newPixels;
        }

        public void Rotate(int n = 1) {
            if (n < 0) n = 2 - n;
            n %= 4;
            for (int i = 0; i < n; i++) {
                Rotate();
            }
        }

        /// <summary>
        /// Resize the image by a factor.
        /// </summary>
        /// <param name="factor">The factor</param>
        public void Resize(double factor) {
            if (factor == 1) return;

            Pixel[,] newPixels = new Pixel[(int) (Height * factor), (int) (Width * factor)];
            
            if (factor > 1) {
                for (int i = 0; i < Height; i++) {
                    for (int j = 0; j < Width; j++) {
                        // to upscale, we just duplicate the pixels
                        for (int k = 0; k < factor; k++) {
                            for (int l = 0; l < factor; l++) {
                                newPixels[(int) (i * factor + k), (int) (j * factor + l)] = Pixels[i, j].Clone();
                            }
                        }
                    }
                }
            } else {
                int factor_inv = (int) (1/factor);
                // to downscale, each pixel in the new image is the average of factor_inv^2 pixels in the old image
                for (int i = 0; i < newPixels.GetLength(0); i++) {
                    for (int j = 0; j < newPixels.GetLength(1); j++) {
                        int r = 0, g = 0, b = 0;
                        for (int k = 0; k < factor_inv; k++) {
                            for (int l = 0; l < factor_inv; l++) {
                                r += Pixels[(int) (i * factor_inv + k), (int) (j * factor_inv + l)].Red;
                                g += Pixels[(int) (i * factor_inv + k), (int) (j * factor_inv + l)].Green;
                                b += Pixels[(int) (i * factor_inv + k), (int) (j * factor_inv + l)].Blue;
                            }
                        }
                        newPixels[i, j] = new Pixel((byte) (b / (factor_inv * factor_inv)), (byte) (g / (factor_inv * factor_inv)), (byte) (r / (factor_inv * factor_inv)));
                    }
                }
            }
            
            Height = newPixels.GetLength(0);
            Width = newPixels.GetLength(1);
            Pixels = newPixels;
        }

        /// <summary>
        /// Crop the image.
        /// </summary>
        /// <param name="x1">The x coordinate of the top left corner</param>
        /// <param name="y1">The y coordinate of the top left corner</param>
        /// <param name="x2">The x coordinate of the bottom right corner</param>
        /// <param name="y2">The y coordinate of the bottom right corner</param>
        public void Crop(int x1, int y1, int x2, int y2) {
            if (x1 < 0 || x1 >= Width || x2 < 0 || x2 >= Width || y1 < 0 || y1 >= Height || y2 < 0 || y2 >= Height)
                throw new ArgumentException("Invalid coordinates");

            Pixel[,] newPixels = new Pixel[x2 - x1, y2 - y1];
            for (int i = y1; i < y2; i++) {
                for (int j = x1; j < x2; j++) {
                    newPixels[i - y1, j - x1] = Pixels[i, j];
                }
            }
            Width = newPixels.GetLength(0);
            Height = newPixels.GetLength(1);
            Pixels = newPixels;
            Size = Offset + Width * Height * BytesPerPixel;
        }
        
        /// <summary>
        /// Convert the image to grayscale.
        /// </summary>
        public void Grayscale() {
            for (int i = 0; i < Height; i++) {
                for (int j = 0; j < Width; j++) {
                    Pixels[i, j].Grayscale();
                }
            }
        }
        #endregion

        #region Filters
        /// <summary>
        /// Apply a "border detection" filter to the image (using convolution matrix).
        /// </summary>
        public void EdgeDetection() {
            int[,] matrix = new int[,] {
                {0, -1, 0},
                {-1, 4, -1},
                {0, -1, 0}
            };
            Convolution(matrix);
        }

        /// <summary>
        /// Apply a "sharpen" filter to the image (using convolution matrix).
        /// </summary>
        public void Sharpen() {
            int[,] matrix = new int[,] {
                {0, -1, 0},
                {-1, 5, -1},
                {0, -1, 0}
            };
            Convolution(matrix);
        }

        /// <summary>
        /// Apply a "gaussian blur 3x3" filter to the image (using convolution matrix).
        /// </summary>
        public void GaussianBlur3() {
            int[,] matrix = new int[,] {
                {1, 2, 1},
                {2, 4, 2},
                {1, 2, 1}
            };
            Convolution(matrix, 16);
        }

        /// <summary>
        /// Apply a "gaussian blur 5x5" filter to the image (using convolution matrix).
        /// </summary>
        public void GaussianBlur5() {
            int[,] matrix = new int[,] {
                {1, 4, 6, 4, 1},
                {4, 16, 24, 16, 4},
                {6, 24, 36, 24, 6},
                {4, 16, 24, 16, 4},
                {1, 4, 6, 4, 1}
            };
            Convolution(matrix, 256);
        }

        /// <summary>
        /// Apply a "unsharp mask 5x5" filter to the image (using convolution matrix).
        /// </summary>
        public void UnsharpMask5() {
            int[,] matrix = new int[,] {
                {1, 4, 6, 4, 1},
                {4, 16, 24, 16, 4},
                {6, 24, -476, 24, 6},
                {4, 16, 24, 16, 4},
                {1, 4, 6, 4, 1}
            };
            Convolution(matrix, -256);
        }



        /// <summary>
        /// Apply a "blur" filter to the image (using convolution matrix).
        /// </summary>
        public void BoxBlur() {
            int[,] matrix = new int[,] {
                {1, 1, 1},
                {1, 1, 1},
                {1, 1, 1}
            };
            Convolution(matrix, 9);
        }
        
        

        /// <summary>
        /// Apply the `mat` matrix to the image.
        /// </summary>
        /// <param name="mat">The matrix to apply</param>
        /// <param name="div">The divisor</param>
        public void Convolution(int[,] mat, int div = 1) {
            Pixel[,] newPixels = new Pixel[Height, Width];
            for (int i = 0; i < Height; i++) {
                for (int j = 0; j < Width; j++) {
                    int red = 0;
                    int green = 0;
                    int blue = 0;
                    for (int k = 0; k < mat.GetLength(0); k++) {
                        for (int l = 0; l < mat.GetLength(1); l++) {
                            int x = i + k - mat.GetLength(0) / 2;
                            int y = j + l - mat.GetLength(1) / 2;
                            if (x < 0 || x >= Height || y < 0 || y >= Width)
                                continue;
                            red += Pixels[x, y].Red * mat[k, l];
                            green += Pixels[x, y].Green * mat[k, l];
                            blue += Pixels[x, y].Blue * mat[k, l];
                        }
                    }
                    red /= div;
                    green /= div;
                    blue /= div;
                    newPixels[i, j] = new Pixel(
                        Math.Min(Math.Max(blue, 0), 255),
                        Math.Min(Math.Max(green, 0), 255),
                        Math.Min(Math.Max(red, 0), 255)
                    );
                }
            }
            Pixels = newPixels;
        }

        #endregion

        #region Utils
        public MyImage Clone() {
            Pixel[,] newPixels = new Pixel[Height, Width];
            for (int i = 0; i < Height; i++) {
                for (int j = 0; j < Width; j++) {
                    newPixels[i, j] = Pixels[i, j].Clone();
                }
            }

            return new MyImage(this, newPixels);
        }

        /// Shows all properties.
        /// the matrix of pixels is shown by rows.
        public string Repr() {
            string s = "";
            s += $"Type: {Type}\n";
            s += $"Size: {Size}\n";
            s += $"Offset: {Offset}\n";
            s += $"Width: {Width}\n";
            s += $"Height: {Height}\n";
            s += $"BitsPerPixel: {BitsPerPixel}\n";
            return s;
        }

        #endregion
        
    }

    
}