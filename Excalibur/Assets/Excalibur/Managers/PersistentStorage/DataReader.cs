using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

namespace Excalibur
{
    public interface IDataReader : IDisposable
    {
        bool ReadBool ();
        sbyte ReadSByte ();
        byte ReadByte ();
        short ReadShort ();
        ushort ReadUShort ();
        int ReadInt ();
        uint ReadUint ();
        long ReadLong ();
        ulong ReadUlong ();
        float ReadFloat ();
        double ReadDouble ();
        decimal ReadDecimal ();
        string ReadString ();
        Vector2 ReadVector2 ();
        Vector3 ReadVector3 ();
        Color ReadColor ();
        Quaternion ReadQuaternion ();
    }

    public sealed class BinaryDataReader : IDataReader
    {
        BinaryReader handler;

        public BinaryDataReader   (BinaryReader reader)
        {
            handler = reader;
        }

        public void Dispose ()
        {
            handler.Dispose ();
        }

        public bool ReadBool ()
        {
            return handler.ReadBoolean();
        }

        public sbyte ReadSByte ()
        {
            return handler.ReadSByte ();
        }

        public byte ReadByte ()
        {
            return handler.ReadByte ();
        }

        public short ReadShort ()
        {
            return handler.ReadInt16 ();
        }

        public ushort ReadUShort ()
        {
            return handler.ReadUInt16 ();
        }

        public int ReadInt ()
        {
            return handler.ReadInt32 ();
        }

        public uint ReadUint ()
        {
            return handler.ReadUInt32 ();
        }

        public long ReadLong ()
        {
            return handler.ReadInt64 ();
        }

        public ulong ReadUlong ()
        {
            return handler.ReadUInt64 ();
        }

        public float ReadFloat ()
        {
            return handler.ReadSingle ();
        }

        public double ReadDouble ()
        {
            return handler.ReadDouble ();
        }

        public decimal ReadDecimal ()
        {
            return handler.ReadDecimal ();
        }

        public string ReadString ()
        {
            return handler.ReadString ();
        }

        public Vector2 ReadVector2 ()
        {
            Vector2 value;
            value.x = ReadFloat ();
            value.y = ReadFloat ();
            return value;
        }

        public Vector3 ReadVector3 ()
        {
            Vector3 value;
            value.x = ReadFloat ();
            value.y = ReadFloat ();
            value.z = ReadFloat ();
            return value;
        }

        public Quaternion ReadQuaternion ()
        {
            Quaternion value;
            value.x = ReadFloat ();
            value.y = ReadFloat ();
            value.z = ReadFloat ();
            value.w = ReadFloat ();
            return value;
        }

        public Color ReadColor ()
        {
            Color value;
            value.r = ReadFloat ();
            value.g = ReadFloat ();
            value.b = ReadFloat ();
            value.a = ReadFloat ();
            return value;
        }
    }

    public sealed class StreamDataReader : IDataReader
    {
        StreamReader handler;

        public StreamDataReader (StreamReader reader)
        {
            handler = reader;
        }

        public void Dispose ()
        {
            handler.Dispose ();
        }

        public bool ReadBool ()
        {
            return bool.Parse(handler.ReadLine());
        }

        public sbyte ReadSByte ()
        {
            return sbyte.Parse (handler.ReadLine ());
        }

        public byte ReadByte ()
        {
            return byte.Parse (handler.ReadLine ());
        }

        public short ReadShort ()
        {
            return short.Parse (handler.ReadLine ());
        }

        public ushort ReadUShort ()
        {
            return ushort.Parse (handler.ReadLine ());
        }

        public int ReadInt ()
        {
            return int.Parse (handler.ReadLine ());
        }

        public uint ReadUint ()
        {
            return uint.Parse (handler.ReadLine ());
        }

        public long ReadLong ()
        {
            return long.Parse (handler.ReadLine ());
        }

        public ulong ReadUlong ()
        {
            return ulong.Parse (handler.ReadLine ());
        }

        public float ReadFloat ()
        {
            return float.Parse (handler.ReadLine ());
        }

        public double ReadDouble ()
        {
            return double.Parse (handler.ReadLine ());
        }

        public decimal ReadDecimal ()
        {
            return decimal.Parse (handler.ReadLine ());
        }

        public string ReadString ()
        {
            return handler.ReadLine ();
        }

        public Vector2 ReadVector2 ()
        {
            Vector2 value;
            value.x = ReadFloat ();
            value.y = ReadFloat ();
            return value;
        }

        public Vector3 ReadVector3 ()
        {
            Vector3 value;
            value.x = ReadFloat ();
            value.y = ReadFloat ();
            value.z = ReadFloat ();
            return value;
        }

        public Quaternion ReadQuaternion ()
        {
            Quaternion value;
            value.x = ReadFloat ();
            value.y = ReadFloat ();
            value.z = ReadFloat ();
            value.w = ReadFloat ();
            return value;
        }

        public Color ReadColor ()
        {
            Color value;
            value.r = ReadFloat ();
            value.g = ReadFloat ();
            value.b = ReadFloat ();
            value.a = ReadFloat ();
            return value;
        }
    }
}
