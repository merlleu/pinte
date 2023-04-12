using System;
using System.Collections.Generic;
using System.Linq;

namespace PSILib;

/// This class handles the Huffman compression algorithm
public class Huffman1D {
    private Dictionary<int, int> occurences;
    private List<PixelNode> nodes;
    private PixelNode root;
    private Pixel[,] matrix;
    private Dictionary<int, int> pixelToBytes;
    private int currentId = 0;

    /// <summary>
    /// Create a new Huffman1D instance.
    /// </summary>
    /// <param name="matrix">The matrix of the right size. An invalid size passed would not work.</param>
    public Huffman1D(Pixel[,] matrix) {
        this.matrix = matrix;
    }

    /// <summary>
    /// Find the frequencies of each pixel in the matrix.
    /// </summary>
    private void FindFrequencies() {
        occurences = new Dictionary<int, int>();
        for (int i = 0; i < matrix.GetLength(0); i++) {
            for (int j = 0; j < matrix.GetLength(1); j++) {
                int id = PixelID(matrix[i, j]);
                if (occurences.ContainsKey(id)) {
                    occurences[id]++;
                } else {
                    occurences[id] = 1;
                }
            }
        }
    }

    /// <summary>
    /// Build the Huffman tree.
    /// To do so, we sort the nodes by their frequency, then we merge the two
    /// nodes with the lowest frequency. We repeat this process until we have
    /// only one node left: the root of the tree.
    /// </summary>
    private void BuildTree() {
        nodes = new List<PixelNode>();
        foreach (var pair in occurences) {
            nodes.Add(new PixelNode {
                Pixel = pair.Key,
                Count = pair.Value
            });
        }

        

        while (nodes.Count > 1) {
            nodes.Sort((a, b) => a.Count - b.Count);
            
            PixelNode left = nodes[0];
            PixelNode right = nodes[1];
            nodes.Remove(left);
            nodes.Remove(right);
            nodes.Add(new PixelNode {
                Left = left,
                Right = right,
                Count = left.Count + right.Count
            });
        }

        root = nodes[0];
    }

    /// <summary>
    /// Assign an ID to each node of the tree.
    /// </summary>
    private void IndexTree(PixelNode node) {
        if (node.IsLeaf) {
            pixelToBytes[node.Pixel] = currentId;
            currentId++;
        } else {
            IndexTree(node.Left);
            IndexTree(node.Right);
        }
    }

    /// <summary>
    /// Encode the matrix using the Huffman tree.
    /// </summary>
    /// <returns>The encoded matrix</returns>
    public byte[] Encode() {
        Console.WriteLine("Encoding...");
        FindFrequencies();
        Console.WriteLine("Frequencies found.");
        BuildTree();
        Console.WriteLine("Tree built.");
        IndexTree(root);
        Console.WriteLine("Tree indexed.");
        
        // clear unused data
        nodes = null;
        occurences = null;

        // encode the tree
        var treeBytes = new List<byte>();
        // number of nodes
        WriteVarInt(currentId, treeBytes);

        // write the nodes
        for(int i = 0; i < currentId; i++) {
            treeBytes.Add((byte) (pixelToBytes[i] >> 16));
            treeBytes.Add((byte) (pixelToBytes[i] >> 8));
            treeBytes.Add((byte) pixelToBytes[i]);
        }

        // encode the matrix
        for (int i = 0; i < matrix.GetLength(0); i++) {
            for (int j = 0; j < matrix.GetLength(1); j++) {
                int id = PixelID(matrix[i, j]);
                WriteVarInt(pixelToBytes[id], treeBytes);
            }
        }

        return treeBytes.ToArray();
    }

    /// <summary>
    /// Decode the matrix using the Huffman tree.
    /// </summary>
    /// <param name="buffer">The encoded buffer</param>
    public void Decode(byte[] buffer, uint index) {
        // read the header
        int nodeCount = ReadVarInt(buffer, ref index);

        // read the nodes
        var bytesToPixel = new Pixel[nodeCount];
        for (int i = 0; i < nodeCount; i++) {
            Pixel px = new Pixel(buffer[index], buffer[index + 1], buffer[index + 2]);
            index += 3;
            bytesToPixel[i] = px;
        }

        // decode the matrix
        for (int i = 0; i < matrix.GetLength(0); i++) {
            for (int j = 0; j < matrix.GetLength(1); j++) {
                int id = ReadVarInt(buffer, ref index);
                matrix[i, j] = bytesToPixel[id].Clone();
            }
        }
        
    }


    /// <summary>
    /// Convert a pixel to an int to use it as a key in a dictionnary.
    /// </summary>
    private int PixelID(Pixel pixel) {
        return pixel.Red << 16 | pixel.Green << 8 | pixel.Blue;
    }


    /// <summary>
    /// Read int from a byte array.
    /// This uses the variable-length encoding of integers:
    /// We check the first bit of each byte. If it is 1, we continue reading the next byte.
    /// If it is 0, we stop reading.
    /// => the smaller the number, the less bytes we need to represent it.
    /// </summary>
    private int ReadVarInt(byte[] bytes, ref uint index) {
        int value = 0;
        int shift = 0;
        while (true) {
            byte b = bytes[index];
            index++;
            value |= (b & 0x7F) << shift;
            if ((b & 0x80) == 0) {
                break;
            }
            shift += 7;
        }
        return value;
    }

    /// <summary>
    /// Write an int to a byte array.
    /// This uses the variable-length encoding of integers.
    /// </summary>
    private void WriteVarInt(int value, List<byte> bytes) {
        while (true) {
            byte b = (byte) (value & 0x7F);
            value >>= 7;
            if (value == 0) {
                bytes.Add(b);
                break;
            } else {
                bytes.Add((byte) (b | 0x80));
            }
        }
    }


}

public class PixelNode {
    public int Pixel { get; set; }
    public PixelNode Left { get; set; }
    public PixelNode Right { get; set; }
    public int Count { get; set; }

    public bool IsLeaf => Left == null && Right == null;
}