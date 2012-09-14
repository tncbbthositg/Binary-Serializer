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

        private object SerializeDeserializeObject(object o)
        {
            using (var stream = new MemoryStream())
            {
                new BinarySerializationWriter(stream).WriteObject(o);
                stream.Position = 0;
                return new BinarySerializationReader(stream).ReadObject();
            }
        }
    }
}
