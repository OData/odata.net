//---------------------------------------------------------------------
// <copyright file="DataProviderSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System.Runtime.Serialization;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Specifies provider-level settings used during data service build process. 
    /// </summary>
    [ImplementationSelector("ServiceBuilder/Provider", DefaultImplementation = "EntityFramework")]
    public class DataProviderSettings : ServiceBuilderSettingsBase
    {
        /// <summary>
        /// Initializes a new instance of the DataProviderSettings class.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        public DataProviderSettings(string providerName)
        {
            this.DataProviderKind = providerName;
            this.SupportsMest = true;
            this.SupportsSpatial = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether using payload-driven verification
        /// </summary>
        [InjectTestParameter("UsePayloadDrivenVerification", DefaultValueDescription = "False", HelpText = "Whether the tests should use payload-driven verification")]
        [IgnoreDataMember]
        public virtual bool UsePayloadDrivenVerification { get; set; }

        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        [IgnoreDataMember]
        public string DataProviderKind { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the provider supports mest of not
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mest", Justification = "Naming is correct")]
        [IgnoreDataMember]
        public bool SupportsMest { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the provider supports mest of not
        /// </summary>
        [IgnoreDataMember]
        public bool SupportsSpatial { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the provider uses sql server.
        /// </summary>
        [IgnoreDataMember]
        public virtual bool IsRunOnSqlServer 
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets the name of the language
        /// </summary>
        [InjectTestParameter("Language")]
        public string Language { get; set; }

        /// <summary>
        /// Returns a fixup for making provider-specific alterations to the model, or null if no alterations are needed
        /// </summary>
        /// <returns>A provider-specific fixup or null</returns>
        public virtual IEntityModelFixup GetProviderSpecificModelFixup()
        {
            return null;
        }

        /// <summary>
        /// Returns a service method resolver that can generate service method code appropriate to the provider
        /// </summary>
        /// <returns>A provider-specific service method resolver</returns>
        public virtual IServiceMethodResolver GetProviderSpecificServiceModelResolver()
        {
            return null;
        }
    }
}
