//---------------------------------------------------------------------
// <copyright file="EntityModelObjectServicesFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.DataGeneration;

    /// <summary>
    /// Entity model object services factory.
    /// </summary>
    [ImplementationName(typeof(IEntityModelObjectServicesFactory), "Default")] 
    public class EntityModelObjectServicesFactory : IEntityModelObjectServicesFactory
    {
        /// <summary>
        /// Gets or sets the random number generator.
        /// </summary>
        [InjectDependency]
        public IRandomNumberGenerator Random { get; set; }

        /// <summary>
        /// Creates the object services.
        /// </summary>
        /// <param name="modelSchema">The entity model schema.</param>
        /// <param name="conceptualDataServices">The structural generators for the specified entity model schema</param>
        /// <param name="assemblies">The assemblies that contain data classes for the specified entity model schema.</param>
        /// <returns>Entity model object services.</returns>
        public IEntityModelObjectServices CreateObjectServices(EntityModelSchema modelSchema, IEntityModelConceptualDataServices conceptualDataServices, IEnumerable<Assembly> assemblies)
        {
            ExceptionUtilities.CheckArgumentNotNull(modelSchema, "modelSchema");
            ExceptionUtilities.CheckArgumentNotNull(conceptualDataServices, "conceptualDataServices");
            ExceptionUtilities.CheckArgumentNotNull(assemblies, "assemblies");

            EntityModelObjectServices services = new EntityModelObjectServices(conceptualDataServices);

            this.RegisterDataGeneratorAndAdaptersForComplexTypes(modelSchema, conceptualDataServices, services, assemblies);

            List<EntityType> processedEntityTypes = new List<EntityType>();
            foreach (EntityContainer entityContainer in modelSchema.EntityContainers)
            {
                var graphCreator = new EntityGraphCreator(entityContainer)
                {
                    ConceptualDataServices = conceptualDataServices,
                    ObjectServices = services,
                    Random = this.Random
                };

                services.RegisterGraphCreator(graphCreator, entityContainer);

                foreach (EntitySet entitySet in entityContainer.EntitySets)
                {
                    string entitySetName = entityContainer.Name + "." + entitySet.Name;
                    foreach (EntityType entityType in modelSchema.EntityTypes.Where(t => t.IsKindOf(entitySet.EntityType) && !t.IsAbstract))
                    {
                        INamedValuesGenerator structuralGenerator = conceptualDataServices.GetStructuralGenerator(entityType.FullName, entitySetName);
                        Type clrType = this.GetClrType(entityType, assemblies);

                        IStructuralDataAdapter adapter = new ObjectStructuralDataAdapter(clrType);

                        IDataGenerator objectDataGen = this.CreateObjectDataGenerator(clrType, adapter, structuralGenerator);

                        EntitySetObjectDataGenerator entitySetObjectDataGen = new EntitySetObjectDataGenerator(entitySetName, objectDataGen);

                        services.RegisterEntitySetDataGenerator(entitySetObjectDataGen, entityType, entitySetName); 
                        services.RegisterDataGenerator(objectDataGen, entityType, entitySetName);
                        
                        if (!processedEntityTypes.Contains(entityType))
                        {
                            services.RegisterDataAdapter(adapter, entityType);
                            this.RegisterEnumTypes(entityType, services, assemblies);
                        }

                        processedEntityTypes.Add(entityType);
                    }
                }
            }

            return services;
        }

        private void RegisterDataGeneratorAndAdaptersForComplexTypes(EntityModelSchema model, IEntityModelConceptualDataServices structuralDataServices, EntityModelObjectServices services, IEnumerable<Assembly> assemblies)
        {
            foreach (ComplexType complexType in model.ComplexTypes)
            {
                INamedValuesGenerator structuralGenerator = structuralDataServices.GetStructuralGenerator(complexType.FullName);
                Type clrType = this.GetClrType(complexType, assemblies);
                IStructuralDataAdapter adapter = new ObjectStructuralDataAdapter(clrType);
                IDataGenerator objectDataGen = this.CreateObjectDataGenerator(clrType, adapter, structuralGenerator);

                services.RegisterDataGenerator(objectDataGen, complexType);
                services.RegisterDataAdapter(adapter, complexType);
            }
        }

        private IDataGenerator CreateObjectDataGenerator(Type clrType, IStructuralDataAdapter dataManager, INamedValuesGenerator memberValuesGenerator)
        {
            ConstructorInfo ci = typeof(StructuralDataGenerator<>).MakeGenericType(clrType).GetInstanceConstructor(true, new Type[] { typeof(IStructuralDataAdapter), typeof(INamedValuesGenerator) });

            IDataGenerator objectDataGen = (IDataGenerator)ci.Invoke(new object[] { dataManager, memberValuesGenerator });

            return objectDataGen;
        }

        private void RegisterEnumTypes(EntityType entityType, EntityModelObjectServices services, IEnumerable<Assembly> assemblies)
        {
            entityType.GetPropertyPaths((path, property) => { this.RegisterEnumType(entityType, path, property.PropertyType, services, assemblies); return true; });
        }

        private void RegisterEnumType(EntityType entityType, string propertyPath, DataType propertyType, EntityModelObjectServices services, IEnumerable<Assembly> assemblies)
        {
            var enumDataType = propertyType as EnumDataType;
            if (enumDataType != null)
            {
                Type clrEnumType = this.GetClrType(enumDataType.Definition, assemblies);
                services.RegisterEnumType(clrEnumType, entityType, propertyPath);
            }
        }

        private Type GetClrType(INamedItem nominalType, IEnumerable<Assembly> assemblies)
        {
            Type clrType = assemblies
                .Where(a => a != null).Select(a => a.GetTypes()
                    .Where(t => t.Name == nominalType.Name && t.Namespace == nominalType.NamespaceName).SingleOrDefault())
                .Where(t => t != null).SingleOrDefault();

            if (clrType == null)
            {
                throw new TaupoInvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "Failed to create object data generator for '{0}.{1}' type because it cannot be found in the supplied assemblies.", nominalType.NamespaceName, nominalType.Name));
            }

            return clrType;
        }

        /// <summary>
        /// Entity model object services.
        /// </summary>
        private class EntityModelObjectServices : EntityModelDataServicesBase, IEntityModelObjectServices
        {
            private Dictionary<string, IDataGenerator<IEntitySetData>> entitySetDataGenerators = new Dictionary<string, IDataGenerator<IEntitySetData>>();
            private Dictionary<string, IDataGenerator> complexTypeDataGenerators = new Dictionary<string, IDataGenerator>(); 
            private Dictionary<string, IDataGenerator> entityTypeDataGenerators = new Dictionary<string, IDataGenerator>();
            private Dictionary<string, IStructuralDataAdapter> dataAdapters = new Dictionary<string, IStructuralDataAdapter>();
            private Dictionary<string, IEntityGraphCreator> graphCreators = new Dictionary<string, IEntityGraphCreator>();
            private Dictionary<string, Type> propertyPathToEnumTypeMap = new Dictionary<string, Type>();

            private IEntityModelConceptualDataServices conceptualDataServices;

            public EntityModelObjectServices(IEntityModelConceptualDataServices conceptualDataServices)
            {
                this.conceptualDataServices = conceptualDataServices;
            }

            /// <summary>
            /// Gets an <see cref="IEntityGraphCreator"/> that can create graphs of entities based on current metadata.
            /// </summary>
            /// <param name="containerName">The name of the entity container for which to retrieve an <see cref="IEntityGraphCreator"/>.</param>
            /// <returns>An <see cref="IEntityGraphCreator"/> based on current metadata.</returns>
            public IEntityGraphCreator GetEntityGraphCreator(string containerName)
            {
                ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(containerName, "containerName");

                return this.GetDataService<IEntityGraphCreator>(containerName, this.graphCreators);
            }

            /// <summary>
            /// Gets an entity set data generator for the specified <see cref="EntityType"/> and entity set name.
            /// </summary>
            /// <param name="entityTypeFullName">An entity type full name.</param>
            /// <param name="entitySetName">A name of the entity set for which data needs to be generated.</param>
            /// <returns>
            /// A generator that generates entity set data where data stored in the form defined by the corresponding <see cref="IStructuralDataAdapter"/>.<seealso cref="GetObjectAdapter"/>
            /// </returns>
            public IDataGenerator<IEntitySetData> GetEntitySetObjectGenerator(string entityTypeFullName, string entitySetName)
            {
                string key = this.DetermineKey(entityTypeFullName, entitySetName);

                return this.GetDataService<IDataGenerator<IEntitySetData>>(key, this.entitySetDataGenerators);
            }

            /// <summary>
            /// Gets data generator for the specified <see cref="ComplexType"/>.
            /// </summary>
            /// <param name="complexTypeFullName">A complex type full name.</param>
            /// <returns>
            /// An <see cref="IDataGenerator"/> that generates data for complex type in the form defined by the corresponding <see cref="IStructuralDataAdapter"/>. <seealso cref="GetObjectAdapter"/>
            /// </returns>
            public IDataGenerator GetObjectGenerator(string complexTypeFullName)
            {
                ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(complexTypeFullName, "complexTypeFullName");

                return this.GetDataService<IDataGenerator>(complexTypeFullName, this.complexTypeDataGenerators);
            }

            /// <summary>
            /// Gets data adapter to store/modify/look-up structural data for the specified <see cref="StructuralType"/>  in forms of object.
            /// </summary>
            /// <param name="structuralTypeFullName">A structural type.</param>
            /// <returns>Structural data adapter.</returns>
            public IStructuralDataAdapter GetObjectAdapter(string structuralTypeFullName)
            {
                ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(structuralTypeFullName, "structuralTypeFullName");

                return this.GetDataService<IStructuralDataAdapter>(structuralTypeFullName, this.dataAdapters);
            }

            /// <summary>
            /// Generates values for the properties with the specified paths.
            /// </summary>
            /// <param name="entityTypeFullName">An entity type full name.</param>
            /// <param name="entitySetName">A name of the entity set for which data needs to be generated.</param>
            /// <param name="propertyPaths">The property paths for which to generate values.</param>
            /// <param name="valuesToAvoid">The collection of named values where name is a property path and value is a property value which should be avoided when generating value for this property.</param>
            /// <returns>Generated properties' values.</returns>
            public IEnumerable<NamedValue> GeneratePropertyValues(string entityTypeFullName, string entitySetName, IEnumerable<string> propertyPaths, IEnumerable<NamedValue> valuesToAvoid)
            {
                var conceptualValuesToAvoid = this.ConvertClrValuesToAvoidToConceptualValues(valuesToAvoid);

                var conceptualValues = this.conceptualDataServices.GeneratePropertyValues(entitySetName, entityTypeFullName, propertyPaths, conceptualValuesToAvoid.ToArray());

                var result = this.ConvertConceptualToClrValues(entityTypeFullName, conceptualValues);

                return result;
            }

            /// <summary>
            /// Registers the entity set data generator for the specified entity type and entity set.
            /// </summary>
            /// <param name="dataGenerator">The entity set data generator.</param>
            /// <param name="entityType">Type of the entity.</param>
            /// <param name="entitySetName">Name of the entity set.</param>
            public void RegisterEntitySetDataGenerator(IDataGenerator<IEntitySetData> dataGenerator, EntityType entityType, string entitySetName)
            {
                this.Register<IDataGenerator<IEntitySetData>>(entityType, entitySetName, dataGenerator, this.entitySetDataGenerators);
            }

            /// <summary>
            /// Registers the data generator for the specified entity type and entity set.
            /// </summary>
            /// <param name="dataGenerator">The object data generator.</param>
            /// <param name="entityType">Type of the entity.</param>
            /// <param name="entitySetName">Name of the entity set.</param>
            public void RegisterDataGenerator(IDataGenerator dataGenerator, EntityType entityType, string entitySetName)
            {
                this.Register<IDataGenerator>(entityType, entitySetName, dataGenerator, this.entityTypeDataGenerators);
            }

            /// <summary>
            /// Registers the data generator for the specified complex type.
            /// </summary>
            /// <param name="dataGenerator">The data generator.</param>
            /// <param name="complexType">Type of the complex.</param>
            public void RegisterDataGenerator(IDataGenerator dataGenerator, ComplexType complexType)
            {
                this.Register<IDataGenerator>(complexType, dataGenerator, this.complexTypeDataGenerators);
            }

            /// <summary>
            /// Registers the structural data manager.
            /// </summary>
            /// <param name="adapter">The data manager.</param>
            /// <param name="structuralType">Type of the structural.</param>
            public void RegisterDataAdapter(IStructuralDataAdapter adapter, NamedStructuralType structuralType)
            {
                this.Register<IStructuralDataAdapter>(structuralType, adapter, this.dataAdapters);
            }

            /// <summary>
            /// Registers a new graph creator.
            /// </summary>
            /// <param name="graphCreator">The <see cref="IEntityGraphCreator"/> to register.</param>
            /// <param name="entityContainer">The <see cref="EntityContainer"/> around which this graph creator is based.</param>
            public void RegisterGraphCreator(IEntityGraphCreator graphCreator, EntityContainer entityContainer)
            {
                this.graphCreators.Add(entityContainer.Name, graphCreator);
            }

            /// <summary>
            /// Registers an enumeration type for the specified property.
            /// </summary>
            /// <param name="enumType">The enumeration type.</param>
            /// <param name="entityType">The entityType.</param>
            /// <param name="propertyPath">The property path.</param>
            public void RegisterEnumType(Type enumType, EntityType entityType, string propertyPath)
            {
                this.propertyPathToEnumTypeMap.Add(entityType.FullName + ":" + propertyPath, enumType);
            }

            private IEnumerable<NamedValue> ConvertConceptualToClrValues(string entityTypeFullName, IEnumerable<NamedValue> conceptulValues)
            {
                var result = new List<NamedValue>();
                foreach (var value in conceptulValues)
                {
                    string key = entityTypeFullName + ":" + value.Name;
                    Type enumType;
                    var valueToAdd = value;
                    if (this.propertyPathToEnumTypeMap.TryGetValue(key, out enumType))
                    {
                        valueToAdd = new NamedValue(value.Name, DataUtilities.ConvertToEnum(enumType, value.Value));
                    }

                    result.Add(valueToAdd);
                }

                return result;
            }

            private IEnumerable<NamedValue> ConvertClrValuesToAvoidToConceptualValues(IEnumerable<NamedValue> clrValues)
            {
                var result = new List<NamedValue>();
                foreach (var value in clrValues)
                {
                    Type valueType = value.Value != null ? value.Value.GetType() : null;
                    if (valueType != null && valueType.IsEnum())
                    {
                        object underlyingValue = DataUtilities.ConvertFromEnum(value.Value);
                        
                        // add as an underlying value
                        result.Add(new NamedValue(value.Name, underlyingValue));

                        string enumMemberName = Enum.GetName(valueType, underlyingValue);
                        if (enumMemberName != null)
                        {
                            // add as a string value
                            result.Add(new NamedValue(value.Name, enumMemberName));
                        }
                    }
                    else
                    {
                        result.Add(value);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Object data generator for the entity set.
        /// </summary>
        private class EntitySetObjectDataGenerator : DataGenerator<IEntitySetData>, IDataGenerator<IEntitySetData>
        {
            private IDataGenerator entityDataGenerator;
            private string entitySetName;

            /// <summary>
            /// Initializes a new instance of the EntitySetObjectDataGenerator class.
            /// </summary>
            /// <param name="entitySetName">Name of the entity set.</param>
            /// <param name="entityObjectGenerator">The entity object generator.</param>
            public EntitySetObjectDataGenerator(string entitySetName, IDataGenerator entityObjectGenerator)
            {
                this.entitySetName = entitySetName;
                this.entityDataGenerator = entityObjectGenerator;
            }

            /// <summary>
            /// Generates the data.
            /// </summary>
            /// <returns>An <see cref="IEntitySetData"/>.</returns>
            public override IEntitySetData GenerateData()
            {
                object obj = this.entityDataGenerator.GenerateData();
                return new EntitySetObjectData(obj, this.entitySetName);
            }
        }
    }
}
