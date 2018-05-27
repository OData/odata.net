//---------------------------------------------------------------------
// <copyright file="NamedValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    /// <summary>
    /// Class representing a single named value (name and value pair).
    /// </summary>
    public sealed class NamedValue
    {
        /// <summary>
        /// The name of the value. Or null if the name was not used for this value.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The value - a literal.
        /// </summary>
        private readonly LiteralToken value;

        /// <summary>
        /// Create a new NamedValue lookup given name and value.
        /// </summary>
        /// <param name="name">The name of the value. Or null if the name was not used for this value.</param>
        /// <param name="value">The value - a literal.</param>
        public NamedValue(string name, LiteralToken value)
        {
            ExceptionUtils.CheckArgumentNotNull(value, "value");

            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// The name of the value. Or null if the name was not used for this value.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// The value - a literal.
        /// </summary>
        public LiteralToken Value
        {
            get { return this.value; }
        }
    }
}