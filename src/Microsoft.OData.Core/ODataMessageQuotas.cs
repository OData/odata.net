//---------------------------------------------------------------------
// <copyright file="ODataMessageQuotas.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
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

        /// <summary>Initializes a new instance of the <see cref="Microsoft.OData.ODataMessageQuotas" /> class.</summary>
        public ODataMessageQuotas()
        {
            this.maxPartsPerBatch = ODataConstants.DefaultMaxPartsPerBatch;
            this.maxOperationsPerChangeset = ODataConstants.DefaultMaxOperationsPerChangeset;
            this.maxNestingDepth = ODataConstants.DefaultMaxRecursionDepth;
            this.maxReceivedMessageSize = ODataConstants.DefaultMaxReadMessageSize;
        }

        /// <summary>Initializes a new instance of the <see cref="Microsoft.OData.ODataMessageQuotas" /> class.</summary>
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
