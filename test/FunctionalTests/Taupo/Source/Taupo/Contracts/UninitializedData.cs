//---------------------------------------------------------------------
// <copyright file="UninitializedData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    /// <summary>
    /// Represents an uninitialized data. This class cannot be inherited.
    /// </summary>
    public sealed class UninitializedData
    {
        private static readonly UninitializedData value = new UninitializedData();

        private UninitializedData()
        {
        }

        /// <summary>
        /// Gets the sole instance of the <see cref="UninitializedData"/> class.
        /// </summary>
        public static UninitializedData Value
        {
            get { return value; }
        }
    }
}
