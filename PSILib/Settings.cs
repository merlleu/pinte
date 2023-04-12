namespace PSILib;

/// <summary>
/// The settings when importing an image from bmp / applying transformations on it.
/// The main goal is to prevent DOS attacks. (e.g a fake image highly compressed to allocate way too much memory).
/// </summary>
public class ImageSettings {
    public uint MaxWidth { get; set; } // in pixels
    public uint MaxHeight { get; set; } // in pixels
    public uint MaxSize { get; set; } // width * height

    

    /// <summary>
    /// The default settings.
    /// </summary>
    /// <remarks>
    /// MaxWidth = 1_000_000
    /// MaxHeight = 1_000_000
    /// MaxSize = 1_000_000_000
    public ImageSettings() {
        MaxWidth = 1_000_000;
        MaxHeight = 1_000_000;
        MaxSize = 1_000_000_000;
    }

    /// <summary>
    /// Create a new image settings.
    /// </summary>
    /// <param name="maxWidth">The maximum width of the image.</param>
    /// <param name="maxHeight">The maximum height of the image.</param>
    /// <param name="maxSize">The maximum size (width*height) of the image.</param>
    public ImageSettings(uint maxWidth, uint maxHeight, uint maxSize) {
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
    public ImageSettings SetMaxWidth(uint maxWidth) {
        MaxWidth = maxWidth;
        return this;
    }

    /// <summary>
    /// Set the maximum height of the image.
    /// </summary>
    /// <param name="maxHeight">The maximum height of the image, must be 1px or more.</param>
    /// <returns>The settings.</returns>
    public ImageSettings SetMaxHeight(uint maxHeight) {
        MaxHeight = maxHeight;
        return this;
    }

    /// <summary>
    /// Set the maximum size of the image.
    /// </summary>
    /// <param name="maxSize">The maximum size of the image, must be 1px or more.</param>
    /// <returns>The settings.</returns>
    public ImageSettings SetMaxSize(uint maxSize) {
        MaxSize = maxSize;
        return this;
    }

    /// <summary>
    /// Check if the image size is valid.
    /// Throws an ArgumentOutOfRangeException if the image size is too big, otherwise does nothing.
    /// </summary>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    public void CheckMaxSize(uint width, uint height) {
        if (MaxSize != 0 && width * height > MaxSize) {
            throw new ArgumentOutOfRangeException("width", $"The image size is too big. Max size is {MaxSize}, but the image size is {width * height}.");
        }

        if (MaxWidth != 0 && width > MaxWidth) {
            throw new ArgumentOutOfRangeException("width", $"The image width is too big. Max width is {MaxWidth}, but the image width is {width}.");
        }

        if (MaxHeight != 0 && height > MaxHeight) {
            throw new ArgumentOutOfRangeException("height", $"The image height is too big. Max height is {MaxHeight}, but the image height is {height}.");
        }
    }


}