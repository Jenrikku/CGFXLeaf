using CGFXLeaf.Data;
using Syroot.BinaryData;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CGFXLeaf.Dictionaries {
    public class CGFXDictionary : Dictionary<string, CGFXDictEntry> {
        public ushort HeaderUnk0;
        public byte[] HeaderUnk1 = new byte[10];

        public CGFXDictDataType DataType;

        internal static CGFXDictionary Read(
            BinaryDataReader reader,
            CGFXDictDataType dataType,
            uint entryCount,
            uint offset,
            string exceptionPrefix = "MAIN") {

            CGFXDictionary dict = new() { DataType = dataType };

            reader.Position = offset;
            if(reader.ReadString(4) != "DICT")
                throw new InvalidDataException(
                    $"{exceptionPrefix}: Failed to read dictionary at position {reader.Position - 4}.");

            reader.Position += 4; // Skip dictionary's length (it is calculated when writing).
            //uint dictLength = reader.ReadUInt32();

            Debug.Assert(entryCount == reader.ReadUInt32());
            Debug.Assert(reader.ReadInt32() == -1);

            dict.HeaderUnk0 = reader.ReadUInt16();
            reader.Read(dict.HeaderUnk1, 0, 10);

            // Read all entries.
            for(uint i = 1; i <= entryCount; i++) {
                ulong unk = reader.ReadUInt64();

                string key;
                using(reader.TemporarySeek()) {
                    reader.MoveToRelativeOffset();
                    key = reader.ReadString(BinaryStringFormat.ZeroTerminated);
                }

                reader.Position += 4;

                dynamic value;
                using(reader.TemporarySeek()) {
                    reader.MoveToRelativeOffset();
                    value = CGFXData.ReadData(reader, dataType);
                }

                reader.Position += 4;

                CGFXDictEntry entry = new() { Unk = unk, Content = value };

                dict.Add(key, entry);
            }

            return dict;
        }
    }

    public class CGFXDictEntry {
        public ulong Unk;

        public dynamic Content;
    }

    public enum CGFXDictDataType : sbyte {
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
        Unknown = 15,
        Other = -1
    }
}
