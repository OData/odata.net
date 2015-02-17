//---------------------------------------------------------------------
// <copyright file="DictionaryDataProviderSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System.Runtime.Serialization;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.EntityModel;

    /// <summary>
    /// Provides settings for building Dictionary-based data services.
    /// </summary>
    [ImplementationName(typeof(DataProviderSettings), "Dictionary")]
    public class DictionaryDataProviderSettings : DataProviderSettings
    {
        /// <summary>
        /// Initializes a new instance of the DictionaryDataProviderSettings class.
        /// </summary>
        public DictionaryDataProviderSettings()
            : base("Dictionary")
        {
            this.DictionaryDebugDataOracle = false;
        }

        /// <summary>
        /// Gets or sets the strategy to use for configuring the data service metadata provider's typing
        /// </summary>
        [InjectDependency(IsRequired = true)]
        [IgnoreDataMember]
        public MakeNamesUniqueFixup MakeNamesUniqueFixup { get; set; }

        /// <summary>
        /// Gets or sets the strategy to use for configuring the data service metadata provider's typing
        /// </summary>
        [InjectDependency(IsRequired = true)]
        [IgnoreDataMember]
        public IMetadataProviderTypingStrategy TypingStrategy { get; set; }

        /// <summary>
        /// Gets or sets the fixup to use for setting default collection types
        /// </summary>
        [InjectDependency(IsRequired = true)]
        [IgnoreDataMember]
        public SetDefaultCollectionTypesFixup CollectionTypesFixup { get; set; }

        /// <summary>
        /// Gets or sets the fixup to use for data type nullability
        /// </summary>
        [InjectDependency(IsRequired = true)]
        [IgnoreDataMember]
        public ClrNullableDataTypesFixup NullableDataTypesFixup { get; set; }

        /// <summary>
        /// Gets or sets the fixup to use for associations
        /// </summary>
        [InjectDependency(IsRequired = true)]
        [IgnoreDataMember]
        public ClrAssociationsFixup AssociationsFixup { get; set; }

        /// <summary>
        /// Gets or sets the service method resolver
        /// </summary>
        [InjectDependency]
        [IgnoreDataMember]
        public DictionaryProviderServiceMethodResolver ServiceMethodResolver { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to turn on debugging of the Dictionary Data Oracle or not
        /// </summary>
        [InjectTestParameter("DictionaryDebugDataOracle", DefaultValueDescription = "false")]
        public bool DictionaryDebugDataOracle { get; set; }

        /// <summary>
        /// Returns a fixup specific to the dictionary provider
        /// </summary>
        /// <returns>The fixup for the dictionary provider</returns>
        public override IEntityModelFixup GetProviderSpecificModelFixup()
        {
            ExceptionUtilities.CheckAllRequiredDependencies(this);
            var metadataFixup = this.TypingStrategy.GetModelFixup();
            return new CompositeEntityModelFixup(metadataFixup, this.CollectionTypesFixup, this.NullableDataTypesFixup, this.AssociationsFixup, this.MakeNamesUniqueFixup);
        }

        /// <summary>
        /// Returns the service method resolver specific to the dictionary provider
        /// </summary>
        /// <returns>the dictionary provider specific service method resolver</returns>
        public override IServiceMethodResolver GetProviderSpecificServiceModelResolver()
        {
            return this.ServiceMethodResolver;
        }
    }
}
