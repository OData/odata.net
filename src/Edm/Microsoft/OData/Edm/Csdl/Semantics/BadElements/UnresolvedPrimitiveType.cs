//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    internal class UnresolvedPrimitiveType : BadPrimitiveType, IUnresolvedElement
    {
        public UnresolvedPrimitiveType(string qualifiedName, EdmLocation location)
            : base(qualifiedName, EdmPrimitiveTypeKind.None, new EdmError[] { new EdmError(location, EdmErrorCode.BadUnresolvedPrimitiveType, Edm.Strings.Bad_UnresolvedPrimitiveType(qualifiedName)) })
        {
        }
    }
}
