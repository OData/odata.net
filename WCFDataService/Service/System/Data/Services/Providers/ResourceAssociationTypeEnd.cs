//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Providers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.Edm;

    /// <summary>
    /// Stores information about an end of an association.
    /// </summary>
    public class ResourceAssociationTypeEnd
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
        /// <param name="multiplicity">Multiplicity of the association.</param>
        /// <param name="deleteBehavior">Delete behavior.</param>
        public ResourceAssociationTypeEnd(string name, ResourceType resourceType, ResourceProperty resourceProperty, string multiplicity, EdmOnDeleteAction deleteBehavior)
        {
            Debug.Assert(!String.IsNullOrEmpty(name), "!String.IsNullOrEmpty(name)");
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(multiplicity == XmlConstants.Many || multiplicity == XmlConstants.One || multiplicity == XmlConstants.ZeroOrOne, "Invalid multiplicity value");
            Debug.Assert(deleteBehavior == EdmOnDeleteAction.None || deleteBehavior == EdmOnDeleteAction.Cascade, "Invalid deleteBehavior value");
            this.name = name;
            this.resourceType = resourceType;
            this.resourceProperty = resourceProperty;
            this.multiplicity = multiplicity;
            this.deleteAction = deleteBehavior;
        }

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
            Debug.Assert(resourceType != null, "resourceType != null");

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

        /// <summary>Type of the relationship end.</summary>
        public ResourceType ResourceType
        {
            get { return this.resourceType; }
        }

        /// <summary>Name of the relationship end </summary>
        internal string Name
        {
            get { return this.name; }
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
        public void AddCustomAnnotation(string annotationNamespace, string annotationName, object annotationValue)
        {
            WebUtil.ValidateAndAddAnnotation(ref this.customAnnotations, annotationNamespace, annotationName, annotationValue);
        }
    }
}
