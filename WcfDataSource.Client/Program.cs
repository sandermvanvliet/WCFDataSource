using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.ReportingServices.DataProcessing;
using SanderVanVliet.WcfDataSource;

namespace WcfDataSource.Client
{
    internal class Program
    {
        private static ServiceHost _serviceHost;
        private static Uri _baseAddress;

        private static void Main(string[] args)
        {
            const string baseUri = "http://localhost:8181/TestService";
            _baseAddress = new Uri(baseUri);
            BootstrapTestService();

            try
            {
                //TestProxyCreation(args);

                TestWcfConnection(baseUri);
            }
            catch (Exception)
            {
                _serviceHost.Close();
            }

            Console.WriteLine("Done. Press enter to exit");
            Console.ReadLine();
        }

        private static void BootstrapTestService()
        {
            _serviceHost = new ServiceHost(typeof(TestService), _baseAddress);

            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            _serviceHost.Description.Behaviors.Add(smb);

            _serviceHost.AddServiceEndpoint(
                typeof (ITestService),
                new BasicHttpBinding(),
                _baseAddress);

            _serviceHost.Open();
        }

        private static void TestWcfConnection(string connectionString)
        {
            var connection = new WcfConnection
            {
                ConnectionString = connectionString,
                IntegratedSecurity = true
            };

            var command = connection.CreateCommand();
            command.CommandText = "GetData";
            var wcfCommandParameters = command.Parameters.OfType<WcfCommandParameter>();

            //wcfCommandParameters.Single(p => p.ParameterName == "reportDate").Value = new DateTime(2014, 8, 8);
            //wcfCommandParameters.Single(p => p.ParameterName == "majorScenarioTriggerLevel").Value = 10;
            //wcfCommandParameters.Single(p => p.ParameterName == "minorScenarioTriggerLevel").Value = 85;

            var reader = command.ExecuteReader(CommandBehavior.SchemaOnly);

            Console.WriteLine("Found " + reader.FieldCount + " fields");

            for (var i = 0; i < reader.FieldCount; i++)
            {
                Console.WriteLine("\t" + reader.GetName(i) + " => " + reader.GetFieldType(i).Name);
            }

            while (reader.Read())
            {
                Console.WriteLine("line!");
            }
        }

        private static void TestProxyCreation(string[] args)
        {
            var proxyGenerator = new ProxyGenerator();
            var uri = new Uri(args[0]);

            var proxy = proxyGenerator.For(uri);

            proxy.Operations.ToList().ForEach(m => Console.WriteLine("\tOperation: " + m.Name));
        }
    }
}