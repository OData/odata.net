//---------------------------------------------------------------------
// <copyright file="TestConfig.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    using System;
    using System.Collections;

    /// <summary>
    /// The test config class
    /// </summary>
    public static class TestConfig
    {
        /// <summary>
        /// The locker for exchange the context properties
        /// </summary>
        private static readonly object LoadContextLocker = new object();

        /// <summary>
        /// The dictionary to overwrite the setting
        /// </summary>
        private static IDictionary contextProperties;

        /// <summary>
        /// The event for config changed
        /// </summary>
        public static event Action ConfigChanged;

        /// <summary>
        /// Load the new context properties if context is changed.
        /// </summary>
        /// <param name="properties">The properties</param>
        public static void LoadContextParameter(IDictionary properties)
        {
            lock (LoadContextLocker)
            {
                if (IsPropertiesChanged(properties))
                {
                    contextProperties = properties;
                    if (ConfigChanged != null)
                    {
                        ConfigChanged();
                    }
                }
            }
        }

        /// <summary>
        /// 1. Try to override with environment variable.
        /// 2. Try to override with test context properties.
        /// 3. Default value
        /// </summary>
        /// <param name="settingName">Setting name</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>The setting values</returns>
        public static string GetSetting(string settingName, string defaultValue)
        {
            string ret = null;
            string var = Environment.GetEnvironmentVariable(settingName);
            if (var != null)
            {
                ret = Environment.ExpandEnvironmentVariables(var);
            }

            if (var == null && contextProperties != null && contextProperties.Contains(settingName))
            {
                ret = contextProperties[settingName].ToString();
            }

            return ret ?? defaultValue;
        }

        /// <summary>
        /// If the properties are changed from the current _contextProperties
        /// </summary>
        /// <param name="properties">The properties</param>
        /// <returns>If the properties changed</returns>
        private static bool IsPropertiesChanged(IDictionary properties)
        {
            if (contextProperties == null)
            {
                return true;
            }

            if (contextProperties.Count != properties.Count)
            {
                return true;
            }

            foreach (DictionaryEntry entry in properties)
            {
                // Skip the ref type expect it is string
                if (entry.Value != null && !entry.Value.GetType().IsValueType && !(entry.Value is string))
                {
                    continue;
                }

                if (!contextProperties.Contains(entry.Key))
                {
                    return true;
                }

                if (!Equals(entry.Value, contextProperties[entry.Key]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
