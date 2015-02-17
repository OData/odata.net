//---------------------------------------------------------------------
// <copyright file="PrimitiveDataTypeRepositoryBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Types
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.DataGeneration;

    /// <summary>
    /// Base class for a repository of primitive data types.
    /// </summary>
    public abstract class PrimitiveDataTypeRepositoryBase : IPrimitiveDataTypeRepository
    {
        private List<PrimitiveDataType> primitiveDataTypes;
        
        /// <summary>
        /// Initializes a new instance of the PrimitiveDataTypeRepositoryBase class.
        /// </summary>
        /// <param name="dataGenerationHintsResolver">Data generation hints resolver.</param>
        protected PrimitiveDataTypeRepositoryBase(IPrimitiveDataTypeToDataGenerationHintsResolver dataGenerationHintsResolver)
        {
            ExceptionUtilities.CheckArgumentNotNull(dataGenerationHintsResolver, "dataGenerationHintsResolver");

            this.RandomDataGeneratorResolver = new RandomDataGeneratorResolver();
            this.UniqueDataGeneratorResolver = new UniqueDataGeneratorResolver();
            this.DataGenerationHintsResolver = dataGenerationHintsResolver;
        }

        /// <summary>
        /// Gets or sets the random data generator to use.
        /// </summary>
        [InjectDependency]
        public IRandomDataGeneratorResolver RandomDataGeneratorResolver { get; set; }

        /// <summary>
        /// Gets or sets the unique data generator to use.
        /// </summary>
        [InjectDependency]
        public IUniqueDataGeneratorResolver UniqueDataGeneratorResolver { get; set; }

        /// <summary>
        /// Gets or sets the string containing semicolon separted list of database type names to include in to the repository.
        /// By default all database types are included based on the database version.
        /// </summary>
        [InjectTestParameter("IncludeDatabaseTypes", DefaultValueDescription = "null, which means all types will be included", HelpText = "Semicolon separted list of database type names to include into the data type repository.")]
        public string IncludeDatabaseTypes { get; set; }

        /// <summary>
        /// Gets or sets the string containing semicolon separted list of database type names to exclude from the repository.
        /// By default all database types are included based on the database version.
        /// </summary>
        [InjectTestParameter("ExcludeDatabaseTypes", DefaultValueDescription = "null, which means no types will be excluded", HelpText = "Semicolon separted list of database type names to exclude from the data type repository.")]
        public string ExcludeDatabaseTypes { get; set; }

        /// <summary>
        /// Gets the DataGenerationHintsResolver to reoslve hints for data types
        /// </summary>
        protected IPrimitiveDataTypeToDataGenerationHintsResolver DataGenerationHintsResolver { get; private set; }

        /// <summary>
        /// Gets primitive data types available in the repository.
        /// </summary>
        /// <returns>Primitive data types available in the repository.</returns>
        public virtual IEnumerable<PrimitiveDataType> GetPrimitiveDataTypes()
        {
            if (this.primitiveDataTypes == null)
            {
                this.primitiveDataTypes = new List<PrimitiveDataType>();
                this.primitiveDataTypes.AddRange(this.GetPrimitiveDataTypesOverride());
            }

            if (string.IsNullOrEmpty(this.IncludeDatabaseTypes) && string.IsNullOrEmpty(this.ExcludeDatabaseTypes))
            {
                return this.primitiveDataTypes;
            }

            List<string> include = !string.IsNullOrEmpty(this.IncludeDatabaseTypes) ? this.IncludeDatabaseTypes.Split(';').Select(n => n.Trim()).ToList() : new List<string>();
            List<string> exclude = !string.IsNullOrEmpty(this.ExcludeDatabaseTypes) ? this.ExcludeDatabaseTypes.Split(';').Select(n => n.Trim()).ToList() : new List<string>();

            return this.primitiveDataTypes.Where(dt =>
                {
                    string name = dt.GetFacetValue<UnqualifiedDatabaseTypeNameFacet, string>(string.Empty);
                    bool shouldInclude = include.Count == 0 || include.Contains(name);
                    bool shouldExclude = exclude.Count > 0 && exclude.Contains(name);
                    return shouldInclude && !shouldExclude;
                });
        }

        /// <summary>
        /// Creates a data generator for the given data type from the repository.
        /// </summary>
        /// <param name="dataType">The data type from the repository to get data generator for.</param>
        /// <param name="random">The random number generator.</param>
        /// <param name="isUnique">The value indicating whether the generator should produce unique data.</param>
        /// <returns>The data generator for the given data type.</returns>
        public virtual IDataGenerator CreateDataGenerator(PrimitiveDataType dataType, IRandomNumberGenerator random, bool isUnique)
        {
            ExceptionUtilities.CheckArgumentNotNull(dataType, "dataType");
            ExceptionUtilities.CheckArgumentNotNull(random, "random");

            if (this.primitiveDataTypes == null)
            {
                this.primitiveDataTypes = new List<PrimitiveDataType>();
                this.primitiveDataTypes.AddRange(this.GetPrimitiveDataTypesOverride());
            }

            if (!this.primitiveDataTypes.Contains(dataType))
            {
                throw new TaupoInvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "Given type does not belong to this repository. Type: '{0}'.", dataType.ToString()));
            }

            var enumDataType = dataType as EnumDataType;
            var clrType = enumDataType != null ? enumDataType.Definition.UnderlyingType : dataType.GetFacetValue<PrimitiveClrTypeFacet, Type>(null);
            ExceptionUtilities.CheckObjectNotNull(clrType, "{0} has to be defined on the type: '{1}'.", enumDataType != null ? "UnderlyingType" : "PrimitiveClrTypeFacet", dataType);

            if (dataType.IsNullable && clrType.IsValueType() && !(clrType.IsGenericType() && clrType.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                clrType = typeof(Nullable<>).MakeGenericType(clrType);
            }

            var hints = this.DataGenerationHintsResolver.ResolveDataGenerationHints(dataType);

            return this.ResolveDataGenerator(clrType, random, isUnique, hints.ToArray());
        }

        /// <summary>
        /// Gets primitive data types. This method should be overriden in the derived class.
        /// </summary>
        /// <returns>Primitive data types.</returns>
        protected abstract IEnumerable<PrimitiveDataType> GetPrimitiveDataTypesOverride();

        private IDataGenerator ResolveDataGenerator(Type clrType, IRandomNumberGenerator random, bool isUnique, params DataGenerationHint[] hints)
        {
            IDataGenerator dataGenerator;
            if (isUnique)
            {
                ExceptionUtilities.CheckObjectNotNull(this.UniqueDataGeneratorResolver, "Cannot create data generator as unique data generator resolver is not setup.");
                dataGenerator = this.UniqueDataGeneratorResolver.ResolveUniqueDataGenerator(clrType, random, hints);
            }
            else
            {
                ExceptionUtilities.CheckObjectNotNull(this.RandomDataGeneratorResolver, "Cannot create data generator as random data generator resolver is not setup.");
                dataGenerator = this.RandomDataGeneratorResolver.ResolveRandomDataGenerator(clrType, random, hints);
            }

            return dataGenerator;
        }
    }
}
