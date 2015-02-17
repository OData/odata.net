//---------------------------------------------------------------------
// <copyright file="ImplementationSelector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Computes implementations for all contracts marked with <see cref="ImplementationSelectorAttribute"/>.
    /// </summary>
    public class ImplementationSelector : IImplementationSelector
    {
        private ICollection<Type> typeClosure = new List<Type>();

        /// <summary>
        /// Initializes a new instance of the ImplementationSelector class.
        /// </summary>
        public ImplementationSelector()
        {
            this.Logger = Logger.Null;
        }

        /// <summary>
        /// Gets or sets the logger used to print diagnostic information.
        /// </summary>
        /// <value>The logger.</value>
        [InjectDependency]
        public Logger Logger { get; set; }

        /// <summary>
        /// Gets the registered types.
        /// </summary>
        public ReadOnlyCollection<Type> Types
        {
            get { return this.typeClosure.ToList().AsReadOnly(); }
        }

        /// <summary>
        /// Adds all types from the assembly to the list of types which should be taken into account when computing default implementation.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public void AddAssembly(Assembly assembly)
        {
            this.AddTypes(assembly.GetExportedTypes());
        }

        /// <summary>
        /// Adds the specified types to the list of types which should be taken into account when computing default implementation.
        /// </summary>
        /// <param name="types">Types to add</param>
        public void AddTypes(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                if (!this.typeClosure.Contains(type))
                {
                    this.typeClosure.Add(type);
                }
            }
        }

        /// <summary>
        /// Gets the implementations for all contracts marked with <see cref="ImplementationSelectorAttribute"/>.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>
        /// Collection of key-value pair where key is the contract type and value is the implementation type for all
        /// contracts marked with <see cref="ImplementationSelectorAttribute"/> in all loaded assemblies.
        /// </returns>
        public IEnumerable<ImplementationInfo> GetImplementations(IDictionary<string, string> arguments)
        {
            ExceptionUtilities.CheckArgumentNotNull(arguments, "arguments");

            // output the test parameters to make debugging lab runs easier
            if (arguments.Count > 0)
            {
                var argumentStrings = arguments.Select(arg => string.Format(CultureInfo.InvariantCulture, "/{0}:{1}", arg.Key, arg.Value)).ToArray();

                this.Logger.WriteLine(LogLevel.Verbose, "Test parameters:");
                foreach (var argument in argumentStrings)
                {
                    this.Logger.WriteLine(LogLevel.Verbose, "    " + argument);
                }

                this.Logger.WriteLine(LogLevel.Verbose, string.Empty);
                this.Logger.WriteLine(LogLevel.Verbose, "Re-constructed command line:");
                this.Logger.WriteLine(LogLevel.Verbose, "    " + string.Join(" ", argumentStrings));
                this.Logger.WriteLine(LogLevel.Verbose, string.Empty);
            }

            List<ImplementationInfo> result = new List<ImplementationInfo>();

            // selectors: contractType => (implementationName => implementationType)
            // selectorArguments: contractType => ImplementationSelectorAttibute
            var selectors = new Dictionary<Type, Dictionary<string, Type>>();
            var selectorArguments = new Dictionary<Type, ImplementationSelectorAttribute>();

            this.CalculateAttributeMap(selectors, selectorArguments);

            var messagesForNonDefaultImplementations = new List<string>();
            var messagesForDefaultImplementations = new List<string>();
            
            foreach (var k in selectors)
            {
                var contractType = k.Key;
                var isa = selectorArguments[contractType];
                var possibleOptions = k.Value;
                string testArgumentName = isa.TestArgumentName;

                bool isTestParameterSpecified = true;
                string parameterValue;
                if (!arguments.TryGetValue(testArgumentName, out parameterValue))
                {
                    if (isa.IsRequired)
                    {
                        throw new TaupoInvalidOperationException("Required parameter " + testArgumentName + " was not specified.");
                    }

                    isTestParameterSpecified = false;
                    parameterValue = isa.DefaultImplementation;
                }

                if (parameterValue != null)
                {
                    Type implementationType;

                    if (possibleOptions.TryGetValue(parameterValue, out implementationType))
                    {
                        // seperate out the messages for non-default implementations to make debugging lab runs easier
                        string message = string.Format(CultureInfo.InvariantCulture, "    {0} ('{1}') => {2} ('{3}'){4}", contractType.Name, testArgumentName, implementationType.Name, parameterValue, isa.IsTransient ? " (transient)" : string.Empty);
                        if (!isTestParameterSpecified)
                        {
                            messagesForDefaultImplementations.Add(message);
                        }
                        else
                        {
                            messagesForNonDefaultImplementations.Add(message);
                        }
                        
                        result.Add(new ImplementationInfo(contractType, implementationType, isa.IsTransient, isa.IsGlobal, isTestParameterSpecified, isa.DefaultImplementation == parameterValue));
                    }
                    else if (parameterValue != isa.DefaultImplementation)
                    {
                        throw new TaupoInfrastructureException(
                            "Invalid value for '" + isa.TestArgumentName + "' parameter (implementation selector for " + contractType.FullName + "). Possible values are: " + Environment.NewLine + Environment.NewLine
                            + string.Join(Environment.NewLine, possibleOptions.Select(c => "'" + c.Key + "' (" + c.Value.FullName + ")").ToArray()));
                    }
                }
            }

            if (messagesForNonDefaultImplementations.Any())
            {
                this.Logger.WriteLine(LogLevel.Verbose, "Implementations selected based on test parameters above:");
                messagesForNonDefaultImplementations.ForEach(m => this.Logger.WriteLine(LogLevel.Verbose, m));
                this.Logger.WriteLine(LogLevel.Verbose, string.Empty);
            }

            if (messagesForDefaultImplementations.Any())
            {
                this.Logger.WriteLine(LogLevel.Verbose, "Default implementations:");
                messagesForDefaultImplementations.ForEach(m => this.Logger.WriteLine(LogLevel.Verbose, m));
                this.Logger.WriteLine(LogLevel.Verbose, string.Empty);
            }

            return result;
        }

        private void CalculateAttributeMap(Dictionary<Type, Dictionary<string, Type>> selectors, Dictionary<Type, ImplementationSelectorAttribute> selectorArguments)
        {
            // selectors: contractType => (implementationName => implementationType)
            // selectorArguments: contractType => ImplementationSelectorAttibute
            foreach (Type type in this.typeClosure)
            {
                var isa = PlatformHelper.GetCustomAttribute<ImplementationSelectorAttribute>(type, false);
                if (isa != null)
                {
                    selectors[type] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
                    selectorArguments[type] = isa;
                }
            }

            foreach (Type type in this.typeClosure)
            {
                var isva = PlatformHelper.GetCustomAttribute<ImplementationNameAttribute>(type, false);
                if (isva != null)
                {
                    Dictionary<string, Type> possibleValues;

                    if (selectors.TryGetValue(isva.ContractType, out possibleValues))
                    {
                        if (possibleValues.ContainsKey(isva.ImplementationName))
                        {
                            this.Logger.WriteLine(LogLevel.Info, "Found multiple implementations for contract type '{0}' and implementation name '{1}'.", isva.ContractType, isva.ImplementationName);
                            this.Logger.WriteLine(LogLevel.Info, "Replacing implementation '{0}' with implementation '{1}'", possibleValues[isva.ImplementationName], type);
                        }

                        possibleValues[isva.ImplementationName] = type;
                    }
                }
            }
        }
    }
}
