using System.Reflection;
using Microsoft.ReportingServices.DataProcessing;

namespace SanderVanVliet.WcfDataSource
{
    public class WcfDataSchemaOnlyReader : WcfDataReader
    {
        public WcfDataSchemaOnlyReader(ServiceProxy proxy, MethodInfo operation, IDataParameterCollection parameters)
            : base(proxy, operation, parameters)
        {
        }

        protected override object[] GetDataFromOperation()
        {
            return new object[0];
        }
    }
}