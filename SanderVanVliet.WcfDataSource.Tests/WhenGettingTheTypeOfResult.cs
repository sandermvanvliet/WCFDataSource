using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SanderVanVliet.WcfDataSource.Tests
{
    [TestClass]
    public class WhenGettingTheTypeOfResult : IDisposable
    {
        private WcfDataReader GetReaderFor(string operation)
        {
            var dummyOperation = GetType().GetMethod(operation);

            var proxyDouble = new ServiceProxyDouble
            {
                Client = this,
                Operations = new[] { dummyOperation }
            };

            return new WcfDataReader(proxyDouble, dummyOperation, new WcfParameterCollection());
        }

        [TestMethod]
        public void GivenTheOperationReturnsAnArrayThenTheTypeIsInferred()
        {
            var reader = GetReaderFor("GetResultsAsArray");
            reader.Read();
        }

        [TestMethod]
        public void GivenTheOperationReturnsAnEnumerableThenTheTypeIsInferred()
        {
            var reader = GetReaderFor("GetResultsAsIEnumerable");
            reader.Read();
        }

        [TestMethod]
        public void GivenTheOperationReturnsASingleTypeThenTheTypeIsInferred()
        {
            var reader = GetReaderFor("GetResultAsSingleType");
            reader.Read();
        }

        public MockResult[] GetResultsAsArray()
        {
            return new MockResult[0];
        }

        public IEnumerable<MockResult> GetResultsAsIEnumerable()
        {
            return new MockResult[0];
        }

        public MockResult GetResultAsSingleType()
        {
            return new MockResult();
        }

        public void Dispose()
        {
        }
    }
}