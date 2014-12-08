using System;
using System.ServiceModel;

namespace WcfDataSource.Client
{
    public class TestService : ITestService
    {
        public MockData[] GetData(DateTime when, int howMany)
        {
            return new[]
            {
                new MockData(), 
                new MockData(), 
                new MockData(),
            };
        }
    }
}