//---------------------------------------------------------------------
// <copyright file="UriOperationParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;

    /// <summary>
    /// Represents a parameter associated with a service operation  or a service function.
    /// </summary>
    public class UriOperationParameter : OperationParameter
    {
        /// <summary> Instantiates a new UriOperationParameter </summary>
        /// <param name="name">The name of the uri operation parameter.</param>
        /// <param name="value">The value of the uri operation parameter.</param>
        public UriOperationParameter(string name, Object value)
            : base(name, value)
        {
        }
    }
}
