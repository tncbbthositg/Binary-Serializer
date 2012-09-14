using System.Text;
using System.IO;
using System;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace com.AutopilotLlc.BinarySerializer
{
    public class BinarySerializationWriter : BinaryWriter
    {
        public BinarySerializationWriter() : base() { }
        public BinarySerializationWriter(Stream output) : base(output) { }
        public BinarySerializationWriter(Stream output, Encoding encoding) : base(output, encoding) { }

        private void WriteType(BinarySerializationType type)
        {
            Write((byte)type);
        }

        public void WriteObject(object item)
        {
            if (item == null)
            {
                WriteType(BinarySerializationType.Null);
                return;
            }

            var type = item.GetType();
            switch (type.Name)
            {
                // built-in types
                case "Byte":
                    WriteType(BinarySerializationType.Byte);
                    Write((byte)item);
                    break;

                case "Char":
                    WriteType(BinarySerializationType.Char);
                    Write((char)item);
                    break;

                case "Decimal":
                    WriteType(BinarySerializationType.Decimal);
                    Write((decimal)item);
                    break;

                case "Double":
                    WriteType(BinarySerializationType.Double);
                    Write((double)item);
                    break;

                case "Int32":
                    WriteType(BinarySerializationType.Int);
                    Write((int)item);
                    break;

                case "Int64":
                    WriteType(BinarySerializationType.Long);
                    Write((long)item);
                    break;

                case "SByte":
                    WriteType(BinarySerializationType.SByte);
                    Write((sbyte)item);
                    break;

                case "Int16":
                    WriteType(BinarySerializationType.Short);
                    Write((short)item);
                    break;

                case "String":
                    WriteType(BinarySerializationType.String);
                    Write((string)item);
                    break;

                case "UInt32":
                    WriteType(BinarySerializationType.UInt);
                    Write((uint)item);
                    break;

                case "UInt64":
                    WriteType(BinarySerializationType.ULong);
                    Write((ulong)item);
                    break;

                case "UInt16":
                    WriteType(BinarySerializationType.UShort);
                    Write((ushort)item);
                    break;

                // other types
                case "Boolean":
                    WriteType((bool)item ? BinarySerializationType.True : BinarySerializationType.False);
                    break;

                case "DateTime":
                    WriteType(BinarySerializationType.DateTime);
                    Write(((DateTime)item).Ticks);
                    break;

                case "TimeSpan":
                    WriteType(BinarySerializationType.TimeSpan);
                    Write(((TimeSpan)item).Ticks);
                    break;

                // special types
                default:
                    if (item is IDictionary)
                    {
                        WriteDictionary((IDictionary)item);
                        break;
                    }

                    if (item is IEnumerable)
                    {
                        WriteEnumerable((IEnumerable)item);
                        break;
                    }

                    WriteType(BinarySerializationType.Object);
                    new BinaryFormatter().Serialize(BaseStream, item);
                    break;
            }
        }

        public void WriteEnumerable(IEnumerable enumerable)
        {
            var collection = enumerable.ToCollection();

            WriteType(BinarySerializationType.Collection);
            WriteLength(collection.Count);

            foreach (var element in collection)
                WriteObject(element);
        }

        public void WriteKeyValues<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
        {
            WriteDictionary(enumerable.ToDictionary());
        }

        public void WriteDictionary(IDictionary dictionary)
        {
            WriteType(BinarySerializationType.Dictionary);
            WriteLength(dictionary.Count);

            foreach (DictionaryEntry entry in dictionary)
            {
                WriteObject(entry.Key);
                WriteObject(entry.Value);
            }
        }

        private void WriteLength(int length)
        {
            Write7BitEncodedInt(length);
        }
    }
}
