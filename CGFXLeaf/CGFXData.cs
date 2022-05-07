using CGFXLeaf.Dictionaries;
using Syroot.BinaryData;
using System.Diagnostics;
using System.Numerics;

namespace CGFXLeaf.Data {
    internal static class CGFXData {
        internal static dynamic ReadData(BinaryDataReader reader, CGFXDictDataType dataType) {
            switch(dataType) {
                case CGFXDictDataType.Models:
                    return CMDL.Read(reader);
                case CGFXDictDataType.Unknown:
                case CGFXDictDataType.Other:
                    return "Unknown / Other offset: " + reader.Position;
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// Stores 3D model data.
    /// </summary>
    public class CMDL {
        // The objects are declared in the same order they appear in the file.
        public bool[] Flags;
        public uint Unk0;
        public string ModelName;
        public byte[] Unk1 = new byte[0x18];
        public CGFXDictionary Animations;
        public Vector3 GlobalScale;
        public Vector3 GlobalRotation;
        public Vector3 GlobalTranlation;
        public Matrix4x4 WorldMatrix;
        public Matrix4x4 LocalMatrix;
        public CGFXDictionary Dict1;
        public CGFXDictionary Dict2;
        public CGFXDictionary Dict3;
        public CGFXDictionary Dict4;

        internal static CMDL Read(BinaryDataReader reader) {
            CMDL cmdl = new();

            cmdl.Flags = reader.ReadBits(4);
            Debug.Assert(reader.ReadString(4) == "CMDL"); // Magic check
            cmdl.Unk0 = reader.ReadUInt32();

            using(reader.TemporarySeek()) { // ModelName
                reader.MoveToRelativeOffset();
                cmdl.ModelName = reader.ReadString(BinaryStringFormat.ZeroTerminated);
            }
            reader.Position += 4;

            reader.Read(cmdl.Unk1, 0, 0x18);

            uint animCount = reader.ReadUInt32();
            using(reader.TemporarySeek()) {
                // Read anim dictionary
                cmdl.Animations = CGFXDictionary.Read(
                    reader,
                    CGFXDictDataType.Other,
                    animCount,
                    reader.ReadRelativeOffset(),
                    "CMDL");
            }
            reader.Position += 4;

            cmdl.GlobalScale = reader.ReadVector3();
            cmdl.GlobalRotation = reader.ReadVector3();
            cmdl.GlobalTranlation = reader.ReadVector3();

            // Matrices
            cmdl.WorldMatrix = new(
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                -1, -1, -1, -1);
            cmdl.LocalMatrix = new(
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                -1, -1, -1, -1);

            // TO-DO: Read dictionaries.
            // NOTE: They do not seem to be working correctly.

            using(reader.TemporarySeek()) { // Meshes' offsets
                uint meshCount = reader.ReadUInt32();
                reader.MoveToRelativeOffset();

                uint[] meshOffsets = reader.ReadRelativeOffsets((int) meshCount);
            }
            reader.Position += 8;

            using(reader.TemporarySeek())
                cmdl.Dict2 = CGFXDictionary.Read(
                    reader,
                    CGFXDictDataType.Other,
                    reader.ReadUInt32(),
                    reader.ReadRelativeOffset(),
                    "CGFX");

            reader.Position += 8;

            using(reader.TemporarySeek())
                cmdl.Dict3 = CGFXDictionary.Read(
                    reader,
                    CGFXDictDataType.Other,
                    reader.ReadUInt32(),
                    reader.ReadRelativeOffset(),
                    "CGFX");

            return cmdl;
        }
    }
}
