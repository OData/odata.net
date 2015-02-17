//---------------------------------------------------------------------
// <copyright file="DefaultLogLevelAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;

    /// <summary>
    /// Specifies default log level for the class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DefaultLogLevelAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the DefaultLogLevelAttribute class.
        /// </summary>
        /// <param name="level">The level.</param>
        public DefaultLogLevelAttribute(LogLevel level)
        {
            this.Level = level;
        }

        /// <summary>
        /// Gets the log level.
        /// </summary>
        /// <value>The log level.</value>
        public LogLevel Level { get; private set; }
    }
}
