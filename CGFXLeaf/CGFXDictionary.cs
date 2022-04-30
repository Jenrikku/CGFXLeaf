using System.Collections.Generic;

namespace CGFXLeaf {
    public class CGFXDictionary : Dictionary<string, CGFXDictEntry> {
        public ushort HeaderUnk0;
        public byte[] HeaderUnk1 = new byte[10];

        public CGFXDictDataType DataType;
    }

    public class CGFXDictEntry {
        public ulong Unk;
    }

    public enum CGFXDictDataType : byte {
        Models = 0,
        Textures = 1,
        LUTS = 2,
        Materials = 3,
        Shaders = 4,
        Cameras = 5,
        Lights = 6,
        Fog = 7,
        Environments = 8,
        SkeletonAnim = 9,
        TextureAnim = 10,
        VisibilityAnim = 11,
        CameraAnim = 12,
        LightAnim = 13,
        Emitters = 14,
        Unknown = 15
    }
}
