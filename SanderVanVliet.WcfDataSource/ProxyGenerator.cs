using System;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace SanderVanVliet.WcfDataSource
{
    public class ProxyGenerator
    {
        public ServiceProxy For(Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            if (!uri.ToString().EndsWith("?wsdl"))
            {
                uri = new Uri(uri + "?wsdl");
            }

            var metadataGenerator = new MetadataGenerator(uri);
            var importer = new MetadataImporter();

            var assembly = importer.ImportFromMetadata(metadataGenerator.DownloadMetadata());

            var contracts = assembly
                .GetTypes()
                .Where(t => t.IsInterface &&
                            t.IsPublic &&
                            t.GetCustomAttributes(typeof (ServiceContractAttribute), false).Any())
                .ToList();

            if (!contracts.Any())
            {
                throw new Exception("No contracts found");
            }

            if (contracts.Count > 1)
            {
                throw new Exception("Multiple contracts found on service");
            }

            var contractType = contracts.Single();
            var contractName = contractType.Name;

            var clientBaseType = typeof (ClientBase<>).MakeGenericType(contractType);

            var proxyType = assembly
                .GetTypes()
                .SingleOrDefault(t => t.IsClass &&
                                      t.IsPublic &&
                                      contractType.IsAssignableFrom(t) &&
                                      t.IsSubclassOf(clientBaseType));

            if (proxyType == null)
            {
                throw new Exception("No client proxy found for contract " + contractName);
            }

            var endpoint = new EndpointAddress(uri);

            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
            binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Windows;

            var clientInstance = Activator.CreateInstance(proxyType, new object[] {binding, endpoint});

            // The binding must allow for impersonation
            // TODO: Make this configurable
            SetImpersonationAllowed(clientInstance);

            var proxyInstance = new ServiceProxy
            {
                Contract = contractType,
                Operations = GetOperationsFromContract(contractType),
                Client = (IDisposable)clientInstance    // ClientBase<> is IDisposable
            };

            return proxyInstance;
        }

        private static void SetImpersonationAllowed(object clientInstance)
        {
            var serviceEndpoint = clientInstance
                .GetType()
                .GetProperty("Endpoint")
                .GetGetMethod()
                .Invoke(clientInstance, null) as ServiceEndpoint;

            var clientCredentials = serviceEndpoint
                .Behaviors
                .SingleOrDefault(b => b is ClientCredentials) as ClientCredentials;

            if (clientCredentials == null)
            {
                clientCredentials = new ClientCredentials();
                serviceEndpoint.Behaviors.Add(clientCredentials);
            }

            clientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation;
        }

        private static MethodInfo[] GetOperationsFromContract(Type contractType)
        {
            return contractType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => m.GetCustomAttributes(typeof (OperationContractAttribute), false).Any())
                .ToArray();
        }
    }
}