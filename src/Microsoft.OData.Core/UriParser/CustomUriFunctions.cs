//---------------------------------------------------------------------
// <copyright file="CustomUriFunctions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Class represents functions signatures of custom uri functions.
    /// </summary>
    public static class CustomUriFunctions
    {
        #region Public Methods

        /// <summary>
        /// Adds a custom URI function to the Edm model with the specified name and signature.
        /// Throws an exception if a built-in function or an existing custom function with the same signature already exists.
        /// </summary>
        /// <param name="model">The Edm model to which the custom function will be added.</param>
        /// <param name="functionName">The name of the custom function.</param>
        /// <param name="functionSignature">The signature and return type of the custom function.</param>
        /// <exception cref="ODataException">
        /// Thrown if a built-in function or an existing custom function with the same signature already exists.
        /// </exception>
        public static void AddCustomUriFunction(this IEdmModel model, string functionName, FunctionSignatureWithReturnType functionSignature)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(functionName, "functionName");
            ExceptionUtils.CheckArgumentNotNull(functionSignature, "functionSignature");

            ValidateFunctionWithReturnType(functionSignature);

            // Check if the function exists as a built-in function
            if (BuiltInUriFunctions.TryGetBuiltInFunction(functionName, out FunctionSignatureWithReturnType[] builtInFunctionSignatures))
            {
                // Check if function signature is one of the overloads
                for (int i = 0; i < builtInFunctionSignatures.Length; i++)
                {
                    if (AreFunctionsSignatureEqual(builtInFunctionSignatures[i], functionSignature))
                    {
                        throw new ODataException(
                            Error.Format(SRResources.CustomUriFunctions_AddCustomUriFunction_BuiltInExistsFullSignature, functionName));
                    }
                }
            }

            CustomUriFunctionsStore store = CustomUriFunctionsStore.GetOrCreate(model);

            // Check existing overloads (case-sensitive per store)
            if (store.TryGet(functionName, ignoreCase: false, out IReadOnlyList<FunctionSignatureWithReturnType> existingSignatures))
            {
                for (int i = 0; i < existingSignatures.Count; i++)
                {
                    if (AreFunctionsSignatureEqual(existingSignatures[i], functionSignature))
                    {
                        throw new ODataException(
                            Error.Format(SRResources.CustomUriFunctions_AddCustomUriFunction_CustomFunctionOverloadExists, functionName));
                    }
                }
            }

            // Add new overload
            store.Add(functionName, functionSignature);
        }

        /// <summary>
        /// Removes a specific overload of a custom URI function from the Edm model.
        /// </summary>
        /// <param name="model">The Edm model from which to remove the custom function.</param>
        /// <param name="functionName">The name of the custom function.</param>
        /// <param name="functionSignature">The signature and return type of the custom function to remove.</param>
        /// <returns>
        /// <c>true</c> if the function overload was found and removed; <c>false</c> otherwise.
        /// </returns>
        public static bool RemoveCustomUriFunction(
            this IEdmModel model,
            string functionName,
            FunctionSignatureWithReturnType functionSignature)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(functionName, "functionName");
            ExceptionUtils.CheckArgumentNotNull(functionSignature, "functionSignature");

            ValidateFunctionWithReturnType(functionSignature);

            CustomUriFunctionsStore store = CustomUriFunctionsStore.GetOrCreate(model);

            // Get existing signatures
            if (!store.TryGet(functionName, ignoreCase: false, out IReadOnlyList<FunctionSignatureWithReturnType> existingSignature))
            {
                return false;
            }

            // Find the *stored instance* that is structurally equal, so removal succeeds
            FunctionSignatureWithReturnType functionSignatureToRemove = null;
            for (int i = 0; i < existingSignature.Count; i++)
            {
                if (AreFunctionsSignatureEqual(existingSignature[i], functionSignature))
                {
                    functionSignatureToRemove = existingSignature[i];
                    break;
                }
            }

            if (functionSignatureToRemove == null)
            {
                return false; // structurally equal signature not found
            }

            return store.Remove(functionName, functionSignatureToRemove);
        }

        /// <summary>
        /// Removes all overloads of a custom URI function with the specified name from the Edm model.
        /// </summary>
        /// <param name="model">The Edm model from which to remove the custom function.</param>
        /// <param name="functionName">The name of the custom function to remove.</param>
        /// <returns>
        /// <c>true</c> if the function was found and removed; <c>false</c> otherwise.
        /// </returns>
        public static bool RemoveCustomUriFunction(this IEdmModel model, string functionName)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(functionName, "functionName");

            CustomUriFunctionsStore store = CustomUriFunctionsStore.GetOrCreate(model);

            return store.Remove(functionName);
        }

        /// <summary>
        /// Attempts to retrieve all overloads of a custom URI function with the specified name from the Edm model.
        /// </summary>
        /// <param name="model">The Edm model to search for the custom function.</param>
        /// <param name="functionName">The name of the custom function to retrieve.</param>
        /// <param name="functionSignatures">
        /// When this method returns, contains a list of key-value pairs of function names and their signatures, if found; otherwise, null.
        /// </param>
        /// <param name="ignoreCase">Whether to ignore case when matching the function name.</param>
        /// <returns>
        /// <c>true</c> if one or more overloads of the function were found; <c>false</c> otherwise.
        /// </returns>
        public static bool TryGetCustomUriFunction(
            this IEdmModel model,
            string functionName,
            out IReadOnlyList<KeyValuePair<string, FunctionSignatureWithReturnType>> functionSignatures,
            bool ignoreCase = false)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(functionName, "functionName");

            functionSignatures = null;

            CustomUriFunctionsStore store = model.GetAnnotationValue<CustomUriFunctionsStore>(model);

            if (store == null)
            {
                return false;
            }

            if (!ignoreCase)
            {
                // Case-sensitive: Exact matches only
                if (!store.TryGet(functionName, ignoreCase: false, out IReadOnlyList<FunctionSignatureWithReturnType> exactMatchSignatures))
                {
                    return false; // No exact match found
                }

                List<KeyValuePair<string, FunctionSignatureWithReturnType>> signatures = new List<KeyValuePair<string, FunctionSignatureWithReturnType>>(exactMatchSignatures.Count);
                for (int i = 0; i < exactMatchSignatures.Count; i++)
                {
                    signatures.Add(new KeyValuePair<string, FunctionSignatureWithReturnType>(functionName, exactMatchSignatures[i]));
                }

                functionSignatures = signatures.AsReadOnly();
                return true;
            }
            else
            {
                // Case-insensitive: All matches regardless of case
                IReadOnlyDictionary<string, IReadOnlyList<FunctionSignatureWithReturnType>> snapshot = store.Snapshot();
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;

                List<KeyValuePair<string, FunctionSignatureWithReturnType>> signatures = null;
                foreach (KeyValuePair<string, IReadOnlyList<FunctionSignatureWithReturnType>> kvPair in snapshot)
                {
                    if (!comparer.Equals(kvPair.Key, functionName))
                    {
                        continue;
                    }

                    signatures ??= new List<KeyValuePair<string, FunctionSignatureWithReturnType>>();

                    for (int i = 0; i < kvPair.Value.Count; i++)
                    {
                        signatures.Add(new KeyValuePair<string, FunctionSignatureWithReturnType>(kvPair.Key, kvPair.Value[i]));
                    }
                }

                if (signatures is { Count: > 0 })
                {
                    functionSignatures = signatures.AsReadOnly();
                }

                return functionSignatures != null && functionSignatures.Count > 0;
            }
        }

        #endregion

        #region Private Methods

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

        #endregion
    }
}
