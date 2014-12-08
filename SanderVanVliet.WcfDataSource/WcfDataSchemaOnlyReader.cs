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
            // Return an empty object array to fake a resultset.
            // We don't even need to create an array of the operation result
            // because we'll never read from it.
            return new object[0];
        }
    }
}