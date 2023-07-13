using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

namespace Excalibur
{
    public interface IDataWriter : IDisposable
    {
        void Write (bool value);
        void Write (sbyte value);
        void Write (byte value);
        void Write (short value);
        void Write (ushort value);
        void Write (int value);
        void Write (uint value);
        void Write (long value);
        void Write (ulong value);
        void Write (float value);
        void Write (double value);
        void Write (decimal value);
        void Write (string value);
        void Write (Vector2 value);
        void Write (Vector3 value);
        void Write (Quaternion value);
        void Write (Color value);
    }

    public sealed class BinaryDataWriter : IDataWriter
    {
        BinaryWriter handler;

        public BinaryDataWriter (BinaryWriter writer)
        {
            handler = writer;
        }

        public void Dispose ()
        {
            handler.Dispose ();
        }

        public void Write (bool value)
        {
            handler.Write(value);
        }

        public void Write (sbyte value)
        {
            handler.Write (value);
        }

        public void Write (byte value)
        {
            handler.Write (value);
        }

        public void Write (short value)
        {
            handler.Write (value);
        }

        public void Write (ushort value)
        {
            handler.Write (value);
        }

        public void Write (int value)
        {
            handler.Write (value);
        }

        public void Write (uint value)
        {
            handler.Write (value);
        }

        public void Write (long value)
        {
            handler.Write (value);
        }

        public void Write (ulong value)
        {
            handler.Write (value);
        }

        public void Write (float value)
        {
            handler.Write (value);
        }

        public void Write (double value)
        {
            handler.Write (value);
        }

        public void Write (decimal value)
        {
            handler.Write (value);
        }

        public void Write (string value)
        {
            handler.Write (value);
        }

        public void Write (Vector2 value)
        {
            handler.Write (value.x);
            handler.Write (value.y);
        }

        public void Write (Vector3 value)
        {
            handler.Write (value.x);
            handler.Write (value.y);
            handler.Write (value.z);
        }

        public void Write (Quaternion value)
        {
            handler.Write (value.x);
            handler.Write (value.y);
            handler.Write (value.z);
            handler.Write (value.w);
        }

        public void Write (Color value)
        {
            handler.Write (value.r);
            handler.Write (value.g);
            handler.Write (value.b);
            handler.Write (value.a);
        }
    }

    public sealed class StreamDataWriter : IDataWriter
    {
        StreamWriter handler;

        public StreamDataWriter (StreamWriter writer)
        {
            handler = writer;
        }

        public void Dispose ()
        {
            handler.Dispose ();
        }

        public void Write (bool value)
        {
            handler.WriteLine(value);
        }

        public void Write (sbyte value)
        {
            handler.WriteLine (value);
        }

        public void Write (byte value)
        {
            handler.WriteLine (value);
        }

        public void Write (short value)
        {
            handler.WriteLine (value);
        }

        public void Write (ushort value)
        {
            handler.WriteLine (value);
        }

        public void Write (int value)
        {
            handler.WriteLine (value);
        }

        public void Write (uint value)
        {
            handler.WriteLine (value);
        }

        public void Write (long value)
        {
            handler.WriteLine (value);
        }

        public void Write (ulong value)
        {
            handler.WriteLine (value);
        }

        public void Write (float value)
        {
            handler.WriteLine (value);
        }

        public void Write (double value)
        {
            handler.WriteLine (value);
        }

        public void Write (decimal value)
        {
            handler.WriteLine (value);
        }

        public void Write (string value)
        {
            handler.WriteLine (value);
        }

        public void Write (Vector2 value)
        {
            handler.WriteLine (value.x);
            handler.WriteLine (value.y);
        }

        public void Write (Vector3 value)
        {
            handler.WriteLine (value.x);
            handler.WriteLine (value.y);
            handler.WriteLine (value.z);
        }

        public void Write (Quaternion value)
        {
            handler.WriteLine (value.x);
            handler.WriteLine (value.y);
            handler.WriteLine (value.z);
            handler.WriteLine (value.w);
        }

        public void Write (Color value)
        {
            handler.WriteLine (value.r);
            handler.WriteLine (value.g);
            handler.WriteLine (value.b);
            handler.WriteLine (value.a);
        }
    }
}
