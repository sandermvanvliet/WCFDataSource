using System;
using System.Linq;
using System.Reflection;
using Microsoft.ReportingServices.DataProcessing;

namespace SanderVanVliet.WcfDataSource
{
    public class WcfCommand : IDbCommand
    {
        private readonly ServiceProxy _proxy;
        private string _commandText;
        private MethodInfo _operation;

        public WcfCommand(IDbConnection wcfConnection)
        {
            _proxy = new ProxyGenerator().For(new Uri(wcfConnection.ConnectionString));
        }

        public void Dispose()
        {
            _proxy?.Client.Dispose();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            if (behavior == CommandBehavior.SchemaOnly)
            {
                return new WcfDataSchemaOnlyReader(_proxy, _operation, Parameters);
            }

            return new WcfDataReader(_proxy, _operation, Parameters);
        }

        public IDataParameter CreateParameter()
        {
            if (Parameters == null)
            {
                Parameters = new WcfParameterCollection();
            }

            return new WcfCommandParameter();
        }

        public void Cancel()
        {
        }

        public string CommandText
        {
            get { return _commandText; }
            set
            {
                _commandText = value;
                SetOperation(_commandText);
            }
        }

        public int CommandTimeout { get; set; }
        public CommandType CommandType { get; set; }
        public IDataParameterCollection Parameters { get; private set; }
        public IDbTransaction Transaction { get; set; }

        private void SetOperation(string commandText)
        {
            _operation = _proxy.Operations.SingleOrDefault(o => o.Name == commandText);

            if (_operation == null)
            {
                throw new Exception("Operation '" + CommandText + "' not found");
            }

            var wcfParameters = (WcfParameterCollection) Parameters;
            if (wcfParameters == null)
            {
                Parameters = wcfParameters = new WcfParameterCollection();
            }

            foreach (var p in _operation.GetParameters())
            {
                if (wcfParameters.All(wp => wp.ParameterName != p.Name))
                {
                    wcfParameters.Add(new WcfCommandParameter {ParameterName = p.Name});
                }
            }
        }
    }
}