using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SanderVanVliet.WcfDataSource.Tests
{
    [TestClass]
    public class WhenGettingTheFieldValues : IDisposable
    {
        private WcfDataReader _reader;
        private MockResult[] _mockResults = new MockResult[0];

        [TestInitialize]
        public void Initialize()
        {
            var dummyOperation = GetType().GetMethod("GetResults");
            var proxyDouble = new ServiceProxyDouble
            {
                Client = this,
                Operations = new[] { dummyOperation }
            };
            _reader = new WcfDataReader(proxyDouble, dummyOperation, new WcfParameterCollection());
        }

        [TestMethod]
        public void GivenTheFieldIndexIsLessThanZeroThenAnArgumentOutOfRangeExceptionIsThrown()
        {
            Action action = () => _reader.GetValue(-1);

            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void GivenTheFieldIndexIsGreaterThanTheFieldCountThenAnArgumentOutOfRangeExceptionIsThrown()
        {
            Action action = () => _reader.GetValue(3);

            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void GivenReadHasNotBeenCalledThenAnInvalidOperationExceptionIsThrown()
        {
            GivenOneResultWith("ExpectedName", 1);

            Action action = () => _reader.GetValue(0);

            action.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void GivenOneResultAndTheFieldIndexIsTheNamePropertyThenTheValueForNameIsReturned()
        {
            GivenOneResultWith("ExpectedName", 1);

            _reader.Read();

            var value = _reader.GetValue(0);

            value.Should().Be("ExpectedName");
        }

        private void GivenOneResultWith(string expectedname, int expectedValue)
        {
            _mockResults = new[]
            {
                new MockResult { Name = expectedname, Value = expectedValue }
            };
        }

        public MockResult[] GetResults()
        {
            return _mockResults;
        }

        public void Dispose()
        {
        }
    }

    public class ServiceProxyDouble : ServiceProxy
    {

    }
}