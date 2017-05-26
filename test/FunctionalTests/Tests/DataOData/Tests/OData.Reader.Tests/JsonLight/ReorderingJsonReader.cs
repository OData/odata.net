//---------------------------------------------------------------------
// <copyright file="ReorderingJsonReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    #region Namespaces
    using System;
    using System.IO;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Json;
    #endregion Namespaces

    /// <summary>
    /// The test wrapper of the ReorderingJsonReader implementation in the product which is internal.
    /// </summary>
    public class ReorderingJsonReader : BufferingJsonReader
    {
        /// <summary>
        /// The type of the ReorderingJsonReader from the product.
        private static Type ReorderingJsonReaderType = typeof(Microsoft.OData.ODataAnnotatable).Assembly.GetType("Microsoft.OData.JsonLight.ReorderingJsonReader");

        /// <summary>
        /// Creates new instance of the reordering Json reader.
        /// </summary>
        /// <param name="textReader">The text reader to read the input from.</param>
        /// <param name="maxInnerErrorDepth">The maximumum number of recursive internalexception objects to allow when reading in-stream errors.</param>
        /// <param name="assert">Optional assertion handler to use to verify the behavior of the reader.</param>
        /// <param name="isIeee754Compatible">If it is IEEE754Compatible</param>
        public ReorderingJsonReader(TextReader textReader, int maxInnerErrorDepth, AssertionHandler assert, bool isIeee754Compatible)
            : base(ReflectionUtils.CreateInstance(ReorderingJsonReaderType, textReader, maxInnerErrorDepth, isIeee754Compatible), assert)
        {
        }
   }
}
