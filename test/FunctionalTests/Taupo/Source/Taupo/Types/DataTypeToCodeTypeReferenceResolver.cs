//---------------------------------------------------------------------
// <copyright file="DataTypeToCodeTypeReferenceResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Types
{
    using System;
    using System.CodeDom;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.CodeDomExtensions;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Resolves DataTypes into their CodeTypeReferences
    /// </summary>
    public class DataTypeToCodeTypeReferenceResolver : IDataTypeToCodeTypeReferenceResolver, IDataTypeVisitor<CodeTypeReference>
    {
        /// <summary>
        /// Resolves the specified type into its type reference.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>CodeTypeReference that should be used in code to refer to the type.</returns>
        public CodeTypeReference Resolve(DataType dataType)
        {
            return dataType.Accept(this);
        }

        /// <summary>
        /// Resolves the specified type into its type reference.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>CodeTypeReference that should be used in code to refer to the type.</returns>
        public virtual CodeTypeReference Visit(CollectionDataType dataType)
        {
            return Code.GenericType("ICollection", this.Resolve(dataType.ElementDataType));
        }

        /// <summary>
        /// Resolves the specified type into its type reference.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>CodeTypeReference that should be used in code to refer to the type.</returns>
        public CodeTypeReference Visit(ComplexDataType dataType)
        {
            return new CodeTypeReference(dataType.Definition.FullName);
        }

        /// <summary>
        /// Resolves the specified type into its type reference.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>CodeTypeReference that should be used in code to refer to the type.</returns>
        public CodeTypeReference Visit(EntityDataType dataType)
        {
            return new CodeTypeReference(dataType.Definition.FullName);
        }

        /// <summary>
        /// Resolves the specified type into its type reference.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>CodeTypeReference that should be used in code to refer to the type.</returns>
        public CodeTypeReference Visit(PrimitiveDataType dataType)
        {
            EnumDataType enumDataType = dataType as EnumDataType;
            if (enumDataType != null)
            {
                if (dataType.IsNullable)
                {
                    return Code.GenericType(typeof(System.Nullable<>).FullName, new CodeTypeReference(enumDataType.Definition.FullName));
                }

                return new CodeTypeReference(enumDataType.Definition.FullName);
            }
            else
            {
                Type type = dataType.GetFacet<PrimitiveClrTypeFacet>().Value;

                if (dataType.IsNullable && type.IsValueType())
                {
                    type = typeof(Nullable<>).MakeGenericType(type);
                }

                return new CodeTypeReference(type);
            }
        }

        /// <summary>
        /// Resolves the specified type into its type reference.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>CodeTypeReference that should be used in code to refer to the type.</returns>
        public virtual CodeTypeReference Visit(ReferenceDataType dataType)
        {
            throw new TaupoNotSupportedException("Taupo does not support code gen for reference data type yet.");
        }

        /// <summary>
        /// Resolves the specified type into its type reference.
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>CodeTypeReference that should be used in code to refer to the type.</returns>
        public virtual CodeTypeReference Visit(RowDataType dataType)
        {
            throw new TaupoNotSupportedException("Taupo does not support code gen for row data type yet.");
        }
    }
}
