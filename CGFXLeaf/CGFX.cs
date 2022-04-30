using CGFXLeaf.Data;
using CGFXLeaf.Dictionaries;
using Syroot.BinaryData;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CGFXLeaf {
    public class CGFX {
        public ByteOrder ByteOrder;
        public uint Version;

        /// <summary>
        /// All the data is stored in this dictionary.
        /// Usage: RootDictionary[<see cref="CGFXDictDataType"/>]
        /// </summary>
        public Dictionary<CGFXDictDataType, CGFXDictionary> RootDictionary = new();

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

            reader.Position += 4; // Skip file's length (it is calculated when writing).
            //uint fileSize = reader.ReadUInt32();
            Debug.Assert(reader.ReadUInt32() == 2);

            // The DATA section is an array of DICT (dictionaries).
            // Each dictionary has its own data type. (Models, Textures, etc)
            if(reader.ReadString(4) != "DATA") // Magic check.
                throw new InvalidDataException("The DATA setion is corrupted or missplaced.");

            reader.Position += 4; // Skip DATA section's length (it is calculated when writing).
            //uint dataLength = reader.ReadUInt32();

            // CGFX's hashes are formed by the number of entries on a dictionary and its relative offset. 
            List<(uint, uint)> hashes = new();
            using(reader.TemporarySeek()) {
                uint entryCount = reader.ReadUInt32();
                while(entryCount != 1413695812) { // Reading up until DICT
                    hashes.Add((entryCount, (uint) reader.Position + reader.ReadUInt32()));
                    entryCount = reader.ReadUInt32();
                }
            }

            for(byte i = 0; i <= 15; i++) {
                if(hashes.Count < i) {
                    Debug.Assert(false);
                    return;
                }

                CGFXDictionary dict = new() { DataType = (CGFXDictDataType) i };

                reader.Position = hashes[i].Item2;
                if(reader.ReadString(4) != "DICT")
                    throw new InvalidDataException($"Failed to read dictionary at position {reader.Position - 4}.");

                reader.Position += 4; // Skip dictionary's length (it is calculated when writing).
                //uint dictLength = reader.ReadUInt32();

                Debug.Assert(hashes[i].Item1 == reader.ReadUInt32());
                Debug.Assert(reader.ReadInt32() == -1);

                dict.HeaderUnk0 = reader.ReadUInt16();
                reader.Read(dict.HeaderUnk1, 0, 10);

                for(uint j = 1; j <= hashes[i].Item1; j++) {
                    ulong unk = reader.ReadUInt64();

                    string key;
                    using(reader.TemporarySeek()) {
                        reader.Position += reader.ReadUInt32();
                        key = reader.ReadString(BinaryStringFormat.ZeroTerminated);
                    }

                    reader.Position += 4;

                    dynamic value;
                    using(reader.TemporarySeek()) {
                        reader.Position += reader.ReadUInt32();
                        value = CGFXData.ReadData(reader, (CGFXDictDataType) i);
                    }

                    CGFXDictEntry entry = new() { Unk = unk, Content = value };

                    dict.Add(key, entry);
                }

                RootDictionary.Add((CGFXDictDataType) i, dict);
            }
        }
    }
}
