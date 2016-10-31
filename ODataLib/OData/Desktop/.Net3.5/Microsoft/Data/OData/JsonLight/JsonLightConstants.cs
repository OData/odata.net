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
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Constants for the JSON Lite format.
    /// </summary>
    internal static class JsonLightConstants
    {
        /// <summary>The prefix for OData annotation names.</summary>
        internal const string ODataAnnotationNamespacePrefix = "odata.";

        /// <summary>The separator of property annotations.</summary>
        internal const char ODataPropertyAnnotationSeparatorChar = '@';

        /// <summary>The value 'true' for the OData null annotation.</summary>
        internal const string ODataNullAnnotationTrueValue = "true";

        /// <summary>The 'value' property name for the Json Light value property.</summary>
        internal const string ODataValuePropertyName = "value";

        /// <summary>The name of the property returned for a singleton $links query.</summary>
        internal const string ODataEntityReferenceLinkUrlName = "url";

        /// <summary>The name of the property returned for a URL of a workspace collection.</summary>
        internal const string ODataWorkspaceCollectionUrlName = "url";

        /// <summary>The name of the property returned for a name of a workspace collection.</summary>
        internal const string ODataWorkspaceCollectionNameName = "name";

        /// <summary>The 'name' property name of an annotation group declaration.</summary>
        internal const string ODataAnnotationGroupNamePropertyName = "name";

        /// <summary>The name of the $select query option.</summary>
        internal const string MetadataUriSelectQueryOptionName = "$select";

        /// <summary>The '=' character used to separate a query option name from its value.</summary>
        internal const char MetadataUriQueryOptionValueSeparator = '=';

        /// <summary>The '&amp;' separator character between query options.</summary>
        internal const char MetadataUriQueryOptionSeparator = '&';

        /// <summary>The hash sign acting as fragment indicator in a metadata URI.</summary>
        internal const char MetadataUriFragmentIndicator = '#';

        /// <summary>The slash sign used as separator in the fragment of a metadata URI.</summary>
        internal const char MetadataUriFragmentPartSeparator = '/';

        /// <summary>The @Element token that indicates that the payload is a single item from a set.</summary>
        internal const string MetadataUriFragmentItemSelector = "@Element";

        /// <summary>The '(' used to mark the start of function parameters in the fragment of a metadata URI.</summary>
        internal const char FunctionParameterStart = '(';

        /// <summary>The ')' used to mark the end of function parameters in the fragment of a metadata URI.</summary>
        internal const char FunctionParameterEnd = ')';

        /// <summary>The "," to use as the separator for the function parameters in the fragment of a metadata URI.</summary>
        internal const string FunctionParameterSeparator = ",";

        /// <summary>The token that indicates the payload is a property with null value.</summary>
        internal const string MetadataUriFragmentNull = "Edm.Null";
    }
}
