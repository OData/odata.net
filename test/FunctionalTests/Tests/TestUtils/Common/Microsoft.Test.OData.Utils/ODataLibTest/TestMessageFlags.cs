//---------------------------------------------------------------------
// <copyright file="TestMessageFlags.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.ODataLibTest
{
    using System;

    /// <summary>
    /// Flags indicating how a test message should behave; this is used to specify faulty behaviors
    /// </summary>
    [Flags]
    public enum TestMessageFlags
    {
        /// <summary>
        /// No message flags were specified, the default behavior will be used.
        /// </summary>
        None = 0,

        /// <summary>
        /// The GetStream or GetStreamAsync methods should fail.
        /// </summary>
        GetStreamFailure = 1,

        /// <summary>
        /// The message should fail if synchronous operation is attempted.
        /// </summary>
        NoSynchronous = 2,

        /// <summary>
        /// The message should fail is asynchronous operation is attempted.
        /// </summary>
        NoAsynchronous = 4
    }
}
