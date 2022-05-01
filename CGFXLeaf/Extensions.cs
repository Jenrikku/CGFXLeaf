using Syroot.BinaryData;
using System.Collections;

namespace CGFXLeaf {
    internal static class Extensions {
       internal static bool[] ReadBits(this BinaryDataReader reader, int byteAmount) {
            bool[] bits = new bool[byteAmount * 8];
            new BitArray(reader.ReadBytes(byteAmount)).CopyTo(bits, 0);

            return bits;
        }
    }
}
