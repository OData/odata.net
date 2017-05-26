//---------------------------------------------------------------------
// <copyright file="AmbiguousBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm
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
