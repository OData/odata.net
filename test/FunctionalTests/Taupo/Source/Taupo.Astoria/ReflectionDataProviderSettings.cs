//---------------------------------------------------------------------
// <copyright file="ReflectionDataProviderSettings.cs" company="Microsoft">
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
    /// Provides settings for building reflection-based data services.
    /// </summary>
    [ImplementationName(typeof(DataProviderSettings), "Reflection")]
    public class ReflectionDataProviderSettings : DataProviderSettings
    {
        /// <summary>
        /// Initializes a new instance of the ReflectionDataProviderSettings class.
        /// </summary>
        public ReflectionDataProviderSettings()
            : base("Reflection")
        {
            this.SupportsMest = false;
        }

        /// <summary>
        /// Gets or sets the fixup to use for setting default collection types
        /// </summary>
        [InjectDependency(IsRequired = true)]
        [IgnoreDataMemberAttribute]
        public SetDefaultCollectionTypesFixup CollectionTypesFixup { get; set; }

        /// <summary>
        /// Gets or sets the fixup to use for data type nullability
        /// </summary>
        [InjectDependency(IsRequired = true)]
        [IgnoreDataMemberAttribute]
        public ClrNullableDataTypesFixup NullableDataTypesFixup { get; set; }

        /// <summary>
        /// Gets or sets the fixup to use for associations
        /// </summary>
        [InjectDependency(IsRequired = true)]
        [IgnoreDataMemberAttribute]
        public ClrAssociationsFixup AssociationsFixup { get; set; }

        /// <summary>
        /// Gets or sets the service method resolver
        /// </summary>
        [InjectDependency]
        [IgnoreDataMember]
        public ServiceMethodResolver ServiceMethodResolver { get; set; }

        /// <summary>
        /// Returns a fixup specific to the reflection provider
        /// </summary>
        /// <returns>An entity model fixup</returns>
        public override IEntityModelFixup GetProviderSpecificModelFixup()
        {
            ExceptionUtilities.CheckAllRequiredDependencies(this);
            return new CompositeEntityModelFixup(this.CollectionTypesFixup, this.NullableDataTypesFixup, this.AssociationsFixup);
        }

        /// <summary>
        /// Returns the service method resolver specific to the reflection provider
        /// </summary>
        /// <returns>the reflection provider specific service method resolver</returns>
        public override IServiceMethodResolver GetProviderSpecificServiceModelResolver()
        {
            return this.ServiceMethodResolver;
        }
    }
}