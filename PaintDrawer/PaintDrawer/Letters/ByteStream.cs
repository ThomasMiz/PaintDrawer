using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintDrawer.Letters
{
    class ByteStream
    {
        byte[] data;
        int index = 0;

        /// <summary>Gets the length of the stream</summary>
        public int Lenght { get { return data.Length; } }
        /// <summary>Gets the current reading position in the stream</summary>
        public int Position { get { return index; } set { index = value; } }

        /// <summary>Creates a ByteStream with a specified byte array</summary>
        /// <param name="data">The data the stream will read</param>
        public ByteStream(byte[] data)
        {
            this.data = data;
        }

        /// <summary>Reads a byte from the current position</summary>
        public byte ReadByte()
        {
            return data[index++];
        }
        /// <summary>Reads a byte from the current position without advancing the stream position</summary>
        public byte PeekByte()
        {
            return data[index];
        }

        /// <summary>Reads a 32-bit integer from the current position</summary>
        public int ReadInt32()
        {
            int r = BitConverter.ToInt32(data, index);
            index += 4;
            return r;
        }
        /// <summary>Reads a 32-bit integer from the current position without advancing the stream position</summary>
        public int PeekInt32()
        {
            return BitConverter.ToInt32(data, index);
        }

        /// <summary>Reads a 16-bit integer from the current position</summary>
        public int ReadInt16()
        {
            int r = BitConverter.ToInt16(data, index);
            index += 2;
            return r;
        }
        /// <summary>Reads a 16-bit integer from the current position without advancing the stream position</summary>
        public int PeekInt16()
        {
            return BitConverter.ToInt16(data, index);
        }

        /// <summary>Reads a 32-bit float from the current position</summary>
        public float ReadFloat()
        {
            float r = BitConverter.ToSingle(data, index);
            index += 4;
            return r;
        }
        /// <summary>Reads a 32-bit float from the current position without advancing the stream position</summary>
        public float PeekFloat()
        {
            return BitConverter.ToSingle(data, index);
        }

        /// <summary>Reads a 64-bit Vector2 from the current position</summary>
        public Vec2 ReadVector2()
        {
            return new Vec2(ReadFloat(), ReadFloat());
        }
        /// <summary>Reads a 64-bit Vector2 from the current position without advancing the stream position</summary>
        public Vec2 PeekVector2()
        {
            return new Vec2(PeekFloat(), BitConverter.ToSingle(data, index + 4));
        }

        /// <summary>Reads a byte N and returns a string of length N, each char being a byte read consecutively.</summary>
        public String ReadString()
        {
            char[] c = new char[ReadByte()];
            for (int i = 0; i < c.Length; i++)
                c[i] = (char)ReadByte();
            return new String(c);
        }

        /// <summary>Reads an integer N and returns a Vector2[] of size N. The elements are read secuentially from the stream.</summary>
        public Vec2[] ReadVector2Array()
        {
            Vec2[] r = new Vec2[ReadInt32()];
            for (int i = 0; i < r.Length; i++)
                r[i] = ReadVector2();
            return r;
        }

        /// <summary>Returns whether the Stream contains another byte</summary>
        public bool HasNext() { return index < data.Length; }
        /// <summary>Returns whether the Stream contains a certain amount of bytes left for reading</summary>
        /// <param name="amount">The amount of bytes to check for availability</param>
        public bool HasNext(int amount) { return index + amount < data.Length; }
        /// <summary>Returns whether the Stream contains enough data for reading a 32-bit integer</summary>
        public bool HasNextInt32() { return index + 4 < data.Length; }
        /// <summary>Returns whether the Stream contains enough data for reading a 16-bit integer</summary>
        public bool HasNextInt16() { return index + 2 < data.Length; }
        /// <summary>Returns whether the Stream contains enough data for reading a 32-bit float</summary>
        public bool HasNextFloat() { return index + 4 < data.Length; }
        /// <summary>Returns whether the Stream contains enough data for reading a 64-bit Vector2</summary>
        public bool HasNextVector2() { return index + 8 < data.Length; }
        /// <summary>Returns whether reading a Vector2 array will succeed by checking the amount of data left for reading</summary>
        public bool HasNextVector2Array() { return HasNext(PeekByte() * 8 + 4); }
    }
}
