//---------------------------------------------------------------------
// <copyright file="ChangeOperationResponseData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents the response data of a single create, update, or delete operation.
    /// </summary>
    public sealed class ChangeOperationResponseData : OperationResponseData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeOperationResponseData"/> class.
        /// </summary>
        /// <param name="descriptorData">The descriptor data that represents tha change operation.</param>
        public ChangeOperationResponseData(DescriptorData descriptorData)
        {
            ExceptionUtilities.CheckArgumentNotNull(descriptorData, "descriptorData");

            this.DescriptorData = descriptorData;
        }

        /// <summary>
        /// Gets the descriptor data that represents tha change operation.
        /// </summary>
        /// <value>The descriptor data.</value>
        public DescriptorData DescriptorData { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{{ Status Code = {0}; Descriptor Data = {1} }}", this.StatusCode, this.DescriptorData);
        }
    }
}
