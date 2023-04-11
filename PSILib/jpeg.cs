namespace PSILib;


/// <summary>
/// [!] We finally did not do the JPEG part, but we kept the code in case we want to do it later.
/// </summary>

// public class FromJPEG {
//     private JPEGComponent[] components;
//     private uint height;
//     private uint width;

//     private List<HuffmanTable> huffmanTables;
//     private List<QuantizationTable> quantizationTables;

//     public FromJPEG(string path) {
//         var buffer = File.ReadAllBytes(path);
//         Read(buffer);
//     }

//     private void Read(byte[] buffer) {
//         uint i = 0;
//         while (i < buffer.Length) {
//             var marker = buffer[i];
//             var length = MyImage.Convertir_Endian_To_Int(buffer, i + 1, 2);
//             if (marker == 0xFF) {
//                 switch (marker) {
//                     case 0xC0:
//                         // SOF0
//                         ReadSOF0(buffer, i + 3, length);
//                         break;
//                     case 0xC4:
//                         // DHT
//                         ReadDHT(buffer, i + 3, length);
//                         break;
//                     case 0xDB:
//                         // DQT
//                         ReadDQT(buffer, i + 3, length);
//                         break;
//                     case 0xDA:
//                         // SOS
//                         ReadSOS(buffer, i + 3, length);
//                         break;
//                     case 0xD9:
//                         // EOI
//                         ReadEOI(buffer, i + 3, length);
//                         break;
//                     default:
//                         Console.WriteLine("Unknown marker: " + marker);
//                         break;
//                 }
//             }

//             i += length + 2;
//         }
//     }

//     private void ReadSOF0(byte[] buffer, uint offset, uint length) {
//         height = MyImage.Convertir_Endian_To_Int(buffer, offset, 2);
//         offset += 2;
//         width = MyImage.Convertir_Endian_To_Int(buffer, offset, 2);
//         offset += 2;
//         var componentsLength = MyImage.Convertir_Endian_To_Int(buffer, offset, 1);
//         offset += 1;

//         components = new JPEGComponent[componentsLength];

//         for (int i = 0; i < componentsLength; i++) {
//             var id = MyImage.Convertir_Endian_To_Int(buffer, offset, 1);
//             offset += 1;
//             var samplingFactor = MyImage.Convertir_Endian_To_Int(buffer, offset, 1);
//             offset += 1;
//             var quantizationTable = MyImage.Convertir_Endian_To_Int(buffer, offset, 1);
//             offset += 1;
//             components[i] = new JPEGComponent(id, samplingFactor, quantizationTable);
//         }
//     }

//     /// <summary>
//     /// Read the Huffman table
//     /// </summary>
//     private void ReadDHT(byte[] buffer, uint offset, uint length) {
//         while (offset < length) {
//             var tableClass = MyImage.Convertir_Endian_To_Int(buffer, offset, 1);
//             offset += 1;
//             var tableId = MyImage.Convertir_Endian_To_Int(buffer, offset, 1);
//             offset += 1;
//             var nbSymbols = MyImage.Convertir_Endian_To_Int(buffer, offset, 1);
//             offset += 1;

//             var table = new uint[256];

//             for (int i = 0; i < nbSymbols; i++) {
//                 table[i] = MyImage.Convertir_Endian_To_Int(buffer, offset, 1);
//                 offset += 1;
//             }

//             var huffmanTable = new HuffmanTable(table, tableClass, tableId);
//             huffmanTables.Add(huffmanTable);
//         }
//     }

//     /// <summary>
//     /// Read the quantization table
//     /// </summary>
//     private void ReadDQT(byte[] buffer, uint offset, uint length) {
//         while (offset < length) {
//             var tablePrecision = MyImage.Convertir_Endian_To_Int(buffer, offset, 1);
//             offset += 1;
//             var tableId = MyImage.Convertir_Endian_To_Int(buffer, offset, 1);
//             offset += 1;
//             var table = new uint[64];
//             for (int i = 0; i < 64; i++) {
//                 table[i] = MyImage.Convertir_Endian_To_Int(buffer, offset, 1);
//                 offset += 1;
//             }

//             var quantizationTable = new QuantizationTable(table, tablePrecision, tableId);
//             quantizationTables.Add(quantizationTable);
//         }
//     }

//     /// <summary>
//     /// Read the start of scan
//     /// </summary>
//     private void ReadSOS(byte[] buffer, uint offset, uint length) {
//         var componentsLength = MyImage.Convertir_Endian_To_Int(buffer, offset, 1);
//         offset += 1;

//         for (int i = 0; i < componentsLength; i++) {
//             var id = MyImage.Convertir_Endian_To_Int(buffer, offset, 1);
//             offset += 1;
//             var tableSelector = MyImage.Convertir_Endian_To_Int(buffer, offset, 1);
//             offset += 1;
//         }

//         var startSpectralSelection = MyImage.Convertir_Endian_To_Int(buffer, offset, 1);
//         offset += 1;
//         var endSpectralSelection = MyImage.Convertir_Endian_To_Int(buffer, offset, 1);
//         offset += 1;
//         var successiveApproximation = MyImage.Convertir_Endian_To_Int(buffer, offset, 1);
//         offset += 1;
//     }

//     private void ReadEOI(byte[] buffer, uint offset, uint length) {
//         Console.WriteLine("End of Image");
//     }
        
// }

// class JPEGComponent {
//     public uint Id { get; set; }
//     public uint SamplingFactor { get; set; }
//     public uint QuantizationTable { get; set; }

//     public JPEGComponent(uint id, uint samplingFactor, uint quantizationTable) {
//         Id = id;
//         SamplingFactor = samplingFactor;
//         QuantizationTable = quantizationTable;
//     }
// }

// class HuffmanTable {
//     public uint[] Table { get; set; }
//     public uint Id { get; set; }
//     public uint TableClass { get; set; }

//     public HuffmanTable(uint[] table, uint id, uint tableClass) {
//         Table = table;
//         Id = id;
//         TableClass = tableClass;
//     }
// }

// class QuantizationTable {
//     public uint[] Table { get; set; }
//     public uint Id { get; set; }
//     public uint TablePrecision { get; set; }

//     public QuantizationTable(uint[] table, uint id, uint tablePrecision) {
//         Table = table;
//         Id = id;
//         TablePrecision = tablePrecision;
//     }
// }