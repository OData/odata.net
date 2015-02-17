//---------------------------------------------------------------------
// <copyright file="ExpectedEdmError.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm.Validation;

    /// <summary>
    /// Represents the expected EdmError
    /// </summary>
    public class ExpectedEdmError
    {
        // TODO: add more interesting fields and potentially merge with the one used in semantic validation
        public ExpectedEdmError(EdmErrorCode errorCode, string messageResourceKey, params string[] messageArguments)
        {
            this.ErrorCode = errorCode;
            this.MessageResourceKey = messageResourceKey;
            this.MessageArguments = messageArguments.ToList();
        }

        public EdmErrorCode ErrorCode { get; private set; }

        public string MessageResourceKey { get; private set; }

        public IList<string> MessageArguments { get; private set; }
    }
}
