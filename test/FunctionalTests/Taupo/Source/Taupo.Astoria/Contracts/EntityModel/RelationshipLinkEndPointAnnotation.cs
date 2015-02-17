//---------------------------------------------------------------------
// <copyright file="RelationshipLinkEndPointAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;

    /// <summary>
    /// RelationshipLinkEndPointAnnotation holds all the required for the RelayService to replace relationship links in payloads
    /// </summary>
    public class RelationshipLinkEndPointAnnotation : Annotation, ICustomAnnotationSerializer
    {       
        /// <summary>
        /// Initializes a new instance of the RelationshipLinkEndPointAnnotation class
        /// </summary>
        /// <param name="name">Name of EndPoint</param>
        /// <param name="navigationPropertyName">Name of navigation property that the entityset relates to</param>
        /// <param name="removeRelationshipLink">Whether to remove the relationship link from payload</param>
        /// <param name="removeLinkHref">Whether to remove href from the relationship link from payload</param>
        /// <param name="customHostName">Custom host name to inject in link uri</param>
        public RelationshipLinkEndPointAnnotation(
            string name, string navigationPropertyName, bool removeRelationshipLink, bool removeLinkHref, string customHostName)
            : base()
        {
            this.Name = name;
            this.NavigationPropertyName = navigationPropertyName;
            this.RemoveRelationshipLink = removeRelationshipLink;
            this.RemoveLinkHref = removeLinkHref;
            this.CustomHostName = customHostName == null ? string.Empty : customHostName;
            this.AppendRequestIdToName = false;
            this.UseRelativeLink = false;
        }

        /// <summary>
        /// Initializes a new instance of the RelationshipLinkEndPointAnnotation class
        /// </summary>
        /// <param name="name">Name of EndPoint</param>
        /// <param name="navigationPropertyName">Name of navigation property that the entityset relates to</param>
        /// <param name="removeRelationshipLink">Whether to remove the relationship link from payload</param>
        /// <param name="removeLinkHref">Whether to remove href from the relationship link from payload</param>
        /// <param name="customHostName">Custom host name to inject in link uri</param>
        /// <param name="appendRequestIdToName">Whether to append request Id to the annotation name</param>
        /// <param name="useRelativeLink">Whether to use relative Uri</param>
        public RelationshipLinkEndPointAnnotation(
            string name, string navigationPropertyName, bool removeRelationshipLink, bool removeLinkHref, string customHostName, bool appendRequestIdToName, bool useRelativeLink)
            : this(name, navigationPropertyName, removeRelationshipLink, removeLinkHref, customHostName)
        {
            this.AppendRequestIdToName = appendRequestIdToName;
            this.UseRelativeLink = useRelativeLink;
        }

        /// <summary>
        /// Initializes a new instance of the RelationshipLinkEndPointAnnotation class
        /// </summary>
        /// <param name="element">XElement that contains the information</param>
        public RelationshipLinkEndPointAnnotation(XElement element)
        {
            this.Name = element.Attribute("Name").Value;
            this.NavigationPropertyName = element.Attribute("NavigationPropertyName").Value;
            this.RemoveRelationshipLink = Convert.ToBoolean(element.Attribute("RemoveRelationshipLink").Value, CultureInfo.InvariantCulture);
            this.RemoveLinkHref = Convert.ToBoolean(element.Attribute("RemoveLinkHref").Value, CultureInfo.InvariantCulture);
            this.CustomHostName = element.Attribute("CustomHostName").Value;
            this.AppendRequestIdToName = Convert.ToBoolean(element.Attribute("AppendRequestIdToName").Value, CultureInfo.InvariantCulture);
            this.UseRelativeLink = Convert.ToBoolean(element.Attribute("UseRelativeLink").Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the Name of Endpoint
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the Name of NavigationPropertyName
        /// </summary>
        public string NavigationPropertyName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to Remove RelationshipLink
        /// </summary>
        public bool RemoveRelationshipLink { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to Remove href in RelationshipLink
        /// </summary>
        public bool RemoveLinkHref { get; private set; }

        /// <summary>
        /// Gets the customized name of host
        /// </summary>
        public string CustomHostName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to append request Id after annotation name
        /// </summary>
        public bool AppendRequestIdToName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to use relative Uri
        /// </summary>
        public bool UseRelativeLink { get; private set; }

        /// <summary>
        /// Creates an XElement that represents an RelationshipLinkEndPointAnnotation
        /// </summary>
        /// <returns>an XElement with required information</returns>
        public XObject GetXObject()
        {
            return new XElement(
                XName.Get("RelationshipLinkEndPointAnnotation", EdmConstants.TaupoAnnotationsNamespace.NamespaceName),
                new XAttribute("Name", this.Name),
                new XAttribute("NavigationPropertyName", this.NavigationPropertyName),
                new XAttribute("RemoveRelationshipLink", Convert.ToString(this.RemoveRelationshipLink, CultureInfo.InvariantCulture)),
                new XAttribute("RemoveLinkHref", Convert.ToString(this.RemoveLinkHref, CultureInfo.InvariantCulture)),
                new XAttribute("CustomHostName", this.CustomHostName),
                new XAttribute("AppendRequestIdToName", Convert.ToString(this.AppendRequestIdToName, CultureInfo.InvariantCulture)),
                new XAttribute("UseRelativeLink", Convert.ToString(this.UseRelativeLink, CultureInfo.InvariantCulture)));
        }
    }
}