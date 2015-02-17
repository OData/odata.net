//---------------------------------------------------------------------
// <copyright file="HelpGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Generates Taupo Help.
    /// </summary>
    public class HelpGenerator
    {
        private ILogLevelResolver logLevelResolver;

        /// <summary>
        /// Initializes a new instance of the HelpGenerator class.
        /// </summary>
        /// <param name="logLevelResolver">The log level resolver.</param>
        public HelpGenerator(ILogLevelResolver logLevelResolver)
        {
            this.logLevelResolver = logLevelResolver;
        }

        /// <summary>
        /// Gets the help text for components from the given the set of types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>Help text.</returns>
        public string GetHelpText(IEnumerable<Type> types)
        {
            List<TestParameterInfo> parameters = new List<TestParameterInfo>();
            StringBuilder sb = new StringBuilder();
            sb.Append("The following test parameters can be specified:");
            sb.AppendLine();

            sb.AppendLine();
            sb.AppendLine("====================================================");
            sb.AppendLine("Dependency options:");
            sb.AppendLine("====================================================");
            this.BuildDependenciesHelpText(types, sb, parameters);

            sb.AppendLine();
            sb.AppendLine("====================================================");
            sb.AppendLine("Test parameters:");
            sb.AppendLine("====================================================");
            this.BuildTestParametersHelpText(types, sb, parameters);

            sb.AppendLine();
            sb.AppendLine("====================================================");
            sb.AppendLine("Logging:");
            sb.AppendLine("====================================================");
            this.BuildLoggingHelpText(types, sb, parameters);

            return sb.ToString();
        }

        /// <summary>
        /// Gets the available parameters for the given list of types.
        /// </summary>
        /// <param name="types">The types to analyze.</param>
        /// <returns>
        /// Array of <see cref="TestParameterInfo"/> that describes allowed test parameters for the given list of types.
        /// </returns>
        /// <remarks>The parameters are determined by scanning given types
        /// and examining <see cref="InjectTestParameterAttribute"/>, <see cref="ImplementationSelectorAttribute"/>.
        /// </remarks>
        public TestParameterInfo[] GetAvailableParameters(IEnumerable<Type> types)
        {
            List<TestParameterInfo> parameters = new List<TestParameterInfo>();
            var sb = new StringBuilder();

            this.BuildDependenciesHelpText(types, sb, parameters);
            this.BuildTestParametersHelpText(types, sb, parameters);
            this.BuildLoggingHelpText(types, sb, parameters);

            return parameters.ToArray();
        }

        private static IndexedKeyValuePairCollection GetPossibleValues(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (type.IsSubclassOf(typeof(Enum)) || typeof(Enum).IsAssignableFrom(underlyingType))
            {
                Type nonNullableEnumType = underlyingType ?? type;
                var items = new List<KeyValuePair<string, string>>(nonNullableEnumType.GetFields().Where(f => f.IsLiteral).OrderBy(f => f.Name).Select(f => new KeyValuePair<string, string>(f.Name, f.Name)));

                if (underlyingType != null)
                {
                    items.Add(new KeyValuePair<string, string>("Null", "Null"));
                }

                return new IndexedKeyValuePairCollection(items);
            }

            if (type == typeof(bool) || underlyingType == typeof(bool))
            {
                var items = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("True", "True"),
                    new KeyValuePair<string, string>("False", "False"),
                };

                if (underlyingType != null)
                {
                    items.Add(new KeyValuePair<string, string>("Null", "Null"));
                }

                return new IndexedKeyValuePairCollection(items);
            }

            return null;
        }

        private void BuildDependenciesHelpText(IEnumerable<Type> types, StringBuilder sb, List<TestParameterInfo> parameterInfos)
        {
            var selectors = new Dictionary<Type, Dictionary<string, Type>>();
            var selectorArguments = new Dictionary<Type, ImplementationSelectorAttribute>();

            this.CalculateAttributeMap(types, selectors, selectorArguments);
            sb.AppendLine("The following parameters can be used to specify which components should be used:");
            sb.AppendLine();

            foreach (var k in selectors.OrderBy(t => selectorArguments[t.Key].TestArgumentName))
            {
                var contractType = k.Key;
                var isa = selectorArguments[contractType];
                var possibleOptions = k.Value;
                string testArgumentName = isa.TestArgumentName;

                if (possibleOptions.Count > 1 || (!isa.IsRequired && isa.DefaultImplementation == null))
                {
                    sb.AppendFormat(
                        CultureInfo.InvariantCulture,
                        "{0}:   {1}{2}{3} {4}. Possible values:",
                        testArgumentName,
                        isa.IsRequired ? " (required)" : string.Empty,
                        isa.IsGlobal ? " (global)" : string.Empty,
                        isa.IsTransient ? " (transient)" : string.Empty,
                        isa.HelpText).AppendLine();

                    var possibleValuesList = new List<KeyValuePair<string, string>>();

                    foreach (var option in possibleOptions.OrderBy(c => c.Key))
                    {
                        string helpText = null;
                        var ina = PlatformHelper.GetCustomAttribute<ImplementationNameAttribute>(option.Value);
                        if (ina != null)
                        {
                            helpText = ina.HelpText;
                        }

                        sb.AppendFormat(CultureInfo.InvariantCulture, "    {0}{1}{2}", option.Key.PadRight(20), (isa.DefaultImplementation == option.Key) ? "[*DEFAULT*] " : string.Empty, helpText);
                        sb.AppendLine();
                        possibleValuesList.Add(new KeyValuePair<string, string>(option.Key, helpText));
                    }

                    if (isa.DefaultImplementation == null)
                    {
                        sb.Append("    (the default is to use none)").AppendLine();
                    }

                    var tpi = new TestParameterInfo
                    {
                        ParameterName = testArgumentName,
                        ParameterCategory = "Dependencies",
                        HelpText = isa.HelpText,
                        DefaultValue = isa.DefaultImplementation,
                        CurrentValue = isa.DefaultImplementation,
                        PossibleValues = new IndexedKeyValuePairCollection(possibleValuesList.ToList()),
                    };
                    tpi.PossibleValues.SelectedIndex = possibleValuesList.Select(p => p.Key).ToList().IndexOf(tpi.CurrentValue);
                    parameterInfos.Add(tpi);
                }
            }
        }

        private void BuildTestParametersHelpText(IEnumerable<Type> types, StringBuilder sb, List<TestParameterInfo> parameterInfos)
        {
            sb.AppendLine("The following scalar parameters can modify the behavior of existing components:");
            sb.AppendLine();

            var parameterUsages = new Dictionary<string, TestParameterUsage>();

            foreach (Type type in types.OrderBy(t => t.Name))
            {
                foreach (PropertyInfo pi in type.GetProperties().Where(c => c.IsDefined(typeof(InjectTestParameterAttribute), false)))
                {
                    var nonObsoleteAttributes = pi.GetCustomAttributes(typeof(InjectTestParameterAttribute), false)
                        .Cast<InjectTestParameterAttribute>()
                        .Where(a => !a.IsObsolete)
                        .ToList();
                    ExceptionUtilities.Assert(nonObsoleteAttributes.Count <= 1, "At most 1 non-obsolete InjectTestParameterAttribute expected");

                    InjectTestParameterAttribute itpa = nonObsoleteAttributes.SingleOrDefault();
                    if (itpa != null)
                    {
                        TestParameterUsage parameterUsage;

                        if (parameterInfos.Any(pinfo => pinfo.ParameterName.Equals(itpa.ParameterName)))
                        {
                            continue;
                        }

                        if (!parameterUsages.TryGetValue(itpa.ParameterName, out parameterUsage))
                        {
                            parameterUsage = new TestParameterUsage() { Attribute = itpa, PropertyType = pi.PropertyType };
                            parameterUsages.Add(itpa.ParameterName, parameterUsage);
                        }

                        parameterUsage.Usages.Add(type);
                    }
                }
            }

            foreach (var kvp in parameterUsages.OrderBy(c => c.Key))
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "{0}:    {1}", kvp.Value.Attribute.ParameterName, kvp.Value.Attribute.HelpText);
                sb.AppendLine();
                sb.AppendFormat(CultureInfo.InvariantCulture, "    Default value: {0}", kvp.Value.Attribute.DefaultValueDescription);
                sb.AppendLine();
                sb.AppendFormat(CultureInfo.InvariantCulture, "    Used by: {0}", string.Join(", ", kvp.Value.Usages.Select(c => c.Name).ToArray()));
                sb.AppendLine();

                parameterInfos.Add(new TestParameterInfo()
                {
                    ParameterCategory = "Test Parameters",
                    ParameterName = kvp.Value.Attribute.ParameterName,
                    HelpText = kvp.Value.Attribute.HelpText,
                    PossibleValues = GetPossibleValues(kvp.Value.PropertyType)
                });
            }
        }

        private void BuildLoggingHelpText(IEnumerable<Type> types, StringBuilder sb, List<TestParameterInfo> parameterInfos)
        {
            sb.Append("Available log levels are:").AppendLine();
            foreach (FieldInfo fi in typeof(LogLevel).GetFields().Where(c => !c.IsSpecialName))
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "    {0}", fi.Name).AppendLine();
            }

            sb.AppendLine();
            sb.AppendLine("The following parameters can be used to override logging level for each component:");

            foreach (var type in types.Where(t => t.GetProperties().Any(p => p.PropertyType == typeof(Logger) && p.IsDefined(typeof(InjectDependencyAttribute), false))).OrderBy(t => t.Name))
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "    LogLevel{0}: Default level: {1}", type.Name, this.logLevelResolver.GetLogLevelForType(type)).AppendLine();
                parameterInfos.Add(this.CreateLoggingTestParameterInfo(type));
            }

            sb.AppendLine();
            sb.AppendLine("You can also specify log level for a base class and it will apply to all derived classes.");
            sb.AppendLine("'LogLevelObject' can be used to change the global log level.");

            parameterInfos.Add(this.CreateLoggingTestParameterInfo(typeof(object)));
        }

        private TestParameterInfo CreateLoggingTestParameterInfo(Type type)
        {
            var tpi = new TestParameterInfo()
            {   
                ParameterCategory = "Logging",
                ParameterName = "LogLevel" + type.Name,
                DefaultValue = this.logLevelResolver.GetLogLevelForType(type).ToString(),
                HelpText = "Log level for " + type.Name + " and derived classes",
                PossibleValues = new IndexedKeyValuePairCollection(this.GetLogLevelsWithDescription()),
            };

            tpi.CurrentValue = tpi.DefaultValue;
            tpi.PossibleValues.SelectedIndex = tpi.PossibleValues.Select(p => p.Key).ToList().IndexOf(tpi.CurrentValue);
            return tpi;
        }

        private IList<KeyValuePair<string, string>> GetLogLevelsWithDescription()
        {
            return typeof(LogLevel).GetFields()
                    .Where(c => !c.IsSpecialName)
                    .Select(c => new KeyValuePair<string, string>(c.Name, c.Name + " Log Level"))
                    .ToList();
        }

        private void CalculateAttributeMap(IEnumerable<Type> types, Dictionary<Type, Dictionary<string, Type>> selectors, Dictionary<Type, ImplementationSelectorAttribute> selectorArguments)
        {
            // contractType => (selector => implementationType)
            foreach (Type type in types.OrderBy(t => t.Name))
            {
                var isa = PlatformHelper.GetCustomAttribute<ImplementationSelectorAttribute>(type, false);
                if (isa != null)
                {
                    selectors[type] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
                    selectorArguments[type] = isa;
                }
            }

            foreach (Type type in types)
            {
                var isva = PlatformHelper.GetCustomAttribute<ImplementationNameAttribute>(type, false);
                if (isva != null)
                {
                    Dictionary<string, Type> possibleValues;

                    if (selectors.TryGetValue(isva.ContractType, out possibleValues))
                    {
                        possibleValues[isva.ImplementationName] = type;
                    }
                }
            }
        }

        /// <summary>
        /// A simple POCO that contains information about where a test parameter is used.
        /// </summary>
        private sealed class TestParameterUsage
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TestParameterUsage"/> class.
            /// </summary>
            public TestParameterUsage()
            {
                this.Usages = new List<Type>();
            }

            /// <summary>
            /// Gets or sets the <see cref="InjectTestParameterAttribute"/> that represents the test parameter.
            /// </summary>
            public InjectTestParameterAttribute Attribute { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="Type"/> of the test parameter.
            /// </summary>
            public Type PropertyType { get; set; }

            /// <summary>
            /// Gets a list containing all of the other CLR <see cref="Type"/>s where this test parameter should be injected.
            /// </summary>
            public List<Type> Usages { get; private set; }
        }
    }
}
