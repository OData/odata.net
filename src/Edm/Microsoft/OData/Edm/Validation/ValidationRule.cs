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

using System;
using System.Diagnostics;

namespace Microsoft.OData.Edm.Validation
{
    /// <summary>
    /// A semantic validation rule.
    /// </summary>
    public abstract class ValidationRule
    {
        internal abstract Type ValidatedType { get; }

        internal abstract void Evaluate(ValidationContext context, object item);
    }

    /// <summary>
    /// A validation rule that is valid for a specific type.
    /// </summary>
    /// <typeparam name="TItem">Type that the rule is valid for.</typeparam>
    public sealed class ValidationRule<TItem> : ValidationRule
        where TItem : IEdmElement
    {
        private readonly Action<ValidationContext, TItem> validate;

        /// <summary>
        /// Initializes a new instance of the ValidationRule class.
        /// </summary>
        /// <param name="validate">Action to perform the validation.</param>
        public ValidationRule(Action<ValidationContext, TItem> validate)
        {
            this.validate = validate;
        }

        internal override Type ValidatedType
        {
            get { return typeof(TItem); }
        }

        internal override void Evaluate(ValidationContext context, object item)
        {
            Debug.Assert(item is TItem, "item should be " + typeof(TItem));
            TItem typedItem = (TItem)item;
            this.validate(context, typedItem);
        }
    }
}
