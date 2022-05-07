using Syroot.BinaryData;
using System.Collections;
using System.Numerics;

namespace CGFXLeaf {
    internal static class Extensions {
        internal static bool[] ReadBits(this BinaryDataReader reader, int byteAmount) {
            bool[] bits = new bool[byteAmount * 8];
            new BitArray(reader.ReadBytes(byteAmount)).CopyTo(bits, 0);

            return bits;
        }

        internal static uint ReadRelativeOffset(this BinaryDataReader reader)
            => (uint) reader.Position + reader.ReadUInt32();

        internal static uint[] ReadRelativeOffsets(this BinaryDataReader reader, int amount) {
            uint[] relativeOffesets = new uint[amount];
            for(int i = 0; i < amount; i++) {
                relativeOffesets[i] = (uint) reader.Position + reader.ReadUInt32();
            }

            return relativeOffesets;
        }

        internal static Vector3 ReadVector3(this BinaryDataReader reader) {
            return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        internal static void MoveToRelativeOffset(this BinaryDataReader reader)
            => reader.Position += reader.ReadUInt32();
    }
}
