//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Providers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Stores information about the referential constraint.
    /// </summary>
    internal class ResourceReferentialConstraint
    {
        /// <summary>list of custom annotations that needs to be flowed via $metadata endpoint.</summary>
        private Dictionary<string, object> customAnnotations;

        /// <summary>
        /// Creates a new instance of ResourceReferentialConstraint.
        /// </summary>
        /// <param name="principalEnd">Principal end of the referential constraint.</param>
        /// <param name="dependentProperties">List of the properties from the dependent end that take part in this constraint.</param>
        internal ResourceReferentialConstraint(ResourceAssociationTypeEnd principalEnd, IEnumerable<ResourceProperty> dependentProperties)
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
        internal void AddCustomAnnotation(string annotationNamespace, string annotationName, object annotationValue)
        {
            WebUtil.ValidateAndAddAnnotation(ref this.customAnnotations, annotationNamespace, annotationName, annotationValue);
        }
    }
}
