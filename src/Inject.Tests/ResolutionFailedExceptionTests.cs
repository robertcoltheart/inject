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
    }
}
