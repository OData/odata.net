//---------------------------------------------------------------------
// <copyright file="NamedValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents named value.
    /// </summary>
    public class NamedValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedValue"/> class.
        /// </summary>
        /// <param name="name">The name of this named value.</param>
        /// <param name="value">The value of this named value.</param>
        public NamedValue(string name, object value)
        {
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(name, "name");

            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets the name of this named value.
        /// </summary>
        /// <value>The name of this named value.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the value of this named value.
        /// </summary>
        /// <value>The value of this named value.</value>
        public object Value { get; private set; }
    }
}
