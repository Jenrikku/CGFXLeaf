using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGFXLeaf {
    internal static class Extensions {
        static byte[] bitArray = new byte[] { 0x01, 0x02, 0x04,0x08, 0x10, 0x20, 0x40, 0x80 };
        public static bool[] ReadBits(this BinaryDataReader reader, int byteAmount) {
            byte[] bytes = reader.ReadBytes(byteAmount);
            bool[] bits = new bool[byteAmount * 8];
            for (int j = 0; j < bytes.Length; j++) {
                for (int i = 0; i < 8; i++)
                {
                    //if (bytes[j] & bitArray[j])
                    {

                    }
                }
            }
        }
    }
}
