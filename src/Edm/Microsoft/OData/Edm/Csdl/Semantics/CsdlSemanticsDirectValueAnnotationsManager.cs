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

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Library.Annotations;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides a CSDL-specific annotations manager.
    /// </summary>
    internal class CsdlSemanticsDirectValueAnnotationsManager : EdmDirectValueAnnotationsManager
    {
        protected override IEnumerable<IEdmDirectValueAnnotation> GetAttachedAnnotations(IEdmElement element)
        {
            CsdlSemanticsElement csdlElement = element as CsdlSemanticsElement;
            if (csdlElement != null)
            {
                return csdlElement.DirectValueAnnotations;
            }

            return Enumerable.Empty<IEdmDirectValueAnnotation>();
        }
    }
}
