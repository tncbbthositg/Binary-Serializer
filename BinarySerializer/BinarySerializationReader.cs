using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace com.AutopilotLlc.BinarySerializer
{
    public class BinarySerializationReader : BinaryReader
    {
        public BinarySerializationReader(Stream stream) : base(stream) { }
        public BinarySerializationReader(Stream stream, Encoding encoding) : base(stream, encoding) { }

        private BinarySerializationType ReadType()
        {
            return (BinarySerializationType)ReadByte();
        }

        private bool IsTypeNullable<T>()
        {
            var type = typeof(T);
            if (!type.IsValueType) return true;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return true;

            return false;
        }

        public T ReadObject<T>()
        {
            var item = ReadObject();

            if (!IsTypeNullable<T>() && item == null)
                throw new InvalidOperationException("Datum at current location is null and cannot be assigned to value type.");

            return (T)item;
        }

        public object ReadObject()
        {
            var type = (BinarySerializationType)ReadByte();

            switch (type)
            {
                // built-in types
                case BinarySerializationType.Byte:
                    return ReadByte();

                case BinarySerializationType.Char:
                    return ReadChar();

                case BinarySerializationType.Decimal:
                    return ReadDecimal();

                case BinarySerializationType.Double:
                    return ReadDouble();

                case BinarySerializationType.Int:
                    return ReadInt32();

                case BinarySerializationType.Long:
                    return ReadInt64();

                case BinarySerializationType.SByte:
                    return ReadSByte();

                case BinarySerializationType.Short:
                    return ReadInt16();

                case BinarySerializationType.String:
                    return ReadString();

                case BinarySerializationType.UInt:
                    return ReadUInt32();

                case BinarySerializationType.ULong:
                    return ReadUInt64();

                case BinarySerializationType.UShort:
                    return ReadUInt16();

                // other types
                case BinarySerializationType.DateTime:
                    return new DateTime(ReadInt64());

                case BinarySerializationType.TimeSpan:
                    return new TimeSpan(ReadInt64());

                // special types
                case BinarySerializationType.Null:
                    return null;

                case BinarySerializationType.Object:
                    return new BinaryFormatter().Deserialize(BaseStream);

                case BinarySerializationType.Collection:
                    return ReadVerifiedList<object>();

                case BinarySerializationType.Dictionary:
                    return ReadVerifiedDictionary<object, object>();

                case BinarySerializationType.True:
                    return true;

                case BinarySerializationType.False:
                    return false;
            }

            throw new InvalidDataException("Cannot determine type of serialized object.");
        }

        public Dictionary<object, object> ReadDictionary()
        {
            return ReadDictionary<object, object>();
        }

        public Dictionary<TKey, TValue> ReadDictionary<TKey, TValue>()
        {
            var type = ReadType();
            if (type != BinarySerializationType.Dictionary)
                throw new InvalidDataException("The object at the current position is not a KeyValuePair collection");

            return ReadVerifiedDictionary<TKey, TValue>();
        }

        private Dictionary<TKey, TValue> ReadVerifiedDictionary<TKey, TValue>()
        {
            var length = ReadLength();
            var dictionary = new Dictionary<TKey, TValue>(length);

            for (int i = 0; i < length; i++)
            {
                var key = ReadObject<TKey>();
                var value = ReadObject<TValue>();

                dictionary.Add(key, value);
            }

            return dictionary;
        }

        public List<object> ReadList()
        {
            return ReadList<object>();
        }

        public List<T> ReadList<T>()
        {
            var type = ReadType();
            if (type != BinarySerializationType.Collection)
                throw new InvalidDataException("The object at the current position is not a collection");

            return ReadVerifiedList<T>();
        }

        private List<T> ReadVerifiedList<T>()
        {
            var length = ReadLength();
            var list = new List<T>(length);

            for (int i = 0; i < length; i++)
                list.Add(ReadObject<T>());

            return list;
        }

        public List<object> ReadToEnd()
        {
            var list = new List<object>();

            while (BaseStream.Position < BaseStream.Length)
                list.Add(ReadObject());

            return list;
        }

        private int ReadLength()
        {
            return Read7BitEncodedInt();
        }
    }
}
