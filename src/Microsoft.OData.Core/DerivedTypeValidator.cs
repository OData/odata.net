//---------------------------------------------------------------------
// <copyright file="DerivedTypeValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;

    /// <summary>
    ///  Helper class to verify the resource type meets the derived type constraints.
    /// </summary>
    internal sealed class DerivedTypeValidator
    {
        private IEnumerable<string> derivedTypeConstraints;
        private IEdmType expectedType;
        private string resourceKind;
        private string resourceName;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="expectedType">The expected type.</param>
        /// <param name="derivedTypeConstraints">The derived type constraints.</param>
        /// <param name="resourceKind">The resource type, be used at error message.</param>
        /// <param name="resourceName">The resource name, be used at error message.</param>
        public DerivedTypeValidator(IEdmType expectedType, IEnumerable<string> derivedTypeConstraints, string resourceKind, string resourceName)
        {
            this.derivedTypeConstraints = derivedTypeConstraints;
            this.expectedType = expectedType;
            this.resourceKind = resourceKind;
            this.resourceName = resourceName;
        }

        /// <summary>
        /// Validates the type of a resource.
        /// </summary>
        /// <param name="resourceType">The type of the resource.</param>
        internal void ValidateResourceType(IEdmType resourceType)
        {
            if (resourceType == null)
            {
                return;
            }

            ValidateResourceType(resourceType.FullTypeName());
        }

        /// <summary>
        /// Validates the full type name of a resource.
        /// </summary>
        /// <param name="resourceTypeName">The full type name of the resource.</param>
        internal void ValidateResourceType(string resourceTypeName)
        {
            if (resourceTypeName == null)
            {
                return;
            }

            if (this.expectedType != null && this.expectedType.FullTypeName() == resourceTypeName)
            {
                return;
            }

            if (derivedTypeConstraints == null || derivedTypeConstraints.Any(c => c == resourceTypeName))
            {
                return;
            }

            throw new ODataException(Strings.ReaderValidationUtils_ValueTypeNotAllowedInDerivedTypeConstraint(resourceTypeName, resourceKind, resourceName));
        }
    }
}
