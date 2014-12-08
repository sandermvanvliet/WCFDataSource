using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SanderVanVliet.WcfDataSource.Tests
{
    [TestClass]
    public class WhenGettingTheFieldType
    {
        private WcfDataReader _reader;

        [TestInitialize]
        public void Initialize()
        {
            var dummyOperation = GetType().GetMethod("GetResults");

            _reader = new WcfDataReader(null, dummyOperation, null);
        }

        [TestMethod]
        public void GivenTheFieldIndexIsLessThanZeroThenAnArgumentOutOfRangeExceptionIsThrown()
        {
            Action action = () => _reader.GetFieldType(-1);

            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void GivenTheFieldIndexIsGreaterThanTheNumberOfFieldsThenAnArgumentOutOfRangeExceptionIsThrown()
        {
            Action action = () => _reader.GetFieldType(2);

            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void GivenTheFieldIndexIsTheNamePropertyThenTheFieldTypeIsReturned()
        {
            var type = _reader.GetFieldType(0);

            type.Should().Be<string>();
        }

        [TestMethod]
        public void GivenTheFieldIndexIsTheValuePropertyThenTheFieldTypeIsReturned()
        {
            var type = _reader.GetFieldType(1);

            type.Should().Be<int>();
        }

        public MockResult[] GetResults()
        {
            return new MockResult[0];
        }
    }
}