//---------------------------------------------------------------------
// <copyright file="CodeDomBodyAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System.CodeDom;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Annotation representing the body of a Function as CodeDom
    /// </summary>
    public class CodeDomBodyAnnotation : Annotation
    {
        /// <summary>
        /// Gets or sets the collection of CodeStatements representing the Function body
        /// </summary>
        public IEnumerable<CodeStatement> Statements { get; set; }
    }
}
