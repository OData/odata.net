//---------------------------------------------------------------------
// <copyright file="JsonReaderTestConfiguration.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Json
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Test.Taupo.Contracts;
    #endregion Namespaces

    /// <summary>
    /// Configuration of a JsonReader tests.
    /// </summary>
    public class JsonReaderTestConfiguration
    {
        /// <summary>
        /// Enumeration of read sizes - number of characters to return per a single TextReader.Read call.
        /// An endless loop will be created from these sizes and used to test the reader.
        /// </summary>
        public IEnumerable<int> ReadSizes { get; set; }

        /// <summary>
        /// A function to create the JsonReader instance to use for the test run.
        /// </summary>
        public Func<TextReader, AssertionHandler, JsonReader> JsonReaderCreatorFunc { get; set; }
    }
}
