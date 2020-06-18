//---------------------------------------------------------------------
// <copyright file="OperationParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;

    /// <summary>Represents a parameter passed to a service action, service function or a service operation when it is Executed.</summary>
    public abstract class OperationParameter
    {
        /// <summary>The name of the operation parameter.</summary>
        private readonly String parameterName;

        /// <summary>The value of the operation parameter.</summary>
        private readonly Object parameterValue;

        /// <summary>Initializes a new instance of the <see cref="Microsoft.OData.Client.OperationParameter" /> class.</summary>
        /// <param name="name">The name of the operation parameter.</param>
        /// <param name="value">The value of the operation parameter.</param>
        protected OperationParameter(string name, Object value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(Strings.Context_MissingOperationParameterName);
            }

            this.parameterName = name;
            this.parameterValue = value;
        }

        /// <summary>Gets the name of the operation parameter. </summary>
        /// <returns>The name of the operation parameter.</returns>
        public String Name
        {
            get
            {
                return this.parameterName;
            }
        }

        /// <summary>Gets the value of the operation parameter. </summary>
        /// <returns>The value of the operation parameter.</returns>
        public Object Value
        {
            get { return this.parameterValue; }
        }
    }
}
