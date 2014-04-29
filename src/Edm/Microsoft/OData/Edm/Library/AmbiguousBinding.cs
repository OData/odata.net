//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a name binding to more than one item.
    /// </summary>
    /// <typeparam name="TElement">Type of the ambiguous element.</typeparam>
    internal class AmbiguousBinding<TElement> : BadElement
        where TElement : class, IEdmNamedElement
    {
        private readonly List<TElement> bindings = new List<TElement>();

        public AmbiguousBinding(TElement first, TElement second)
            : base(new EdmError[] { new EdmError(null, EdmErrorCode.BadAmbiguousElementBinding, Edm.Strings.Bad_AmbiguousElementBinding(first.Name)) })
        {
            this.AddBinding(first);
            this.AddBinding(second);
        }

        public IEnumerable<TElement> Bindings
        {
            get { return this.bindings; }
        }

        public string Name
        {
            get { return this.bindings.First().Name ?? string.Empty; }
        }

        public void AddBinding(TElement binding)
        {
            if (!this.bindings.Contains(binding))
            {
                this.bindings.Add(binding);
            }
        }
    }
}
