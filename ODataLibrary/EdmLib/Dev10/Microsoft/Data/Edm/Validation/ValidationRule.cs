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

using System;
using System.Diagnostics;

namespace Microsoft.Data.Edm.Validation
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
    public class ValidationRule<TItem> : ValidationRule
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
