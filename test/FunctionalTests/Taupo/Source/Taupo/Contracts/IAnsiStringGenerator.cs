//---------------------------------------------------------------------
// <copyright file="IAnsiStringGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    /// <summary>
    /// Generates random ansi strings of a specified length.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Need this interface for dependency injection to inject proper implementation.")]
    public interface IAnsiStringGenerator : IStringGenerator
    {
    }
}
