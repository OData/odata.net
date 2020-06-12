//---------------------------------------------------------------------
// <copyright file="UriHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using Microsoft.OData.Client.Metadata;

    /// <summary>utility class for helping construct uris</summary>
    internal static class UriHelper
    {
        /// <summary>forwardslash character</summary>
        internal const char FORWARDSLASH = '/';

        /// <summary>leftparen character</summary>
        internal const char LEFTPAREN = '(';

        /// <summary>rightparan character</summary>
        internal const char RIGHTPAREN = ')';

        /// <summary>questionmark character</summary>
        internal const char QUESTIONMARK = '?';

        /// <summary>ampersand character</summary>
        internal const char AMPERSAND = '&';

        /// <summary>equals character</summary>
        internal const char EQUALSSIGN = '=';

        /// <summary>at sign</summary>
        internal const char ATSIGN = '@';

        /// <summary>dollar sign character</summary>
        internal const char DOLLARSIGN = '$';

        /// <summary>space</summary>
        internal const char SPACE = ' ';

        /// <summary>comma</summary>
        internal const char COMMA = ',';

        /// <summary>colon</summary>
        internal const char COLON = ':';

        /// <summary> semicolon </summary>
        internal const char SEMICOLON = ';';

        /// <summary>single quote</summary>
        internal const char QUOTE = '\'';

        /// <summary>asterisk</summary>
        internal const char ASTERISK = '*';

        /// <summary>top</summary>
        internal const string OPTIONTOP = "top";

        /// <summary>skip</summary>
        internal const string OPTIONSKIP = "skip";

        /// <summary>orderby</summary>
        internal const string OPTIONORDERBY = "orderby";

        /// <summary>where</summary>
        internal const string OPTIONFILTER = "filter";

        /// <summary>desc</summary>
        internal const string OPTIONDESC = "desc";

        /// <summary>expand</summary>
        internal const string OPTIONEXPAND = "expand";

        /// <summary>count</summary>
        internal const string OPTIONCOUNT = "count";

        /// <summary>select</summary>
        internal const string OPTIONSELECT = "select";

        /// <summary>The $format query option.</summary>
        internal const string OPTIONFORMAT = "format";

        /// <summary>true</summary>
        internal const string COUNTTRUE = "true";

        /// <summary>false</summary>
        internal const string COUNTFALSE = "false";

        /// <summary>value</summary>
        internal const string COUNT = "count";

        /// <summary>and</summary>
        internal const string AND = "and";

        /// <summary>or</summary>
        internal const string OR = "or";

        /// <summary>eq</summary>
        internal const string EQ = "eq";

        /// <summary>ne</summary>
        internal const string NE = "ne";

        /// <summary>lt</summary>
        internal const string LT = "lt";

        /// <summary>le</summary>
        internal const string LE = "le";

        /// <summary>gt</summary>
        internal const string GT = "gt";

        /// <summary>ge</summary>
        internal const string GE = "ge";

        /// <summary>add</summary>
        internal const string ADD = "add";

        /// <summary>sub</summary>
        internal const string SUB = "sub";

        /// <summary>mul</summary>
        internal const string MUL = "mul";

        /// <summary>div</summary>
        internal const string DIV = "div";

        /// <summary>mod</summary>
        internal const string MOD = "mod";

        /// <summary>negate</summary>
        internal const string NEGATE = "-";

        /// <summary>not</summary>
        internal const string NOT = "not";

        /// <summary>null</summary>
        internal const string NULL = "null";

        /// <summary>isof</summary>
        internal const string ISOF = "isof";

        /// <summary>cast</summary>
        internal const string CAST = "cast";

        /// <summary>has</summary>
        internal const string HAS = "has";

        /// <summary>The encoded @ sign</summary>
        internal const string ENCODEDATSIGN = "%40";

        /// <summary>The encoded [ sign</summary>
        internal const string ENCODEDSQUAREBRACKETSIGN = "%5B";

        /// <summary>The encoded { sign</summary>
        internal const string ENCODEDBRACESIGN = "%7B";

        /// <summary>Gets the type name to be used in the URI for the given <paramref name="type"/>.</summary>
        /// <param name="type">Type to get name for.</param>
        /// <param name="context">Data context used to generate type names for types.</param>
        /// <returns>The name for the <paramref name="type"/>, suitable for including in a URI.</returns>
        internal static string GetTypeNameForUri(Type type, DataServiceContext context)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(context != null, "context != null");
            type = Nullable.GetUnderlyingType(type) ?? type;

            PrimitiveType pt;
            if (PrimitiveType.TryGetPrimitiveType(type, out pt))
            {
                if (pt.HasReverseMapping)
                {
                    return pt.EdmTypeName;
                }
                else
                {
                    // unsupported primitive type
                    throw new NotSupportedException(Strings.ALinq_CantCastToUnsupportedPrimitive(type.Name));
                }
            }
            else
            {
                return context.ResolveNameFromTypeInternal(type) ?? ClientTypeUtil.GetServerDefinedTypeFullName(type);
            }
        }

        /// <summary>Gets the type name to be used in the URI for the given <paramref name="type"/>.</summary>
        /// <param name="type">Type to get name for.</param>
        /// <param name="context">Data context used to generate type names for types.</param>
        /// <param name="uriVersion">Data service version for the uri</param>
        /// <returns>The name for the <paramref name="type"/>, suitable for including in a URI.</returns>
        internal static string GetEntityTypeNameForUriAndValidateMaxProtocolVersion(Type type, DataServiceContext context, ref Version uriVersion)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(context != null, "context != null");

            if (context.MaxProtocolVersionAsVersion < Util.ODataVersion4)
            {
                throw new NotSupportedException(Strings.ALinq_TypeAsNotSupportedForMaxDataServiceVersionLessThan3);
            }

            if (!ClientTypeUtil.TypeOrElementTypeIsEntity(type))
            {
                throw new NotSupportedException(Strings.ALinq_TypeAsArgumentNotEntityType(type.FullName));
            }

            // Raise the uriVersion each time we write the type segment on the uri.
            WebUtil.RaiseVersion(ref uriVersion, Util.ODataVersion4);

            return context.ResolveNameFromTypeInternal(type) ?? ClientTypeUtil.GetServerDefinedTypeFullName(type);
        }

        /// <summary>
        /// Appends a type segment to the <see cref="StringBuilder"/> which is building up a URI from a query.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        /// <param name="type">The type for the segment.</param>
        /// <param name="dataServiceContext">The data service context.</param>
        /// <param name="inPath">Whether or not the type segment is being appended within the path (as opposed to within a $filter or $orderby expression).</param>
        /// <param name="version">The current version.</param>
        internal static void AppendTypeSegment(StringBuilder stringBuilder, Type type, DataServiceContext dataServiceContext, bool inPath, ref Version version)
        {
            // The '$' segment is used to escape known metadata segments in the key-as-segments mode to avoid ambiguity.
            // Because keys are not allowed inside filter or orderby expressions, we do not need to add it in those cases.
            if (inPath && dataServiceContext.UrlKeyDelimiter == DataServiceUrlKeyDelimiter.Slash)
            {
                stringBuilder.Append('$');
                stringBuilder.Append(FORWARDSLASH);
            }

            string typeName = GetTypeNameForUri(type, dataServiceContext);
            stringBuilder.Append(typeName);
        }

        /// <summary>
        /// If the value represented by the string is primitive value or not.
        /// </summary>
        /// <param name="value">The string value represent the odata value.</param>
        /// <returns>True if the value is primitive value.</returns>
        internal static bool IsPrimitiveValue(string value)
        {
            return !(value.StartsWith(UriHelper.ENCODEDBRACESIGN, StringComparison.Ordinal) || value.StartsWith(UriHelper.ENCODEDSQUAREBRACKETSIGN, StringComparison.Ordinal));
        }
    }
}
