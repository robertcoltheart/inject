using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inject
{
    [TestClass]
    public class ResolutionFailedExceptionTests
    {
        [TestMethod]
        public void IsTypeOfException()
        {
            Assert.IsTrue(typeof(Exception).IsAssignableFrom(typeof(ResolutionFailedException)), "Type is not an exception.");
        }

        [TestMethod]
        public void InnerExceptionSet()
        {
            var innerException = new Exception();
            var exception = new ResolutionFailedException("msg", innerException);

            Assert.AreSame(innerException, exception.InnerException);
        }

        [TestMethod]
        public void HasMessage()
        {
            var exception = new ResolutionFailedException("msg");

            Assert.AreEqual("msg", exception.Message);
        }

        [TestMethod]
        public void IsSerializableDecorated()
        {
            Assert.IsTrue(typeof(ResolutionFailedException).GetCustomAttributes(typeof(SerializableAttribute), true).Length == 1, "Type must be serializable.");
        }

        [TestMethod]
        public void IsSerializable()
        {
            var exception = new ResolutionFailedException();

            try
            {
                using (var stream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, exception);

                    stream.Position = 0;

                    exception = (ResolutionFailedException)formatter.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                Assert.Fail("Type must be serializable.");
            }

            Assert.IsNotNull(exception, "Type could not be deserialized.");
        }  
    }
}