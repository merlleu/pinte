namespace PSILib;

/// This class handles the Huffman compression algorithm, each node of the tree is a Pixel (3bytes).
public class HuffmanPixel1D {
    private Pixel[,] Pixels;

    public HuffmanPixel1D(Pixel[,] pixels) {
        Pixels = pixels;
    }
}

public class PixelNode {
    public Pixel Pixel { get; set; }
    public PixelNode Left { get; set; }
    public PixelNode Right { get; set; }
}