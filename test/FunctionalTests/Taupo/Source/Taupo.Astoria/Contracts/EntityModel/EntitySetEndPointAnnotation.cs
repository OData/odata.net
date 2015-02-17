//---------------------------------------------------------------------
// <copyright file="EntitySetEndPointAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    
    /// <summary>
    /// EntitySetEndPointAnnotation holds all the required for the RelayService so that it knows the endpoints
    /// </summary>
    public class EntitySetEndPointAnnotation : Annotation, ICustomAnnotationSerializer
    {
        /// <summary>
        /// Initializes a new instance of the EntitySetEndPointAnnotation class
        /// </summary>
        /// <param name="name">Name of EndPoint</param>
        /// <param name="allowedVerbs">Access of EndPoint as a list of allowed verbs</param>
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "3#", Justification = "Keeping test api and Product api the same")]
        public EntitySetEndPointAnnotation(
            string name,
            HttpVerb[] allowedVerbs)
            : base()
        {
            this.Name = name;
            this.AllowedVerbs = new Collection<HttpVerb>(allowedVerbs);
        }

         /// <summary>
        /// Initializes a new instance of the EntitySetEndPointAnnotation class
        /// </summary>
        /// <param name="element">XElement that contains the information</param>
        public EntitySetEndPointAnnotation(XElement element)
        {
            this.Name = element.Attribute("Name").Value;
            this.AllowedVerbs = new Collection<HttpVerb>();
            foreach (XElement childElement in element.Elements().Elements())
            {
                HttpVerb verb = (HttpVerb)Enum.Parse(typeof(HttpVerb), childElement.Value, true);
                this.AllowedVerbs.Add(verb);
            }
        }

        /// <summary>
        /// Gets AllowedVerbs
        /// </summary>
        public Collection<HttpVerb> AllowedVerbs { get; private set; }

        /// <summary>
        /// Gets the Name of Endpoint
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Creates an XElement that represents an EntitySetEndPointAnnotation
        /// </summary>
        /// <returns>an XElement with required information</returns>
        public XObject GetXObject()
        {
            return new XElement(
                XName.Get("EntitySetEndPointAnnotation", EdmConstants.TaupoAnnotationsNamespace.NamespaceName),
                new XAttribute("Name", this.Name),
                new XElement("AllowedVerbs", this.AllowedVerbs.Select(av => new XElement("AllowedVerb", av))));
        }
    }
}