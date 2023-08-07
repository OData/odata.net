//---------------------------------------------------------------------
// <copyright file="CustomUriFunctions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;

    #endregion

    /// <summary>
    /// Class represents functions signatures of custom uri functions.
    /// </summary>
    public static class CustomUriFunctions
    {
        #region Static Fields

        /// <summary>
        /// Dictionary of the name of the custom function and all the signatures.
        /// </summary>
        private static readonly Dictionary<string, FunctionSignatureWithReturnType[]> CustomFunctions
            = new Dictionary<string, FunctionSignatureWithReturnType[]>(StringComparer.Ordinal);

        private static readonly object Locker = new object();

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a custom uri function to extend uri functions.
        /// In case the function name already exists as a custom function, the signature will be added as an another overload.
        /// </summary>
        /// <param name="functionName">The new custom function name</param>
        /// <param name="functionSignature">The new custom function signature</param>
        /// <param name="model">The provided model. If it's non-null, the custom Uri functions will be model scoped, otherwise, it's global scoped.</param>
        /// <exception cref="ArgumentNullException">Arguments are null, or function signature return type is null</exception>
        /// <exception cref="ODataException">Throws if built-in function name already exists.</exception>
        /// <exception cref="ODataException">Throws if built-in function signature overload already exists.</exception>
        /// <exception cref="ODataException">Throws if custom function signature overload already exists</exception>
        public static void AddCustomUriFunction(string functionName, FunctionSignatureWithReturnType functionSignature, IEdmModel model = null)
        {
            // Parameters validation
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(functionName, "functionName");
            ExceptionUtils.CheckArgumentNotNull(functionSignature, "functionSignature");

            ValidateFunctionWithReturnType(functionSignature);

            // Thread safety - before using the custom functions dictionary
            lock (Locker)
            {
                // Check if the function does already exists in the Built-In functions
                // If 'addAsOverloadToBuiltInFunction' parameter is false - throw expectation
                // Else, add as a custom function
                FunctionSignatureWithReturnType[] existingBuiltInFunctionOverload;
                if (BuiltInUriFunctions.TryGetBuiltInFunction(functionName, out existingBuiltInFunctionOverload))
                {
                    // Function name exists, check if full signature exists among the overloads.
                    if (existingBuiltInFunctionOverload.Any(builtInFunction =>
                            AreFunctionsSignatureEqual(functionSignature, builtInFunction)))
                    {
                        throw new ODataException(Strings.CustomUriFunctions_AddCustomUriFunction_BuiltInExistsFullSignature(functionName));
                    }
                }

                AddCustomFunction(functionName, functionSignature, model);
            }
        }


        /// <summary>
        /// Removes the specific function overload from the custom uri functions.
        /// </summary>
        /// <param name="functionName">Custom function name to remove</param>
        /// <param name="functionSignature">The specific signature overload of the function to remove</param>
        /// <param name="model">The provided model. If it's non-null, the custom Uri functions will be model scoped, otherwise, it's global scoped.</param>
        /// <returns>'False' if custom function signature doesn't exist. 'True' if function has been removed successfully</returns>
        /// <exception cref="ArgumentNullException">Arguments are null, or function signature return type is null</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("ApiDesign", "RS0026:Do not add multiple public overloads with optional parameters", Justification = "<Pending>")]
        public static bool RemoveCustomUriFunction(string functionName, FunctionSignatureWithReturnType functionSignature, IEdmModel model = null)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(functionName, "functionName");
            ExceptionUtils.CheckArgumentNotNull(functionSignature, "functionSignature");

            ValidateFunctionWithReturnType(functionSignature);

            lock (Locker)
            {
                Dictionary<string, FunctionSignatureWithReturnType[]> functionsDict = GetFunctionsDict(model);

                FunctionSignatureWithReturnType[] existingCustomFunctionOverloads;
                if (!functionsDict.TryGetValue(functionName, out existingCustomFunctionOverloads))
                {
                    return false;
                }

                // Get all function sigature overloads without the overload which is requested to be removed
                FunctionSignatureWithReturnType[] customFunctionOverloadsWithoutTheOneToRemove =
                    existingCustomFunctionOverloads.SkipWhile(funcOverload => AreFunctionsSignatureEqual(funcOverload, functionSignature)).ToArray();

                // Nothing was removed - Requested overload doesn't exist
                if (customFunctionOverloadsWithoutTheOneToRemove.Length == existingCustomFunctionOverloads.Length)
                {
                    return false;
                }

                // No overloads have left in this function name. Delete the function name
                if (customFunctionOverloadsWithoutTheOneToRemove.Length == 0)
                {
                    return functionsDict.Remove(functionName);
                }
                else
                {
                    // Requested overload has been removed.
                    // Update the custom functions to the overloads without that one requested to be removed
                    functionsDict[functionName] = customFunctionOverloadsWithoutTheOneToRemove;
                    return true;
                }
            }
        }


        /// <summary>
        /// Removes all the function overloads from the custom uri functions.
        /// </summary>
        /// <param name="functionName">The custom function name</param>
        /// <param name="model">The provided model. If it's non-null, the custom Uri functions will be model scoped, otherwise, it's global scoped.</param>
        /// <returns>'False' if custom function signature doesn't exist. 'True' if function has been removed successfully</returns>
        /// <exception cref="ArgumentNullException">Arguments are null, or function signature return type is null</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("ApiDesign", "RS0026:Do not add multiple public overloads with optional parameters", Justification = "<Pending>")]
        public static bool RemoveCustomUriFunction(string functionName, IEdmModel model = null)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(functionName, "functionName");

            lock (Locker)
            {
                Dictionary<string, FunctionSignatureWithReturnType[]> functionsDict = GetFunctionsDict(model);
                return functionsDict.Remove(functionName);
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Returns a list of name-signature pairs for a function name.
        /// </summary>
        /// <param name="functionCallToken">The name of the function to look for.</param>
        /// <param name="nameSignatures">
        /// Output for the list of signature objects for matched function names, with canonical name of the function;
        /// null if no matches found.
        /// </param>
        /// <param name="enableCaseInsensitive">Whether to perform case-insensitive match for function name.</param>
        /// <param name="model">The provided model. If it's non-null, the custom Uri functions will be model scoped, otherwise, it's global scoped.</param>
        /// <returns>true if the function was found, or false otherwise.</returns>
        internal static bool TryGetCustomFunction(string functionCallToken, out IList<KeyValuePair<string, FunctionSignatureWithReturnType>> nameSignatures,
            bool enableCaseInsensitive = false, IEdmModel model = null)
        {
            Debug.Assert(functionCallToken != null, "name != null");

            lock (Locker)
            {
                IList<KeyValuePair<string, FunctionSignatureWithReturnType>> bufferedKeyValuePairs
                    = new List<KeyValuePair<string, FunctionSignatureWithReturnType>>();

                Dictionary<string, FunctionSignatureWithReturnType[]> functionsDict = GetFunctionsDict(model);

                foreach (KeyValuePair<string, FunctionSignatureWithReturnType[]> func in functionsDict)
                {
                    if (func.Key.Equals(functionCallToken, enableCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                    {
                        foreach (FunctionSignatureWithReturnType sig in func.Value)
                        {
                            bufferedKeyValuePairs.Add(new KeyValuePair<string, FunctionSignatureWithReturnType>(func.Key, sig));
                        }
                    }
                }

                // Setup the output values.
                nameSignatures = bufferedKeyValuePairs.Count != 0 ? bufferedKeyValuePairs : null;

                return nameSignatures != null;
            }
        }

        #endregion

        #region Private Methods

        private static void AddCustomFunction(string customFunctionName, FunctionSignatureWithReturnType newCustomFunctionSignature, IEdmModel model)
        {
            FunctionSignatureWithReturnType[] existingCustomFunctionOverloads;

            Dictionary<string, FunctionSignatureWithReturnType[]> functionsDict = GetFunctionsDict(model);

            // In case the function doesn't already exist
            if (!functionsDict.TryGetValue(customFunctionName, out existingCustomFunctionOverloads))
            {
                functionsDict.Add(customFunctionName, new FunctionSignatureWithReturnType[] { newCustomFunctionSignature });
            }
            else
            {
                // Function does already exist as a custom function in cache
                // Check if the function overload doesn't already exist
                bool isOverloadAlreadyExist =
                    existingCustomFunctionOverloads.Any(existingFunction => AreFunctionsSignatureEqual(existingFunction, newCustomFunctionSignature));

                if (isOverloadAlreadyExist)
                {
                    // Throw if already exists - User is stupid (inserted the same function twice)
                    throw new ODataException(Strings.CustomUriFunctions_AddCustomUriFunction_CustomFunctionOverloadExists(customFunctionName));
                }

                // Add the custom function as an overload to the same function name
                functionsDict[customFunctionName] =
                    existingCustomFunctionOverloads.Concat(new FunctionSignatureWithReturnType[] { newCustomFunctionSignature }).ToArray();
            }
        }

        private static bool AreFunctionsSignatureEqual(FunctionSignatureWithReturnType functionOne, FunctionSignatureWithReturnType functionTwo)
        {
            Debug.Assert(functionOne != null, "functionOne != null");
            Debug.Assert(functionTwo != null, "functionTwo != null");

            // Check if ReturnTypes are equal
            if (!functionOne.ReturnType.IsEquivalentTo(functionTwo.ReturnType))
            {
                return false;
            }

            // Check the length of the Arguments of the two functions
            if (functionOne.ArgumentTypes.Length != functionTwo.ArgumentTypes.Length)
            {
                return false;
            }

            // Check if the arguments are equal
            for (int argumentIndex = 0; argumentIndex < functionOne.ArgumentTypes.Length; argumentIndex++)
            {
                if (!functionOne.ArgumentTypes[argumentIndex].IsEquivalentTo(functionTwo.ArgumentTypes[argumentIndex]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if FunctionSignatureWithReturnType is valid.
        /// Valid if the signature has a ReturnType
        /// </summary>
        /// <param name="functionSignature">Function signature to validate</param>
        private static void ValidateFunctionWithReturnType(FunctionSignatureWithReturnType functionSignature)
        {
            if (functionSignature == null)
            {
                return;
            }

            ExceptionUtils.CheckArgumentNotNull(functionSignature.ReturnType, "functionSignatureWithReturnType must contain a return type");
        }

        private static Dictionary<string, FunctionSignatureWithReturnType[]> GetFunctionsDict(IEdmModel model)
        {
            return model == null ? CustomFunctions : GetCustomUriFunctionsContainer(model).CustomFunctions;
        }

        #endregion

        // The below should be private, set it as internal for unit test only.
        internal static CustomUriFunctionsContainer GetCustomUriFunctionsContainer(IEdmModel model)
        {
            CustomUriFunctionsContainer container = model.GetAnnotationValue<CustomUriFunctionsContainer>(model);
            if (container == null)
            {
                container = new CustomUriFunctionsContainer();
                model.SetAnnotationValue(model, container);
            }

            return container;
        }

        internal class CustomUriFunctionsContainer
        {
            public Dictionary<string, FunctionSignatureWithReturnType[]> CustomFunctions { get; }
                = new Dictionary<string, FunctionSignatureWithReturnType[]>(StringComparer.Ordinal);
        }
    }
}
