//---------------------------------------------------------------------
// <copyright file="LogLevelResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Utilities;

    /// <summary>
    /// Manages log levels for all classes in the system.
    /// </summary>
    public class LogLevelResolver : ILogLevelResolver
    {
        private const LogLevel DefaultLogLevel = LogLevel.Verbose;
        private readonly IDictionary<string, string> testParameters;

        /// <summary>
        /// Initializes a new instance of the LogLevelResolver class.
        /// </summary>
        /// <param name="testParameters">The test parameters.</param>
        public LogLevelResolver(IDictionary<string, string> testParameters)
        {
            this.testParameters = testParameters;
        }

        /// <summary>
        /// Gets the the log level for specified type.
        /// </summary>
        /// <param name="type">The type to get log level for.</param>
        /// <returns>Log level for the specified type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "LogLevel", Justification = "LogLevel is the prefix for all parameter names which control logging.")]
        public LogLevel GetLogLevelForType(Type type)
        {
            // parameters always take precedence
            for (Type t = type; t != null; t = t.GetBaseType())
            {
                string level;
                string parameterName = "LogLevel" + t.Name;

                if (this.testParameters.TryGetValue(parameterName, out level))
                {
                    LogLevel minLevel;
                    bool succeeded = Enum.TryParse<LogLevel>(level, out minLevel);

                    if (succeeded)
                    {
                        return minLevel;
                    }
                    else
                    {
                        throw new TaupoArgumentException("Invalid value of parameter '" + parameterName + "'. Must be one of { " + string.Join("; ", typeof(LogLevel).GetFields().Where(c => !c.IsSpecialName).Select(c => c.Name).ToArray()) + " }");
                    }
                }
            }

            // followed by attributes on types
            for (Type t = type; t != null; t = t.GetBaseType())
            {
                var dlla = PlatformHelper.GetCustomAttribute<DefaultLogLevelAttribute>(t);
                if (dlla != null)
                {
                    return dlla.Level;
                }
            }

            return DefaultLogLevel;
        }
    }
}
