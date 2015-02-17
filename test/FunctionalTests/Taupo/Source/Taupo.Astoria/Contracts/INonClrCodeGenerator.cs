//---------------------------------------------------------------------
// <copyright file="INonClrCodeGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Interface for Code Generator for Non-CLR Languages
    /// </summary>
    [ImplementationSelector("NonClrCodeGenerator", DefaultImplementation = "Php")]
    public interface INonClrCodeGenerator
    {
        /// <summary>
        /// Generates code from given Query expression
        /// </summary>
        /// <param name="expression">The query expression to generate code from</param>
        /// <returns>The generated code</returns>
        string GenerateCode(QueryExpression expression);
    }
}