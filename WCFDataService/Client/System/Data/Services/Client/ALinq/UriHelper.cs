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

namespace System.Data.Services.Client
{
    using System.Data.Services.Client.Metadata;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Text;

    /// <summary>utility class for helping construct uris</summary>
    internal static class UriHelper
    {
        /// <summary>forwardslash character</summary>
        internal const char FORWARDSLASH = '/';

        /// <summary>leftparan character</summary>
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

        /// <summary>inlinecount</summary>
        internal const string OPTIONCOUNT = "inlinecount";

        /// <summary>select</summary>
        internal const string OPTIONSELECT = "select";

        /// <summary>The $format query option.</summary>
        internal const string OPTIONFORMAT = "format";

        /// <summary>allpages</summary>
        internal const string COUNTALL = "allpages";

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
                return context.ResolveNameFromType(type) ?? type.FullName;
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

            if (context.MaxProtocolVersionAsVersion < Util.DataServiceVersion3)
            {
                throw new NotSupportedException(Strings.ALinq_TypeAsNotSupportedForMaxDataServiceVersionLessThan3);
            }

            if (!ClientTypeUtil.TypeOrElementTypeIsEntity(type))
            {
                throw new NotSupportedException(Strings.ALinq_TypeAsArgumentNotEntityType(type.FullName));
            }

            // Raise the uriVersion each time we write the type segment on the uri.
            WebUtil.RaiseVersion(ref uriVersion, Util.DataServiceVersion3);
            return context.ResolveNameFromType(type) ?? type.FullName;
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
            if (inPath && dataServiceContext.UrlConventions == DataServiceUrlConventions.KeyAsSegment)
            {
                stringBuilder.Append('$');
                stringBuilder.Append(FORWARDSLASH);
            }

            string typeName = GetEntityTypeNameForUriAndValidateMaxProtocolVersion(type, dataServiceContext, ref version);
            stringBuilder.Append(typeName);
        }
    }
}
