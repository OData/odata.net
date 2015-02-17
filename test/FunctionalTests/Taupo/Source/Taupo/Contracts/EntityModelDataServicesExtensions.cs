//---------------------------------------------------------------------
// <copyright file="EntityModelDataServicesExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Extension methods for the IEntityModelObjectServices and IEntityModelConceptualDataServices.
    /// </summary>
    public static class EntityModelDataServicesExtensions
    {
        /// <summary>
        /// Creates a default instance of a CLR object which represents the specified <paramref name="entityType"/> and <paramref name="entitySet"/>.
        /// </summary>
        /// <param name="objectServices">The object services.</param>
        /// <param name="entitySet">The <see cref="EntitySet"/> in which the <paramref name="entityType"/> resides.</param>
        /// <param name="entityType">The <see cref="EntityType"/> from which to create a CLR object.</param>
        /// <returns>A CLR object that maps to the <paramref name="entityType"/>.</returns>
        public static object CreateData(this IEntityModelObjectServices objectServices, EntitySet entitySet, EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(objectServices, "objectServices");
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            if (entityType.IsAbstract)
            {
                throw new TaupoArgumentException("Cannot create data for abstract entity type.");
            }

            var generator = objectServices.GetEntitySetObjectGenerator(entityType.FullName, entitySet.ContainerQualifiedName);
            var entity = generator.GenerateData().Data;
            return entity;
        }

        /// <summary>
        /// Gets the properties values for all scalar and complex properties in the form of <see cref="NamedValue"/>.
        /// </summary>
        /// <param name="objectServices">The object services.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The properties values.</returns>
        public static IList<NamedValue> GetPropertiesValues(this IEntityModelObjectServices objectServices, object entity, EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(objectServices, "objectServices");
            ExceptionUtilities.CheckArgumentNotNull(entity, "entity");
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            List<NamedValue> result = new List<NamedValue>();
            CachePropertiesValues(result, string.Empty, entityType.FullName, entityType.AllProperties, entity, objectServices);
            return result;
        }

        /// <summary>
        /// Generates values for the properties with the specified paths.
        /// </summary>
        /// <param name="objectServices">The object services.</param>
        /// <param name="entityTypeFullName">An entity type full name.</param>
        /// <param name="entitySetName">A name of the entity set for which data needs to be generated.</param>
        /// <param name="propertyPaths">The property paths for which to generate values.</param>
        /// <param name="valuesToAvoid">The collection of named values where name is a property path and value is a property value which should be avoided when generating value for this property.</param>
        /// <returns>Generated properties' values.</returns>
        public static IEnumerable<NamedValue> GeneratePropertyValues(this IEntityModelObjectServices objectServices, string entityTypeFullName, string entitySetName, IEnumerable<string> propertyPaths, params NamedValue[] valuesToAvoid)
        {
            ExceptionUtilities.CheckArgumentNotNull(objectServices, "objectServices");
            return objectServices.GeneratePropertyValues(entityTypeFullName, entitySetName, propertyPaths, (IEnumerable<NamedValue>)valuesToAvoid);
        }

        /// <summary>
        /// Generates values for the properties with the specified paths.
        /// </summary>
        /// <param name="conceptualDataServices">Conceptual data services.</param>
        /// <param name="entitySet">The <see cref="EntitySet"/> in which the <paramref name="entityType"/> resides.</param>
        /// <param name="entityType">The <see cref="EntityType"/> for which to generate properties' values.</param>
        /// <param name="propertyPaths">The property paths for which to generate values.</param>
        /// <param name="valuesToAvoid">The collection of named values where name is a property path and value is a property value which should be avoided when generating value for this property.</param>
        /// <returns>Generated properties' values.</returns>
        public static IEnumerable<NamedValue> GeneratePropertyValues(this IEntityModelConceptualDataServices conceptualDataServices, EntitySet entitySet, EntityType entityType, IEnumerable<string> propertyPaths, params NamedValue[] valuesToAvoid)
        {
            return conceptualDataServices.GeneratePropertyValues(entitySet, entityType, propertyPaths, (IEnumerable<NamedValue>)valuesToAvoid);
        }

        /// <summary>
        /// Generates values for the properties with the specified paths.
        /// </summary>
        /// <param name="conceptualDataServices">Conceptual data services.</param>
        /// <param name="entitySet">The <see cref="EntitySet"/> in which the <paramref name="entityType"/> resides.</param>
        /// <param name="entityType">The <see cref="EntityType"/> for which to generate properties' values.</param>
        /// <param name="propertyPaths">The property paths for which to generate values.</param>
        /// <param name="valuesToAvoid">The collection of named values where name is a property path and value is a property value which should be avoided when generating value for this property.</param>
        /// <returns>Generated properties' values.</returns>
        public static IEnumerable<NamedValue> GeneratePropertyValues(this IEntityModelConceptualDataServices conceptualDataServices, EntitySet entitySet, EntityType entityType, IEnumerable<string> propertyPaths, IEnumerable<NamedValue> valuesToAvoid)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            return conceptualDataServices.GeneratePropertyValues(entitySet.ContainerQualifiedName, entityType.FullName, propertyPaths, valuesToAvoid);
        }

        /// <summary>
        /// Generates values for the properties with the specified paths.
        /// </summary>
        /// <param name="conceptualDataServices">Conceptual data services.</param>
        /// <param name="entitySetName">A name of the entity set for which data needs to be generated.</param>
        /// <param name="entityTypeFullName">An entity type full name.</param>
        /// <param name="propertyPaths">The property paths for which to generate values.</param>
        /// <param name="valuesToAvoid">The collection of named values where name is a property path and value is a property value which should be avoided when generating value for this property.</param>
        /// <returns>Generated properties' values.</returns>
        public static IEnumerable<NamedValue> GeneratePropertyValues(this IEntityModelConceptualDataServices conceptualDataServices, string entitySetName, string entityTypeFullName, IEnumerable<string> propertyPaths, params NamedValue[] valuesToAvoid)
        {
            return conceptualDataServices.GeneratePropertyValues(entitySetName, entityTypeFullName, propertyPaths, (IEnumerable<NamedValue>)valuesToAvoid);
        }

        /// <summary>
        /// Generates values for the properties with the specified paths.
        /// </summary>
        /// <param name="conceptualDataServices">Conceptual data services.</param>
        /// <param name="entitySetName">A name of the entity set for which data needs to be generated.</param>
        /// <param name="entityTypeFullName">An entity type full name.</param>
        /// <param name="propertyPaths">The property paths for which to generate values.</param>
        /// <param name="valuesToAvoid">The collection of named values where name is a property path and value is a property value which should be avoided when generating value for this property.</param>
        /// <returns>Generated properties' values.</returns>
        public static IEnumerable<NamedValue> GeneratePropertyValues(this IEntityModelConceptualDataServices conceptualDataServices, string entitySetName, string entityTypeFullName, IEnumerable<string> propertyPaths, IEnumerable<NamedValue> valuesToAvoid)
        {
            ExceptionUtilities.CheckArgumentNotNull(conceptualDataServices, "conceptualDataServices");
            ExceptionUtilities.CheckArgumentNotNull(propertyPaths, "propertyPaths");
            ExceptionUtilities.CheckArgumentNotNull(valuesToAvoid, "valuesToAvoid");

            var memberDataGenerators = conceptualDataServices.GetStructuralGenerator(entityTypeFullName, entitySetName);
            const int MaxAttempts = 10;
            List<NamedValue> generatedValues = new List<NamedValue>();

            foreach (string propertyPath in propertyPaths)
            {
                IDataGenerator propertyGenerator = memberDataGenerators.GetMemberDataGenerator(propertyPath);
                var valuesToAvoidForThisProperty = valuesToAvoid.Where(nv => nv.Name == propertyPath).Select(nv => nv.Value).ToList();
                
                int attemptCount = 0;
                object value;
                do
                {
                    value = propertyGenerator.GenerateData();
                    attemptCount++;
                }
                while (valuesToAvoidForThisProperty.Contains(value, ValueComparer.Instance) && attemptCount < MaxAttempts);

                generatedValues.AddMemberData(propertyPath, value);
            }

            return generatedValues;
        }

        private static void CachePropertiesValues(List<NamedValue> list, string path, string structuralTypeFullName, IEnumerable<MemberProperty> properties, object obj, IEntityModelObjectServices objectServices)
        {
            var adapter = objectServices.GetObjectAdapter(structuralTypeFullName);

            foreach (MemberProperty property in properties.Where(p => !(p.PropertyType is StreamDataType)))
            {
                object value = adapter.GetMemberValue<object>(obj, property.Name);

                ComplexDataType complexDataType = property.PropertyType as ComplexDataType;
                CollectionDataType collectionDataType = property.PropertyType as CollectionDataType;

                if (collectionDataType != null && value != null)
                {
                    var complexElementDataType = collectionDataType.ElementDataType as ComplexDataType;

                    IEnumerable enumerable = value as IEnumerable;
                    ExceptionUtilities.CheckObjectNotNull(enumerable, "Property type is a collection but does not implement IEnumerable. Property path: '{0}'.", path + property.Name);

                    int count = 0;
                    foreach (var collectionElement in enumerable)
                    {
                        string currentPath = path + property.Name + "." + count;
                        if (complexElementDataType == null || collectionElement == null)
                        {
                            list.Add(new NamedValue(currentPath, collectionElement));
                        }
                        else
                        {
                            CachePropertiesValues(
                                list,
                                currentPath + ".",
                                complexElementDataType.Definition.FullName,
                                complexElementDataType.Definition.Properties,
                                collectionElement,
                                objectServices);
                        }

                        count++;
                    }

                    if (count == 0)
                    {
                        list.Add(new NamedValue(path + property.Name, EmptyData.Value));
                    }
                }
                else if (value == null || complexDataType == null)
                {
                    list.Add(new NamedValue(path + property.Name, value));
                }
                else
                {
                    CachePropertiesValues(list, path + property.Name + ".", complexDataType.Definition.FullName, complexDataType.Definition.Properties, value, objectServices);
                }
            }
        }
    }
}