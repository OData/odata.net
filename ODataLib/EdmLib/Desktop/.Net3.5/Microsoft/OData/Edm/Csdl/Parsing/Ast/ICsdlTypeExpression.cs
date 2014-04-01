//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
