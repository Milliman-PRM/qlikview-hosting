using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.IO;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using ComponentPro.Saml2;
using ComponentPro.Saml2.Metadata;

namespace MillimanDev
{
    /// <summary>
    /// Summary description for SSOConfiguration
    /// </summary>
    public static class SSOConfiguration
    {
        public static string IdPSingleSignonIdProviderUrl;
        public static string IdPArtifactIdProviderUrl;
        public static string IdPLogoutIdProviderUrl;
        public static string BindingUri;

        public static void LoadMetadataFromFile(string QualifiedFilename)
        {
            XmlElement xmlElement;

            XmlDocument doc = new XmlDocument();

            doc.Load(QualifiedFilename);
            
            xmlElement = doc.DocumentElement;

            EntityDescriptor entityDescriptor;

            if (EntitiesDescriptor.IsValid(xmlElement))
            {
                System.Diagnostics.Trace.WriteLine("Reading SAML entities descriptor metadata");

                EntitiesDescriptor entitiesDescriptor = new EntitiesDescriptor(xmlElement);

                System.Diagnostics.Trace.WriteLine(entitiesDescriptor.GetXml().OuterXml);

                if (entitiesDescriptor.EntityDescriptors.Count == 0)
                    throw new ArgumentException("No entity descriptors found");

                entityDescriptor = entitiesDescriptor.EntityDescriptors[0];
            }
            else if (EntityDescriptor.IsValid(xmlElement))
            {
                System.Diagnostics.Trace.WriteLine("Reading SAML entity descriptor metadata");

                entityDescriptor = new EntityDescriptor(xmlElement);

                System.Diagnostics.Trace.WriteLine(entityDescriptor.GetXml().OuterXml);
            }
            else
            {
                throw new ArgumentException("Expecting entities descriptor or entity descriptor");
            }
#if STREAMED_CERT
           if (!entityDescriptor.IsSigned())
                throw new ArgumentException("Expecting signed entity descriptor");

            // Load up the cached certificate.
            X509Certificate2 x509Certificate = (X509Certificate2)HttpContext.Current.Application[Global.IdPCertKey];

            // Verify the entity descriptor's signature
            if (!entityDescriptor.Validate(x509Certificate))
                throw new ArgumentException("Invalid entity descriptor's signature");
#endif
            foreach (IdpSsoDescriptor idp in entityDescriptor.IdpSsoDescriptors)
            {
                if (idp.ArtifactResolutionServices.Count > 0)
                    IdPArtifactIdProviderUrl = idp.ArtifactResolutionServices[0].Location;

                if (idp.SingleLogoutServices.Count > 0)
                    IdPLogoutIdProviderUrl = idp.SingleLogoutServices[0].Location;

                if (idp.SingleSignOnServices.Count > 0)
                    IdPSingleSignonIdProviderUrl = idp.SingleSignOnServices[0].Location;
            }
        }

        public static XmlElement CreateSPMetadata(string rootPath, string bindingUri)
        {
            // Load your certificate.
            X509Certificate2 x509Certificate = new X509Certificate2(HttpContext.Current.Server.MapPath("~/SSOConfigure/SPKey.pfx"), "password");

            // Create Entity Descriptor with ID received from the IdP.
            EntityDescriptor descriptor = new EntityDescriptor();
            descriptor.Id = "84CCAA9F05EE4BA1B13F8943FDF1D320";
            SpSsoDescriptor spd = new SpSsoDescriptor();
            spd.Id = "MillimanHCIntel";
            spd.AuthnRequestsSigned = true;
            spd.ProtocolSupportEnumeration = "urn:oasis:names:tc:SAML:2.0:protocol";

            // Creating a signing key.
            KeyDescriptor signingKey = new KeyDescriptor();
            signingKey.Use = "signing";
            KeyInfoX509Data keyData = new KeyInfoX509Data(x509Certificate);
            signingKey.KeyInfo = keyData.GetXml();
            spd.KeyDescriptors.Add(signingKey);

            // Assign assertion service url.
            AssertionConsumerService consumerService = new AssertionConsumerService();
            consumerService.Index = 0;
            consumerService.IsDefault = true;
            consumerService.Location = rootPath + "AssertionService.aspx";
            consumerService.Binding = bindingUri;
            spd.AssertionConsumerServices.Add(consumerService);

            // Assign Single Logout Service Url
            ComponentPro.Saml2.Metadata.SingleLogoutService slo = new ComponentPro.Saml2.Metadata.SingleLogoutService();
            slo.Location = rootPath + "SingleLogoutService.aspx";
            spd.SingleLogoutServices.Add(slo);

            // Assign Artifact Service Url
            ComponentPro.Saml2.Metadata.ArtifactResolutionService ars = new ComponentPro.Saml2.Metadata.ArtifactResolutionService();
            ars.Location = rootPath + "SamlArtifactResolve.aspx";
            spd.ArtifactResolutionServices.Add(ars);

            descriptor.SpSsoDescriptors.Add(spd);

            // Add some information.
            // Organization information
            descriptor.Organization = new Organization();
            descriptor.Organization.OrganizationNames.Add(new OrganizationName("Milliman", "en"));
            descriptor.Organization.OrganizationDisplayNames.Add(new OrganizationDisplayName("Milliman", "en"));
            descriptor.Organization.OrganizationUrls.Add(new OrganizationUrl("https://www.milliman.com", "en"));

            // Add contact person info.
            ContactPerson person = new ContactPerson();
            person.Company = "Support";
            person.EmailAddresses.Add("prm.support@milliman.com");
            // Contact information
            descriptor.ContactPeople.Add(person);
            // Sign metadata with service provider key
            descriptor.Sign(x509Certificate);

            BindingUri = bindingUri;

            return descriptor.GetXml();
        }
    }
}