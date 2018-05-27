//---------------------------------------------------------------------
// <copyright file="ICsdlTypeExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents an inline type expression, such as <see cref="CsdlCollectionType"/> and <see cref="CsdlEntityReferenceType"/>
    /// in the context of <see cref="CsdlExpressionTypeReference"/>.
    /// Note that nominal type declarations, such as entity, complex and primitive types, are not considered to be type expressions in the context
    /// of <see cref="CsdlExpressionTypeReference"/> - these types are handled in <see cref="CsdlNamedTypeReference"/>.
    /// </summary>
    internal interface ICsdlTypeExpression
    {
    }
}
