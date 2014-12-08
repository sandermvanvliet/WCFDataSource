using Microsoft.ReportingServices.DataProcessing;

namespace SanderVanVliet.WcfDataSource
{
    public class WcfCommandParameter : IDataParameter
    {
        public string ParameterName { get; set; }
        public object Value { get; set; }
    }
}