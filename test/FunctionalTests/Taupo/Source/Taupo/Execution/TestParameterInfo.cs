//---------------------------------------------------------------------
// <copyright file="TestParameterInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Test parameter information determined using reflection.
    /// </summary>
    [Serializable]
    public class TestParameterInfo
    {
        private static List<string> possibleBooleanValues = new List<string>() { "false", "true" };

        /// <summary>
        /// Gets the parameter category.
        /// </summary>
        public string ParameterCategory { get; internal set; }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        /// <value>The name of the parameter.</value>
        public string ParameterName { get; internal set; }

        /// <summary>
        /// Gets the help text that explains parameter usage.
        /// </summary>
        /// <value>The help text.</value>
        public string HelpText { get; internal set; }

        /// <summary>
        /// Gets the default value of the parameter.
        /// </summary>
        /// <value>The default value.</value>
        public string DefaultValue { get; internal set; }

        /// <summary>
        /// Gets or sets the Current Value of the parameter
        /// </summary>
        public string CurrentValue { get; set; }

        /// <summary>
        /// Gets the list of possible values for the parameter along with corresponding help texts.
        /// </summary>
        public IndexedKeyValuePairCollection PossibleValues { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this instance is a boolean parameter.
        /// </summary>
        public bool IsBooleanParameter
        {
            get
            {
                return this.PossibleValues != null &&
                    this.PossibleValues.Count == 2 &&
                    this.PossibleValues.All(kvp => possibleBooleanValues.Contains(kvp.Key, StringComparer.OrdinalIgnoreCase));
            }
        }
    }
}
