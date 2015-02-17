//---------------------------------------------------------------------
// <copyright file="MetadataExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Extension methods that make writing astoria tests easier
    /// </summary>
    public static class MetadataExtensionMethods
    {
        /// <summary>
        /// Filters the list of properties and returns only MultiValue of primitive types and complex types.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns>Filtered list of properties.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        public static IEnumerable<QueryProperty> MultiValue(this IEnumerable<QueryProperty> properties)
        {
            ExceptionUtilities.CheckArgumentNotNull(properties, "properties");

            return properties.Where(property => property.IsMultiValue());
        }

        /// <summary>
        /// Filters the list of properties and returns only MultiValue of primitive types and complex types.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="specificType">Type of multivalue to select, Primitive or Complex</param>
        /// <returns>Filtered list of properties.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        public static IEnumerable<QueryProperty> MultiValue(this IEnumerable<QueryProperty> properties, MultiValueType specificType)
        {
            ExceptionUtilities.CheckArgumentNotNull(properties, "properties");

            return properties.Where(property => property.IsMultiValue(specificType));
        }

        /// <summary>
        /// Filters the list of properties and returns only MultiValue of primitive types and complex types.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns>Filtered list of properties.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        public static IEnumerable<MemberProperty> MultiValue(this IEnumerable<MemberProperty> properties)
        {
            ExceptionUtilities.CheckArgumentNotNull(properties, "properties");

            return properties.Where(property => property.IsMultiValue());
        }

        /// <summary>
        /// Filters the list of properties and returns only ComplexProperties.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns>Filtered list of properties.</returns>
        public static IEnumerable<MemberProperty> ComplexProperties(this IEnumerable<MemberProperty> properties)
        {
            ExceptionUtilities.CheckArgumentNotNull(properties, "properties");

            return properties.Where(property => property.PropertyType is ComplexDataType);
        }

        /// <summary>
        /// Filters the list of properties and returns only MultiValue of primitive types and complex types.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="specificType">Type of multivalue to select, Primitive or Complex</param>
        /// <returns>Filtered list of properties.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        public static IEnumerable<MemberProperty> MultiValue(this IEnumerable<MemberProperty> properties, MultiValueType specificType)
        {
            ExceptionUtilities.CheckArgumentNotNull(properties, "properties");

            return properties.Where(property => property.IsMultiValue(specificType));
        }

        /// <summary>
        /// Builds a MultiValue Type based on the DataType passed in
        /// </summary>
        /// <param name="elementType">Element Type to be used to create a MultiValue type</param>
        /// <returns>MultiValue Type Name</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        public static string BuildMultiValueTypeName(this DataType elementType)
        {
            ExceptionUtilities.CheckArgumentNotNull(elementType, "elementType");

            PrimitiveDataType primitiveDataType = elementType as PrimitiveDataType;
            ComplexDataType complexDataType = elementType as ComplexDataType;

            if (primitiveDataType != null)
            {
                string edmTypeName = primitiveDataType.GetFacetValue<EdmTypeNameFacet, string>(null);
                string edmNamespace = primitiveDataType.GetFacetValue<EdmNamespaceFacet, string>(null);

                if (!string.IsNullOrEmpty(edmTypeName) && !string.IsNullOrEmpty(edmNamespace))
                {
                    edmTypeName = edmNamespace + '.' + edmTypeName;
                }

                if (!string.IsNullOrEmpty(edmTypeName))
                {
                    edmTypeName = "Collection(" + edmTypeName + ")";
                }

                return edmTypeName;
            }
            else
            {
                ExceptionUtilities.Assert(complexDataType != null, "Unexpected TypeName to create for a Collection '{0}'", elementType);
                return "Collection(" + complexDataType.Definition.FullName + ")";
            }
        }

        /// <summary>
        /// Returns true if the Query Property is a collection/MultiValue type.
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>true, if it is Edm.MultiValue; false otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        public static bool IsMultiValue(this QueryProperty property)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");
            var bagType = property.PropertyType as QueryCollectionType;
            return (bagType != null) && (bagType.ElementType is QueryScalarType || bagType.ElementType is QueryComplexType);
        }

        /// <summary>
        /// Returns true if the Query Property is a collection/MultiValue type.
        /// </summary>
        /// <param name="property">The property</param>
        /// <param name="specificType">Primitive or Complex param to look at type and determine if its either</param>
        /// <returns>true, if it is Edm.MultiValue; false otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        public static bool IsMultiValue(this QueryProperty property, MultiValueType specificType)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");

            var propertyBagType = property.PropertyType as QueryCollectionType;
            if (propertyBagType != null && specificType == MultiValueType.Primitive && propertyBagType.ElementType is QueryScalarType)
            {
                return true;
            }
            else if (propertyBagType != null && specificType == MultiValueType.Complex && propertyBagType.ElementType is QueryComplexType)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the Query Property is a collection/MultiValue type.
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>true, if it is Edm.MultiValue; false otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        public static bool IsMultiValue(this MemberProperty property)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");

            var bagType = property.PropertyType as CollectionDataType;
            return (bagType != null) && (bagType.ElementDataType is PrimitiveDataType || bagType.ElementDataType is ComplexDataType);
        }

        /// <summary>
        /// Returns true if the Query Property is a collection/MultiValue type.
        /// </summary>
        /// <param name="dataType">The data type</param>
        /// <returns>true, if it is or contains an Edm.MultiValue; false otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        public static bool HasCollection(this DataType dataType)
        {
            ExceptionUtilities.CheckArgumentNotNull(dataType, "dataType");

            return VersionHelper.GatherDataTypes(dataType).Any(dt => dt is CollectionDataType);
        }

        /// <summary>
        /// Returns true if the DataType is a spatial primitive type or a collection of primitive or a complex/entity type with a spatial property
        /// </summary>
        /// <param name="dataType">The data type</param>
        /// <returns>true, if it is a spatial property or contains spatial properties; false otherwise.</returns>
        public static bool IsSpatial(this DataType dataType)
        {
            return VersionHelper.GatherDataTypes(dataType).Any(dt => dt is SpatialDataType);
        }

        /// <summary>
        /// Returns true if the Query Property is a collection/MultiValue type.
        /// </summary>
        /// <param name="property">The property</param>
        /// <param name="specificType">Primitive or Complex param to look at type and determine if its either</param>
        /// <returns>true, if it is Edm.MultiValue; false otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        public static bool IsMultiValue(this MemberProperty property, MultiValueType specificType)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");

            var propertyBagType = property.PropertyType as CollectionDataType;
            if (propertyBagType != null && specificType == MultiValueType.Primitive && propertyBagType.ElementDataType is PrimitiveDataType)
            {
                return true;
            }
            else if (propertyBagType != null && specificType == MultiValueType.Complex && propertyBagType.ElementDataType is ComplexDataType)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the Member Property is a stream type.
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>true, if it is stream; false otherwise.</returns>
        public static bool IsStream(this MemberProperty property)
        {
            return property.PropertyType is StreamDataType;
        }

        /// <summary>
        /// Returns true if the Query Property is a stream type.
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>true, if it is stream; false otherwise.</returns>
        public static bool IsStream(this QueryProperty property)
        {
            var streamType = property.PropertyType as AstoriaQueryStreamType;
            return streamType != null;
        }

        /// <summary>
        /// Filters the list of properties and returns only streams.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns>Filtered list of properties.</returns>
        public static IEnumerable<QueryProperty> Streams(this IEnumerable<QueryProperty> properties)
        {
            return properties.Where(property => property.IsStream() && property.Name != AstoriaQueryStreamType.DefaultStreamPropertyName);
        }

        /// <summary>
        /// Gets the member properties that correspond to a given path for the entity type
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="pathPieces">The property names from the path</param>
        /// <returns>The series of properties for the path</returns>
        public static IEnumerable<MemberProperty> GetPropertiesForPath(this EntityType entityType, string[] pathPieces)
        {
            var currentProperties = entityType.AllProperties;
            foreach (string propertyName in pathPieces)
            {
                MemberProperty property = currentProperties.Single(p => p.Name == propertyName);

                var complexDataType = property.PropertyType as ComplexDataType;
                if (complexDataType != null)
                {
                    currentProperties = complexDataType.Definition.Properties;
                }

                var collectionDataType = property.PropertyType as CollectionDataType;
                if (collectionDataType != null)
                {
                    var complexElementType = collectionDataType.ElementDataType as ComplexDataType;
                    if (complexElementType != null)
                    {
                        currentProperties = complexElementType.Definition.Properties;
                    }
                }

                yield return property;
            }
        }

        /// <summary>
        /// Returns all complex types that are in the namedstructuraltype provided
        /// Ex, if a ComplexType1 has ComplexType2, and Collection(ComplexType3), then all three are returned
        /// </summary>
        /// <param name="structuralType">structuralType to search</param>
        /// <returns>a List of all ComplexTypes</returns>
        public static IEnumerable<ComplexType> AllComplexTypes(this StructuralType structuralType)
        {
            ExceptionUtilities.CheckArgumentNotNull(structuralType, "structuralType");

            List<ComplexDataType> foundComplexTypes = new List<ComplexDataType>();

            List<StructuralType> typesLeftToProcess = new List<StructuralType>();
            typesLeftToProcess.Add(structuralType);

            while (typesLeftToProcess.Count > 0)
            {
                List<ComplexDataType> newTypesToProcess = new List<ComplexDataType>();
                foreach (StructuralType unprocessedNamedStructuralType in typesLeftToProcess)
                {
                    List<ComplexDataType> complexTypes = unprocessedNamedStructuralType.Properties.ComplexProperties().Select(p => p.PropertyType).OfType<ComplexDataType>().Where(dt => !foundComplexTypes.Contains(dt)).ToList();
                    foundComplexTypes.AddRange(complexTypes);
                    newTypesToProcess.AddRange(complexTypes);

                    List<ComplexDataType> complexTypesFromBags = unprocessedNamedStructuralType.Properties.MultiValue(MultiValueType.Complex).Select(p => p.PropertyType).OfType<CollectionDataType>().Select(ct => ct.ElementDataType).OfType<ComplexDataType>().Where(dt => !foundComplexTypes.Contains(dt)).ToList();
                    foundComplexTypes.AddRange(complexTypesFromBags);
                    newTypesToProcess.AddRange(complexTypesFromBags);
                }

                typesLeftToProcess.Clear();
                typesLeftToProcess.AddRange(newTypesToProcess.Select(cdt => cdt.Definition).Cast<StructuralType>());
            }

            return foundComplexTypes.Select(cdt => cdt.Definition).ToList();
        }

        /// <summary>
        /// Gets the MaxProtocolVersion for the EntityModelSchema from the DataServiceBehavior
        /// </summary>
        /// <param name="entityModelSchema">Entity Model Schema</param>
        /// <returns>Data Service Protocol version of the service</returns>
        public static DataServiceProtocolVersion GetMaxProtocolVersion(this EntityModelSchema entityModelSchema)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityModelSchema, "entityModelSchema");
            return entityModelSchema.GetDefaultEntityContainer().GetDataServiceBehavior().MaxProtocolVersion;
        }
        
        /// <summary>
        /// Returns all MemberProperty that are part of the primary key of the EntityType provided and that have a referential constraint
        /// </summary>
        /// <param name="entityType">EntityType to search</param>
        /// <returns>a List of all MemberProperty</returns>
        public static IEnumerable<MemberProperty> PrimaryKeyPropertiesWithReferentialConstraints(this EntityType entityType)
        {
            List<MemberProperty> result = new List<MemberProperty>();

            // find all the referential constraints so we can check for dependent/FK properties
            var referentialConstraints = entityType.AllNavigationProperties.Select(n =>
                n.Association.ReferentialConstraint).Where(r => r != null);

            // for each key property, check for referential constraints
            foreach (var key in entityType.AllKeyProperties)
            {
                if (referentialConstraints.Any(r => r.DependentProperties.Contains(key)))
                {
                    result.Add(key);
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether an entity set is expected to be returned from a function call
        /// </summary>
        /// <param name="function">the function being called</param>
        /// <param name="entitySet">the expected entity set, if appropriate</param>
        /// <returns>whether or not an entity set is expected</returns>
        public static bool TryGetExpectedServiceOperationEntitySet(this Function function, out EntitySet entitySet)
        {
            ExceptionUtilities.CheckArgumentNotNull(function, "function");

            var bodyAnnotation = function.Annotations.OfType<FunctionBodyAnnotation>().SingleOrDefault();
            if (bodyAnnotation != default(FunctionBodyAnnotation))
            {
                var entityBodyType = bodyAnnotation.FunctionBody.ExpressionType as QueryEntityType;
                if (entityBodyType == null)
                {
                    var collectionBodyType = bodyAnnotation.FunctionBody.ExpressionType as QueryCollectionType;
                    if (collectionBodyType != null)
                    {
                        entityBodyType = collectionBodyType.ElementType as QueryEntityType;
                    }
                }

                if (entityBodyType != null)
                {
                    entitySet = entityBodyType.EntitySet;
                    return true;
                }
            }

            entitySet = null;
            return false;
        }

        /// <summary>
        /// Determines whether an entity set is expected to be returned from a function call
        /// </summary>
        /// <param name="function">the function being called</param>
        /// <param name="previousEntitySet">the binding entity set</param>
        /// <param name="returningEntitySet">the expected entity set, if appropriate</param>
        /// <returns>whether or not an entity set is expected</returns>
        public static bool TryGetExpectedActionEntitySet(this Function function, EntitySet previousEntitySet, out EntitySet returningEntitySet)
        {
            ExceptionUtilities.CheckArgumentNotNull(function, "function");

            ServiceOperationAnnotation serviceOperationAnnotation = function.Annotations.OfType<ServiceOperationAnnotation>().Single();
            EntityDataType entityDataType = function.ReturnType as EntityDataType;
            CollectionDataType collectionDataType = function.ReturnType as CollectionDataType;
            if (collectionDataType != null)
            {
                entityDataType = collectionDataType.ElementDataType as EntityDataType;
            }

            if (entityDataType != null)
            {
                if (serviceOperationAnnotation.EntitySetPath == null)
                {
                    returningEntitySet = function.Model.GetDefaultEntityContainer().EntitySets.Single(es => es.EntityType == entityDataType.Definition);
                }
                else
                {
                    string navigationPropertyName = serviceOperationAnnotation.EntitySetPath.Substring(serviceOperationAnnotation.EntitySetPath.LastIndexOf('/') + 1);
                    NavigationProperty navigationProperty = previousEntitySet.EntityType.AllNavigationProperties.Single(np => np.Name == navigationPropertyName);
                    var associationSets = function.Model.GetDefaultEntityContainer().AssociationSets.Where(a => a.AssociationType == navigationProperty.Association);
                    AssociationSet associationSet = associationSets.Single(a => a.Ends.Any(e => e.EntitySet == previousEntitySet));
                    returningEntitySet = associationSet.Ends.Single(es => es.EntitySet != previousEntitySet).EntitySet;
                }

                return true;
            }

            returningEntitySet = null;
            return false;
        }
    }
}
