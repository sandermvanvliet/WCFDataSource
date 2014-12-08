using System;
using System.Collections.Generic;
using System.ServiceModel.Description;
using System.Web.Services.Discovery;
using System.Xml;
using System.Xml.Schema;
using ServiceDescription = System.Web.Services.Description.ServiceDescription;

namespace SanderVanVliet.WcfDataSource
{
    public class MetadataGenerator
    {
        private readonly Uri _uri;

        public MetadataGenerator(Uri uri)
        {
            _uri = uri;
        }

        public IEnumerable<MetadataSection> DownloadMetadata()
        {
            var discovery = new DiscoveryClientProtocol
            {
                AllowAutoRedirect = true,
                UseDefaultCredentials = true
            };

            discovery.DiscoverAny(_uri.ToString());
            discovery.ResolveAll();

            var sections = new List<MetadataSection>();

            foreach (object document in discovery.Documents.Values)
            {
                var wsdl = document as ServiceDescription;
                if (wsdl != null)
                {
                    sections.Add(MapFromServiceDecsription(wsdl));
                }

                var schema = document as XmlSchema;
                if (schema != null)
                {
                    sections.Add(MapFromXmlSchema(schema));
                }

                var element = document as XmlElement;
                if (element != null && element.LocalName == "Policy")
                {
                    sections.Add(MapFromXmlElement(element));
                }
            }

            return sections;
        }

        private MetadataSection MapFromXmlSchema(XmlSchema schema)
        {
            return MetadataSection.CreateFromSchema(schema);
        }

        private MetadataSection MapFromXmlElement(XmlElement document)
        {
            return MetadataSection.CreateFromPolicy(document, null);
        }

        private MetadataSection MapFromServiceDecsription(ServiceDescription description)
        {
            return MetadataSection.CreateFromServiceDescription(description);
        }
    }
}