using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SanderVanVliet.WcfDataSource.Tests
{
    [TestClass]
    public class WhenReadingData : IDisposable
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
        public void GivenThereAreTwoItemsInTheResultThenTrueIsReturned()
        {
            GivenTwoItemsInTheResult();

            _reader
                .Read()
                .Should()
                .BeTrue();
        }

        [TestMethod]
        public void GivenTwoItemsInTheResultAndReadCalledThreeTimesThenTheLastReadReturnsFalse()
        {
            GivenTwoItemsInTheResult();
            
            AndReadCalled(2);

            _reader
                .Read()
                .Should()
                .BeFalse();
        }

        [TestMethod]
        public void GivenNoItemsInTheResultThenTheFirstReadReturnsFalse()
        {
            _reader
                .Read()
                .Should()
                .BeFalse();
        }

        private void AndReadCalled(int numberOfCalls)
        {
            for (var i = 0; i < numberOfCalls; i++)
            {
                _reader.Read();
            }
        }

        private void GivenTwoItemsInTheResult()
        {
            _mockResults = new[]
            {
                new MockResult { Name = "Name1"},
                new MockResult { Name = "Name2"}
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
}