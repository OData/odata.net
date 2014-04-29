//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.Validation;

    /// <summary>
    /// Represents a navigation property path that could not be resolved.
    /// </summary>
    internal class UnresolvedNavigationPropertyPath : BadNavigationProperty, IUnresolvedElement
    {
        public UnresolvedNavigationPropertyPath(IEdmEntityType startingType, string path, EdmLocation location)
            : base(startingType, path, new[] { new EdmError(location, EdmErrorCode.BadUnresolvedNavigationPropertyPath, Edm.Strings.Bad_UnresolvedNavigationPropertyPath(path, startingType.FullName())) })
        {
        }
    }
}
