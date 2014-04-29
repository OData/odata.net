//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.JsonLight
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Helper methods used by the OData reader for the JsonLight format.
    /// </summary>
    internal static class ODataJsonLightUtils
    {
        /// <summary>
        /// The character array used for splitting apart the operation parameter type names in a metadata link. Contains ','.
        /// </summary>
        private static readonly char[] ParameterSeparatorSplitCharacters = new[] { JsonLightConstants.FunctionParameterSeparator[0] };

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
        /// <param name="firstParameterTypeName">The first parameter name, if any are present in the given string.</param>
        /// <returns>The fully qualified operation import name.</returns>
        internal static string GetFullyQualifiedOperationName(Uri metadataDocumentUri, string metadataReferencePropertyName, out string firstParameterTypeName)
        {
            Debug.Assert(metadataDocumentUri != null, "metadataDocumentUri != null");
            Debug.Assert(!String.IsNullOrEmpty(metadataReferencePropertyName), "!string.IsNullOrEmpty(metadataReferencePropertyName)");

            string fullyQualifiedFunctionImportName = GetUriFragmentFromMetadataReferencePropertyName(metadataDocumentUri, metadataReferencePropertyName);
            firstParameterTypeName = null;

            int indexOfLeftParenthesis = fullyQualifiedFunctionImportName.IndexOf(JsonLightConstants.FunctionParameterStart);
            if (indexOfLeftParenthesis > -1)
            {
                string parameters = fullyQualifiedFunctionImportName.Substring(indexOfLeftParenthesis + 1);
                fullyQualifiedFunctionImportName = fullyQualifiedFunctionImportName.Substring(0, indexOfLeftParenthesis);

                // The first parameter name is everything after the first paren up to the first comma or the end of the string, with all parentheses removed.
                firstParameterTypeName = parameters.Split(ParameterSeparatorSplitCharacters).First().Trim(CharactersToTrimFromParameters);
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
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed",
            Justification = "This method is used for matching the name of the operation to something written by the server. So using the name is safe without resolving the type from the client.")]
        internal static string GetMetadataReferenceName(IEdmModel model, IEdmOperation operation)
        {
            Debug.Assert(operation != null, "operation != null");

            string metadataReferenceName = operation.FullName();
            bool hasOverload = model.FindDeclaredOperations(operation.FullName()).Take(2).Count() > 1;
            if (hasOverload)
            {
                metadataReferenceName = operation.FullNameWithParameters();
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
            operation.Metadata = GetAbsoluteUriFromMetadataReferencePropertyName(metadataDocumentUri, metadataReferencePropertyName);
            return operation;
        }
    }
}
