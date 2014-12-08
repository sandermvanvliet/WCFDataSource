using System;
using Microsoft.ReportingServices.DataProcessing;

namespace SanderVanVliet.WcfDataSource
{
    public class WcfConnection : IDbConnectionExtension
    {
        public void Dispose()
        {
        }

        public void SetConfiguration(string configuration)
        {
        }

        public void Open()
        {
        }

        public void Close()
        {
        }

        public IDbCommand CreateCommand()
        {
            return new WcfCommand(this);
        }

        public IDbTransaction BeginTransaction()
        {
            throw new NotSupportedException();
        }

        public string LocalizedName
        {
            get { return "WCF Data Source"; }
        }

        public string ConnectionString { get; set; }

        public int ConnectionTimeout { get; private set; }
        public string Impersonate { set; private get; }
        public string UserName { set; private get; }
        public string Password { set; private get; }
        public bool IntegratedSecurity { get; set; }
    }
}