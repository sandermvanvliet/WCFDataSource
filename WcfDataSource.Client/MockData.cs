using System;

namespace WcfDataSource.Client
{
    public class MockData
    {
        public MockData()
        {
            var random = (new Random()).Next()*100;

            Name = "Mock " + random;
            Counter = random;
        }

        public string Name { get; set; }
        public int Counter { get; set; }
    }
}