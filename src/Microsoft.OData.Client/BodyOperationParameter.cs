//---------------------------------------------------------------------
// <copyright file="BodyOperationParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;

    /// <summary> Represents a parameter associated with a service action.  </summary>
    public sealed class BodyOperationParameter : OperationParameter
    {
        /// <summary> Instantiates a new BodyOperationParameter </summary>
        /// <param name="name">The name of the body operation parameter.</param>
        /// <param name="value">The value of the body operation parameter.</param>
        public BodyOperationParameter(string name, Object value)
            : base(name, value)
        {
        }
    }
}
