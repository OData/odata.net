//---------------------------------------------------------------------
// <copyright file="MetadataSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Serializers
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Service.Providers;

    #endregion Namespaces

    /// <summary>
    /// Provides support for serializing responses in CSDL format.
    /// </summary>
    internal static class MetadataSerializer
    {
        /// <summary>
        /// The set of schema validation rules that we exclude from the default rule set.
        /// </summary>
        private static readonly ValidationRule[] excludedSchemaValidationRules = new ValidationRule[]
        {
            // Do not perform name checks since we sometimes generate invalid names from CLR class names (e.g., MyClassName`1)
            ValidationRules.NamedElementNameIsNotAllowed,

            // Allow annotations to use terms that do not have formal definitions
            ValidationRules.AnnotationInaccessibleTerm,
        };

        /// <summary>
        /// The set of schema validation rules that we added to the default rule set.
        /// </summary>
        private static readonly ValidationRule[] additionalSchemaValidationRules = new ValidationRule[]
        {
            // Do not allow property names that contain one of the reserved characters '.', ':', '@'.
            MetadataProviderUtils.PropertyNameIncludesReservedODataCharacters,
        };

        /// <summary>
        /// The fixed EDMX version used by OData.  OData has no reason to update the 
        /// version number since it breaks all the clients that read it including the
        /// Add Service Reference code in VS.
        /// </summary>
        private static readonly Version edmxVersion = CsdlConstants.EdmxVersion4;

        /// <summary>
        /// Validates the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> to check.</param>
        /// <param name="edmSchemaVersion">The EDM version to be used.</param>
        internal static void ValidateModel(IEdmModel model, Version edmSchemaVersion)
        {
            ValidationRuleSet ruleSet = ValidationRuleSet.GetEdmModelRuleSet(edmSchemaVersion);
            ValidationRuleSet ruleSetWithoutTypeNameChecks = new ValidationRuleSet(
                ruleSet.Except(excludedSchemaValidationRules).Concat(additionalSchemaValidationRules));

            IEnumerable<EdmError> validationErrors;
            model.Validate(ruleSetWithoutTypeNameChecks, out validationErrors);

            if (validationErrors != null && validationErrors.Any())
            {
                StringBuilder builder = new StringBuilder();
                foreach (EdmError validationError in validationErrors)
                {
                    builder.AppendLine(validationError.ToString());
                }

                throw new DataServiceException(500, Microsoft.OData.Service.Strings.MetadataSerializer_ModelValidationErrors(builder.ToString()));
            }
        }

        /// <summary>
        /// Prepares the service's model for serialization during a $metadata request by adding versioning annotations and running validation.
        /// </summary>
        /// <param name="provider">The service's provider</param>
        /// <param name="configuration">The service's configuration</param>
        /// <returns>Returns the prepared model.</returns>
        internal static IEdmModel PrepareModelForSerialization(DataServiceProviderWrapper provider, DataServiceConfiguration configuration)
        {
            Debug.Assert(provider != null, "provider != null");
            Debug.Assert(configuration != null, "configuration != null");

            MetadataProviderEdmModel model = provider.GetMetadataProviderEdmModel();
            model.AssertCacheEmpty();

            // For computing the metadata version, we need to walk through all the types and operations.
            // This causes the model to be also completely populated and hence the metadata is loaded
            // based on the MPV on the server.
            // But there are some changes we made in 5.0 OOB release that we will display the right
            // value of Nullable attribute in $metadata for V3 and above. Since the metadata version
            // is not known, this is not correctly populated. Hence we need to compute the metadata
            // version first, and then clear the model so that it can be populated again based on the
            // the computed metadata version.
            Version minMetadataEdmSchemaVersion = model.MinMetadataEdmSchemaVersion;

            // NOTE: The below annotations on the model are only relevant for $metadata serialization.
            //       We set them every time the metadata document is written.   
            Version metadataEdmSchemaVersion = provider.SchemaVersion.ToVersion();

            if (minMetadataEdmSchemaVersion > metadataEdmSchemaVersion)
            {
                metadataEdmSchemaVersion = minMetadataEdmSchemaVersion;
            }

            model.SetEdmVersion(metadataEdmSchemaVersion);

            // For Astoria, set the EDMX version of the model always to "1.0"
            model.SetEdmxVersion(edmxVersion);

            // now load the entire model.
            model.EnsureFullMetadataLoaded();

            // even though there may not be user annotations there may be an annotation for Azure key url, this must always be called
            model.AnnotationsCache.PopulateFromConfiguration(configuration);

            // If model validation is enabled, validate the model before writing.
            if (!configuration.DisableValidationOnMetadataWrite)
            {
                ValidateModel(model, metadataEdmSchemaVersion);
            }

            return model;
        }
    }
}
