//---------------------------------------------------------------------
// <copyright file="ExpectedEdmErrors.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System.Collections.Generic;
    using Microsoft.OData.Edm.Validation;
  
    /// <summary>
    /// A thin wrapper on top of List, to enable syntactic sugar like:
    ///   new ExpectedEdmErrors
    ///   {
    ///     {code1, "key1"},
    ///     {code2, "key2"},
    ///   }
    /// </summary>
    public class ExpectedEdmErrors : List<ExpectedEdmError>
    {
        public void Add(EdmErrorCode edmErrorCode, string resourceKey, params string[] arguments)
        {
            base.Add(new ExpectedEdmError(edmErrorCode, resourceKey, arguments));
        }
    }
}
