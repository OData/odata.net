//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
