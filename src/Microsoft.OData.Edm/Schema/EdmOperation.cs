//---------------------------------------------------------------------
// <copyright file="EdmOperation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an EDM operation.
    /// </summary>
    public abstract class EdmOperation : EdmNamedElement, IEdmOperation, IEdmFullNamedElement
    {
        private readonly string fullName;
        private readonly List<IEdmOperationParameter> parameters = new List<IEdmOperationParameter>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmOperation"/> class.
        /// </summary>
        /// <param name="namespaceName">Name of the namespace.</param>
        /// <param name="name">The name.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="isBound">if set to <c>true</c> [is bound].</param>
        /// <param name="entitySetPathExpression">The entity set path expression.</param>
        protected EdmOperation(string namespaceName, string name, IEdmTypeReference returnType, bool isBound, IEdmPathExpression entitySetPathExpression)
            : base(name)
        {
            EdmUtil.CheckArgumentNull(namespaceName, "namespaceName");

            this.ReturnType = BuildOperationReturnType(returnType);
            this.Namespace = namespaceName;
            this.IsBound = isBound;
            this.EntitySetPath = entitySetPathExpression;
            this.fullName = EdmUtil.GetFullNameForSchemaElement(namespaceName, this.Name);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmOperation"/> class.
        /// </summary>
        /// <param name="namespaceName">Name of the namespace.</param>
        /// <param name="name">The name.</param>
        /// <param name="returnType">Type of the return.</param>
        protected EdmOperation(string namespaceName, string name, IEdmTypeReference returnType)
            : this(namespaceName, name, returnType, false, null)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance is bound.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is bound; otherwise, <c>false</c>.
        /// </value>
        public bool IsBound { get; private set; }

        /// <summary>
        /// Gets the entity set path expression.
        /// </summary>
        /// <value>
        /// The entity set path expression.
        /// </value>
        public IEdmPathExpression EntitySetPath { get; private set; }

        /// <summary>
        /// Gets the element kind of this operation, which is always Operation.
        /// virtual will be removed in the near future, stop gap to enable testing for now.
        /// </summary>
        public abstract EdmSchemaElementKind SchemaElementKind { get; }

        /// <summary>
        /// Gets the namespace of this function.
        /// </summary>
        public string Namespace { get; private set; }

        /// <summary>
        /// Gets the full name of this schema element.
        /// </summary>
        public string FullName
        {
            get { return this.fullName; }
        }

        /// <summary>
        /// Gets the return type of this function.
        /// Be noted, it should be different with the input object in the constructor.
        /// </summary>
        public IEdmTypeReference ReturnType { get; private set; }

        /// <summary>
        /// Gets the parameters of this function.
        /// </summary>
        public IEnumerable<IEdmOperationParameter> Parameters
        {
            get { return this.parameters; }
        }

        /// <summary>
        /// Searches for a parameter with the given name in this function and returns null if no such parameter exists.
        /// </summary>
        /// <param name="name">The name of the parameter to be found.</param>
        /// <returns>The requested parameter, or null if no such parameter exists.</returns>
        public IEdmOperationParameter FindParameter(string name)
        {
            foreach (IEdmOperationParameter parameter in this.Parameters)
            {
                if (parameter.Name == name)
                {
                    return parameter;
                }
            }

            return null;
        }

        /// <summary>
        /// Creates and adds a parameter to this function (as the last parameter).
        /// </summary>
        /// <param name="name">The name of the parameter being added.</param>
        /// <param name="type">The type of the parameter being added.</param>
        /// <returns>Created parameter.</returns>
        public EdmOperationParameter AddParameter(string name, IEdmTypeReference type)
        {
            EdmOperationParameter parameter = new EdmOperationParameter(this, name, type);
            this.parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// Creates and adds an optional parameter to this function (as the last parameter).
        /// </summary>
        /// <param name="name">The name of the parameter being added.</param>
        /// <param name="type">The type of the parameter being added.</param>
        /// <returns>Created parameter.</returns>
        public EdmOptionalParameter AddOptionalParameter(string name, IEdmTypeReference type)
        {
            return AddOptionalParameter(name, type, null);
        }

        /// <summary>
        /// Creates and adds an optional parameter to this function (as the last parameter).
        /// </summary>
        /// <param name="name">The name of the parameter being added.</param>
        /// <param name="type">The type of the parameter being added.</param>
        /// <param name="defaultValue">The default value for the parameter being added.</param>
        /// <returns>Created parameter.</returns>
        public EdmOptionalParameter AddOptionalParameter(string name, IEdmTypeReference type, string defaultValue)
        {
            EdmOptionalParameter parameter = new EdmOptionalParameter(this, name, type, defaultValue);
            this.parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// Adds a parameter to this function (as the last parameter).
        /// </summary>
        /// <param name="parameter">The parameter being added.</param>
        public void AddParameter(IEdmOperationParameter parameter)
        {
            EdmUtil.CheckArgumentNull(parameter, "parameter");

            this.parameters.Add(parameter);
        }

        // Build the real return type based on the input type reference.
        private IEdmTypeReference BuildOperationReturnType(IEdmTypeReference typeReference)
        {
            // Basically, the input type reference should never be an "IEdmOperationReturnType"
            // Because the end user can't create an "Operation Return Type".
            // But for safety, we return the input if that's an "IEdmOperationReturnType".
            if (typeReference == null || typeReference is IEdmOperationReturnType)
            {
                return typeReference;
            }

            EdmTypeKind kind = typeReference.TypeKind();
            switch (kind)
            {
                case EdmTypeKind.Primitive:
                    return BuildOperationPrimitiveReturnType(typeReference.AsPrimitive());

                case EdmTypeKind.Entity:
                    return new EdmOperationEntityReturnType(this, typeReference.AsEntity());

                case EdmTypeKind.Complex:
                    return new EdmOperationComplexReturnType(this, typeReference.AsComplex());

                case EdmTypeKind.Collection:
                    return new EdmOperationCollectionReturnType(this, typeReference.AsCollection());

                case EdmTypeKind.EntityReference:
                    return new EdmOperationEntityReferenceReturnType(this, typeReference.AsEntityReference());

                case EdmTypeKind.Enum:
                    return new EdmOperationEnumReturnType(this, typeReference.AsEnum());

                case EdmTypeKind.TypeDefinition:
                    return new EdmOperationTypeDefinitionReturnType(this, typeReference.AsTypeDefinition());

                case EdmTypeKind.Untyped:
                    return new EdmOperationUntypedReturnType(this, (IEdmUntypedTypeReference)typeReference);

                case EdmTypeKind.Path:
                    return new EdmOperationPathReturnType(this, (IEdmPathTypeReference)typeReference);

                case EdmTypeKind.None:
                default:
                    // Just return the type reference if we don't know the type kind.
                    return typeReference;
            }
        }

        private IEdmTypeReference BuildOperationPrimitiveReturnType(IEdmPrimitiveTypeReference primitiveReference)
        {
            if (primitiveReference == null)
            {
                return null;
            }

            switch (primitiveReference.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Boolean:
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.Double:
                case EdmPrimitiveTypeKind.Guid:
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.Stream:
                case EdmPrimitiveTypeKind.Date:
                case EdmPrimitiveTypeKind.PrimitiveType:
                    return new EdmOperationPrimitiveReturnType(this, primitiveReference);
                case EdmPrimitiveTypeKind.Binary:
                    return new EdmOperationBinaryReturnType(this, primitiveReference.AsBinary());
                case EdmPrimitiveTypeKind.String:
                    return new EdmOperationStringReturnType(this, primitiveReference.AsString());
                case EdmPrimitiveTypeKind.Decimal:
                    return new EdmOperationDecimalReturnType(this, primitiveReference.AsDecimal());
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Duration:
                case EdmPrimitiveTypeKind.TimeOfDay:
                    return new EdmOperationTemporalReturnType(this, primitiveReference.AsTemporal());
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
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                case EdmPrimitiveTypeKind.GeometryCollection:
                    return new EdmOperationSpatialReturnType(this, primitiveReference.AsSpatial());

                case EdmPrimitiveTypeKind.None:
                default:
                    // Just return the primitive type reference if we don't know the primitive type kind.
                    return primitiveReference;
            }
        }
    }
}
