using System.Collections.Generic;
using Microsoft.ReportingServices.DataProcessing;

namespace SanderVanVliet.WcfDataSource
{
    public class WcfParameterCollection : List<IDataParameter>, IDataParameterCollection
    {
        public new int Add(IDataParameter parameter)
        {
            base.Add(parameter);

            return 0;
        }
    }
}