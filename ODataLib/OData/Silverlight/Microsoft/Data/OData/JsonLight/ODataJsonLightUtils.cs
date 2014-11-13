//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.JsonLight
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;

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
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!String.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            return propertyName.IndexOf(JsonLightConstants.MetadataUriFragmentIndicator) >= 0;
        }

        /// <summary>
        /// Gets the fully qualified function import name from the metadata reference property name.
        /// </summary>
        /// <param name="metadataDocumentUri">The metadata document Uri.</param>
        /// <param name="metadataReferencePropertyName">The metadata reference property name.</param>
        /// <param name="firstParameterTypeName">The first parameter name, if any are present in the given string.</param>
        /// <returns>The fully qualified function import name.</returns>
        internal static string GetFullyQualifiedFunctionImportName(Uri metadataDocumentUri, string metadataReferencePropertyName, out string firstParameterTypeName)
        {
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(metadataDocumentUri != null, "metadataDocumentUri != null");
            Debug.Assert(!String.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            if (propertyName[0] == JsonLightConstants.MetadataUriFragmentIndicator)
            {
                propertyName = UriUtils.EnsureEscapedFragment(propertyName);
                return new Uri(metadataDocumentUri, propertyName);
            }

            Debug.Assert(Uri.IsWellFormedUriString(propertyName, UriKind.Absolute), "The propertyName should be an absolute Uri.");
            return new Uri(propertyName, UriKind.Absolute);
        }

        /// <summary>
        /// Calculates the metadata reference name for the given function import. When there is no overload to the function, this method will
        /// return the container qualified function import name.  When there is overload to the function this method will
        /// return FQFN([comma separated parameter type names]) to disambiguate between different overloads.
        /// </summary>
        /// <param name="functionImport">The function import in question.</param>
        /// <returns>The metadata reference name for the given function import.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed",
            Justification = "This method is used for matching the name of the function import to something written by the server. So using the name is safe without resolving the type from the client.")]
        internal static string GetMetadataReferenceName(IEdmFunctionImport functionImport)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(functionImport != null, "functionImport != null");

            string metadataReferenceName = functionImport.FullName();
            Debug.Assert(functionImport.Container != null, "functionImport.Container != null");
            bool hasOverload = functionImport.Container.FindFunctionImports(functionImport.Name).Take(2).Count() > 1;
            if (hasOverload)
            {
                metadataReferenceName = functionImport.FullNameWithParameters();
            }

            return metadataReferenceName;
        }

        /// <summary>
        /// Creates an ODataAction or ODataFunction from a function import.
        /// </summary>
        /// <param name="metadataDocumentUri">The metadata document uri.</param>
        /// <param name="metadataReferencePropertyName">The metadata reference property name.</param>
        /// <param name="functionImport">The function import to create the ODataOperation for.</param>
        /// <param name="isAction">true if the created ODataOperation is an ODataAction, false otherwise.</param>
        /// <returns>The created ODataAction or ODataFunction.</returns>
        internal static ODataOperation CreateODataOperation(Uri metadataDocumentUri, string metadataReferencePropertyName, IEdmFunctionImport functionImport, out bool isAction)
        {
            DebugUtils.CheckNoExternalCallers();
            
            Debug.Assert(metadataDocumentUri != null, "metadataDocumentUri != null");
            Debug.Assert(!string.IsNullOrEmpty(metadataReferencePropertyName), "!string.IsNullOrEmpty(metadataReferencePropertyName)");
            Debug.Assert(functionImport != null, "functionImport != null");

            isAction = functionImport.IsSideEffecting;
            ODataOperation operation = isAction ? (ODataOperation)new ODataAction() : new ODataFunction();

            // Note that the property name can be '#name' which is not a valid Uri. We need to prepend the metadata document uri in that case.
            operation.Metadata = GetAbsoluteUriFromMetadataReferencePropertyName(metadataDocumentUri, metadataReferencePropertyName);
            return operation;
        }
    }
}
