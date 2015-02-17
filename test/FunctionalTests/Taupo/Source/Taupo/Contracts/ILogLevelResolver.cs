//---------------------------------------------------------------------
// <copyright file="ILogLevelResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Manages log levels for all classes in the system.
    /// </summary>
    public interface ILogLevelResolver
    {
        /// <summary>
        /// Gets the log level for a given type.
        /// </summary>
        /// <param name="type">The type to compute log level for.</param>
        /// <returns>Log level for that type.</returns>
        LogLevel GetLogLevelForType(Type type);
    }
}
