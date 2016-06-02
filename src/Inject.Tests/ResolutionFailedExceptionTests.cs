using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace Inject
{
    [TestFixture]
    public class ResolutionFailedExceptionTests
    {
        [Test]
        public void IsTypeOfException()
        {
            Assert.IsTrue(typeof(Exception).IsAssignableFrom(typeof(ResolutionFailedException)), "Type is not an exception.");
        }

        [Test]
        public void InnerExceptionSet()
        {
            var innerException = new Exception();
            var exception = new ResolutionFailedException("msg", innerException);

            Assert.AreSame(innerException, exception.InnerException);
        }

        [Test]
        public void HasMessage()
        {
            var exception = new ResolutionFailedException("msg");

            Assert.AreEqual("msg", exception.Message);
        }

        [Test]
        public void IsSerializableDecorated()
        {
            Assert.IsTrue(typeof(ResolutionFailedException).GetCustomAttributes(typeof(SerializableAttribute), true).Length == 1, "Type must be serializable.");
        }

        [Test]
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
