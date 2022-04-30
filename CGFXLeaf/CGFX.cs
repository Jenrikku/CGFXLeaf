using Syroot.BinaryData;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CGFXLeaf {
    public class CGFX {
        public ByteOrder ByteOrder;
        public uint Version;

        public CGFX(byte[] data) : this(new MemoryStream(data)) { }
        
        public CGFX(string filename) : this(new FileStream(filename, FileMode.Open)) { }

        public CGFX(Stream stream, bool leaveOpen = false) {
            using BinaryDataReader reader = new(stream, Encoding.ASCII, leaveOpen);
            reader.ByteOrder = ByteOrder.BigEndian;

            if(reader.ReadString(4) != "CGFX") // Magic check.
                throw new InvalidDataException("The given data is not a valid CGFX.");

            ByteOrder = (ByteOrder) reader.ReadInt16();
            reader.ByteOrder = ByteOrder;

            Debug.Assert(reader.ReadUInt16() == 0x14);

            Version = reader.ReadUInt32();

            uint fileSize = reader.ReadUInt32();
            uint entryCount = reader.ReadUInt32();

            // The DATA section is an array of DICT (dictionaries).
            if(reader.ReadString(4) != "DATA") // Magic check.
                throw new InvalidDataException("The DATA setion is corrupted or missplaced.");

            uint dataLength = reader.ReadUInt32();

            // CGFX's hashes are formed by the number of entries on a dictionary and its relative offset. 
            List<(uint, uint)> hashes = new();
            using(reader.TemporarySeek()) {
                entryCount = reader.ReadUInt32();
                while(entryCount != 1413695812) { // Reading up until DICT
                    hashes.Add((entryCount, (uint) reader.Position + reader.ReadUInt32()));
                    entryCount = reader.ReadUInt32();
                }
            }

            List<CGFXDictionary> root = new();
            using(reader.TemporarySeek()) {
                foreach((uint, uint) hash in hashes) {
                    CGFXDictionary dict = new();

                    reader.Position = hash.Item2;
                    if(reader.ReadString(4) != "DICT")
                        throw new InvalidDataException($"Failed to read dictionary at position {reader.Position - 4}.");

                    uint dictLength = reader.ReadUInt32();

                    Debug.Assert(hash.Item1 == reader.ReadUInt32());
                    Debug.Assert(reader.ReadInt32() == -1);

                    dict.HeaderUnk0 = reader.ReadUInt16();
                    reader.Read(dict.HeaderUnk1, 0, 10);

                    for(uint i = 1; i <= hash.Item1; i++) {
                        ulong unk = reader.ReadUInt64();

                        using(reader.TemporarySeek()) {
                            reader.Position += reader.ReadUInt32();
                            string key = reader.ReadString(BinaryStringFormat.ZeroTerminated);
                        }

                        reader.Position += 4;

                        using(reader.TemporarySeek()) {
                            reader.Position += reader.ReadUInt32();
                            // TO-DO: Find file size and read file.
                        }
                    }
                }
            }
        }
    }
}
