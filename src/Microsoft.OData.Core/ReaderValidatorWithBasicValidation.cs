//---------------------------------------------------------------------
// <copyright file="ReaderValidatorWithBasicValidation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.JsonLight;
    using Microsoft.OData.Metadata;

    /// <summary>
    /// Reader validator with BasicValidation functionality.
    /// </summary>
    internal class ReaderValidatorWithBasicValidation : ReaderValidatorWithoutBasicValidation
    {
        internal ReaderValidatorWithBasicValidation(ODataMessageReaderSettings settings)
            : base(settings)
        {
            // nop
        }

        /// <summary>
        /// Validates that the specified <paramref name="resource"/> is a valid resource as per the specified type.
        /// </summary>
        /// <param name="resource">The resource to validate.</param>
        /// <param name="resourceType">Optional entity type to validate the resource against.</param>
        /// <param name="model">Model containing the entity type.</param>
        /// <remarks>If the <paramref name="resourceType"/> is available only resource-level tests are performed,
        /// properties and such are not validated.</remarks>
        public override void ValidateMediaResource(ODataResource resource, IEdmEntityType resourceType, IEdmModel model)
        {
            ValidationUtils.ValidateMediaResource(resource, resourceType, model, true);
        }
    }
}
