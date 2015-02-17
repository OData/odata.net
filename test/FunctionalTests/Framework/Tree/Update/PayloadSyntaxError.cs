//---------------------------------------------------------------------
// <copyright file="PayloadSyntaxError.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using AstoriaUnitTests.Data;

    public enum PayloadSyntaxErrorKind
    {
        /// <summary>No syntax error requested.</summary>
        None,

        #region String formatting.
        
        /// <summary>"abc</summary>
        UnterminatedStringValue,

        /// <summary>Name : "ab\"</summary>
        TruncatedEscapeSequence,

        /// <summary>Name = "ab\m"</summary>
        IncorrectEscapeSequence,

        /// <summary>Name : "ab\u123"</summary>
        TruncatedHexEscape,

        /// <summary>Name : "ab\u12fg"</summary>
        IncorrectHexEscape,

        /// <summary>Name : "abc'</summary>
        IncorrectQuotes,

        /// <summary>ID : id</summary>
        UnquotedValue,
        
        #endregion String formatting.

        #region Numeric formatting.

        /// <summary>-UInt64-1</summary>
        NumberTooSmall,
        
        /// <summary>UInt64+1</summary>
        NumberTooLarge,

        /// <summary>0.UInt64-1</summary>
        NumberTooPrecise,

        /// <summary>+123</summary>
        LeadingPlusSign,

        /// <summary>123.123.123</summary>
        MultipleDots,

        /// <summary>123E1E2</summary>
        MultipleE,

        /// <summary>--1</summary>
        MultipleLeadingMinus,

        /// <summary>++1</summary>
        MultipleLeadingPlus,

        /// <summary>123.E--1</summary>
        MultipleEMinus,

        /// <summary>123.E++1</summary>
        MultipleEPlus,

        /// <summary>123.E+1.2</summary>
        DotAfterE,

        #endregion Numeric formatting.

        #region JSON object structures.

        /// <summary> : 123</summary>
        MissingPropertyName,
        
        /// <summary>ID 123</summary>
        MissingPropertyColon,
        
        /// <summary>ID : </summary>
        MissingPropertyValue,
        
        /// <summary>... }</summary>
        OpeningBracesMissing,

        /// <summary>{ ...</summary>
        ClosingBracesMissing,

        #endregion JSON object structures.

        #region JSON array structures.

        /// <summary>ID : 123 Name : "Customer"</summary>
        MissingComma,

        /// <summary>{} , {} ,</summary>
        ExtraTailingComma,

        /// <summary>, {} , {}</summary>
        ExtraLeadingComma,

        /// <summary>... ]</summary>
        OpeningBracketMissing,

        /// <summary>[ ...</summary>
        ClosingBracketMissing,

        #endregion JSON array structures.
    }

    /// <summary>Includes the </summary>
    public class PayloadSyntaxError
    {
        /// <summary>Kind of error to inject when building payload.</summary>
        private PayloadSyntaxErrorKind kind;

        /// <summary>
        /// Initializes a new <see cref="PayloadSyntaxError"/> instance.
        /// </summary>
        /// <param name="kind">Kind of error to inject when building payload.</param>
        public PayloadSyntaxError(PayloadSyntaxErrorKind kind)
        {
            this.kind = kind;
        }

        /// <summary>Provides an array of all possible error kinds.</summary>
        public static PayloadSyntaxErrorKind[] ErrorKinds
        {
            get
            {
                return (PayloadSyntaxErrorKind[])Enum.GetValues(typeof(PayloadSyntaxErrorKind));
            }
        }

        /// <summary>Kind of error to inject when building payload.</summary>
        public PayloadSyntaxErrorKind Kind
        {
            get { return this.kind; }
        }

        /// <summary>Provides an array of all errors applicable to the specified node.</summary>
        public static PayloadSyntaxErrorKind[] GetApplicableKindsForNode(Node node)
        {
            List<PayloadSyntaxErrorKind> result = new List<PayloadSyntaxErrorKind>();
            foreach (PayloadSyntaxErrorKind kind in ErrorKinds)
            {
                if (KindAppliesToNode(kind, node))
                {
                    result.Add(kind);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Checks whether the specified <paramref name="kind"/> of syntax 
        /// error is applicable to the given <paramref name="node"/>.
        /// </summary>
        /// <param name="kind">Kind of error to check.</param>
        /// <param name="node">Candidate node for applicability.</param>
        /// <remarks>true if <paramref name="kind"/> is applicable to <paramref name="node"/>; false otherwise.</remarks>
        public static bool KindAppliesToNode(PayloadSyntaxErrorKind kind, Node node)
        {
            switch (kind)
            {
                case PayloadSyntaxErrorKind.None:
                    return true;

                case PayloadSyntaxErrorKind.ClosingBracesMissing:
                case PayloadSyntaxErrorKind.OpeningBracesMissing:
                case PayloadSyntaxErrorKind.MissingPropertyColon:
                case PayloadSyntaxErrorKind.MissingPropertyName:
                case PayloadSyntaxErrorKind.MissingPropertyValue:
                    return node is KeyedResourceInstance;

                case PayloadSyntaxErrorKind.ClosingBracketMissing:
                case PayloadSyntaxErrorKind.OpeningBracketMissing:
                    return node is ResourceInstanceCollection;

                case PayloadSyntaxErrorKind.MissingComma:
                case PayloadSyntaxErrorKind.ExtraLeadingComma:
                case PayloadSyntaxErrorKind.ExtraTailingComma:
                    return node is KeyedResourceInstance || node is ResourceInstanceCollection;

                case PayloadSyntaxErrorKind.DotAfterE:
                case PayloadSyntaxErrorKind.LeadingPlusSign:
                case PayloadSyntaxErrorKind.MultipleDots:
                case PayloadSyntaxErrorKind.MultipleE:
                case PayloadSyntaxErrorKind.MultipleEMinus:
                case PayloadSyntaxErrorKind.MultipleEPlus:
                case PayloadSyntaxErrorKind.MultipleLeadingMinus:
                case PayloadSyntaxErrorKind.MultipleLeadingPlus:
                case PayloadSyntaxErrorKind.NumberTooSmall:
                case PayloadSyntaxErrorKind.NumberTooLarge:
                case PayloadSyntaxErrorKind.NumberTooPrecise:
                    // return node is NodeValue && ((NodeValue)node).Type.IsNumeric;
                    return node is ResourceInstanceSimpleProperty && TypeData.IsTypeNumeric(((ResourceInstanceSimpleProperty)node).ClrType);

                case PayloadSyntaxErrorKind.TruncatedEscapeSequence:
                case PayloadSyntaxErrorKind.TruncatedHexEscape:
                case PayloadSyntaxErrorKind.IncorrectEscapeSequence:
                case PayloadSyntaxErrorKind.IncorrectHexEscape:
                case PayloadSyntaxErrorKind.IncorrectQuotes:
                case PayloadSyntaxErrorKind.UnterminatedStringValue:
                case PayloadSyntaxErrorKind.UnquotedValue:
                    // return node is NodeValue && ((NodeValue)node).Type.ClrType == typeof(string);
                    return node is ResourceInstanceSimpleProperty && ((ResourceInstanceSimpleProperty)node).ClrType == typeof(string);

                default:
                    throw new NotImplementedException("KindAppliedToNode does not support " + kind);
            }
        }

        /// <summary>Checks whether the specified <paramref name="node"/> contains any syntax errors.</summary>
        /// <param name="node">Update node to check.</param>
        /// <returns>
        /// true if there are any errors in the <paramref name="node"/> tree;
        /// false otherwise.
        /// </returns>
        public static bool ContainsSyntaxError(ResourceBodyTree node)
        {
            return InternalContainsSyntaxError(node);
        }

        private static Dictionary<string, System.Text.RegularExpressions.Regex> regexCache;

        /// <remarks>
        /// See http://msdn2.microsoft.com/en-us/library/system.text.regularexpressions.regex.aspx for
        /// an overview of the Regex class, and http://msdn2.microsoft.com/en-us/library/hs600312.aspx
        /// for the Regular Expressions developer guide.
        /// </remarks>
        private static System.Text.RegularExpressions.Regex CachedRegex(string pattern)
        {
            if (regexCache == null)
            {
                regexCache = new Dictionary<string, System.Text.RegularExpressions.Regex>();
            }

            System.Text.RegularExpressions.Regex regex;
            if (!regexCache.TryGetValue(pattern, out regex))
            {
                regex = new System.Text.RegularExpressions.Regex(pattern);
                regexCache[pattern] = regex;
            }

            return regex;
        }

        /// <summary>Applies this syntax error to the specified node.</summary>
        /// <param name="node">Node to apply error to.</param>
        public static string ApplyErrorsToNodeText(Node node, string text)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            string result = text;
            foreach (var f in System.Linq.Enumerable.OfType<PayloadSyntaxErrorFacet>(node.Facets))
            {
                PayloadSyntaxErrorKind kind = f.PayloadSyntaxError.Kind;
                switch (kind)
                {
                    case PayloadSyntaxErrorKind.None:
                        continue;

                    case PayloadSyntaxErrorKind.ClosingBracesMissing:
                        result = CachedRegex(@"}\s*$").Replace(result, "", 1);
                        continue;

                    case PayloadSyntaxErrorKind.OpeningBracesMissing:
                        result = CachedRegex(@"^\s*\{").Replace(result, "", 1);
                        continue;

                    case PayloadSyntaxErrorKind.MissingPropertyColon:
                        result = CachedRegex("(\\{.*):").Replace(result, "$1", 1);
                        continue;

                    case PayloadSyntaxErrorKind.MissingPropertyName:
                        result = "{ : 123}";
                        continue;

                    case PayloadSyntaxErrorKind.MissingPropertyValue:
                        result = CachedRegex(@"\{(.*):.*,").Replace(result, "{ $1 :,", 1);
                        continue;

                    case PayloadSyntaxErrorKind.ClosingBracketMissing:
                        result = CachedRegex(@"]\s*$").Replace(result, "", 1);
                        continue;

                    case PayloadSyntaxErrorKind.OpeningBracketMissing:
                        result = CachedRegex(@"^\s*\[").Replace(result, "", 1);
                        continue;

                    case PayloadSyntaxErrorKind.MissingComma:
                        // A comma followed by optional whitespace and a letter.
                        result = CachedRegex(@",(\s*\p{L})").Replace(result, "$1", 1);
                        continue;

                    case PayloadSyntaxErrorKind.ExtraLeadingComma:
                        result = CachedRegex(@"\{").Replace(result, "{,", 1);
                        continue;

                    case PayloadSyntaxErrorKind.ExtraTailingComma:
                        result = CachedRegex(@"}\s*\z").Replace(result, ",}", 1);
                        continue;

                    case PayloadSyntaxErrorKind.DotAfterE:
                        result = "1.1E+1.2";
                        continue;

                    case PayloadSyntaxErrorKind.LeadingPlusSign:
                        result = "+1";
                        continue;

                    case PayloadSyntaxErrorKind.MultipleDots:
                        result = "1.2.3";
                        continue;
                    
                    case PayloadSyntaxErrorKind.MultipleE:
                        result = "1.1E+1E+2";
                        continue;
                    
                    case PayloadSyntaxErrorKind.MultipleEMinus:
                        result = "1.1E--1";
                        continue;
                    
                    case PayloadSyntaxErrorKind.MultipleEPlus:
                        result = "1.1E++1";
                        continue;
                    
                    case PayloadSyntaxErrorKind.MultipleLeadingMinus:
                        result = "--1";
                        continue;
                    
                    case PayloadSyntaxErrorKind.MultipleLeadingPlus:
                        result = "++1";
                        continue;
                    
                    case PayloadSyntaxErrorKind.NumberTooSmall:
                        result = Int64.MinValue.ToString(CultureInfo.InvariantCulture) + "00";
                        continue;
                    
                    case PayloadSyntaxErrorKind.NumberTooLarge:
                        result = UInt64.MaxValue.ToString(CultureInfo.InvariantCulture) + "00";
                        continue;
                    
                    case PayloadSyntaxErrorKind.NumberTooPrecise:
                        result = "0." + new string('0', 64) + "1";
                        continue;

                    case PayloadSyntaxErrorKind.TruncatedEscapeSequence:
                        result = "\"ab\\\"";
                        continue;

                    case PayloadSyntaxErrorKind.TruncatedHexEscape:
                        result = "\"ab\\u123\"";
                        continue;

                    case PayloadSyntaxErrorKind.IncorrectEscapeSequence:
                        result = "\"ab\\m\"";
                        continue;

                    case PayloadSyntaxErrorKind.IncorrectHexEscape:
                        result = "\"ab\\u123g\"";
                        continue;

                    case PayloadSyntaxErrorKind.IncorrectQuotes:
                        result = "'abc'";
                        continue;

                    case PayloadSyntaxErrorKind.UnterminatedStringValue:
                        result = "\"ab";
                        continue;

                    case PayloadSyntaxErrorKind.UnquotedValue:
                        result = "a";
                        continue;

                    default:
                        throw new NotImplementedException("KindAppliedToNode does not support " + kind);
                }
            }
            
            return result;
        }

        /// <summary>Checks whether the specified <paramref name="node"/> contains any syntax error facets.</summary>
        /// <param name="node">Node to check.</param>
        /// <returns>
        /// true if there are any error facets on the <paramref name="node"/>;
        /// false otherwise.
        /// </returns>
        private static bool ContainsSyntaxErrorFacet(Node node)
        {
            Debug.Assert(node != null, "node != null");

            foreach (var f in System.Linq.Enumerable.OfType<PayloadSyntaxErrorFacet>(node.Facets))
            {
                if (f.PayloadSyntaxError.Kind != PayloadSyntaxErrorKind.None)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Checks whether the specified <paramref name="node"/> contains any syntax errors.</summary>
        /// <param name="node">Update node to check.</param>
        /// <returns>
        /// true if there are any errors in the <paramref name="node"/> tree;
        /// false otherwise.
        /// </returns>
        private static bool InternalContainsSyntaxError(Node node)
        {
            if (ContainsSyntaxErrorFacet(node))
            {
                return true;
            }

            if (node is ComplexResourceInstance)
            {
                var n = (ComplexResourceInstance)node;
                foreach (ResourceInstanceProperty p in n.Properties)
                {
                    if (InternalContainsSyntaxError(p))
                    {
                        return true;
                    }
                }
            }

            if (node is KeyedResourceInstance)
            {
                var n = (KeyedResourceInstance)node;
                foreach (ResourceInstanceProperty p in n.KeyProperties)
                {
                    if (InternalContainsSyntaxError(p))
                    {
                        return true;
                    }
                }
            }

            if (node is ResourceInstanceCollection)
            {
                var n = (ResourceInstanceCollection)node;
                foreach (ResourceBodyTree updateNode in n.NodeList)
                {
                    if (InternalContainsSyntaxError(updateNode))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Provides a <see cref="NodeFacet"/> that annotates update nodes to 
    /// request syntax errors to be generated when the payload is built.
    /// </summary>
    public class PayloadSyntaxErrorFacet : NodeFacet
    {
        /// <summary>
        /// Initializes a new <see cref="PayloadSyntaxErrorFacet"/>
        /// instance with the sepcified <paramref name="error"/>.
        /// </summary>
        /// <param name="error"><see cref="PayloadSyntaxError"/> for annotated node.</param>
        public PayloadSyntaxErrorFacet(PayloadSyntaxError error)
            : base("PayloadSyntaxErrorFacet", new NodeValue(error, null))
        {
        }

        /// <param name="error"><see cref="PayloadSyntaxError"/> for annotated node.</param>
        public PayloadSyntaxError PayloadSyntaxError
        {
            get
            {
                return (PayloadSyntaxError)this.Value.ClrValue;
            }
        }
    }
}
