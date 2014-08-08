//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData
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

        /// <summary>The maximum number of entity mapping attributes to be found for an entity type (on the type itself and all its base types).</summary>
        private int maxEntityPropertyMappingsPerType;

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.Data.OData.ODataMessageQuotas" /> class.</summary>
        public ODataMessageQuotas()
        {
            this.maxPartsPerBatch = ODataConstants.DefaultMaxPartsPerBatch;
            this.maxOperationsPerChangeset = ODataConstants.DefulatMaxOperationsPerChangeset;
            this.maxNestingDepth = ODataConstants.DefaultMaxRecursionDepth;
            this.maxReceivedMessageSize = ODataConstants.DefaultMaxReadMessageSize;
            this.maxEntityPropertyMappingsPerType = ODataConstants.DefaultMaxEntityPropertyMappingsPerType;
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.Data.OData.ODataMessageQuotas" /> class.</summary>
        /// <param name="other">The instance to copy.</param>
        public ODataMessageQuotas(ODataMessageQuotas other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");

            this.maxPartsPerBatch = other.maxPartsPerBatch;
            this.maxOperationsPerChangeset = other.maxOperationsPerChangeset;
            this.maxNestingDepth = other.maxNestingDepth;
            this.maxReceivedMessageSize = other.maxReceivedMessageSize;
            this.maxEntityPropertyMappingsPerType = other.maxEntityPropertyMappingsPerType;
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

        /// <summary>Gets or sets the maximum number of entity mapping attributes to be found for an entity type (on the type itself and all its base types).</summary>
        /// <returns>The maximum number of entity mapping attributes to be found for an entity type.</returns>
        public int MaxEntityPropertyMappingsPerType
        {
            get
            {
                return this.maxEntityPropertyMappingsPerType;
            }

            set
            {
                ExceptionUtils.CheckIntegerNotNegative(value, "MaxEntityPropertyMappingsPerType");
                this.maxEntityPropertyMappingsPerType = value;
            }
        }
    }
}
