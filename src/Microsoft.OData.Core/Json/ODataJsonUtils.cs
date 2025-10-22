//---------------------------------------------------------------------
// <copyright file="ODataJsonUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;

    /// <summary>
    /// Helper methods used by the OData reader for the Json format.
    /// </summary>
    internal static class ODataJsonUtils
    {
        /// <summary>
        /// The set of characters to trim from the parameters of an operation. Contains '(' and ')'.
        /// </summary>
        private static readonly char[] CharactersToTrimFromParameters = new[] { ODataJsonConstants.FunctionParameterStart, ODataJsonConstants.FunctionParameterEnd };

        /// <summary>
        /// Determines if the specified property name is a name of a metadata reference property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>true if <paramref name="propertyName"/> is a name of a metadata reference property, false otherwise.</returns>
        internal static bool IsMetadataReferenceProperty(string propertyName)
        {
            Debug.Assert(!String.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            return propertyName.IndexOf(ODataConstants.ContextUriFragmentIndicator, StringComparison.Ordinal) >= 0;
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

            int indexOfLeftParenthesis = fullyQualifiedFunctionImportName.IndexOf(ODataJsonConstants.FunctionParameterStart, StringComparison.Ordinal);
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
            if (isAction && (parameterStartIndex = metadataReferencePropertyName.IndexOf(ODataJsonConstants.FunctionParameterStart, StringComparison.Ordinal)) > 0)
            {
                metadataReferencePropertyName = metadataReferencePropertyName.Substring(0, parameterStartIndex);
            }

            operation.Metadata = GetAbsoluteUriFromMetadataReferencePropertyName(metadataDocumentUri, metadataReferencePropertyName);
            return operation;
        }

        /// <summary>
        /// Attempts to match a given span of characters to a predefined set of common OData property names and returns the corresponding interned string if a match is found.
        /// The method is intended to reduce memory usage by  interning commonly used property names in OData payloads
        /// </summary>
        /// <param name="span">The span of characters to evaluate. This span is compared against known OData property names.</param>
        /// <param name="value">When this method returns, contains the interned string corresponding to the matched OData property name,  if
        /// the match is successful; otherwise, <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the span matches one of the predefined OData property names;  otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool TryGetMatchingCommonValueString(ReadOnlySpan<char> span, out string value)
        {
            value = null;
            if (span.Length == 0)
                return false;

            // Fast-path: check length and first char(s) before comparing full span
            switch (span.Length)
            {
                case 2: // "id", "Id"
                    if (span[0] == 'i' && span[1] == 'd')
                    {
                        value = ODataJsonConstants.ODataIdPropertyName;
                        return true;
                    }
                    if (span[0] == 'I' && span[1] == 'd')
                    {
                        value = ODataJsonConstants.ODataIdPascalCasePropertyName;
                        return true;
                    }
                    break;

                case 3: // "url", "@id"
                    if (span[0] == 'u' && span[1] == 'r' && span[2] == 'l')
                    {
                        value = ODataJsonConstants.ODataServiceDocumentElementUrlName;
                        return true;
                    }
                    if (span[0] == '@' && span[1] == 'i' && span[2] == 'd')
                    {
                        value = ODataJsonConstants.SimplifiedODataIdPropertyName;
                        return true;
                    }
                    break;

                case 4:
                    // "null", "true", "name", "type"
                    if (span[0] == 'n' && span.SequenceEqual(ODataJsonConstants.ODataNullPropertyName.AsSpan()))
                    {
                        value = ODataJsonConstants.ODataNullPropertyName;
                        return true;
                    }
                    if (span[0] == 't' && span.SequenceEqual(ODataJsonConstants.ODataNullAnnotationTrueValue.AsSpan()))
                    {
                        value = ODataJsonConstants.ODataNullAnnotationTrueValue;
                        return true;
                    }
                    if (span[0] == 'n' && span.SequenceEqual(ODataJsonConstants.ODataServiceDocumentElementName.AsSpan()))
                    {
                        value = ODataJsonConstants.ODataServiceDocumentElementName;
                        return true;
                    }
                    if (span[0] == 't' && span.SequenceEqual(ODataJsonConstants.ODataTypePropertyName.AsSpan()))
                    {
                        value = ODataJsonConstants.ODataTypePropertyName;
                        return true;
                    }
                    break;

                case 5:
                    // "false", "value", "error", "delta", "@type"
                    if (span[0] == 'f' && span.SequenceEqual(JsonConstants.JsonFalseLiteral.AsSpan()))
                    {
                        value = JsonConstants.JsonFalseLiteral;
                        return true;
                    }
                    if (span[0] == 'v' && span.SequenceEqual(ODataJsonConstants.ODataValuePropertyName.AsSpan()))
                    {
                        value = ODataJsonConstants.ODataValuePropertyName;
                        return true;
                    }
                    if (span[0] == '@' && span[1] == 't' && span.SequenceEqual(ODataJsonConstants.SimplifiedODataTypePropertyName.AsSpan()))
                    {
                        value = ODataJsonConstants.SimplifiedODataTypePropertyName;
                        return true;
                    }
                    if (span[0] == 'd' && span.SequenceEqual(ODataJsonConstants.ODataDeltaPropertyName.AsSpan()))
                    {
                        value = ODataJsonConstants.ODataDeltaPropertyName;
                        return true;
                    }
                    if (span[0] == 'e' && span.SequenceEqual(ODataJsonConstants.ODataErrorPropertyName.AsSpan()))
                    {
                        value = ODataJsonConstants.ODataErrorPropertyName;
                        return true;
                    }
                    break;

                case 6:
                    // "reason", "source", "target"
                    if (span[0] == 'r' && span.SequenceEqual(ODataJsonConstants.ODataReasonPropertyName.AsSpan()))
                    {
                        value = ODataJsonConstants.ODataReasonPropertyName;
                        return true;
                    }
                    if (span[0] == 's' && span.SequenceEqual(ODataJsonConstants.ODataSourcePropertyName.AsSpan()))
                    {
                        value = ODataJsonConstants.ODataSourcePropertyName;
                        return true;
                    }
                    if (span[0] == 't' && span.SequenceEqual(ODataJsonConstants.ODataTargetPropertyName.AsSpan()))
                    {
                        value = ODataJsonConstants.ODataTargetPropertyName;
                        return true;
                    }
                    break;

                case 7:
                    // "changed", "deleted"
                    if (span[0] == 'c' && span.SequenceEqual(ODataJsonConstants.ODataReasonChangedValue.AsSpan()))
                    {
                        value = ODataJsonConstants.ODataReasonChangedValue;
                        return true;
                    }
                    if (span[0] == 'd' && span.SequenceEqual(ODataJsonConstants.ODataReasonDeletedValue.AsSpan()))
                    {
                        value = ODataJsonConstants.ODataReasonDeletedValue;
                        return true;
                    }
                    break;

                case 8:
                    // "@context", "@removed"
                    if (span[0] == '@' && span[1] == 'c' && span.SequenceEqual(ODataJsonConstants.SimplifiedODataContextPropertyName.AsSpan()))
                    {
                        value = ODataJsonConstants.SimplifiedODataContextPropertyName;
                        return true;
                    }
                    if (span[0] == '@' && span[1] == 'r' && span.SequenceEqual(ODataJsonConstants.SimplifiedODataRemovedPropertyName.AsSpan()))
                    {
                        value = ODataJsonConstants.SimplifiedODataRemovedPropertyName;
                        return true;
                    }
                    break;

                case 9:
                    // "@odata.id"
                    if (span[0] == '@' && span[7] == 'i' && span[8] == 'd' && span.SequenceEqual(ODataJsonConstants.PrefixedODataIdPropertyName.AsSpan()))
                    {
                        value = ODataJsonConstants.PrefixedODataIdPropertyName;
                        return true;
                    }
                    break;

                case 11:
                    // "@odata.type", "@odata.null"
                    if (span[0] == '@' && span[7] == 't' && span[8] == 'y' && span.SequenceEqual(ODataJsonConstants.PrefixedODataTypePropertyName.AsSpan()))
                    {
                        value = ODataJsonConstants.PrefixedODataTypePropertyName;
                        return true;
                    }
                    if (span[0] == '@' && span[7] == 'n' && span[8] == 'u' && span.SequenceEqual(ODataJsonConstants.PrefixedODataNullPropertyName.AsSpan()))
                    {
                        value = ODataJsonConstants.PrefixedODataNullPropertyName;
                        return true;
                    }
                    break;

                case 14:
                    // "@odata.context", "@odata.removed"
                    if (span[0] == '@' && span[7] == 'c' && span[8] == 'o' && span.SequenceEqual(ODataJsonConstants.PrefixedODataContextPropertyName.AsSpan()))
                    {
                        value = ODataJsonConstants.PrefixedODataContextPropertyName;
                        return true;
                    }
                    if (span[0] == '@' && span[7] == 'r' && span[8] == 'e' && span.SequenceEqual(ODataJsonConstants.PrefixedODataRemovedPropertyName.AsSpan()))
                    {
                        value = ODataJsonConstants.PrefixedODataRemovedPropertyName;
                        return true;
                    }
                    break;
            }

            return false;
        }
    }
}
