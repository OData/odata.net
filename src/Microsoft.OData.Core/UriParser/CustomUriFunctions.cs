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
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Edm;

    #endregion

    /// <summary>
    /// Class represents functions signatures of custom uri functions.
    /// </summary>
    public static class CustomUriFunctions
    {
        #region Public Methods

        /// <summary>
        /// Adds a custom URI function to the EDM model with the specified name and signature.
        /// Throws an exception if a built-in function or an existing custom function with the same signature already exists.
        /// </summary>
        /// <param name="model">The EDM model to which the custom function will be added.</param>
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

            CustomUriFunctionsAnnotation customUriFunctionsAnnotation = model.GetOrSetCustomUriFunctionsAnnotation();

            if (!customUriFunctionsAnnotation.CustomUriFunctions.TryGetValue(functionName, out FunctionSignatureWithReturnType[] existingSignatures))
            {
                // Add the new function with its signature
                customUriFunctionsAnnotation.CustomUriFunctions.TryAdd(functionName, [functionSignature]);
            }
            else
            {
                // Check if function signature is one of the overloads
                for (int i = 0; i < existingSignatures.Length; i++)
                {
                    if (AreFunctionsSignatureEqual(existingSignatures[i], functionSignature))
                    {
                        throw new ODataException(
                            Error.Format(SRResources.CustomUriFunctions_AddCustomUriFunction_CustomFunctionOverloadExists, functionName));
                    }
                }

                // Add the new signature as an overload
                customUriFunctionsAnnotation.CustomUriFunctions[functionName] =
                    existingSignatures.Concat([functionSignature]).ToArray();
            }
        }

        /// <summary>
        /// Removes a specific overload of a custom URI function from the EDM model.
        /// </summary>
        /// <param name="model">The EDM model from which to remove the custom function.</param>
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

            CustomUriFunctionsAnnotation customUriFunctionsAnnotation = model.GetOrSetCustomUriFunctionsAnnotation();

            if (!customUriFunctionsAnnotation.CustomUriFunctions.TryGetValue(functionName, out FunctionSignatureWithReturnType[] existingSignatures))
            {
                return false;
            }

            int indexOfFunctionSignatureFound = -1;
            for (int i = 0; i < existingSignatures.Length; i++)
            {
                if (AreFunctionsSignatureEqual(existingSignatures[i], functionSignature))
                {
                    // Found the function signature to remove
                    indexOfFunctionSignatureFound = i;
                }
            }

            if (indexOfFunctionSignatureFound == -1)
            {
                // Function signature to remove was not found
                return false;
            }

            if (existingSignatures.Length == 1)
            {
                // Only one function signature exists, remove the whole function name
                return customUriFunctionsAnnotation.CustomUriFunctions.TryRemove(functionName, out _);
            }

            FunctionSignatureWithReturnType[] functionSignatures = new FunctionSignatureWithReturnType[existingSignatures.Length - 1];
            for (int i = 0, j = 0; i < existingSignatures.Length; i++)
            {
                // Skip the function signature to remove
                if (i != indexOfFunctionSignatureFound)
                {
                    functionSignatures[j++] = existingSignatures[i];
                }
            }

            customUriFunctionsAnnotation.CustomUriFunctions[functionName] = functionSignatures;

            return true;
        }

        /// <summary>
        /// Removes all overloads of a custom URI function with the specified name from the EDM model.
        /// </summary>
        /// <param name="model">The EDM model from which to remove the custom function.</param>
        /// <param name="functionName">The name of the custom function to remove.</param>
        /// <returns>
        /// <c>true</c> if the function was found and removed; <c>false</c> otherwise.
        /// </returns>
        public static bool RemoveCustomUriFunction(this IEdmModel model, string functionName)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(functionName, "functionName");

            CustomUriFunctionsAnnotation customUriFunctionsAnnotation = model.GetOrSetCustomUriFunctionsAnnotation();
            return customUriFunctionsAnnotation.CustomUriFunctions.TryRemove(functionName, out _);
        }

        /// <summary>
        /// Attempts to retrieve all overloads of a custom URI function with the specified name from the EDM model.
        /// </summary>
        /// <param name="model">The EDM model to search for the custom function.</param>
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
            out IList<KeyValuePair<string, FunctionSignatureWithReturnType>> functionSignatures,
            bool ignoreCase = false)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(functionName, "functionName");

            functionSignatures = null;

            CustomUriFunctionsAnnotation customUriFunctionsAnnotation = model.GetAnnotationValue<CustomUriFunctionsAnnotation>(model);

            if (customUriFunctionsAnnotation == null)
            {
                return false;
            }

            foreach (KeyValuePair<string, FunctionSignatureWithReturnType[]> customUriFunction in customUriFunctionsAnnotation.CustomUriFunctions)
            {
                if (customUriFunction.Key.Equals(functionName, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                {
                    foreach (FunctionSignatureWithReturnType functionSignature in customUriFunction.Value)
                    {
                        functionSignatures ??= new List<KeyValuePair<string, FunctionSignatureWithReturnType>>();

                        functionSignatures.Add(new KeyValuePair<string, FunctionSignatureWithReturnType>(customUriFunction.Key, functionSignature));
                    }
                }
            }

            return functionSignatures != null;
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

        private static CustomUriFunctionsAnnotation GetOrSetCustomUriFunctionsAnnotation(this IEdmModel model)
        {
            Debug.Assert(model != null, "model != null");

            CustomUriFunctionsAnnotation annotation = model.GetAnnotationValue<CustomUriFunctionsAnnotation>(model);
            if (annotation == null)
            {
                annotation = new CustomUriFunctionsAnnotation();
                model.SetAnnotationValue(model, annotation);
            }

            return annotation;
        }

        #endregion
    }
}
