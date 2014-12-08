using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SanderVanVliet.WcfDataSource.Tests
{
    [TestClass]
    public class WhenGettingTheOrdinal
    {
        private WcfDataReader _reader;

        [TestInitialize]
        public void Initialize()
        {
            var dummyOperation = GetType().GetMethod("GetResults");

            _reader = new WcfDataReader(null, dummyOperation, null);
        }

        [TestMethod]
        public void GivenTheFieldNameIsNullAnArgumentNullExceptionIsThrown()
        {
            Action action = () => _reader.GetOrdinal(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void GivenTheFieldNameIsAnEmptyStringThenAnArgumentNullExceptionIsThrown()
        {
            Action action = () => _reader.GetOrdinal("");

            action.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void GivenTheFieldNameIsNameThenZeroIsReturned()
        {
            var type = _reader.GetOrdinal("Name");

            type.Should().Be(0);
        }

        [TestMethod]
        public void GivenTheFieldNameIsValueThenOneIsReturned()
        {
            var type = _reader.GetOrdinal("Value");

            type.Should().Be(1);
        }

        [TestMethod]
        public void GivenTheFieldNameDoesNotExistThenAnExceptionIsThrown()
        {
            Action action = () => _reader.GetOrdinal("DoesNotExist");

            action.ShouldThrow<Exception>().And.Message.Should().Be("Field 'DoesNotExist' not found");
        }

        public MockResult[] GetResults()
        {
            return new MockResult[0];
        }
    }
}