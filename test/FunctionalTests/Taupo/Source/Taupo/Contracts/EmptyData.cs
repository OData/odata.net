//---------------------------------------------------------------------
// <copyright file="EmptyData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    /// <summary>
    /// Represents an empty collection value. This class cannot be inherited.
    /// </summary>
    public sealed class EmptyData
    {
        private static readonly EmptyData value = new EmptyData();

        private EmptyData()
        {
        }

        /// <summary>
        /// Gets the sole instance of the <see cref="EmptyData"/> class.
        /// </summary>
        public static EmptyData Value
        {
            get { return value; }
        }
    }
}
