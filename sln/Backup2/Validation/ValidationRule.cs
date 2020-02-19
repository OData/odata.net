//---------------------------------------------------------------------
// <copyright file="ValidationRule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
