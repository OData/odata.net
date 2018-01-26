//---------------------------------------------------------------------
// <copyright file="EdmCoreModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Provides predefined declarations relevant to EDM semantics.
    /// </summary>
    public class EdmCoreModel : EdmElement, IEdmModel, IEdmCoreModelElement
    {
        /// <summary>
        /// The default core EDM model.
        /// </summary>
        public static readonly EdmCoreModel Instance = new EdmCoreModel();

        private readonly IDictionary<string, EdmPrimitiveTypeKind> _primitiveTypeKinds = new Dictionary<string, EdmPrimitiveTypeKind>();
        private readonly IDictionary<EdmPrimitiveTypeKind, EdmCoreModelPrimitiveType> _primitiveTypesByKind = new Dictionary<EdmPrimitiveTypeKind, EdmCoreModelPrimitiveType>();

        private readonly IDictionary<string, EdmPathTypeKind> _pathTypeKinds = new Dictionary<string, EdmPathTypeKind>();
        private readonly IDictionary<EdmPathTypeKind, EdmCoreModelPathType> _pathTypesByKind = new Dictionary<EdmPathTypeKind, EdmCoreModelPathType>();

        private readonly IEdmDirectValueAnnotationsManager _annotationsManager = new EdmDirectValueAnnotationsManager();

        private readonly IList<IEdmSchemaElement> _coreSchemaElements = new List<IEdmSchemaElement>();

        private readonly IDictionary<string, IEdmSchemaType> _coreSchemaTypes = new Dictionary<string, IEdmSchemaType>();

        private readonly EdmCoreModelComplexType _complexType = EdmCoreModelComplexType.Instance;
        private readonly EdmCoreModelEntityType _entityType = EdmCoreModelEntityType.Instance;
        private readonly EdmCoreModelUntypedType _untypedType = EdmCoreModelUntypedType.Instance;

        private EdmCoreModel()
        {
            IList<EdmCoreModelPrimitiveType> primitiveTypes = new List<EdmCoreModelPrimitiveType>
            {
                new EdmCoreModelPrimitiveType("Double", EdmPrimitiveTypeKind.Double),
                new EdmCoreModelPrimitiveType("Single", EdmPrimitiveTypeKind.Single),
                new EdmCoreModelPrimitiveType("Int64", EdmPrimitiveTypeKind.Int64),
                new EdmCoreModelPrimitiveType("Int32", EdmPrimitiveTypeKind.Int32),
                new EdmCoreModelPrimitiveType("Int16", EdmPrimitiveTypeKind.Int16),
                new EdmCoreModelPrimitiveType("SByte", EdmPrimitiveTypeKind.SByte),
                new EdmCoreModelPrimitiveType("Byte", EdmPrimitiveTypeKind.Byte),
                new EdmCoreModelPrimitiveType("Boolean", EdmPrimitiveTypeKind.Boolean),
                new EdmCoreModelPrimitiveType("Guid", EdmPrimitiveTypeKind.Guid),
                new EdmCoreModelPrimitiveType("Duration", EdmPrimitiveTypeKind.Duration),
                new EdmCoreModelPrimitiveType("TimeOfDay", EdmPrimitiveTypeKind.TimeOfDay),
                new EdmCoreModelPrimitiveType("DateTimeOffset", EdmPrimitiveTypeKind.DateTimeOffset),
                new EdmCoreModelPrimitiveType("Date", EdmPrimitiveTypeKind.Date),
                new EdmCoreModelPrimitiveType("Decimal", EdmPrimitiveTypeKind.Decimal),
                new EdmCoreModelPrimitiveType("Binary", EdmPrimitiveTypeKind.Binary),
                new EdmCoreModelPrimitiveType("String", EdmPrimitiveTypeKind.String),
                new EdmCoreModelPrimitiveType("Stream", EdmPrimitiveTypeKind.Stream),

                new EdmCoreModelPrimitiveType("Geography", EdmPrimitiveTypeKind.Geography),
                new EdmCoreModelPrimitiveType("GeographyPoint", EdmPrimitiveTypeKind.GeographyPoint),
                new EdmCoreModelPrimitiveType("GeographyLineString", EdmPrimitiveTypeKind.GeographyLineString),
                new EdmCoreModelPrimitiveType("GeographyPolygon", EdmPrimitiveTypeKind.GeographyPolygon),
                new EdmCoreModelPrimitiveType("GeographyCollection", EdmPrimitiveTypeKind.GeographyCollection),
                new EdmCoreModelPrimitiveType("GeographyMultiPolygon", EdmPrimitiveTypeKind.GeographyMultiPolygon),
                new EdmCoreModelPrimitiveType("GeographyMultiLineString", EdmPrimitiveTypeKind.GeographyMultiLineString),
                new EdmCoreModelPrimitiveType("GeographyMultiPoint", EdmPrimitiveTypeKind.GeographyMultiPoint),
                new EdmCoreModelPrimitiveType("Geometry", EdmPrimitiveTypeKind.Geometry),
                new EdmCoreModelPrimitiveType("GeometryPoint", EdmPrimitiveTypeKind.GeometryPoint),
                new EdmCoreModelPrimitiveType("GeometryLineString", EdmPrimitiveTypeKind.GeometryLineString),
                new EdmCoreModelPrimitiveType("GeometryPolygon", EdmPrimitiveTypeKind.GeometryPolygon),
                new EdmCoreModelPrimitiveType("GeometryCollection", EdmPrimitiveTypeKind.GeometryCollection),
                new EdmCoreModelPrimitiveType("GeometryMultiPolygon", EdmPrimitiveTypeKind.GeometryMultiPolygon),
                new EdmCoreModelPrimitiveType("GeometryMultiLineString", EdmPrimitiveTypeKind.GeometryMultiLineString),
                new EdmCoreModelPrimitiveType("GeometryMultiPoint", EdmPrimitiveTypeKind.GeometryMultiPoint),

                new EdmCoreModelPrimitiveType("PrimitiveType", EdmPrimitiveTypeKind.PrimitiveType), // Edm.PrimitiveType
            };

            foreach (var primitive in primitiveTypes)
            {
                _primitiveTypeKinds[primitive.Name] = primitive.PrimitiveKind;
                _primitiveTypeKinds[primitive.Namespace + '.' + primitive.Name] = primitive.PrimitiveKind;
                 _primitiveTypesByKind[primitive.PrimitiveKind] = primitive;

                _coreSchemaTypes[primitive.Namespace + '.' + primitive.Name] = primitive;
                _coreSchemaTypes[primitive.Name] = primitive;

                _coreSchemaElements.Add(primitive);
            }

            _coreSchemaElements.Add(_complexType); // Edm.EntityType
            _coreSchemaTypes[_complexType.Namespace + '.' + _complexType.Name] = _complexType;
            _coreSchemaTypes[_complexType.Name] = _complexType;

            _coreSchemaElements.Add(_entityType); // Edm.ComplexType
            _coreSchemaTypes[_entityType.Namespace + '.' + _entityType.Name] = _entityType;
            _coreSchemaTypes[_entityType.Name] = _entityType;

            _coreSchemaElements.Add(_untypedType); // Edm.Untyped
            _coreSchemaTypes[_untypedType.Namespace + '.' + _untypedType.Name] = _untypedType;
            _coreSchemaTypes[_untypedType.Name] = _untypedType;

            EdmCoreModelPathType[] pathTypes =
            {
                new EdmCoreModelPathType("AnnotationPath", EdmPathTypeKind.Annotation),  // Edm.AnotationPath
                new EdmCoreModelPathType("PropertyPath", EdmPathTypeKind.Property), // Edm.PropertyPath
                new EdmCoreModelPathType("NavigationPropertyPath", EdmPathTypeKind.NavigationProperty), // Edm.NavigationPropertyPath
            };

            foreach (var pathType in pathTypes)
            {
                _pathTypeKinds[pathType.Name] = pathType.PathKind;
                _pathTypeKinds[pathType.Namespace + '.' + pathType.Name] = pathType.PathKind;
                _pathTypesByKind[pathType.PathKind] = pathType;

                _coreSchemaTypes[pathType.Namespace + '.' + pathType.Name] = pathType;
                _coreSchemaTypes[pathType.Name] = pathType;

                _coreSchemaElements.Add(pathType);
            }
        }

        /// <summary>
        /// Gets the namespace of this core model.
        /// </summary>
        public static string Namespace
        {
            get { return EdmCoreModelConstants.EdmNamespace; }
        }

        /// <summary>
        /// Gets the types defined in this core model.
        /// </summary>
        public IEnumerable<IEdmSchemaElement> SchemaElements
        {
            get { return _coreSchemaElements; }
        }

        /// <summary>
        /// Gets the collection of namespaces that schema elements use contained in this model.
        /// </summary>
        public IEnumerable<string> DeclaredNamespaces
        {
            get { return Enumerable.Empty<string>(); }
        }

        /// <summary>
        /// Gets the vocabulary annotations defined in this model.
        /// </summary>
        public IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations
        {
            get { return Enumerable.Empty<IEdmVocabularyAnnotation>(); }
        }

        /// <summary>
        /// Gets the collection of models referred to by this model.
        /// </summary>
        public IEnumerable<IEdmModel> ReferencedModels
        {
            get { return Enumerable.Empty<IEdmModel>(); }
        }

        /// <summary>
        /// Gets the model's annotations manager.
        /// </summary>
        public IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager
        {
            get { return this._annotationsManager; }
        }

        /// <summary>
        /// Gets the only one entity container of the model.
        /// </summary>
        public IEdmEntityContainer EntityContainer
        {
            get { return null; }
        }

        /// <summary>
        /// Gets a reference to a non-atomic collection type definition.
        /// </summary>
        /// <param name="elementType">Type of elements in the collection.</param>
        /// <returns>A new non-atomic collection type reference.</returns>
        public static IEdmCollectionTypeReference GetCollection(IEdmTypeReference elementType)
        {
            return new EdmCollectionTypeReference(new EdmCollectionType(elementType));
        }

        /// <summary>
        /// Searches for a type with the given name in this model only and returns null if no such type exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the type being found.</param>
        /// <returns>The requested type, or null if no such type exists.</returns>
        public IEdmSchemaType FindDeclaredType(string qualifiedName)
        {
            IEdmSchemaType element;
            if (_coreSchemaTypes.TryGetValue(qualifiedName, out element))
            {
                return element;
            }

            return null;
        }

        /// <summary>
        /// Searches for bound operations based on the binding type, returns an empty enumerable if no operation exists.
        /// </summary>
        /// <param name="bindingType">Type of the binding.</param>
        /// <returns>A set of operations that share the binding type or empty enumerable if no such operation exists.</returns>
        public IEnumerable<IEdmOperation> FindDeclaredBoundOperations(IEdmType bindingType)
        {
            return Enumerable.Empty<IEdmOperation>();
        }

        /// <summary>
        /// Searches for bound operations based on the qualified name and binding type, returns an empty enumerable if no operation exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the operation.</param>
        /// <param name="bindingType">Type of the binding.</param>
        /// <returns>
        /// A set of operations that share the qualified name and binding type or empty enumerable if no such operation exists.
        /// </returns>
        public IEnumerable<IEdmOperation> FindDeclaredBoundOperations(string qualifiedName, IEdmType bindingType)
        {
            return Enumerable.Empty<IEdmOperation>();
        }

        /// <summary>
        /// Searches for a term with the given name in this model and returns null if no such term exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the term being found.</param>
        /// <returns>The requested term, or null if no such term exists.</returns>
        public IEdmTerm FindDeclaredTerm(string qualifiedName)
        {
            return null;
        }

        /// <summary>
        /// Searches for operations with the given name in this model and returns an empty enumerable if no such operation exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the operation being found.</param>
        /// <returns>A set operations sharing the specified qualified name, or an empty enumerable if no such operation exists.</returns>
        public IEnumerable<IEdmOperation> FindDeclaredOperations(string qualifiedName)
        {
            return Enumerable.Empty<IEdmOperation>();
        }

        /// <summary>
        /// Searches for any functionImport or actionImport by name and parameter names.
        /// </summary>
        /// <param name="operationImportName">The name of the operation imports to find. May be qualified with the namespace.</param>
        /// <param name="parameterNames">The parameter names of the parameters.</param>
        /// <returns>The operation imports that matches the search criteria or empty there was no match.</returns>
        public IEnumerable<IEdmOperationImport> FindOperationImportsByNameNonBindingParameterType(string operationImportName, IEnumerable<string> parameterNames)
        {
            return Enumerable.Empty<IEdmOperationImport>();
        }

        /// <summary>
        /// Gets primitive type by kind.
        /// </summary>
        /// <param name="kind">Kind of the primitive type.</param>
        /// <returns>Primitive type definition.</returns>
        public IEdmPrimitiveType GetPrimitiveType(EdmPrimitiveTypeKind kind)
        {
            return this.GetCoreModelPrimitiveType(kind);
        }

        /// <summary>
        /// Gets Edm.ComplexType type.
        /// </summary>
        /// <returns>IEdmComplexType type definition.</returns>
        public IEdmComplexType GetComplexType()
        {
            return this._complexType;
        }

        /// <summary>
        /// Gets Edm.EntityType type.
        /// </summary>
        /// <returns>IEdmEntityType type definition.</returns>
        public IEdmEntityType GetEntityType()
        {
            return this._entityType;
        }

        /// <summary>
        /// Gets Edm.Untyped type.
        /// </summary>
        /// <returns>IEdmUntypedType type definition.</returns>
        public IEdmUntypedType GetUntypedType()
        {
            return this._untypedType;
        }

        /// <summary>
        /// Gets path type by kind.
        /// </summary>
        /// <param name="kind">Kind of the path type.</param>
        /// <returns>Path type definition.</returns>
        public IEdmPathType GetPathType(EdmPathTypeKind kind)
        {
            EdmCoreModelPathType definition;
            return _pathTypesByKind.TryGetValue(kind, out definition) ? definition : null;
        }

        /// <summary>
        /// Gets the EdmPrimitiveTypeKind by the type name.
        /// </summary>
        /// <param name="typeName">Name of the type to look up.</param>
        /// <returns>EdmPrimitiveTypeKind of the type.</returns>
        public EdmPrimitiveTypeKind GetPrimitiveTypeKind(string typeName)
        {
            EdmPrimitiveTypeKind kind;
            return _primitiveTypeKinds.TryGetValue(typeName, out kind) ? kind : EdmPrimitiveTypeKind.None;
        }

        /// <summary>
        /// Gets a reference to a primitive type of the specified kind.
        /// </summary>
        /// <param name="kind">Primitive kind of the type reference being created.</param>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new primitive type reference.</returns>
        public IEdmPrimitiveTypeReference GetPrimitive(EdmPrimitiveTypeKind kind, bool isNullable)
        {
            IEdmPrimitiveType primitiveDefinition = this.GetCoreModelPrimitiveType(kind);
            if (primitiveDefinition != null)
            {
                return primitiveDefinition.GetPrimitiveTypeReference(isNullable);
            }
            else
            {
                throw new InvalidOperationException(Edm.Strings.EdmPrimitive_UnexpectedKind);
            }
        }

        /// <summary>
        /// Gets the EdmPathTypeKind by the type name.
        /// </summary>
        /// <param name="typeName">Name of the type to look up.</param>
        /// <returns>EdmPrimitiveTypeKind of the type.</returns>
        public EdmPathTypeKind GetPathTypeKind(string typeName)
        {
            EdmPathTypeKind kind;
            return _pathTypeKinds.TryGetValue(typeName, out kind) ? kind : EdmPathTypeKind.None;
        }

        /// <summary>
        /// Gets a reference to a path type of the specified kind.
        /// </summary>
        /// <param name="kind">Primitive kind of the type reference being created.</param>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new primitive type reference.</returns>
        public IEdmPathTypeReference GetPathType(EdmPathTypeKind kind, bool isNullable)
        {
            IEdmPathType pathDefinition = this.GetPathType(kind);
            if (pathDefinition != null)
            {
                return new EdmPathTypeReference(pathDefinition, isNullable);
            }
            else
            {
                throw new InvalidOperationException(Edm.Strings.EdmPath_UnexpectedKind);
            }
        }

        /// <summary>
        /// Gets a reference to the Edm.AnnotationPath type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new Edm.AnnotationPath type reference.</returns>
        public IEdmPathTypeReference GetAnnotationPath(bool isNullable)
        {
            return new EdmPathTypeReference(this.GetPathType(EdmPathTypeKind.Annotation), isNullable);
        }

        /// <summary>
        /// Gets a reference to the Edm.PropertyPath type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new Edm.PropertyPath type reference.</returns>
        public IEdmPathTypeReference GetPropertyPath(bool isNullable)
        {
            return new EdmPathTypeReference(this.GetPathType(EdmPathTypeKind.Property), isNullable);
        }

        /// <summary>
        /// Gets a reference to the Edm.NavigationPropertyPath type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new Edm.NavigationPropertyPath type reference.</returns>
        public IEdmPathTypeReference GetNavigationPropertyPath(bool isNullable)
        {
            return new EdmPathTypeReference(this.GetPathType(EdmPathTypeKind.NavigationProperty), isNullable);
        }

        /// <summary>
        /// Gets a reference to the Edm.EntityType type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new Edm.EntityType type reference.</returns>
        public IEdmEntityTypeReference GetEntityType(bool isNullable)
        {
            return new EdmEntityTypeReference(this._entityType, isNullable);
        }

        /// <summary>
        /// Gets a reference to the Edm.ComplexType type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new Edm.ComplexType type reference.</returns>
        public IEdmComplexTypeReference GetComplexType(bool isNullable)
        {
            return new EdmComplexTypeReference(this._complexType, isNullable);
        }

        /// <summary>
        /// Gets a reference to the Int16 primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new primitive type reference.</returns>
        public IEdmPrimitiveTypeReference GetInt16(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Int16), isNullable);
        }

        /// <summary>
        /// Gets a reference to the Int32 primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new primitive type reference.</returns>
        public IEdmPrimitiveTypeReference GetInt32(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Int32), isNullable);
        }

        /// <summary>
        /// Gets a reference to the Int64 primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new primitive type reference.</returns>
        public IEdmPrimitiveTypeReference GetInt64(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Int64), isNullable);
        }

        /// <summary>
        /// Gets a reference to the Boolean primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new primitive type reference.</returns>
        public IEdmPrimitiveTypeReference GetBoolean(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Boolean), isNullable);
        }

        /// <summary>
        /// Gets a reference to the Byte primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new primitive type reference.</returns>
        public IEdmPrimitiveTypeReference GetByte(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Byte), isNullable);
        }

        /// <summary>
        /// Gets a reference to the SByte primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new primitive type reference.</returns>
        public IEdmPrimitiveTypeReference GetSByte(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.SByte), isNullable);
        }

        /// <summary>
        /// Gets a reference to the Guid primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new primitive type reference.</returns>
        public IEdmPrimitiveTypeReference GetGuid(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Guid), isNullable);
        }

        /// <summary>
        /// Get a reference to the Date primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new primitive type reference</returns>
        public IEdmPrimitiveTypeReference GetDate(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Date), isNullable);
        }

        /// <summary>
        /// Gets a reference to a datetime with offset primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new datetime with offset type reference.</returns>
        public IEdmTemporalTypeReference GetDateTimeOffset(bool isNullable)
        {
            return new EdmTemporalTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), isNullable);
        }

        /// <summary>
        /// Gets a reference to a duration primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new duration type reference.</returns>
        public IEdmTemporalTypeReference GetDuration(bool isNullable)
        {
            return new EdmTemporalTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Duration), isNullable);
        }

        /// <summary>
        /// Gets a reference to a TimeOfDay primitive type definition
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new TimeOfDay type reference.</returns>
        public IEdmTemporalTypeReference GetTimeOfDay(bool isNullable)
        {
            return new EdmTemporalTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.TimeOfDay), isNullable);
        }

        /// <summary>
        /// Gets a reference to a decimal primitive type definition.
        /// </summary>
        /// <param name="precision">Precision of values of this type.</param>
        /// <param name="scale">Scale of values of this type.</param>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new decimal type reference.</returns>
        public IEdmDecimalTypeReference GetDecimal(int? precision, int? scale, bool isNullable)
        {
            if (precision.HasValue || scale.HasValue)
            {
                // Facet values may render this reference as semantically invalid, so can't return an IEdmValidCoreModelElement.
                return new EdmDecimalTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Decimal), isNullable, precision, scale);
            }
            else
            {
                return new EdmDecimalTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Decimal), isNullable);
            }
        }

        /// <summary>
        /// Gets a reference to a decimal primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new decimal type reference.</returns>
        public IEdmDecimalTypeReference GetDecimal(bool isNullable)
        {
            return new EdmDecimalTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Decimal), isNullable);
        }

        /// <summary>
        /// Gets a reference to a single primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new decimal type reference.</returns>
        public IEdmPrimitiveTypeReference GetSingle(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Single), isNullable);
        }

        /// <summary>
        /// Gets a reference to a double primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new decimal type reference.</returns>
        public IEdmPrimitiveTypeReference GetDouble(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Double), isNullable);
        }

        /// <summary>
        /// Gets a reference to a stream primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new stream type reference.</returns>
        public IEdmPrimitiveTypeReference GetStream(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Stream), isNullable);
        }

        /// <summary>
        /// Gets a reference to a temporal primitive type definition.
        /// </summary>
        /// <param name="kind">Primitive kind of the type reference being created.</param>
        /// <param name="precision">Precision of values of this type.</param>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new temporal type reference.</returns>
        public IEdmTemporalTypeReference GetTemporal(EdmPrimitiveTypeKind kind, int? precision, bool isNullable)
        {
            switch (kind)
            {
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Duration:
                case EdmPrimitiveTypeKind.TimeOfDay:
                    return new EdmTemporalTypeReference(this.GetCoreModelPrimitiveType(kind), isNullable, precision);
                default:
                    throw new InvalidOperationException(Edm.Strings.EdmPrimitive_UnexpectedKind);
            }
        }

        /// <summary>
        /// Gets a reference to a temporal primitive type definition.
        /// </summary>
        /// <param name="kind">Primitive kind of the type reference being created.</param>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new temporal type reference.</returns>
        public IEdmTemporalTypeReference GetTemporal(EdmPrimitiveTypeKind kind, bool isNullable)
        {
            switch (kind)
            {
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Duration:
                case EdmPrimitiveTypeKind.TimeOfDay:
                    return new EdmTemporalTypeReference(this.GetCoreModelPrimitiveType(kind), isNullable);
                default:
                    throw new InvalidOperationException(Edm.Strings.EdmPrimitive_UnexpectedKind);
            }
        }

        /// <summary>
        /// Gets a reference to a binary primitive type definition.
        /// </summary>
        /// <param name="isUnbounded">Flag specifying if max length is unbounded.</param>
        /// <param name="maxLength">Maximum length of the type.</param>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new binary type reference.</returns>
        public IEdmBinaryTypeReference GetBinary(bool isUnbounded, int? maxLength, bool isNullable)
        {
            return new EdmBinaryTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Binary), isNullable, isUnbounded, maxLength);
        }

        /// <summary>
        /// Gets a reference to a binary primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new binary type reference.</returns>
        public IEdmBinaryTypeReference GetBinary(bool isNullable)
        {
            return new EdmBinaryTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.Binary), isNullable);
        }

        /// <summary>
        /// Gets a reference to a spatial primitive type definition.
        /// </summary>
        /// <param name="kind">Primitive kind of the type reference being created.</param>
        /// <param name="spatialReferenceIdentifier">Spatial Reference Identifier for the spatial type being created.</param>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new spatial type reference.</returns>
        public IEdmSpatialTypeReference GetSpatial(EdmPrimitiveTypeKind kind, int? spatialReferenceIdentifier, bool isNullable)
        {
            switch (kind)
            {
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    return new EdmSpatialTypeReference(this.GetCoreModelPrimitiveType(kind), isNullable, spatialReferenceIdentifier);
                default:
                    throw new InvalidOperationException(Edm.Strings.EdmPrimitive_UnexpectedKind);
            }
        }

        /// <summary>
        /// Gets a reference to a spatial primitive type definition.
        /// </summary>
        /// <param name="kind">Primitive kind of the type reference being created.</param>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new spatial type reference.</returns>
        public IEdmSpatialTypeReference GetSpatial(EdmPrimitiveTypeKind kind, bool isNullable)
        {
            switch (kind)
            {
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    return new EdmSpatialTypeReference(this.GetCoreModelPrimitiveType(kind), isNullable);
                default:
                    throw new InvalidOperationException(Edm.Strings.EdmPrimitive_UnexpectedKind);
            }
        }

        /// <summary>
        /// Gets a reference to a string primitive type definition.
        /// </summary>
        /// <param name="isUnbounded">Flag specifying if max length is the maximum allowable value.</param>
        /// <param name="maxLength">Maximum length of the type.</param>
        /// <param name="isUnicode">Flag specifying if the type should support unicode encoding.</param>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new string type reference.</returns>
        public IEdmStringTypeReference GetString(bool isUnbounded, int? maxLength, bool? isUnicode, bool isNullable)
        {
            return new EdmStringTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.String), isNullable, isUnbounded, maxLength, isUnicode);
        }

        /// <summary>
        /// Gets a reference to a binary string type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new string type reference.</returns>
        public IEdmStringTypeReference GetString(bool isNullable)
        {
            return new EdmStringTypeReference(this.GetCoreModelPrimitiveType(EdmPrimitiveTypeKind.String), isNullable);
        }

        /// <summary>
        /// Gets a reference to a Edm.Untyped type definition.
        /// </summary>
        /// <returns>A new Edm.Untyped type reference.</returns>
        public IEdmUntypedTypeReference GetUntyped()
        {
            return new EdmUntypedTypeReference(this.GetUntypedType());
        }

        /// <summary>
        /// Searches for vocabulary annotations specified by this model or a referenced model for a given element.
        /// </summary>
        /// <param name="element">The annotated element.</param>
        /// <returns>The vocabulary annotations for the element.</returns>
        public IEnumerable<IEdmVocabularyAnnotation> FindDeclaredVocabularyAnnotations(IEdmVocabularyAnnotatable element)
        {
            return Enumerable.Empty<IEdmVocabularyAnnotation>();
        }

        /// <summary>
        /// Finds a list of types that derive from the supplied type.
        /// </summary>
        /// <param name="baseType">The base type that derived types are being searched for.</param>
        /// <returns>A list of types that derive from the type.</returns>
        public IEnumerable<IEdmStructuredType> FindDirectlyDerivedTypes(IEdmStructuredType baseType)
        {
            return Enumerable.Empty<IEdmStructuredType>();
        }

        private EdmCoreModelPrimitiveType GetCoreModelPrimitiveType(EdmPrimitiveTypeKind kind)
        {
            EdmCoreModelPrimitiveType definition;
            return _primitiveTypesByKind.TryGetValue(kind, out definition) ? definition : null;
        }
    }
}
