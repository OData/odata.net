//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
