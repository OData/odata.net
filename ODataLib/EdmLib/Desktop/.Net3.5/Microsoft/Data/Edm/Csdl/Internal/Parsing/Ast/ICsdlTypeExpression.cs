//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast
{
    /// <summary>
    /// Represents an inline type expression, such as <see cref="CsdlCollectionType"/>, <see cref="CsdlRowType"/> and <see cref="CsdlEntityReferenceType"/> 
    /// in the context of <see cref="CsdlExpressionTypeReference"/>.
    /// Note that nominal type declarations, such as entity, complex, association and primitive types, are not considered to be type expressions in the context
    /// of <see cref="CsdlExpressionTypeReference"/> - these types are handled in <see cref="CsdlNamedTypeReference"/>.
    /// </summary>
    internal interface ICsdlTypeExpression
    {
    }
}
