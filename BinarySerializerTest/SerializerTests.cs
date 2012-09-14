using System.IO;
using com.AutopilotLlc.BinarySerializer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;

namespace BinarySerializerTest
{
    [TestClass]
    public class SerializerTests
    {
        [TestMethod]
        public void CanSerializeAndDeserializePrimitives()
        {
            Assert.AreEqual(2172012, SerializeDeserializeObject(2172012));
            Assert.AreEqual(406.1978, SerializeDeserializeObject(406.1978));
            Assert.AreEqual("Lauren Caldwell", SerializeDeserializeObject("Lauren Caldwell"));
        }

        [TestMethod]
        public void CanSerializeAndDeserializeCollections()
        {
            var list = new List<object> { 2172012, 406.1979, "Lauren Caldwell" };
            var listResult = SerializeDeserializeObject(list) as List<object>;

            for (var i = 0; i < list.Count; i++)
                Assert.AreEqual(list[i], listResult[i]);

            var dictionary = new Dictionary<object, object>
            {
                {"Lauren Caldwell", new DateTime(1979, 4, 6)},
                {"Piper Emmaline", new DateTime(2012, 2, 17)},
                {"Patrick Caldwell", new DateTime(1981, 4, 19)}
            };
            var dictionaryResult = SerializeDeserializeObject(dictionary) as Dictionary<object, object>;

            foreach (var pair in dictionary)
            {
                Assert.IsTrue(dictionaryResult.ContainsKey(pair.Key));
                Assert.AreEqual(pair.Value, dictionaryResult[pair.Key]);
            }

            Assert.AreEqual(dictionary.Count, dictionaryResult.Count);
        }

        [TestMethod]
        public void CanSerializeAndDeserializeTypedPrimitives()
        {
            Assert.AreEqual(2172012, SerializeDeserialize<int>(2172012));
            Assert.AreEqual(406.1978, SerializeDeserialize<double>(406.1978));
            Assert.AreEqual("Lauren Caldwell", SerializeDeserialize<string>("Lauren Caldwell"));
        }

        [TestMethod]
        public void CanSerializeAndDeserializeTypedCollections()
        {
            var list = new List<int> { 2172012, 4061979, 4191981 };
            var listResult = SerializeDeserialize<int>(list);

            for (var i = 0; i < list.Count; i++)
                Assert.AreEqual(list[i], listResult[i]);

            var dictionary = new Dictionary<string, DateTime>
            {
                {"Lauren Caldwell", new DateTime(1979, 4, 6)},
                {"Piper Emmaline", new DateTime(2012, 2, 17)},
                {"Patrick Caldwell", new DateTime(1981, 4, 19)}
            };
            var dictionaryResult = SerializeDeserialize<string, DateTime>(dictionary);

            foreach (var pair in dictionary)
            {
                Assert.IsTrue(dictionaryResult.ContainsKey(pair.Key));
                Assert.AreEqual(pair.Value, dictionaryResult[pair.Key]);
            }

            Assert.AreEqual(dictionary.Count, dictionaryResult.Count);
        }

        [TestMethod]
        public void DeserializingNullOnlyWorksForReferenceTypes()
        {
            String isNull = null;
            Assert.AreEqual(isNull, SerializeDeserialize<string>(isNull));

            try
            {
                using (var stream = new MemoryStream())
                {
                    new BinarySerializationWriter(stream).WriteObject(null);
                    stream.Position = 0;
                    new BinarySerializationReader(stream).ReadObject<int>();
                }
            }

            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(InvalidOperationException));
            }
        }

        private object SerializeDeserializeObject(object o)
        {
            return SerializeDeserialize<object>(o);
        }

        private T SerializeDeserialize<T>(T item)
        {
            using (var stream = new MemoryStream())
            {
                new BinarySerializationWriter(stream).WriteObject(item);
                stream.Position = 0;
                return new BinarySerializationReader(stream).ReadObject<T>();
            }
        }

        private List<T> SerializeDeserialize<T>(List<T> items)
        {
            using (var stream = new MemoryStream())
            {
                new BinarySerializationWriter(stream).WriteObject(items);
                stream.Position = 0;
                return new BinarySerializationReader(stream).ReadList<T>();
            }
        }

        private Dictionary<TKey, TValue> SerializeDeserialize<TKey, TValue>(Dictionary<TKey, TValue> items)
        {
            using (var stream = new MemoryStream())
            {
                new BinarySerializationWriter(stream).WriteObject(items);
                stream.Position = 0;
                return new BinarySerializationReader(stream).ReadDictionary<TKey, TValue>();
            }
        }
    }
}
