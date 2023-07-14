using System.IO;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Excalibur
{
    public interface IPersistantBehaviour
    {
        void Save ();
        void Load ();
    }

    public interface IPersistant
    {
        void Save (IDataWriter writer);
        void Load (IDataReader reader);
    }

    public sealed class PersistentStorage : Singleton<PersistentStorage>
    {
        public void BinarySave (string filePath, IPersistant o)
        {
            using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(filePath, FileMode.OpenOrCreate)))
            {
                IDataWriter writer = new BinaryDataWriter (binaryWriter);
                o.Save(writer);
            }
        }

        public void BinaryLoad (string filePath, IPersistant o)
        {
            using (BinaryReader binaryReader = new BinaryReader(File.Open(filePath, FileMode.OpenOrCreate)))
            {
                IDataReader reader = new BinaryDataReader (binaryReader);
                o.Load(reader);
            }
        }

        public void StreamSave (string filePath, IPersistant o)
        {
            using (StreamWriter streamWriter = new StreamWriter(File.Open(filePath, FileMode.OpenOrCreate)))
            {
                IDataWriter writer = new StreamDataWriter(streamWriter);
                o.Save(writer);
            }
        }

        public void StreamLoad (string filePath, IPersistant o)
        {
            using (StreamReader streamReader = new StreamReader(File.Open(filePath, FileMode.OpenOrCreate)))
            {
                IDataReader reader = new StreamDataReader(streamReader);
                o.Load(reader);
            }
        }
    }
}
