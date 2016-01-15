//---------------------------------------------------------------------
// <copyright file="CustomUriFunctions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
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
        /// <param name="customFunctionName">The new custom function name</param>
        /// <param name="newCustomFunctionSignature">The new custom function signature</param>
        /// <exception cref="ArgumentNullException">Arguments are null, or function signature return type is null</exception>
        /// <exception cref="ODataException">Throws if built-in function name already exists.</exception>
        /// <exception cref="ODataException">Throws if built-in function signature overload already exists.</exception>
        /// <exception cref="ODataException">Throws if custom function signature overload already exists</exception>
        public static void AddCustomUriFunction(string customFunctionName, FunctionSignatureWithReturnType newCustomFunctionSignature)
        {
            AddCustomUriFunction(customFunctionName, newCustomFunctionSignature, false);
        }

        /// <summary>
        /// Add a custom uri function to extend or override the built-in OData protocol of uri functions.
        /// In case the function signature already exists as a built-in function, if requested (addAsOverloadToBuiltInFunction = true), the new function signature will be added as another overload.
        /// In case the function name already exists as a custom function, the signature will be added as an another overload.
        /// </summary>
        /// <param name="customFunctionName">The new custom function name</param>
        /// <param name="newCustomFunctionSignature">The new custom function signature</param>
        /// <param name="addAsOverloadToBuiltInFunction">If 'True', add as another overload to the existing built-in function in case signature already exists</param>
        /// <exception cref="ArgumentNullException">Arguments are null, or function signature return type is null</exception>
        /// <exception cref="ODataException">Throws if built-in function name already exists, and parameter 'addAsOverloadToBuiltInFunction' is not 'True'</exception>
        /// <exception cref="ODataException">Throws if built-in function signature overload already exists.</exception>
        /// <exception cref="ODataException">Throws if custom function signature overload already exists</exception>
        public static void AddCustomUriFunction(string customFunctionName, FunctionSignatureWithReturnType newCustomFunctionSignature, bool addAsOverloadToBuiltInFunction)
        {
            // Parameters validation
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(customFunctionName, "customFunctionName");
            ExceptionUtils.CheckArgumentNotNull(newCustomFunctionSignature, "newCustomFunctionSignature");

            ValidateFunctionWithReturnType(newCustomFunctionSignature);

            // Thread saftey - before using the custom functions dictionary
            lock (Locker)
            {
                // Check if the function does already exists in the Built-In functions
                // If 'addAsOverloadToBuiltInFunction' parameter is false - throw expection
                // Else, add as a custom function
                FunctionSignatureWithReturnType[] existingBuiltInFunctionOverload;
                if (BuiltInUriFunctions.TryGetBuiltInFunction(customFunctionName, out existingBuiltInFunctionOverload))
                {
                    // Built-In function with the same signature already exists, and will not be added as an another overload by user request.
                    if (!addAsOverloadToBuiltInFunction)
                    {
                        throw new ODataException(Strings.CustomUriFunctions_AddCustomUriFunction_BuiltInExistsNotAddingAsOverload(customFunctionName));
                    }

                    // Function name exists, check if full siganture exists among the overloads.
                    if (existingBuiltInFunctionOverload.Any(builtInFunction =>
                            AreFunctionsSignatureEqual(newCustomFunctionSignature, builtInFunction)))
                    {
                        throw new ODataException(Strings.CustomUriFunctions_AddCustomUriFunction_BuiltInExistsFullSignature(customFunctionName));
                    }
                }

                AddCustomFunction(customFunctionName, newCustomFunctionSignature);
            }
        }

        /// <summary>
        /// Removes the specific function overload from the custum uri functions.
        /// </summary>
        /// <param name="customFunctionName">Custom function name to remove</param>
        /// <param name="customFunctionSignature">The specific signature overload of the function to remove</param>
        /// <returns>'False' if custom function signature doesn't exist. 'True' if function has been removed successfully</returns>
        /// <exception cref="ArgumentNullException">Arguments are null, or function signature return type is null</exception>
        public static bool RemoveCustomUriFunction(string customFunctionName, FunctionSignatureWithReturnType customFunctionSignature)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(customFunctionName, "customFunctionName");
            ExceptionUtils.CheckArgumentNotNull(customFunctionSignature, "customFunctionSignature");

            ValidateFunctionWithReturnType(customFunctionSignature);

            lock (Locker)
            {
                FunctionSignatureWithReturnType[] existingCustomFunctionOverloads;
                if (!CustomFunctions.TryGetValue(customFunctionName, out existingCustomFunctionOverloads))
                {
                    return false;
                }

                // Get all function sigature overloads without the overload which is requested to be removed
                FunctionSignatureWithReturnType[] customFunctionOverloadsWithoutTheOneToRemove =
                    existingCustomFunctionOverloads.SkipWhile(funcOverload => AreFunctionsSignatureEqual(funcOverload, customFunctionSignature)).ToArray();

                // Nothing was removed - Requested overload doesn't exist
                if (customFunctionOverloadsWithoutTheOneToRemove.Length == existingCustomFunctionOverloads.Length)
                {
                    return false;
                }

                // No overloads have left in this function name. Delete the function name
                if (customFunctionOverloadsWithoutTheOneToRemove.Length == 0)
                {
                    return CustomFunctions.Remove(customFunctionName);
                }
                else
                {
                    // Requested overload has been removed.
                    // Update the custom functions to the overloads wihtout that one requested to be removed
                    CustomFunctions[customFunctionName] = customFunctionOverloadsWithoutTheOneToRemove;
                    return true;
                }
            }
        }

        /// <summary>
        /// Removes all the function overloads from the custom uri functions.
        /// </summary>
        /// <param name="customFunctionName">The custom function name</param>
        /// <returns>'False' if custom function signature doesn't exist. 'True' if function has been removed successfully</returns>
        /// <exception cref="ArgumentNullException">Arguments are null, or function signature return type is null</exception>
        public static bool RemoveCustomUriFunction(string customFunctionName)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(customFunctionName, "customFunctionName");

            lock (Locker)
            {
                return CustomFunctions.Remove(customFunctionName);
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Returns a list of signatures for a function name.
        /// </summary>
        /// <param name="name">The name of the function to look for.</param>
        /// <param name="signatures">The list of signatures available for the function name.</param>
        /// <returns>true if the function was found, or false otherwise.</returns>
        internal static bool TryGetCustomFunction(string name, out FunctionSignatureWithReturnType[] signatures)
        {
            Debug.Assert(name != null, "name != null");

            lock (Locker)
            {
                return CustomFunctions.TryGetValue(name, out signatures);
            }
        }

        #endregion

        #region Private Methods

        private static void AddCustomFunction(string customFunctionName, FunctionSignatureWithReturnType newCustomFunctionSignature)
        {
            FunctionSignatureWithReturnType[] existingCustomFunctionOverloads;

            // In case the function doesn't already exist
            if (!CustomFunctions.TryGetValue(customFunctionName, out existingCustomFunctionOverloads))
            {
                CustomFunctions.Add(customFunctionName, new FunctionSignatureWithReturnType[] { newCustomFunctionSignature });
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
                CustomFunctions[customFunctionName] = 
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
        /// Vaild if the signature has a ReturnType
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

        #endregion
    }
}
