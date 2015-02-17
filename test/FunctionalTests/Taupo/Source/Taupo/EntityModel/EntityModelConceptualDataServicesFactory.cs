//---------------------------------------------------------------------
// <copyright file="EntityModelConceptualDataServicesFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Spatial;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.DataGeneration;

    /// <summary>
    /// Entity model structural data services factory.
    /// </summary>
    [ImplementationName(typeof(IEntityModelConceptualDataServicesFactory), "Default")]
    public class EntityModelConceptualDataServicesFactory : IEntityModelConceptualDataServicesFactory
    {
        private const int NumberOfRandomEnumValues = 3;
        private ConceptualDataServices structuralGenerators;

        /// <summary>
        /// Initializes a new instance of the EntityModelConceptualDataServicesFactory class.
        /// </summary>
        public EntityModelConceptualDataServicesFactory()
        {
            this.UniqueDataGeneratorResolver = new UniqueDataGeneratorResolver();
            this.RandomDataGeneratorResolver = new RandomDataGeneratorResolver();
        }

        /// <summary>
        /// Gets or sets random number generator which is used for random data generators.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IRandomNumberGenerator Random { get; set; }

        /// <summary>
        /// Gets or sets the unique data generator resolver.
        /// </summary>
        [InjectDependency]
        public IUniqueDataGeneratorResolver UniqueDataGeneratorResolver { get; set; }

        /// <summary>
        /// Gets or sets the random data generator resolver.
        /// </summary>
        [InjectDependency]
        public IRandomDataGeneratorResolver RandomDataGeneratorResolver { get; set; }

        /// <summary>
        /// Gets or sets the spatial data generator resolver.
        /// </summary>
        [InjectDependency]
        public ISpatialDataGeneratorResolver SpatialDataGeneratorResolver { get; set; }

        /// <summary>
        /// Creates structural data services for the entity model.
        /// </summary>
        /// <param name="modelSchema">Entity model schema.</param>
        /// <returns>
        /// An <see cref="IEntityModelConceptualDataServices"/>.
        /// </returns>
        public IEntityModelConceptualDataServices CreateConceptualDataServices(EntityModelSchema modelSchema)
        {
            ExceptionUtilities.CheckArgumentNotNull(modelSchema, "modelSchema");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            try
            {
                this.structuralGenerators = new ConceptualDataServices();

                foreach (ComplexType complexType in modelSchema.ComplexTypes)
                {
                    this.GetOrCreateAndRegisterStructuralDataGeneratorForComplexType(complexType);
                }

                foreach (EntityContainer entityContainer in modelSchema.EntityContainers)
                {
                    foreach (var entitySet in entityContainer.EntitySets)
                    {
                        this.CreateAndRegisterStructuralDataGeneratorsForEntitySet(entitySet);
                    }
                }

                return this.structuralGenerators;
            }
            finally
            {
                this.structuralGenerators = null;
            }
        }

        private INamedValuesGenerator GetOrCreateAndRegisterStructuralDataGeneratorForComplexType(ComplexType complexType)
        {
            INamedValuesGenerator dataGen;
            if (this.structuralGenerators.TryGetStructuralGenerator(complexType, out dataGen))
            {
                return dataGen;
            }

            MemberValueGenerators memberValuesGenerator = new MemberValueGenerators(this.Random);

            // to avoid infinite recursion, register the member-value generator before recursing into the properties
            this.structuralGenerators.RegisterStructuralGenerator(memberValuesGenerator, complexType);

            foreach (MemberProperty property in complexType.Properties)
            {
                memberValuesGenerator[property.Name] = this.GetPropertyDataGenerator(property, false);
            }

            return memberValuesGenerator;
        }

        private IDataGenerator GetOrCreateAndRegisterNonCollectionDataGeneratorForEnumType(EnumDataType enumDataType, bool isUnique, IList<DataGenerationHint> dataGenHints)
        {
            if (isUnique)
            {
                var clrType = enumDataType.GetUnderlyingTypeInEdm().GetFacetValue<PrimitiveClrTypeFacet, Type>(null);
                return this.UniqueDataGeneratorResolver.ResolveUniqueDataGenerator(clrType, this.Random, dataGenHints.ToArray());
            }

            IDataGenerator dataGen;
            if (this.structuralGenerators.TryGetEnumGenerator(enumDataType.Definition, enumDataType.IsNullable, out dataGen))
            {
                return dataGen;
            }

            List<object> interestingData = enumDataType.Definition.Members.Select(enumMember => enumMember.Name).Cast<object>().ToList();

            // For Enums with flags. Values can be combination of member values - 
            if (enumDataType.Definition.IsFlags == true)
            {
                var enumRange = (int)Math.Pow(2, enumDataType.Definition.Members.Count) - 1;
                for (int i = 0; i < NumberOfRandomEnumValues; i++)
                {
                    interestingData.Add(this.Random.NextFromRange(0, enumRange));
                }
            }

            if (!dataGenHints.Any(a => a == DataGenerationHints.EnumMembersOnly))
            {
                for (int i = 0; i < NumberOfRandomEnumValues; i++)
                {
                    // Use the maximum value of the smallest possible underlying type, this is a more realistic usage scenario than much larger random values.
                    var underLyingType = enumDataType.GetUnderlyingTypeInEdm().GetFacetValue<PrimitiveClrTypeFacet, Type>(typeof(int));
                    if (underLyingType == typeof(byte))
                    {
                        interestingData.Add(this.Random.NextFromRange(0, byte.MaxValue - 1));
                    }
                    else if (underLyingType == typeof(short))
                    {
                        interestingData.Add(this.Random.NextFromRange(0, short.MaxValue - 1));
                    }
                    else if (underLyingType == typeof(int) || underLyingType == typeof(long))
                    {
                        interestingData.Add(this.Random.NextFromRange(0, int.MaxValue - 1));
                    }
                    else
                    {
                        interestingData.Add(this.Random.NextFromRange(0, sbyte.MaxValue - 1));
                    }
                }
            }

            if (enumDataType.IsNullable)
            {
                interestingData.Add(null);
            }

            var dataGenerator = new FixedSetDataGenerator<object>(this.Random, interestingData.ToArray());
            this.structuralGenerators.RegisterEnumGenerator(dataGenerator, enumDataType.Definition, enumDataType.IsNullable);
            return dataGenerator;
        }

        private void CreateAndRegisterStructuralDataGeneratorsForEntitySet(EntitySet entitySet)
        {
            // Share the same data generators for common properties across all types in the entity set.
            Dictionary<MemberProperty, IDataGenerator> propertyDataGenerators = new Dictionary<MemberProperty, IDataGenerator>();

            var allTypes = entitySet.Container.Model.EntityTypes.Where(t => t.IsKindOf(entitySet.EntityType));

            // Property on a base type that is not part of any Edm Unique Constraint can be part of a Edm Unique Constraint on derived types =>
            // find all properties that are part of any Edm Unique Constraint upfront and 
            // create unique generators for them as they have to be unique across different types in the entity set.
            var uniqueProperties = allTypes.SelectMany(t => t.EdmUniqueConstraints.SelectMany(c => c.Properties)).Distinct();
            uniqueProperties = uniqueProperties.Concat(allTypes.SelectMany(t => t.Properties.Where(p => p.IsPrimaryKey))).Distinct();

            // TODO: Some Taupo framework pieces skip over StreamDataType properties
            foreach (var property in uniqueProperties.Where(p => !(p.PropertyType is StreamDataType)))
            {
                propertyDataGenerators[property] = this.GetPropertyDataGenerator(property, true);
            }

            foreach (EntityType entityType in allTypes.Where(t => !t.IsAbstract))
            {
                this.CreateAndRegisterStructuralDataGeneratorForEntityType(entityType, entitySet, propertyDataGenerators);
            }
        }

        private void CreateAndRegisterStructuralDataGeneratorForEntityType(EntityType entityType, EntitySet entitySet, Dictionary<MemberProperty, IDataGenerator> propertyDataGenerators)
        {
            MemberValueGenerators generator = new MemberValueGenerators(this.Random);

            // Filter out all foreign key properties, except if it's a primary key
            // TODO: Some Taupo framework pieces skip over StreamDataType properties
            foreach (var property in entityType.AllProperties.Where(p => (p.IsPrimaryKey || !p.IsForeignKey(entitySet)) && !(p.PropertyType is StreamDataType)))
            {
                IDataGenerator propertyGenerator;
                if (!propertyDataGenerators.TryGetValue(property, out propertyGenerator))
                {
                    propertyGenerator = this.GetPropertyDataGenerator(property, false);
                    propertyDataGenerators[property] = propertyGenerator;
                }

                generator[property.Name] = propertyGenerator;
            }

            string entitySetName = entitySet.ContainerQualifiedName;
            this.structuralGenerators.RegisterStructuralGenerator(generator, entityType, entitySetName);
        }

        private IDataGenerator GetPropertyDataGenerator(MemberProperty property, bool isUnique)
        {
            if (property.PropertyType == null)
            {
                throw new TaupoInvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "Cannot create data generator for the property '{0}' because it's type is null.", property.Name));
            }

            // First check if custom data generator has been specified for this property and use it.
            var customDataGen = property.Annotations.OfType<DataGeneratorAnnotation>().SingleOrDefault();
            if (customDataGen != null)
            {
                return customDataGen.DataGenerator;
            }

            // If there is no custom data generator create data generator based on data generation hints.
            List<DataGenerationHint> dataGenHints = property.Annotations.OfType<DataGenerationHintsAnnotation>().SelectMany(a => a.Hints).ToList();

            CollectionDataType collectionDataType = property.PropertyType as CollectionDataType;

            if (collectionDataType != null)
            {
                return this.ResolveCollectionDataGenerator(collectionDataType, dataGenHints);
            }
            else
            {
                return this.ResolveNonCollectionDataGenerator(property.PropertyType, property.IsPrimaryKey || isUnique, dataGenHints);
            }
        }

        private IDataGenerator ResolveNonCollectionDataGenerator(DataType dataType, bool isUnique, IList<DataGenerationHint> dataGenHints)
        {
            ComplexDataType complexDataType = dataType as ComplexDataType;
            if (complexDataType != null)
            {
                var complexGenerator = this.GetOrCreateAndRegisterStructuralDataGeneratorForComplexType(complexDataType.Definition);
                if (dataType.IsNullable)
                {
                    return new NullableNamedValuesGeneratorProxy(complexGenerator, this.Random, dataGenHints);
                }

                return complexGenerator;
            }

            EnumDataType enumDataType = dataType as EnumDataType;
            if (enumDataType != null)
            {
                return this.GetOrCreateAndRegisterNonCollectionDataGeneratorForEnumType(enumDataType, isUnique, dataGenHints);
            }

            PrimitiveDataType primitiveDataType = dataType as PrimitiveDataType;
            SpatialDataType spatialType = dataType as SpatialDataType;

            if (primitiveDataType == null)
            {
                throw new TaupoNotSupportedException(
                    string.Format(CultureInfo.InvariantCulture, "Data generator creation is not supported for this data type: '{0}'.", dataType.ToString()));
            }
            else if (spatialType != null)
            {
                ExceptionUtilities.CheckObjectNotNull(
                    this.SpatialDataGeneratorResolver,
                    "Cannot generate value for spatial data type '{0}' without SpatialDataGeneratorResolver being set",
                    dataType);

                var isUniqueHint = dataGenHints.OfType<AllUniqueHint>().SingleOrDefault();

                if (isUniqueHint != null)
                {
                    isUnique = true;
                }

                return this.SpatialDataGeneratorResolver.GetDataGenerator(spatialType, isUnique, this.Random, dataGenHints.ToArray());
            }
            else
            {
                Type clrType = null;
                bool isNullable = true;

                clrType = primitiveDataType.GetFacetValue<PrimitiveClrTypeFacet, Type>(null);
                ExceptionUtilities.CheckObjectNotNull(clrType, "Facet of type '{0}' not defined on a property type '{1}'.", typeof(PrimitiveClrTypeFacet).Name, dataType);
                isNullable = primitiveDataType.IsNullable;

                return this.ResolvePrimitiveDataGeneratorBasedOnClrType(clrType, isUnique, isNullable, dataGenHints);
            }
        }

        private IDataGenerator ResolvePrimitiveDataGeneratorBasedOnClrType(Type clrType, bool isUnique, bool isNullable, IList<DataGenerationHint> dataGenHints)
        {
            IDataGenerator dataGenerator;

            if (isUnique)
            {
                dataGenerator = this.UniqueDataGeneratorResolver.ResolveUniqueDataGenerator(clrType, this.Random, dataGenHints.ToArray());
            }
            else
            {
                if (isNullable && clrType.IsValueType() && !(clrType.IsGenericType() && clrType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    clrType = typeof(Nullable<>).MakeGenericType(clrType);
                }
                else if (!isNullable)
                {
                    dataGenHints.Add(DataGenerationHints.NoNulls);
                }

                dataGenerator = this.RandomDataGeneratorResolver.ResolveRandomDataGenerator(clrType, this.Random, dataGenHints.ToArray());
            }

            return dataGenerator;
        }

        private IDataGenerator ResolveCollectionDataGenerator(CollectionDataType collectionType, IList<DataGenerationHint> dataGenHints)
        {
            IDataGenerator elementDataGenerator = this.ResolveNonCollectionDataGenerator(collectionType.ElementDataType, false, dataGenHints);
            int minCount = dataGenHints.Max<CollectionMinCountHint, int>(0);
            int maxCount = dataGenHints.Min<CollectionMaxCountHint, int>(Math.Max(minCount, 10));

            return new CollectionStructuralDataGenerator(elementDataGenerator, this.Random, minCount, maxCount);
        }

        /// <summary>
        /// Helper class used to stand-in for an INamedValuesGenerator when the complex type can be null
        /// </summary>
        internal class NullableNamedValuesGeneratorProxy : DataGenerator<IList<NamedValue>>, INamedValuesGenerator
        {
            // null's will be generated 1/n times
            private const int NullFrequency = 10;

            private readonly INamedValuesGenerator underlying;
            private readonly IRandomNumberGenerator random;
            private readonly ReadOnlyCollection<DataGenerationHint> hints;
            private readonly bool noNullsHintPresent;
            private readonly bool allNullsHintPresent;

            /// <summary>
            /// Initializes a new instance of the NullableNamedValuesGeneratorProxy class
            /// </summary>
            /// <param name="underlying">The underlying named-values generator</param>
            /// <param name="random">The random number generator to use when deciding whether to generate null</param>
            /// <param name="hints">The data generation hints</param>
            internal NullableNamedValuesGeneratorProxy(INamedValuesGenerator underlying, IRandomNumberGenerator random, IEnumerable<DataGenerationHint> hints)
            {
                ExceptionUtilities.CheckArgumentNotNull(underlying, "underlying");
                ExceptionUtilities.CheckArgumentNotNull(random, "random");
                ExceptionUtilities.CheckArgumentNotNull(hints, "hints");
                this.underlying = underlying;
                this.random = random;
                this.hints = hints.ToList().AsReadOnly();
                this.noNullsHintPresent = this.hints.OfType<NoNullsHint>().Any();
                this.allNullsHintPresent = this.hints.OfType<AllNullsHint>().Any();
            }

            /// <summary>
            /// Generates data as a sequence of named-value pairs or sometimes null.
            /// </summary>
            /// <returns>Named-value pairs or null.</returns>
            public override IList<NamedValue> GenerateData()
            {
                bool generateNull = false;
                if (!this.noNullsHintPresent)
                {
                    if (this.allNullsHintPresent)
                    {
                        generateNull = true;
                    }
                    else
                    {
                        generateNull = this.random.Next(NullFrequency) == 0;
                    }
                }

                if (generateNull)
                {
                    return null;
                }

                return this.underlying.GenerateData();
            }

            /// <summary>
            /// Gets a data generator for the specified member.
            /// </summary>
            /// <param name="memberName">The member name.</param>
            /// <returns>A data generator for the specified member.</returns>
            public IDataGenerator GetMemberDataGenerator(string memberName)
            {
                return this.underlying.GetMemberDataGenerator(memberName);
            }
        }

        /// <summary>
        /// Data manager for the entity model that manipulates structural data in form structural data, i.e. name - value pairs.
        /// </summary>
        private class ConceptualDataServices : EntityModelDataServicesBase, IEntityModelConceptualDataServices
        {
            private Dictionary<string, INamedValuesGenerator> complexTypeMemberValuesGenerators = new Dictionary<string, INamedValuesGenerator>();
            private Dictionary<string, INamedValuesGenerator> entitySetMemberValuesGenerators = new Dictionary<string, INamedValuesGenerator>();
            private Dictionary<string, IDataGenerator> enumTypeMemberGenerators = new Dictionary<string, IDataGenerator>();
            private Dictionary<string, IDataGenerator> enumTypeNullableMemberGenerators = new Dictionary<string, IDataGenerator>();

            /// <summary>
            /// Gets structural data generator for the specified entity set and type.
            /// </summary>
            /// <param name="entityTypeFullName">An entity type full name.</param>
            /// <param name="entitySetName">A name of the entity set for which data needs to be generated.</param>
            /// <returns>
            /// An <see cref="IDataGenerator"/> that generates data for entity type in the form of collection of named values.
            /// </returns>
            public INamedValuesGenerator GetStructuralGenerator(string entityTypeFullName, string entitySetName)
            {
                string key = this.DetermineKey(entityTypeFullName, entitySetName);

                return this.GetDataService<INamedValuesGenerator>(key, this.entitySetMemberValuesGenerators);
            }

            /// <summary>
            /// Gets structural data generator for the specified complex type.
            /// </summary>
            /// <param name="complexTypeFullName">A complex type full name.</param>
            /// <returns>
            /// An <see cref="IDataGenerator"/> that generates data for complex type in the form of collection of named values.
            /// </returns>
            public INamedValuesGenerator GetStructuralGenerator(string complexTypeFullName)
            {
                ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(complexTypeFullName, "complexTypeFullName");

                return this.GetDataService<INamedValuesGenerator>(complexTypeFullName, this.complexTypeMemberValuesGenerators);
            }

            /// <summary>
            /// Tries to get members' values generator.
            /// </summary>
            /// <param name="complexType">Complex type.</param>
            /// <param name="dataGenerator">The data generator.</param>
            /// <returns>True if values generator is found, false otherwise.</returns>
            public bool TryGetStructuralGenerator(ComplexType complexType, out INamedValuesGenerator dataGenerator)
            {
                return this.complexTypeMemberValuesGenerators.TryGetValue(complexType.FullName, out dataGenerator);
            }

            /// <summary>
            /// Tries to get enum data generator.
            /// </summary>
            /// <param name="enumType">Enum type.</param>
            /// <param name="isNullable">The value indicating if data generator which generates null values should be returned.</param>
            /// <param name="dataGenerator">The data generator.</param>
            /// <returns>True if enum generator is found, false otherwise.</returns>
            public bool TryGetEnumGenerator(EnumType enumType, bool isNullable, out IDataGenerator dataGenerator)
            {
                if (isNullable)
                {
                    return this.enumTypeNullableMemberGenerators.TryGetValue(enumType.FullName, out dataGenerator);
                }
                else
                {
                    return this.enumTypeMemberGenerators.TryGetValue(enumType.FullName, out dataGenerator);
                }
            }

            /// <summary>
            /// Registers the member values generator.
            /// </summary>
            /// <param name="dataGenerator">The data generator.</param>
            /// <param name="entityType">Type of the entity.</param>
            /// <param name="entitySetName">Name of the entity set.</param>
            public void RegisterStructuralGenerator(INamedValuesGenerator dataGenerator, EntityType entityType, string entitySetName)
            {
                this.Register<INamedValuesGenerator>(entityType, entitySetName, dataGenerator, this.entitySetMemberValuesGenerators);
            }

            /// <summary>
            /// Registers the member values generator.
            /// </summary>
            /// <param name="dataGenerator">The data generator.</param>
            /// <param name="complexType">The complex type.</param>
            public void RegisterStructuralGenerator(INamedValuesGenerator dataGenerator, ComplexType complexType)
            {
                this.Register<INamedValuesGenerator>(complexType, dataGenerator, this.complexTypeMemberValuesGenerators);
            }

            /// <summary>
            /// Registers the enum member generator.
            /// </summary>
            /// <param name="dataGenerator">The data generator.</param>
            /// <param name="enumType">The enum type.</param>
            /// <param name="isNullable">The value indicating if the data generator generates null values.</param>
            public void RegisterEnumGenerator(IDataGenerator dataGenerator, EnumType enumType, bool isNullable)
            {
                this.Register<IDataGenerator>(enumType, dataGenerator, isNullable ? this.enumTypeNullableMemberGenerators : this.enumTypeMemberGenerators);
            }
        }
    }
}
