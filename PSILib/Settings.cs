namespace PSILib;

/// <summary>
/// The settings when importing an image from bmp / applying transformations on it.
/// The main goal is to prevent DOS attacks. (e.g a fake image highly compressed to allocate way too much memory).
/// </summary>
public class ImageSettings {
    public int MaxWidth { get; set; } // in pixels
    public int MaxHeight { get; set; } // in pixels
    public int MaxSize { get; set; } // width * height

    /// <summary>
    /// The default settings.
    /// </summary>
    /// <remarks>
    /// MaxWidth = 1000
    /// MaxHeight = 1000
    /// MaxSize = 1_000_000
    public ImageSettings() {
        MaxWidth = 1000;
        MaxHeight = 1000;
        MaxSize = MaxWidth * MaxHeight;
    }

    /// <summary>
    /// Create a new image settings.
    /// </summary>
    /// <param name="maxWidth">The maximum width of the image.</param>
    /// <param name="maxHeight">The maximum height of the image.</param>
    /// <param name="maxSize">The maximum size (width*height) of the image.</param>
    public ImageSettings(int maxWidth, int maxHeight, int maxSize) {
        SetMaxWidth(maxWidth);
        SetMaxHeight(maxHeight);
        SetMaxSize(maxSize);
    }

    /// <summary>
    /// String representation of the settings.
    /// </summary>
    /// <returns>The string representation of the settings.</returns>
    public override string ToString() {
        return $"ImageSettings(maxWidth: {MaxWidth}, maxHeight: {MaxHeight}, maxSize: {MaxSize})";
    }


    /// <summary>
    /// Set the maximum width of the image.
    /// </summary>
    /// <param name="maxWidth">The maximum width of the image, must be 1px or more.</param>
    /// <returns>The settings.</returns>
    public ImageSettings SetMaxWidth(int maxWidth) {
        MaxWidth = ValidateSupToZero(maxWidth, "maxWidth");
        return this;
    }

    /// <summary>
    /// Set the maximum height of the image.
    /// </summary>
    /// <param name="maxHeight">The maximum height of the image, must be 1px or more.</param>
    /// <returns>The settings.</returns>
    public ImageSettings SetMaxHeight(int maxHeight) {
        MaxHeight = ValidateSupToZero(maxHeight, "maxHeight");
        return this;
    }

    /// <summary>
    /// Set the maximum size of the image.
    /// </summary>
    /// <param name="maxSize">The maximum size of the image, must be 1px or more.</param>
    /// <returns>The settings.</returns>
    public ImageSettings SetMaxSize(int maxSize) {
        MaxSize = ValidateSupToZero(maxSize, "maxSize");
        return this;
    }

    /// <summary>
    /// Validate that a value is superior to 0.
    /// Throws an ArgumentOutOfRangeException if the value is inferior to 0, otherwise returns the value.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="name">The name of the value.</param>
    /// <returns>The value.</returns>
    private int ValidateSupToZero(int value, string name) {
        if (value < 1) {
            throw new ArgumentOutOfRangeException(name, $"{name} must be greater than 0.");
        }
        return value;
    }

    /// <summary>
    /// Check if the image size is valid.
    /// Throws an ArgumentOutOfRangeException if the image size is too big, otherwise does nothing.
    /// </summary>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    public void CheckMaxSize(int width, int height) {
        if (width * height > MaxSize) {
            throw new ArgumentOutOfRangeException("width", $"The image size is too big. Max size is {MaxSize}, but the image size is {width * height}.");
        }

        if (width > MaxWidth) {
            throw new ArgumentOutOfRangeException("width", $"The image width is too big. Max width is {MaxWidth}, but the image width is {width}.");
        }

        if (height > MaxHeight) {
            throw new ArgumentOutOfRangeException("height", $"The image height is too big. Max height is {MaxHeight}, but the image height is {height}.");
        }
    }


}

/// <summary>
/// The settings when exporting an image to bmp.
/// </summary>
public class ExportSettings {
    #region Properties
    /// <summary>
    /// The compression mode of the image.
    /// </summary>
    /// <remarks>
    /// 0 - BI_RGB
    /// 1 - BI_RLE8
    /// 2 - BI_RLE4
    /// 3 - BI_BITFIELDS
    /// </remarks>
    public int CompressionMode { get; private set; }
    /// <summary>
    /// The number of bits per pixel.
    /// </summary>
    /// <remarks>
    /// 1 - 2 colors
    /// 4 - 16 colors
    /// 8 - 256 colors
    /// 16 - 65536 colors
    /// 24 - (R, G, B): 256 * 256 * 256 colors
    /// 32 - (R, G, B, A): 256 * 256 * 256 * 256 colors
    /// </remarks>
    public int BitsPerPixel { get; private set; }
    #endregion

    #region Constructors
    /// <summary>
    /// The default settings.
    /// </summary>
    public ExportSettings() {
        CompressionMode = 0;
        BitsPerPixel = 24;
    }

    /// <summary>
    /// The settings with the given compression mode and bits per pixel.
    /// </summary>
    /// <param name="compressionMode">The compression mode of the image.</param>
    /// <param name="bitsPerPixel">The number of bits per pixel.</param>
    public ExportSettings(int compressionMode, int bitsPerPixel) {
        SetCompressionMode(compressionMode);
        SetBitsPerPixel(bitsPerPixel);
    }
    #endregion

    #region Serialization
    /// <summary>
    /// String representation of the settings.
    /// </summary>
    /// <returns>The string representation of the settings.</returns>
    public override string ToString() {
        return $"ExportSettings(compressionMode: {CompressionMode}, bitsPerPixel: {BitsPerPixel})";
    }
    #endregion

    #region Setters
    /// <summary>
    /// Set the compression mode of the image.
    /// </summary>
    /// <param name="compressionMode">The compression mode of the image, default: BI_RGB = 0 (no compression).</param>
    /// <remarks>
    /// 0 - BI_RGB
    /// 1 - BI_RLE8
    /// 2 - BI_RLE4
    /// 3 - BI_BITFIELDS
    /// </remarks>
    /// <returns>The settings.</returns>
    public ExportSettings SetCompressionMode(int compressionMode) {
        if (compressionMode < 0 || compressionMode > 3) {
            throw new ArgumentOutOfRangeException("compressionMode", "Compression mode must be between 0 and 3.");
        }

        CompressionMode = compressionMode;
        return this;
    }


    /// <summary>
    /// Set the number of bits per pixel.
    /// </summary>
    /// <param name="bitsPerPixel">The number of bits per pixel, default: 24.</param>
    /// <remarks>
    /// 1 - 2 colors
    /// 4 - 16 colors
    /// 8 - 256 colors
    /// 16 - 65536 colors
    /// 24 - (R, G, B): 256 * 256 * 256 colors
    /// 32 - (R, G, B, A): 256 * 256 * 256 * 256 colors
    public ExportSettings SetBitsPerPixel(int bitsPerPixel) {
        if (
            bitsPerPixel != 1 &&
            bitsPerPixel != 4 &&
            bitsPerPixel != 8 &&
            bitsPerPixel != 16 &&
            bitsPerPixel != 24 &&
            bitsPerPixel != 32
        ) {
            throw new ArgumentOutOfRangeException("bitsPerPixel", "Bits per pixel must be 1, 4, 8, 16, 24 or 32.");
        }

        BitsPerPixel = bitsPerPixel;
        return this;
    }
    #endregion
}