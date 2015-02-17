//---------------------------------------------------------------------
// <copyright file="ClrTypeReferenceResolverBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Common
{
    using System.CodeDom;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Base implementation of the backing clr type resolver contract using a visitor pattern
    /// </summary>
    public abstract class ClrTypeReferenceResolverBase : IClrTypeReferenceResolver, IDataTypeVisitor<CodeTypeReference>
    {
        /// <summary>
        /// Resolves a clr type for the given data type by visiting the type
        /// </summary>
        /// <param name="dataType">The data type to resolve</param>
        /// <returns>The resolved clr type</returns>
        public CodeTypeReference ResolveClrTypeReference(DataType dataType)
        {
            ExceptionUtilities.CheckArgumentNotNull(dataType, "dataType");
            return dataType.Accept(this);
        }

        /// <summary>
        /// Build a Type reference for a structural type
        /// </summary>
        /// <param name="structuralType">Structural Type</param>
        /// <returns>Code Type Reference</returns>
        public abstract CodeTypeReference ResolveClrTypeReference(NamedStructuralType structuralType);

        /// <summary>
        /// Visits a collection data type
        /// </summary>
        /// <param name="dataType">The data type to visit</param>
        /// <returns>The backing type for the data type</returns>
        public CodeTypeReference Visit(CollectionDataType dataType)
        {
            throw new TaupoNotSupportedException("Collection backing types cannot be determined without a property");
        }

        /// <summary>
        /// Visits a complex data type
        /// </summary>
        /// <param name="dataType">The data type to visit</param>
        /// <returns>The backing type for the data type</returns>
        public CodeTypeReference Visit(ComplexDataType dataType)
        {
            return this.ResolveClrTypeReference(dataType.Definition);
        }

        /// <summary>
        /// Visits an entity data type
        /// </summary>
        /// <param name="dataType">The data type to visit</param>
        /// <returns>The backing type for the data type</returns>
        public CodeTypeReference Visit(EntityDataType dataType)
        {
            return this.ResolveClrTypeReference(dataType.Definition);
        }

        /// <summary>
        /// Visits a primitive data type
        /// </summary>
        /// <param name="dataType">The data type to visit</param>
        /// <returns>The backing type for the data type</returns>
        public CodeTypeReference Visit(PrimitiveDataType dataType)
        {
            EnumDataType enumDataType = dataType as EnumDataType;
            if (enumDataType != null)
            {
                throw new TaupoNotSupportedException("Not supported");
            }
            else
            {
                var facet = dataType.GetFacet<PrimitiveClrTypeFacet>();
                ExceptionUtilities.CheckObjectNotNull(facet, "Type did not have a primitive clr type facet: {0}", dataType);
                var type = Code.TypeRef(facet.Value);
                if (dataType.IsNullable && !facet.Value.IsClass())
                {
                    type = Code.GenericType("Nullable", type);
                }

                return type;
            }
        }

        /// <summary>
        /// Visits a reference data type
        /// </summary>
        /// <param name="dataType">The data type to visit</param>
        /// <returns>The backing type for the data type</returns>
        public CodeTypeReference Visit(ReferenceDataType dataType)
        {
            throw new TaupoNotSupportedException("Not supported");
        }
    }
}