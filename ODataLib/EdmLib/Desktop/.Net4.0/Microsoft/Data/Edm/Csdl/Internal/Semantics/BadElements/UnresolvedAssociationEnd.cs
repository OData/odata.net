//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    internal class UnresolvedAssociationEnd : BadAssociationEnd, IUnresolvedElement
    {
        public UnresolvedAssociationEnd(IEdmAssociation declaringAssociation, string role, EdmLocation location)
            : base(declaringAssociation, role, new EdmError[] { new EdmError(location, EdmErrorCode.BadNonComputableAssociationEnd, Edm.Strings.Bad_UncomputableAssociationEnd(role)) })
        {
        }
    }
}
