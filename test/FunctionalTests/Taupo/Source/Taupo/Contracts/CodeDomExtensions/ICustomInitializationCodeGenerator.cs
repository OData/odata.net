//---------------------------------------------------------------------
// <copyright file="ICustomInitializationCodeGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.CodeDomExtensions
{
    using System.CodeDom;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for generating code expressions for custom initialization.
    /// </summary>
    [ImplementationSelector("CustomInitializationCodeGenerator", DefaultImplementation = "Default")]
    public interface ICustomInitializationCodeGenerator
    {
        /// <summary>
        /// Generates a code expression to initialize an object.
        /// </summary>
        /// <param name="value">The object to generate an initialization expression for.</param>
        /// <param name="initializationExpression">An initialize expression for the specified object.</param>
        /// <returns>True if initialization expression was generated, false otherwise.</returns>
        bool TryGenerateInitialization(object value, out CodeExpression initializationExpression);
    }
}
