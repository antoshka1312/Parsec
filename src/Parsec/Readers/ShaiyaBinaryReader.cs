﻿using System;
using System.IO;
using System.Text;
using Parsec.Extensions;

namespace Parsec.Readers
{
    /// <summary>
    /// A binary reader made specifically to read Shaiya file formats
    /// </summary>
    public sealed class ShaiyaBinaryReader
    {
        private byte[] _buffer;

        /// <summary>
        /// The binary reader's data buffer
        /// </summary>
        public byte[] Buffer
        {
            get => _buffer;
            set
            {
                _buffer = new byte[value.Length];
                Array.Copy(value, Buffer, value.Length);
            }
        }

        // TODO: Refactor code to make offset of type 'long'. Although 'int' is enough for most
        // files (~2gb), it shouldn't be enough for data.saf which is usually 4gb+.
        private int _offset;

        /// <summary>
        /// The binary reader's current reading position
        /// </summary>
        public int Offset => _offset;

        public ShaiyaBinaryReader(string filePath)
        {
            using var binaryReader = new BinaryReader(File.OpenRead(filePath));
            Buffer = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
        }

        /// <summary>
        /// Reads a generic type from the byte buffer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Read value</returns>
        /// <exception cref="NotSupportedException">When the provided type is not supported</exception>
        public T Read<T>()
        {
            var type = Type.GetTypeCode(typeof(T));

            object value = type switch
            {
                TypeCode.Byte => ReadByte(),
                TypeCode.SByte => ReadSByte(),
                TypeCode.Char => ReadChar(),
                TypeCode.Boolean => ReadBoolean(),
                TypeCode.Int16 => ReadInt16(),
                TypeCode.UInt16 => ReadUInt16(),
                TypeCode.Int32 => ReadInt32(),
                TypeCode.UInt32 => ReadUInt32(),
                TypeCode.Int64 => ReadInt64(),
                TypeCode.UInt64 => ReadUInt64(),
                TypeCode.Single => ReadFloat(),
                TypeCode.Double => ReadDouble(),
                _ => throw new NotSupportedException()
            };

            return (T)value;
        }

        /// <summary>
        /// Reads a byte (unsigned)
        /// </summary>
        public byte ReadByte()
        {
            var result = Buffer[_offset];
            _offset += sizeof(byte);
            return result;
        }

        /// <summary>
        /// Reads a signed byte (sbyte)
        /// </summary>
        public sbyte ReadSByte()
        {
            var result = Convert.ToSByte(Buffer[_offset]);
            _offset += sizeof(sbyte);
            return result;
        }

        /// <summary>
        /// Reads a number of bytes
        /// </summary>
        /// <param name="count">Number of bytes to read</param>
        public byte[] ReadBytes(int count)
        {
            var result = Buffer.SubArray(_offset, _offset + count);
            _offset += count;
            return result;
        }

        /// <summary>
        /// Reads a boolean value
        /// </summary>
        public bool ReadBoolean()
        {
            var result = Convert.ToBoolean(Buffer[_offset]);
            _offset += sizeof(bool);
            return result;
        }

        /// <summary>
        /// Reads a char value
        /// </summary>
        public char ReadChar()
        {
            var result = Convert.ToChar(Buffer[_offset]);
            _offset += sizeof(char);
            return result;
        }

        /// <summary>
        /// Reads a signed short value (int16)
        /// </summary>
        public short ReadInt16()
        {
            var result = BitConverter.ToInt16(Buffer, _offset);
            _offset += sizeof(short);
            return result;
        }

        /// <summary>
        /// Reads an unsigned short value (uint16)
        /// </summary>
        public ushort ReadUInt16()
        {
            var result = BitConverter.ToUInt16(Buffer, _offset);
            _offset += sizeof(ushort);
            return result;
        }

        /// <summary>
        /// Reads a signed int value (int32)
        /// </summary>
        public int ReadInt32()
        {
            var result = BitConverter.ToInt32(Buffer, _offset);
            _offset += sizeof(int);
            return result;
        }

        /// <summary>
        /// Reads an unsigned int value (uint32)
        /// </summary>
        public uint ReadUInt32()
        {
            var result = BitConverter.ToUInt32(Buffer, _offset);
            _offset += sizeof(uint);
            return result;
        }

        /// <summary>
        /// Reads a signed long (int64) value
        /// </summary>
        public long ReadInt64()
        {
            var result = BitConverter.ToInt64(Buffer, _offset);
            _offset += sizeof(long);
            return result;
        }

        /// <summary>
        /// Reads an unsigned long (uint64) value
        /// </summary>
        public ulong ReadUInt64()
        {
            var result = BitConverter.ToUInt64(Buffer, _offset);
            _offset += sizeof(ulong);
            return result;
        }

        /// <summary>
        /// Reads a float (single) value
        /// </summary>
        public float ReadFloat()
        {
            var result = BitConverter.ToSingle(Buffer, _offset);
            _offset += sizeof(float);
            return result;
        }

        /// <summary>
        /// Reads a double value
        /// </summary>
        public double ReadDouble()
        {
            var result = BitConverter.ToDouble(Buffer, _offset);
            _offset += sizeof(double);
            return result;
        }

        /// <summary>
        /// Reads a variable string based on a provided length
        /// </summary>
        /// <param name="length">The length of the string</param>
        public string ReadString(int length)
        {
            var result = Encoding.ASCII.GetString(Buffer, _offset, length);
            _offset += length;
            return result;
        }

        /// <summary>
        /// Reads a variable string which has its length prefixed with little endian encoding.
        /// </summary>
        /// <param name="removeStringTerminator">Indicates whether the string terminator (\0) should be removed or not</param>
        public string ReadString(bool removeStringTerminator = true)
        {
            var length = ReadInt32();
            var result = Encoding.ASCII.GetString(Buffer, _offset, length);

            if (removeStringTerminator)
                result = result.Trim('\0');

            _offset += length;
            return result;
        }

        /// <summary>
        /// Resets the reading offset
        /// </summary>
        public void ResetOffset() => SetOffset(0);

        /// <summary>
        /// Sets the reading offset
        /// </summary>
        /// <param name="offset">Offset value to set</param>
        public void SetOffset(int offset) => _offset = offset;

        /// <summary>
        /// Sets the cursor to the current position + the specified number of bytes to skip
        /// </summary>
        /// <param name="count">Number of bytes to skip</param>
        public void Skip(int count) => SetOffset(_offset + count);
    }
}