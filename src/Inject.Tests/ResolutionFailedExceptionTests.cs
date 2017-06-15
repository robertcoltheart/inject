using System;
using Xunit;

namespace Inject
{
    public class ResolutionFailedExceptionTests
    {
        [Fact]
        public void IsTypeOfException()
        { 
            var exception = new ResolutionFailedException();

            Assert.IsAssignableFrom<Exception>(exception);
        }

        [Fact]
        public void InnerExceptionSet()
        {
            var innerException = new Exception();
            var exception = new ResolutionFailedException("msg", innerException);

            Assert.Same(innerException, exception.InnerException);
        }

        [Fact]
        public void HasMessage()
        {
            var exception = new ResolutionFailedException("msg");

            Assert.Equal("msg", exception.Message);
        }
    }
}
