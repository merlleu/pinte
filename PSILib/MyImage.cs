namespace PSILib
{
    using static Constants;
    public class MyImage
    {
        #region Properties
        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public uint BitsPerPixel { get; private set; }
        private Pixel[,] Pixels;
        #endregion

        #region Constructors

        /// <summary>
        /// Create a new image reading the file at the given path.
        /// </summary>
        /// <param name="path">Path to the image file.</param>
        public MyImage(string path) {
            var bytes = File.ReadAllBytes(path);
            if (bytes.Length < 54) {
                throw new ArgumentException("File is too small.");
            }

            // headers
            // check if it's a BMP file
            if (bytes[0] != BMP_MAGIC_0 || bytes[1] != BMP_MAGIC_1) {
                throw new ArgumentException("Not a BMP file");
            }

            uint size = Convertir_Endian_To_Int(bytes, 4, 2);
            uint offset = Convertir_Endian_To_Int(bytes, 4, 10);

            // DIB Header
            uint dib_header_size = Convertir_Endian_To_Int(bytes, 4, 14);
            if (dib_header_size != 40) {
                throw new ArgumentException("DIB Header size must be 40.");
            }

            Width = Convertir_Endian_To_Int(bytes, 4, 18);
            Height = Convertir_Endian_To_Int(bytes, 4, 22);

            uint planes = Convertir_Endian_To_Int(bytes, 2, 26);
            if (planes != 1) {
                throw new ArgumentException("Planes must be 1.");
            }

            BitsPerPixel = Convertir_Endian_To_Int(bytes, 2, 28);
            // for simplicity reasons, we only handle the 24BPP mode.
            if (BitsPerPixel != 24) {
                throw new ArgumentException("Bits per pixel must be 24.");
            }

            uint compression = Convertir_Endian_To_Int(bytes, 4, 30);
            if (compression != BI_RGB && compression != BI_BITFIELDS) {
                throw new ArgumentException("Compression must be BI_RGB or BI_BITFIELDS.");
            }

            // check file length
            if (Width*Height > 1_000_000_000) {
                throw new ArgumentException("Image is too big.");
            }
            if (offset + BitsPerPixel*Width*Height/8 > bytes.Length) {
                throw new ArgumentException("File length is not correct.");
            }
            Pixels = new Pixel[Height, Width];
            ParsePixels(bytes, offset);
        }

        public MyImage(uint Width, uint Height, uint BitsPerPixel, Pixel[,] Pixels) {
            this.Width = Width;
            this.Height = Height;
            this.BitsPerPixel = BitsPerPixel;
            this.Pixels = Pixels;
        }

        /// <summary>
        /// Create a new image from a 2D array of pixels
        /// </summary>
        private MyImage(MyImage image) {
            Pixels = new Pixel[image.Height, image.Width];
            for (int i = 0; i < image.Height; i++) {
                for (int j = 0; j < image.Width; j++) {
                    Pixels[i, j] = image.Pixels[i, j].Clone();
                }
            }
            Width = image.Width;
            Height = image.Height;
            BitsPerPixel = image.BitsPerPixel;
        }
        #endregion

        #region Serialization
        /// <summary>
        /// Deserialize a BMP grid from a byte array.
        /// </summary>
        /// <param name="bytes">The byte array to deserialize</param>
        /// <returns>A 2D array of pixels</returns>
        private void ParsePixels(byte[] bytes, uint offset) {
            uint RowSize = (uint) Math.Ceiling((double)Width * BitsPerPixel / 32) * 4;

            for (uint i = 0; i < Height; i++) {
                uint pos = offset + RowSize * i;
                for (int j = 0; j < Width; j++) {
                    Pixels[i, j] = new Pixel(
                        (byte)Convertir_Endian_To_Int(bytes, 1, pos),
                        (byte)Convertir_Endian_To_Int(bytes, 1, pos + 1),
                        (byte)Convertir_Endian_To_Int(bytes, 1, pos + 2)
                    );
                    pos += 3;
                }
            }
        }

        /// <summary>
        /// Convert n bits from tab[pos] to a little endian int
        /// </summary>
        /// <param name="tab">The array</param>
        /// <param name="bits">The size of the integer in bytes</param>
        /// <param name="pos">The position in the array</param>
        /// <returns>The little endian int</returns>
        private uint Convertir_Endian_To_Int(byte[] tab, uint bytes, uint pos) {
            // convert n bits from tab[pos] to a little endian int
            uint res = 0;
            for (int i = 0; i < bytes; i++) {
                res += (uint) tab[pos + i] << (i * 8);
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
        private byte[] Convertir_Int_To_Endian(uint n, uint bytes) {
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
        private void Convertir_Int_To_Endian(uint val, byte[] tab, uint bytes, uint pos) {
            // convert n bits from tab[pos] to a little endian int
            for (int i = 0; i < bytes; i++) {
                tab[pos + i] = (byte) (val >> (i * 8));
            }
        }

        /// <summary>
        /// Save the image to a file
        /// </summary>
        /// <param name="path">The path to the file</param>
        public void Save(string path) {
            uint RowSize = (uint) Math.Ceiling((double)Width * BitsPerPixel / 32) * 4;
            uint dib_header_size = 40;
            uint offset = 14 + dib_header_size;
            uint size = offset + RowSize * Height;

            var bytes = new byte[size];
            // headers
            bytes[0] = 66;
            bytes[1] = 77;
            Convertir_Int_To_Endian(size, bytes, 4, 2);
            Convertir_Int_To_Endian(offset, bytes, 4, 10);
            Convertir_Int_To_Endian(dib_header_size, bytes, 4, 14);
            Convertir_Int_To_Endian(Width, bytes, 4, 18);
            Convertir_Int_To_Endian(Height, bytes, 4, 22);
            Convertir_Int_To_Endian(1, bytes, 2, 26);
            Convertir_Int_To_Endian(BitsPerPixel, bytes, 2, 28);

            uint pos = offset;

            for (uint i = 0; i < Height; i++) {
                pos = i*RowSize + offset;
                for (int j = 0; j < Width; j++) {
                    Convertir_Int_To_Endian(Pixels[i, j].Blue, bytes, 1, pos);
                    pos += 1;
                    Convertir_Int_To_Endian(Pixels[i, j].Green, bytes, 1, pos);
                    pos += 1;
                    Convertir_Int_To_Endian(Pixels[i, j].Red, bytes, 1, pos);
                    pos += 1;
                }
            }

            File.WriteAllBytes(path, bytes);
        }

        #endregion

        #region Operations
        /// <summary>
        /// Rotate the image by a given angle.
        /// If the angle is not a multiple of 90, the image will be enlarged with black pixels.
        /// Note that artifacts may appear with these angles,
        /// to reduce them, upscale the image before rotating, then downscale it again.
        /// Also, you can use the RemoveBorder method to resize the image after rotating it multiple times to remove the black pixels.
        /// </summary>
        /// <param name="angle">The angle in degrees</param>
        /// <param name="resize_factor">The factor to resize the image by</param>
        public void Rotate(int angle, double resize_factor = 1) {
            if (angle % 90 != 0 && resize_factor != 1) {
                Resize(resize_factor);
            }
            double angleR = angle * Math.PI / 180;
            uint nw = (uint)Math.Ceiling(Math.Abs(Width * Math.Cos(angleR)) + Math.Abs(Height * Math.Sin(angleR)));
            uint nh = (uint)Math.Ceiling(Math.Abs(Width * Math.Sin(angleR)) + Math.Abs(Height * Math.Cos(angleR)));

            Pixel[,] newPixels = new Pixel[nh, nw];
            for (int row = 0; row < nh; row++) {
                for (int col = 0; col < nw; col++) {
                    newPixels[row, col] = new Pixel();
                }
            }
            
            for (int row = 0; row < Height; row++) {
                for (int col = 0; col < Width; col++) {
                    uint x = (uint)Math.Round((col - Width / 2) * Math.Cos(angleR) - (row - Height / 2) * Math.Sin(angleR) + nw / 2);
                    uint y = (uint)Math.Round((col - Width / 2) * Math.Sin(angleR) + (row - Height / 2) * Math.Cos(angleR) + nh / 2);

                    if (x < nw && y < nh) newPixels[y, x] = Pixels[row, col];

                    if (x > 0 && x < nw - 1 && y > 0 && y < nh - 1) {
                        newPixels[y, x + 1].UpdateIfEmpty(Pixels[row, col]);
                        newPixels[y, x - 1].UpdateIfEmpty(Pixels[row, col]);
                        newPixels[y + 1, x].UpdateIfEmpty(Pixels[row, col]);
                        newPixels[y - 1, x].UpdateIfEmpty(Pixels[row, col]);
                    }
                }
            }

            Width = nw;
            Height = nh;
            Pixels = newPixels;

            if (angle % 90 != 0 && resize_factor != 1) {
                Resize(1/resize_factor);
            }
        }


        /// <summary>
        /// Add a border to the image
        /// </summary>
        /// <param name="size">The size of the border</param>
        /// <param name="color">The color of the border</param>
        public void AddBorder(uint size, Pixel color) {
            uint nw = Width + 2 * size;
            uint nh = Height + 2 * size;
            Pixel[,] newPixels = new Pixel[nh, nw];
            for (int i = 0; i < nh; i++) {
                for (int j = 0; j < nw; j++) {
                    if (i < size || i >= Height + size || j < size || j >= Width + size) {
                        newPixels[i, j] = color;
                    } else {
                        newPixels[i, j] = Pixels[i - size, j - size];
                    }
                }
            }
            Height = (uint)newPixels.GetLength(0);
            Width = (uint)newPixels.GetLength(1);
            Pixels = newPixels;
        }

        
        /// <summary>
        /// Get the border length of the image for a given color.
        /// </summary>
        /// <param name="color">The color of the border</param>
        /// <returns>The border length</returns>
        public uint GetBorderLength(Pixel color) {
            // We iterate over each pixel of the n-th line/row parallel to a border.
            // If the pixel is not the color of the border, we stop the iteration.
    
            for (uint length = 0;; length++) {
                // Top/Bottom border
                for (int i = 0; i < Width; i++) {
                    if (!Pixels[length, i].Equals(color) || !Pixels[Height - length - 1, i].Equals(color)) {
                        return length;
                    }
                }

                // Left/Right border
                for (int i = 0; i < Height; i++) {
                    if (!Pixels[i, length].Equals(color) || !Pixels[i, Width - length - 1].Equals(color)) {
                        return length;
                    }
                }
            }
        }

        /// <summary>
        /// Remove the border of the image.
        /// </summary>
        /// <param name="color">The color of the border</param>
        /// <param name="max_length">The maximum length of the border to remove</param>
        public uint RemoveBorder(Pixel? color = null, int max_length = -1) {
            if (color == null) {
                color = new Pixel();
            }
            uint length = GetBorderLength(color);
            if (max_length > 0) {
                length = (uint)Math.Min(length, max_length);
            }

            if (length != 0) {
                uint nw = Width - 2 * length;
                uint nh = Height - 2 * length;
                Crop(length, length, nw, nh);
            }
            return length;
        }

        /// <summary>
        /// Resize the image by a factor.
        /// If the factor is greater than 1, the image is upscaled.
        /// If the factor is less than 1, the image is downscaled.
        /// If the factor is 1, nothing happens.
        /// </summary>
        /// <param name="factor">The factor</param>
        public void Resize(double factor) {
            if (factor == 1) return;

            Pixel[,] newPixels = new Pixel[(int) (Height * factor), (int) (Width * factor)];
            
            if (factor > 1) {
                Upscale_Default(newPixels, factor);
            } else {
                Downscale_Default(newPixels, factor);
            }
            
            Height = (uint)newPixels.GetLength(0);
            Width = (uint)newPixels.GetLength(1);
            Pixels = newPixels;
        }

        /// <summary>
        /// Upscale the image by a factor.
        /// </summary>
        /// <param name="newPixels">The new pixels matrix</param>
        /// <param name="factor">The factor</param>
        private void Upscale_Default(Pixel[,] newPixels, double factor) {
            for (int i = 0; i < newPixels.GetLength(0); i++) {
                for (int j = 0; j < newPixels.GetLength(1); j++) {
                    newPixels[i, j] = Pixels[(int) (i/factor), (int) (j/factor)];
                }
            }
        }

        /// <summary>
        /// Downscale the image by a factor.
        /// </summary>
        /// <param name="newPixels">The new pixels matrix</param>
        /// <param name="factor">The factor</param>
        private void Downscale_Default(Pixel[,] newPixels, double factor) {
            int factor_inv = (int) (1/factor);
            // to downscale, each pixel in the new image is the average of factor_inv^2 pixels in the old image
            for (int i = 0; i < newPixels.GetLength(0); i++) {
                for (int j = 0; j < newPixels.GetLength(1); j++) {
                    // we can't just average using pixel operations because of the possibility of overflow
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

        /// <summary>
        /// Crop the image.
        /// </summary>
        /// <param name="x1">The x coordinate of the bottom left corner</param>
        /// <param name="y1">The y coordinate of the bottom left corner</param>
        /// <param name="dx">The width of the crop</param>
        /// <param name="dy">The height of the crop</param>
        public void Crop(uint x, uint y, uint dx, uint dy) {
            if (x < 0 || y < 0) throw new ArgumentException("Invalid crop");
            CheckDimensions(x+dx, y+dy);

            Pixel[,] newPixels = new Pixel[dy, dx];
            for (int i = 0; i < dy; i++) {
                for (int j = 0; j < dx; j++) {
                    newPixels[i, j] = Pixels[y + i, x + j];
                }
            }
            Height = (uint)newPixels.GetLength(0);
            Width = (uint)newPixels.GetLength(1);
            Pixels = newPixels;
        }
        #endregion
        
        #region Comparisons
        /// <summary>
        /// Compares the image to another image.
        /// Mutate the image to highlight the differences.
        /// </summary>
        /// <param name="other">The other image</param>
        /// <returns>True if the images are equal, false otherwise</returns>
        public bool Diff(MyImage other) {
            if (Width != other.Width || Height != other.Height) {
                throw new ArgumentException("Images must have the same dimensions");
            }
            bool equal = true;
            for (int i = 0; i < Height; i++) {
                for (int j = 0; j < Width; j++) {
                    if (!Pixels[i, j].Equals(other.Pixels[i, j])) {
                        equal = false;
                        Pixels[i, j].DistanceAbs(other.Pixels[i, j]);
                    } else {
                        Pixels[i, j] = new Pixel();
                    }
                }
            }
            return equal;
        }
        #endregion
        
        #region Filters
        /// <summary>
        /// Revert all RGB colors of the image.
        /// </summary>
        public void Invert() {
            for (int i = 0; i < Height; i++) {
                for (int j = 0; j < Width; j++) {
                    Pixels[i, j].Invert();
                }
            }
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

        /// <summary>
        /// Apply a "border detection" filter to the image (using convolution matrix).
        /// </summary>
        /// <param name="discardImageArea">If true, the borders of the image are discarded.</param>
        /// <param name="gray">If true, the image is converted to grayscale before applying the filter.</param>
        public void EdgeDetection(bool discardImageArea = true, bool gray = true) {
            if (gray) Grayscale();

            int[,] matrix = new int[,] {
                {0, -1, 0},
                {-1, 4, -1},
                {0, -1, 0}
            };
            Convolution(matrix);

            // We found the borders of the new pixel matrix useless, so we discard them.
            if (discardImageArea) {
                for (int i = 0; i < Height; i++) {
                    Pixels[i, 0] = new Pixel(0, 0, 0);
                    Pixels[i, Width - 1] = new Pixel(0, 0, 0);
                }
                for (int i = 0; i < Width; i++) {
                    Pixels[0, i] = new Pixel(0, 0, 0);
                    Pixels[Height - 1, i] = new Pixel(0, 0, 0);
                }
            }
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
                        (byte)Math.Min(Math.Max(blue, 0), 255),
                        (byte)Math.Min(Math.Max(green, 0), 255),
                        (byte)Math.Min(Math.Max(red, 0), 255)
                    );
                }
            }
            Pixels = newPixels;
        }

        #endregion
        
        #region Steganography
        /// <summary>
        /// Hide the image `img` in the current image, at the position (x, y).
        /// This stores the N 
        /// </summary>
        /// <param name="img">The image to hide</param>
        /// <param name="x">The x position</param>
        /// <param name="y">The y position</param>
        public void HideImage(MyImage img, uint x, uint y, int bits = 4) {
            CheckDimensions(x+img.Width, y+img.Height);

            for (int i = 0; i < img.Height; i++) {
                for (int j = 0; j < img.Width; j++) {
                    Pixels[i + y, j + x].InsertLSB(img.Pixels[i, j], bits );
                }
            }
        }

        /// <summary>
        /// Extract an image of size (dx,dy) from the current image, at the position (x, y).
        /// </summary>
        /// <param name="x">The x position</param>
        /// <param name="y">The y position</param>
        /// <param name="dx">The width of the image to extract</param>
        /// <param name="dy">The height of the image to extract</param>
        public void ExtractImage(uint x=0, uint y=0, uint dx=0, uint dy=0, int bits = 4) {
            if (dx == 0) dx = Width;
            if (dy == 0) dy = Height;
            
            CheckDimensions(x+dx, y+dy);

            for (int i = 0; i < dy; i++) {
                for (int j = 0; j < dx; j++) {
                    Pixels[i + y, j + x].ExtractLSB(bits);
                }
            }
        }
        #endregion
        
        #region Utils
        /// <summary>
        /// Clone the image.
        /// </summary>
        public MyImage Clone() {
            return new MyImage(this);
        }

        /// <summary>
        /// Return a string representation of the image.
        /// </summary>
        public override string ToString() {
            return $"MyImage(Width: {Width}, Height: {Height}, BitsPerPixel: {BitsPerPixel})";
        }

        /// <summary>
        /// Checks wether the image dimensions are larger or equal to the given dimensions.
        /// </summary>
        /// <param name="width">The width to check</param>
        /// <param name="height">The height to check</param>
        public void CheckDimensions(uint width, uint height) {
            if (Width < width || Height < height)
                throw new Exception($"{this} is too small, required: (width: {width}, height: {height}).");
        }
        #endregion 
    } 
}