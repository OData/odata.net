//---------------------------------------------------------------------
// <copyright file="OperationSegmentParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Represents a named parameter value for invoking an operation in an OData path.
    /// </summary>
    public sealed class OperationSegmentParameter
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OperationSegmentParameter"/>.
        /// </summary>
        /// <param name="name">The name of the parameter. Cannot be null or empty.</param>
        /// <param name="value">The value of the parameter.</param>
        public OperationSegmentParameter(string name, object value)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(name, "name");
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The parameter value.
        /// </summary>
        public object Value { get; private set; }
    }
}