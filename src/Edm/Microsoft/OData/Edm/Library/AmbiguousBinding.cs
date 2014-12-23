//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
