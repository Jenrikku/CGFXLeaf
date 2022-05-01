using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGFXLeaf {
    internal static class Extensions {
        public static bool[] ReadBits(this BinaryDataReader reader, int byteAmount) {
            foreach(byte bits in reader.ReadBytes(byteAmount)) {
                //bits & 0b_1000_0000;
            }
        }
    }
}
