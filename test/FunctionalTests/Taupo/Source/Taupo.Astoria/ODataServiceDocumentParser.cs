//---------------------------------------------------------------------
// <copyright file="ODataServiceDocumentParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Implements the IServiceDocumentParser interface.
    /// </summary>
    [ImplementationName(typeof(IServiceDocumentParser), "Default")]
    public class ODataServiceDocumentParser : IServiceDocumentParser
    {
        /// <summary>
        /// This field contains a Dictionary of entityset name to the collection root Uri
        /// </summary>
        private Dictionary<string, Uri> entitySetLinks;

        /// <summary>
        /// Parses the Service document and builds a Dictionary of entity set name to Uri 
        /// </summary>
        /// <param name="serviceDocument">The Service Document of an OData compliant service</param>
        /// <returns>A Dictionary of entity set name to Uri </returns>
        public IEntitySetResolver ParseServiceDocument(XElement serviceDocument)
        {
            this.entitySetLinks = new Dictionary<string, Uri>();
            this.ParseInternal(serviceDocument);
            ServiceDocumentBasedEntitySetResolver entitySetResolver = new ServiceDocumentBasedEntitySetResolver(this.entitySetLinks);
            return entitySetResolver;
        }

        /// <summary>
        /// Parses the service document and initializes a dictionary of entityset name to collection root Uri
        /// </summary>
        /// <param name="serviceElement">The service document of the OData Service</param>
        private void ParseInternal(XElement serviceElement)
        {
            // typical service documents look like this 
            // <service>
            //  <workspace>
            //      <collection href="root uri of collection">
            //          <atom:title>name of the collection</atom:title>
            //      </collection>
            //  </workspace>
            // </service>
            // inline expanded service documents look like this 
            // <service>
            //  <workspace>
            //      <collection href="root uri of collection">
            //          <atom:title>name of the collection</atom:title>
            //          <m:inline xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">
            //              <service>
            //               <workspace>
            //                   <collection href="root uri of collection">
            //                       <atom:title>name of the collection</atom:title>
            //                   </collection>
            //               </workspace>
            //              </service>
            //          </m:inline>
            //      </collection>
            //  </workspace>
            // </service>
            // this section parses the <service> element in a service document
            (from workspaceElement in serviceElement.Elements(ODataConstants.WorkspaceElement)
             from collectionElement in workspaceElement.Elements(ODataConstants.CollectionElement)
             select collectionElement).ToList()
                        .ForEach(collection => this.ParseCollectionElement(collection));
        }

        /// <summary>
        /// Parses the &lt;collection&gt; element in a service document and initializes a dictionary of entitySet name to collection root Uri
        /// </summary>
        /// <param name="collectionElement">The collection element from the service document</param>
        private void ParseCollectionElement(XElement collectionElement)
        {
            string hrefAttribute = string.Empty;
            string collectionName = string.Empty;

            // <collection href="root uri of collection">
            //      <atom:title>name of the collection</atom:title>
            // </collection>
            ExceptionUtilities.CheckObjectNotNull(collectionElement.Element(ODataConstants.TitleElement), "Collection element did not contain a <title> element");
            collectionName = collectionElement.Element(ODataConstants.TitleElement).Value;

            if (collectionElement.Attribute(ODataConstants.HrefAttribute) == null)
            {
                hrefAttribute = collectionName;
            }
            else
            {
                hrefAttribute = collectionElement.Attribute(ODataConstants.HrefAttribute).Value;
            }

            Uri collectionRoot = new Uri(hrefAttribute, UriKind.RelativeOrAbsolute);

            // if the <href> attribute on a <collection> element points to a relative uri,
            if (!collectionRoot.IsAbsoluteUri)
            {
                var baseAttribute = collectionElement.AncestorsAndSelf().Select(element => element.Attribute(ODataConstants.XmlBaseAttribute)).Where(baseElement => baseElement != null).FirstOrDefault();
                ExceptionUtilities.Assert(baseAttribute != null, "Service document did not contain a valid base uri for the collection '{0}'", collectionName);
                collectionRoot = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", baseAttribute.Value.TrimEnd('/'), collectionRoot.OriginalString.TrimStart('/')), UriKind.Absolute);
            }

            this.entitySetLinks[collectionName] = collectionRoot;

            // <collection href="root uri of collection">
            //     <atom:title>name of the collection</atom:title>
            //     <m:inline xmlns:m="http://docs.oasis-open.org/odata/ns/metadata">
            //         <service>
            //          <workspace>
            //              <collection href="root uri of collection">
            //                  <atom:title>name of the collection</atom:title>
            //              </collection>
            //          </workspace>
            //         </service>
            //     </m:inline>
            // </collection>
            // if the service document contains any inline expanded service documents, we will parse those too.
            (from inlineElement in collectionElement.Elements(ODataConstants.InlineElement)
             from serviceElement in inlineElement.Elements(ODataConstants.ServiceElement)
             select serviceElement).ToList().ForEach(inlineService => this.ParseInternal(inlineService));
        }
    }
}
