//---------------------------------------------------------------------
// <copyright file="ODataJsonLightUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Helper methods used by the OData reader for the JsonLight format.
    /// </summary>
    internal static class ODataJsonLightUtils
    {
        /// <summary>
        /// The set of characters to trim from the parameters of an operation. Contains '(' and ')'.
        /// </summary>
        private static readonly char[] CharactersToTrimFromParameters = new[] { JsonLightConstants.FunctionParameterStart, JsonLightConstants.FunctionParameterEnd };

        /// <summary>
        /// Determines if the specified property name is a name of a metadata reference property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>true if <paramref name="propertyName"/> is a name of a metadata reference property, false otherwise.</returns>
        internal static bool IsMetadataReferenceProperty(string propertyName)
        {
            Debug.Assert(!String.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            return propertyName.IndexOf(ODataConstants.ContextUriFragmentIndicator) >= 0;
        }

        /// <summary>
        /// Gets the fully qualified operation import name from the metadata reference property name.
        /// </summary>
        /// <param name="metadataDocumentUri">The metadata document Uri.</param>
        /// <param name="metadataReferencePropertyName">The metadata reference property name.</param>
        /// <param name="parameterNames">The parameter names, if any are present in the given string.</param>
        /// <returns>The fully qualified operation import name.</returns>
        internal static string GetFullyQualifiedOperationName(Uri metadataDocumentUri, string metadataReferencePropertyName, out string parameterNames)
        {
            Debug.Assert(metadataDocumentUri != null, "metadataDocumentUri != null");
            Debug.Assert(!String.IsNullOrEmpty(metadataReferencePropertyName), "!string.IsNullOrEmpty(metadataReferencePropertyName)");

            string fullyQualifiedFunctionImportName = GetUriFragmentFromMetadataReferencePropertyName(metadataDocumentUri, metadataReferencePropertyName);
            parameterNames = null;

            int indexOfLeftParenthesis = fullyQualifiedFunctionImportName.IndexOf(JsonLightConstants.FunctionParameterStart);
            if (indexOfLeftParenthesis > -1)
            {
                string parameters = fullyQualifiedFunctionImportName.Substring(indexOfLeftParenthesis + 1);
                fullyQualifiedFunctionImportName = fullyQualifiedFunctionImportName.Substring(0, indexOfLeftParenthesis);

                parameterNames = parameters.Trim(CharactersToTrimFromParameters);
            }

            return fullyQualifiedFunctionImportName;
        }

        /// <summary>
        /// Gets the Uri fragment from the metadata reference property name.
        /// </summary>
        /// <param name="metadataDocumentUri">The metadata document Uri.</param>
        /// <param name="propertyName">The metadata reference property name.</param>
        /// <returns>The Uri fragment which corresponds to action/function names, etc.</returns>
        internal static string GetUriFragmentFromMetadataReferencePropertyName(Uri metadataDocumentUri, string propertyName)
        {
            Debug.Assert(metadataDocumentUri != null, "metadataDocumentUri != null");
            Debug.Assert(!String.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            string fragment = GetAbsoluteUriFromMetadataReferencePropertyName(metadataDocumentUri, propertyName).GetComponents(UriComponents.Fragment, UriFormat.Unescaped);
            Debug.Assert(!String.IsNullOrEmpty(fragment), "!string.IsNullOrEmpty(fragment)");
            return fragment;
        }

        /// <summary>
        /// Converts the metadata reference property name to an absolute Uri.
        /// </summary>
        /// <param name="metadataDocumentUri">The metadata document uri.</param>
        /// <param name="propertyName">The metadata reference property name.</param>
        /// <returns>The absolute Uri for the metadata reference property name.</returns>
        internal static Uri GetAbsoluteUriFromMetadataReferencePropertyName(Uri metadataDocumentUri, string propertyName)
        {
            Debug.Assert(metadataDocumentUri != null, "metadataDocumentUri != null");
            Debug.Assert(!String.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            if (propertyName[0] == ODataConstants.ContextUriFragmentIndicator)
            {
                propertyName = UriUtils.EnsureEscapedFragment(propertyName);
                return new Uri(metadataDocumentUri, propertyName);
            }

            Debug.Assert(Uri.IsWellFormedUriString(propertyName, UriKind.Absolute), "The propertyName should be an absolute Uri.");
            return new Uri(propertyName, UriKind.Absolute);
        }

        /// <summary>
        /// Calculates the metadata reference name for the given operation. When there is no overload to the function, this method will
        /// return the namespace qualified operation name.  When there is overload to the operation this method will
        /// return FQFN([comma separated parameter type names]) to disambiguate between different overloads.
        /// </summary>
        /// <param name="model">The model of the operations.</param>
        /// <param name="operation">The operation in question.</param>
        /// <returns>The metadata reference name for the given operation.</returns>
        internal static string GetMetadataReferenceName(IEdmModel model, IEdmOperation operation)
        {
            Debug.Assert(operation != null, "operation != null");

            string metadataReferenceName = operation.FullName();
            bool hasOverload = model.FindDeclaredOperations(operation.FullName()).Take(2).Count() > 1;

            if (hasOverload)
            {
                if (operation is IEdmFunction)
                {
                    metadataReferenceName = operation.FullNameWithNonBindingParameters();
                }
            }

            return metadataReferenceName;
        }

        /// <summary>
        /// Creates an ODataAction or ODataFunction from a operation import.
        /// </summary>
        /// <param name="metadataDocumentUri">The metadata document uri.</param>
        /// <param name="metadataReferencePropertyName">The metadata reference property name.</param>
        /// <param name="edmOperation">The operation to create the ODataOperation for.</param>
        /// <param name="isAction">true if the created ODataOperation is an ODataAction, false otherwise.</param>
        /// <returns>The created ODataAction or ODataFunction.</returns>
        internal static ODataOperation CreateODataOperation(Uri metadataDocumentUri, string metadataReferencePropertyName, IEdmOperation edmOperation, out bool isAction)
        {
            Debug.Assert(metadataDocumentUri != null, "metadataDocumentUri != null");
            Debug.Assert(!string.IsNullOrEmpty(metadataReferencePropertyName), "!string.IsNullOrEmpty(metadataReferencePropertyName)");
            Debug.Assert(edmOperation != null, "edmOperation != null");

            isAction = edmOperation.IsAction();
            ODataOperation operation = isAction ? (ODataOperation)new ODataAction() : new ODataFunction();

            // Note that the property name can be '#name' which is not a valid Uri. We need to prepend the metadata document uri in that case.
            int parameterStartIndex = 0;
            if (isAction && (parameterStartIndex = metadataReferencePropertyName.IndexOf(JsonLightConstants.FunctionParameterStart)) > 0)
            {
                metadataReferencePropertyName = metadataReferencePropertyName.Substring(0, parameterStartIndex);
            }

            operation.Metadata = GetAbsoluteUriFromMetadataReferencePropertyName(metadataDocumentUri, metadataReferencePropertyName);
            return operation;
        }
    }
}
