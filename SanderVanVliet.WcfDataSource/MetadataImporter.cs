using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace SanderVanVliet.WcfDataSource
{
    public class MetadataImporter
    {
        private Collection<Binding> _bindings;
        private Collection<ContractDescription> _contracts;
        private ServiceEndpointCollection _endpoints;
        private ServiceContractGenerator _contractGenerator;

        public Assembly ImportFromMetadata(IEnumerable<MetadataSection> metadata)
        {
            var compilationUnit = new CodeCompileUnit();
            var codeProvider = CreateCodeDomProvider();

            var importer = new WsdlImporter(new MetadataSet(metadata));

            _contracts = importer.ImportAllContracts();
            _bindings = importer.ImportAllBindings();
            _endpoints = importer.ImportAllEndpoints();

            if (importer.Errors.Any(error => !error.IsWarning))
            {
                var errorMessage = string.Join(Environment.NewLine, importer.Errors.Select(e => e.Message).ToArray());

                throw new Exception("Import failed: " + errorMessage);
            }

            _contractGenerator = new ServiceContractGenerator(compilationUnit);
            _contractGenerator.Options |= ServiceContractGenerationOptions.ClientClass;

            return CreateProxy(codeProvider, compilationUnit);
        }

        private Assembly CreateProxy(CodeDomProvider codeProvider, CodeCompileUnit compileUnit)
        {
            foreach (var contract in _contracts)
            {
                _contractGenerator.GenerateServiceContractType(contract);
            }

            if (_contractGenerator.Errors.Any(error => !error.IsWarning))
            {
                var errorMessage = string.Join(Environment.NewLine, _contractGenerator.Errors.Select(e => e.Message).ToArray());

                throw new Exception("Contract generation failed: " + errorMessage);
            }

            string rawSourceCode = null;

            using (var writer = new StringWriter())
            {
                var options = new CodeGeneratorOptions
                {
                    BracingStyle = "C"
                };

                codeProvider.GenerateCodeFromCompileUnit(compileUnit, writer, options);

                writer.Flush();

                rawSourceCode = writer.ToString();
            }

            var parameters = new CompilerParameters();

            AddAllAssemblyReferences(parameters);

            var results = codeProvider.CompileAssemblyFromSource(parameters, rawSourceCode);

            if (results.Errors != null && results.Errors.HasErrors)
            {
                var errorMessage = string.Join(Environment.NewLine, results.Errors.OfType<CompilerError>().Select(e => e.ErrorText).ToArray());

                throw new Exception("Proxy generation failed: " + errorMessage);
            }

            return Assembly.LoadFile(results.PathToAssembly);
        }

        private void AddAllAssemblyReferences(CompilerParameters parameters)
        {
            AddAssemblyReference(
                typeof (System.ServiceModel.ServiceContractAttribute).Assembly,
                parameters.ReferencedAssemblies);

            AddAssemblyReference(
                typeof (System.Web.Services.Description.ServiceDescription).Assembly,
                parameters.ReferencedAssemblies);

            AddAssemblyReference(
                typeof (System.Runtime.Serialization.DataContractAttribute).Assembly,
                parameters.ReferencedAssemblies);

            AddAssemblyReference(typeof (System.Xml.XmlElement).Assembly,
                parameters.ReferencedAssemblies);

            AddAssemblyReference(typeof (System.Uri).Assembly,
                parameters.ReferencedAssemblies);

            AddAssemblyReference(typeof (System.Data.DataSet).Assembly,
                parameters.ReferencedAssemblies);
        }

        private static CodeDomProvider CreateCodeDomProvider()
        {
            return CodeDomProvider.CreateProvider("CS");
        }

        private void AddAssemblyReference(Assembly referencedAssembly, StringCollection refAssemblies)
        {
            var path = Path.GetFullPath(referencedAssembly.Location);
            var name = Path.GetFileName(path);

            if (!(refAssemblies.Contains(name) || refAssemblies.Contains(path)))
            {
                refAssemblies.Add(path);
            }
        }
    }
}