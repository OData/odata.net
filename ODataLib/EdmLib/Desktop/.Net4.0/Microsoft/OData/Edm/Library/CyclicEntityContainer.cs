//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents an EDM entity container that cannot be determined due to a cyclic reference.
    /// </summary>
    internal class CyclicEntityContainer : BadEntityContainer
    {
        public CyclicEntityContainer(string name, EdmLocation location)
            : base(name, new EdmError[] { new EdmError(location, EdmErrorCode.BadCyclicEntityContainer, Edm.Strings.Bad_CyclicEntityContainer(name)) })
        {
        }
    }
}
