//---------------------------------------------------------------------
// <copyright file="ResourceAssociationTypeEnd.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Stores information about an end of an association.
    /// </summary>
    internal class ResourceAssociationTypeEnd
    {
        /// <summary>Name of the relationship end </summary>
        private readonly string name;

        /// <summary>Type of the relationship end.</summary>
        private readonly ResourceType resourceType;

        /// <summary>Property of the relationship end.</summary>
        private readonly ResourceProperty resourceProperty;

        /// <summary>Property on the related end that points to this end. The multiplicity of this end is determined from the fromProperty.</summary>
        private readonly string multiplicity;

        /// <summary>Operation action for this end.</summary>
        /// Since this is a internal field, its better to use the EdmLib enum for this rather than redefining the enum.
        private readonly EdmOnDeleteAction deleteAction;

        /// <summary>list of custom annotations that needs to be flowed via $metadata endpoint.</summary>
        private Dictionary<string, object> customAnnotations;

        /// <summary>
        /// Creates a new instance of EndInfo.
        /// </summary>
        /// <param name="name">name of the end.</param>
        /// <param name="resourceType">resource type that the end refers to.</param>
        /// <param name="resourceProperty">property of the end.</param>
        /// <param name="fromProperty">Property on the related end that points to this end. The multiplicity of this end is determined from the fromProperty.</param>
        internal ResourceAssociationTypeEnd(string name, ResourceType resourceType, ResourceProperty resourceProperty, ResourceProperty fromProperty)
        {
            Debug.Assert(!String.IsNullOrEmpty(name), "!String.IsNullOrEmpty(name)");
            Debug.Assert(resourceType != null, "type != null");

            this.name = name;
            this.resourceType = resourceType;
            this.resourceProperty = resourceProperty;

            if (fromProperty != null && fromProperty.Kind == ResourcePropertyKind.ResourceReference)
            {
                this.multiplicity = XmlConstants.ZeroOrOne;
            }
            else
            {
                this.multiplicity = XmlConstants.Many;
            }
        }

        /// <summary>
        /// Creates a new instance of EndInfo.
        /// </summary>
        /// <param name="name">name of the end.</param>
        /// <param name="resourceType">resource type that the end refers to.</param>
        /// <param name="resourceProperty">property of the end.</param>
        /// <param name="multiplicity">Multiplicity of the association.</param>
        /// <param name="deleteBehavior">Delete behavior.</param>
        internal ResourceAssociationTypeEnd(string name, ResourceType resourceType, ResourceProperty resourceProperty, string multiplicity, EdmOnDeleteAction deleteBehavior)
        {
            Debug.Assert(!String.IsNullOrEmpty(name), "!String.IsNullOrEmpty(name)");
            Debug.Assert(resourceType != null, "type != null");
            Debug.Assert(multiplicity == XmlConstants.Many || multiplicity == XmlConstants.One || multiplicity == XmlConstants.ZeroOrOne, "Invalid multiplicity value");
            Debug.Assert(deleteBehavior == EdmOnDeleteAction.None || deleteBehavior == EdmOnDeleteAction.Cascade, "Invalid deleteBehavior value");
            this.name = name;
            this.resourceType = resourceType;
            this.resourceProperty = resourceProperty;
            this.multiplicity = multiplicity;
            this.deleteAction = deleteBehavior;
        }

        /// <summary>Name of the relationship end </summary>
        internal string Name
        {
            get { return this.name; }
        }

        /// <summary>Type of the relationship end.</summary>
        internal ResourceType ResourceType
        {
            get { return this.resourceType; }
        }

        /// <summary>Property of the relationship end.</summary>
        internal ResourceProperty ResourceProperty
        {
            get { return this.resourceProperty; }
        }

        /// <summary>Mulitplicity of the relationship end </summary>
        internal string Multiplicity
        {
            get
            {
                return this.multiplicity;
            }
        }

        /// <summary>Action to be performed on the other end when the entity on this end is deleted.</summary>
        internal EdmOnDeleteAction DeleteBehavior
        {
            get
            {
                return this.deleteAction;
            }
        }

        /// <summary>
        /// Returns the list of custom annotations defined on this set.
        /// </summary>
        internal IEnumerable<KeyValuePair<string, object>> CustomAnnotations
        {
            get
            {
                if (this.customAnnotations == null)
                {
                    return WebUtil.EmptyKeyValuePairStringObject;
                }

                return this.customAnnotations;
            }
        }

        /// <summary>
        /// Add the given annotation to the list of annotations that needs to flowed via the $metadata endpoint
        /// </summary>
        /// <param name="annotationNamespace">NamespaceName to which the custom annotation belongs to.</param>
        /// <param name="annotationName">Name of the annotation.</param>
        /// <param name="annotationValue">Value of the annotation.</param>
        internal void AddCustomAnnotation(string annotationNamespace, string annotationName, object annotationValue)
        {
            WebUtil.ValidateAndAddAnnotation(ref this.customAnnotations, annotationNamespace, annotationName, annotationValue);
        }
    }
}
