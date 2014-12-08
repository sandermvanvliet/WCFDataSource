using System;
using System.ServiceModel;

namespace WcfDataSource.Client
{
    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        MockData[] GetData(DateTime when, int howMany);
    }
}