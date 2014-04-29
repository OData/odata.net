//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces

    #endregion Namespaces

    /// <summary>Quotas to use for limiting resource consumption when reading or writing OData messages.</summary>
    public sealed class ODataMessageQuotas
    {
        /// <summary>The maximum number of top level query operations and changesets allowed in a single batch.</summary>
        private int maxPartsPerBatch;

        /// <summary>The maximum number of operations allowed in a single changeset.</summary>
        private int maxOperationsPerChangeset;

        /// <summary>The maximum depth of nesting allowed when reading or writing recursive payloads.</summary>
        private int maxNestingDepth;

        /// <summary>The maximum number of bytes that should be read from the message.</summary>
        private long maxReceivedMessageSize;

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Core.ODataMessageQuotas" /> class.</summary>
        public ODataMessageQuotas()
        {
            this.maxPartsPerBatch = ODataConstants.DefaultMaxPartsPerBatch;
            this.maxOperationsPerChangeset = ODataConstants.DefulatMaxOperationsPerChangeset;
            this.maxNestingDepth = ODataConstants.DefaultMaxRecursionDepth;
            this.maxReceivedMessageSize = ODataConstants.DefaultMaxReadMessageSize;
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Core.ODataMessageQuotas" /> class.</summary>
        /// <param name="other">The instance to copy.</param>
        public ODataMessageQuotas(ODataMessageQuotas other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");

            this.maxPartsPerBatch = other.maxPartsPerBatch;
            this.maxOperationsPerChangeset = other.maxOperationsPerChangeset;
            this.maxNestingDepth = other.maxNestingDepth;
            this.maxReceivedMessageSize = other.maxReceivedMessageSize;
        }

        /// <summary>Gets or sets the maximum number of top level query operations and changesets allowed in a single batch.</summary>
        /// <returns>The maximum number of top level query operations and changesets allowed in a single batch.</returns>
        public int MaxPartsPerBatch
        {
            get
            {
                return this.maxPartsPerBatch;
            }

            set
            {
                ExceptionUtils.CheckIntegerNotNegative(value, "MaxPartsPerBatch");
                this.maxPartsPerBatch = value;
            }
        }

        /// <summary>Gets or sets the maximum number of operations allowed in a single changeset.</summary>
        /// <returns>The maximum number of operations allowed in a single changeset.</returns>
        public int MaxOperationsPerChangeset
        {
            get
            {
                return this.maxOperationsPerChangeset;
            }

            set
            {
                ExceptionUtils.CheckIntegerNotNegative(value, "MaxOperationsPerChangeset");
                this.maxOperationsPerChangeset = value;
            }
        }

        /// <summary>Gets or sets the maximum depth of nesting allowed when reading or writing recursive payloads.</summary>
        /// <returns>The maximum depth of nesting allowed when reading or writing recursive payloads.</returns>
        public int MaxNestingDepth
        {
            get
            {
                return this.maxNestingDepth;
            }

            set
            {
                ExceptionUtils.CheckIntegerPositive(value, "MaxNestingDepth");
                this.maxNestingDepth = value;
            }
        }

        /// <summary>Gets or sets the maximum number of bytes that should be read from the message.</summary>
        /// <returns>The maximum number of bytes that should be read from the message.</returns>
        public long MaxReceivedMessageSize
        {
            get
            {
                return this.maxReceivedMessageSize;
            }

            set
            {
                ExceptionUtils.CheckLongPositive(value, "MaxReceivedMessageSize");
                this.maxReceivedMessageSize = value;
            }
        }
    }
}
