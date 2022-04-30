using System.Collections.Generic;

namespace CGFXLeaf {
    public class CGFXDictionary : Dictionary<string, CGFXDictEntry> {
        public ushort HeaderUnk0;
        public byte[] HeaderUnk1 = new byte[10];
    }

    public class CGFXDictEntry {
        public ulong Unk;
    }
}
