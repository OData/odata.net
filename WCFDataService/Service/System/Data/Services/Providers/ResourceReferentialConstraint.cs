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
    using System.Linq;

    /// <summary>
    /// Stores information about the referential constraint.
    /// </summary>
    public class ResourceReferentialConstraint
    {
        /// <summary>list of custom annotations that needs to be flowed via $metadata endpoint.</summary>
        private Dictionary<string, object> customAnnotations;

        /// <summary>
        /// Creates a new instance of ResourceReferentialConstraint.
        /// </summary>
        /// <param name="principalEnd">Principal end of the referential constraint.</param>
        /// <param name="dependentProperties">List of the properties from the dependent end that take part in this constraint.</param>
        public ResourceReferentialConstraint(ResourceAssociationTypeEnd principalEnd, IEnumerable<ResourceProperty> dependentProperties)
        {
            Debug.Assert(principalEnd != null, "principalEnd != null");
            Debug.Assert(dependentProperties != null, "dependentProperties != null");
            Debug.Assert(principalEnd.ResourceType.KeyProperties.Count == dependentProperties.Count(), "principalEnd.ResourceType.KeyProperties.Count == dependentProperties.Count()");

            this.PrincipalEnd = principalEnd;
            this.DependentProperties = dependentProperties;
        }

        /// <summary>Returns the principal end for this referential constraint.</summary>
        internal ResourceAssociationTypeEnd PrincipalEnd
        {
            get;
            private set;
        }

        /// <summary>List of properties of the dependent end that take part in this constraint.</summary>
        internal IEnumerable<ResourceProperty> DependentProperties
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the list of custom annotations defined on this property.
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
