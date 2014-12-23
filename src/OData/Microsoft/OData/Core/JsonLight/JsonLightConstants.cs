//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Core.JsonLight
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

        /// <summary>The 'error' property name for the Json Light value property.</summary>
        internal const string ODataErrorPropertyName = "error";

        /// <summary>The 'source' property name for the Json Light value property.</summary>
        internal const string ODataSourcePropertyName = "source";

        /// <summary>The 'target' property name for the Json Light value property.</summary>
        internal const string ODataTargetPropertyName = "target";

        /// <summary>The 'relationship' property name for the Json Light value property.</summary>
        internal const string ODataRelationshipPropertyName = "relationship";

        /// <summary>The 'id' property name for the Json Light value property.</summary>
        internal const string ODataIdPropertyName = "id";

        /// <summary>The 'reason' property name for the Json Light value property.</summary>
        internal const string ODataReasonPropertyName = "reason";

        /// <summary>The value 'changed' for the Json Light 'reason' property.</summary>
        internal const string ODataReasonChangedValue = "changed";

        /// <summary>The value 'deleted' for the Json Light 'reason' property.</summary>
        internal const string ODataReasonDeletedValue = "deleted";

        /// <summary>The name of the property returned for a URL of a service document element.</summary>
        internal const string ODataServiceDocumentElementUrlName = "url";

        /// <summary>The string used for the title attribute for the service document element.</summary>
        internal const string ODataServiceDocumentElementTitle = "title";

        /// <summary>The string used for the kind attribute for the service document element.</summary>
        internal const string ODataServiceDocumentElementKind = "kind";

        /// <summary>The name of the property returned for a name of a service document element.</summary>
        internal const string ODataServiceDocumentElementName = "name";

        /// <summary>The name of the $select query option.</summary>
        internal const string ContextUriSelectQueryOptionName = "$select";

        /// <summary>The '=' character used to separate a query option name from its value.</summary>
        internal const char ContextUriQueryOptionValueSeparator = '=';

        /// <summary>The '&amp;' separator character between query options.</summary>
        internal const char ContextUriQueryOptionSeparator = '&';

        /// <summary>The '(' used to mark the start of function parameters in the fragment of a context URI.</summary>
        internal const char FunctionParameterStart = '(';

        /// <summary>The ')' used to mark the end of function parameters in the fragment of a context URI.</summary>
        internal const char FunctionParameterEnd = ')';

        /// <summary>The "," to use as the separator for the function parameters in the fragment of a context URI.</summary>
        internal const string FunctionParameterSeparator = ",";

        /// <summary>The ',' to use as the separator for the function parameters.</summary>
        internal const char FunctionParameterSeparatorChar = ',';

        /// <summary> THe "=@" to use as the separator for the function parameter in target.</summary>
        internal const string FunctionParameterAssignment = "=@";

        /// <summary> The kind name of the service document singleton element. </summary>
        internal const string ServiceDocumentSingletonKindName = "Singleton";

        /// <summary> The kind name of the service document function import element. </summary>
        internal const string ServiceDocumentFunctionImportKindName = "FunctionImport";

        /// <summary> The kind name of the service document entity set element. </summary>
        internal const string ServiceDocumentEntitySetKindName = "EntitySet";
    }
}
