//---------------------------------------------------------------------
// <copyright file="EdmXmlDocumentParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.Parsing.Common
{
    internal abstract class EdmXmlDocumentParser<TResult> : XmlDocumentParser<TResult>
    {
        protected XmlElementInfo currentElement;
        private readonly Stack<XmlElementInfo> elementStack = new Stack<XmlElementInfo>();
        private HashSetInternal<string> edmNamespaces;

        internal EdmXmlDocumentParser(string artifactLocation, XmlReader reader)
            : base(reader, artifactLocation)
        {
        }

        internal abstract IEnumerable<KeyValuePair<Version, string>> SupportedVersions { get; }

        internal static XmlAttributeInfo GetOptionalAttribute(XmlElementInfo element, string attributeName)
        {
            return element.Attributes[attributeName];
        }

        internal XmlAttributeInfo GetRequiredAttribute(XmlElementInfo element, string attributeName)
        {
            var attr = element.Attributes[attributeName];
            if (attr.IsMissing)
            {
                this.ReportError(element.Location, EdmErrorCode.MissingAttribute, Error.Format(SRResources.XmlParser_MissingAttribute, attributeName, element.Name));
                return attr;
            }

            return attr;
        }

        protected override XmlReader InitializeReader(XmlReader reader)
        {
            XmlReaderSettings readerSettings = new XmlReaderSettings
            {
                CheckCharacters = true,
                CloseInput = false,
                IgnoreWhitespace = true,
                ConformanceLevel = ConformanceLevel.Auto,
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                DtdProcessing = DtdProcessing.Prohibit
            };

            // user specified a stream to read from, read from it.
            // The Uri is just used to identify the stream in errors.
            return XmlReader.Create(reader, readerSettings);
        }

        protected override bool TryGetDocumentVersion(string xmlNamespaceName, out Version version, out string[] expectedNamespaces)
        {
            expectedNamespaces = this.SupportedVersions.Select(v => v.Value).ToArray();
            version = this.SupportedVersions.Where(v => v.Value == xmlNamespaceName).Select(v => v.Key).FirstOrDefault();
            return version != null;
        }

        protected override bool IsOwnedNamespace(string namespaceName)
        {
            return this.IsEdmNamespace(namespaceName);
        }

        #region Utility Methods for derived document parsers

        protected XmlElementParser<TItem> CsdlElement<TItem>(string elementName, Func<XmlElementInfo, XmlElementValueCollection, TItem> initializer, params XmlElementParser[] childParsers)
           where TItem : class
        {
            return Element<TItem>(
                elementName,
                (element, childValues) =>
                {
                    BeginItem(element);
                    TItem result = initializer(element, childValues);
                    AnnotateItem(result, childValues);
                    EndItem();

                    return result;
                },
                childParsers);
        }

        protected void BeginItem(XmlElementInfo element)
        {
            this.elementStack.Push(element);
            this.currentElement = element;
        }

        protected abstract void AnnotateItem(object result, XmlElementValueCollection childValues);

        protected void EndItem()
        {
            this.elementStack.Pop();
            this.currentElement = this.elementStack.Count == 0 ? null : this.elementStack.Peek();
        }

        protected int? OptionalInteger(string attributeName)
        {
            XmlAttributeInfo attr = GetOptionalAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                int? value;
                if (!EdmValueParser.TryParseInt(attr.Value, out value))
                {
                    this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidInteger, Error.Format(SRResources.ValueParser_InvalidInteger, attr.Value));
                }

                return value;
            }

            return null;
        }

        protected long? OptionalLong(string attributeName)
        {
            XmlAttributeInfo attr = GetOptionalAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                long? value;
                if (!EdmValueParser.TryParseLong(attr.Value, out value))
                {
                    this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidLong, Error.Format(SRResources.ValueParser_InvalidLong, attr.Value));
                }

                return value;
            }

            return null;
        }

        protected int? OptionalSrid(string attributeName, int defaultSrid)
        {
            XmlAttributeInfo attr = GetOptionalAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                int? srid;
                if (attr.Value.EqualsOrdinalIgnoreCase(CsdlConstants.Value_SridVariable))
                {
                    srid = null;
                }
                else
                {
                    if (!EdmValueParser.TryParseInt(attr.Value, out srid))
                    {
                        this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidSrid, Error.Format(SRResources.ValueParser_InvalidSrid, attr.Value));
                    }
                }

                return srid;
            }

            return defaultSrid;
        }

        /// <summary>
        /// Parses the scale attribute if it's set.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <returns>The parsed scale value.</returns>
        protected int? OptionalScale(string attributeName)
        {
            XmlAttributeInfo attr = GetOptionalAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                int? scale;
                if (attr.Value.EqualsOrdinalIgnoreCase(CsdlConstants.Value_ScaleVariable))
                {
                    scale = null;
                }
                else
                {
                    if (!EdmValueParser.TryParseInt(attr.Value, out scale))
                    {
                        this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidSrid, Error.Format(SRResources.ValueParser_InvalidScale, attr.Value));
                    }
                }

                return scale;
            }

            return CsdlConstants.Default_Scale;
        }

        protected int? OptionalMaxLength(string attributeName)
        {
            XmlAttributeInfo attr = GetOptionalAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                int? value;
                if (!EdmValueParser.TryParseInt(attr.Value, out value))
                {
                    this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidMaxLength, Error.Format(SRResources.ValueParser_InvalidMaxLength, attr.Value));
                }

                return value;
            }

            return null;
        }

        protected EdmMultiplicity RequiredMultiplicity(string attributeName)
        {
            XmlAttributeInfo attr = this.GetRequiredAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                switch (attr.Value)
                {
                    case CsdlConstants.Value_EndRequired:
                        return EdmMultiplicity.One;
                    case CsdlConstants.Value_EndOptional:
                        return EdmMultiplicity.ZeroOrOne;
                    case CsdlConstants.Value_EndMany:
                        return EdmMultiplicity.Many;
                    default:
                        this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidMultiplicity, Error.Format(SRResources.CsdlParser_InvalidMultiplicity, attr.Value));
                        break;
                }
            }

            return EdmMultiplicity.One;
        }

        protected EdmOnDeleteAction RequiredOnDeleteAction(string attributeName)
        {
            XmlAttributeInfo attr = this.GetRequiredAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                switch (attr.Value)
                {
                    case CsdlConstants.Value_None:
                        return EdmOnDeleteAction.None;
                    case CsdlConstants.Value_Cascade:
                        return EdmOnDeleteAction.Cascade;
                    default:
                        this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidOnDelete, Error.Format(SRResources.CsdlParser_InvalidDeleteAction, attr.Value));
                        break;
                }
            }

            return EdmOnDeleteAction.None;
        }

        protected bool? OptionalBoolean(string attributeName)
        {
            XmlAttributeInfo attr = GetOptionalAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                bool? value;
                if (!EdmValueParser.TryParseBool(attr.Value, out value))
                {
                    this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidBoolean, Error.Format(SRResources.ValueParser_InvalidBoolean, attr.Value));
                }

                return value;
            }

            return null;
        }

        protected string Optional(string attributeName)
        {
            XmlAttributeInfo attr = GetOptionalAttribute(this.currentElement, attributeName);
            return !attr.IsMissing ? attr.Value : null;
        }

        protected string Required(string attributeName)
        {
            XmlAttributeInfo attr = this.GetRequiredAttribute(this.currentElement, attributeName);
            return !attr.IsMissing ? attr.Value : string.Empty;
        }

        protected string OptionalAlias(string attributeName)
        {
            XmlAttributeInfo attr = GetOptionalAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                return this.ValidateAlias(attr.Value);
            }

            return null;
        }

        protected string RequiredAlias(string attributeName)
        {
            XmlAttributeInfo attr = this.GetRequiredAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                return this.ValidateAlias(attr.Value);
            }

            return null;
        }

        protected string RequiredEntitySetPath(string attributeName)
        {
            XmlAttributeInfo attr = this.GetRequiredAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                return this.ValidateEntitySetPath(attr.Value);
            }

            return null;
        }

        protected string RequiredEnumMemberPath(string attributeName)
        {
            XmlAttributeInfo attr = this.GetRequiredAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                return this.ValidateEnumMemberPath(attr.Value);
            }

            return null;
        }

        protected string RequiredEnumMemberPath(XmlTextValue text)
        {
            string enumMemberPath = text != null ? text.TextValue : string.Empty;
            return this.ValidateEnumMembersPath(enumMemberPath);
        }

        protected string OptionalType(string attributeName)
        {
            XmlAttributeInfo attr = GetOptionalAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                return this.CheckAndReportTypeNameValidity(attr.Value);
            }

            return null;
        }

        protected string RequiredType(string attributeName)
        {
            XmlAttributeInfo attr = this.GetRequiredAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                return this.CheckAndReportTypeNameValidity(attr.Value);
            }

            return null;
        }

        protected string OptionalQualifiedName(string attributeName)
        {
            XmlAttributeInfo attr = GetOptionalAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                return this.ValidateQualifiedName(attr.Value);
            }

            return null;
        }

        protected string RequiredQualifiedName(string attributeName)
        {
            XmlAttributeInfo attr = this.GetRequiredAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                return this.ValidateQualifiedName(attr.Value);
            }

            return null;
        }

        protected string ValidateEnumMembersPath(string path)
        {
            if (string.IsNullOrEmpty(path.Trim()))
            {
                this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidEnumMemberPath, Error.Format(SRResources.CsdlParser_InvalidEnumMemberPath, path));
            }

            string[] enumValues = path.Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            string enumType = null;
            foreach (var enumValue in enumValues)
            {
                string[] segments = enumValue.Split('/');
                if (!(segments.Length == 2 &&
                    EdmUtil.IsValidDottedName(segments[0]) &&
                    EdmUtil.IsValidUndottedName(segments[1])))
                {
                    this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidEnumMemberPath, Error.Format(SRResources.CsdlParser_InvalidEnumMemberPath, path));
                }

                if (enumType != null && segments[0] != enumType)
                {
                    this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidEnumMemberPath, Error.Format(SRResources.CsdlParser_InvalidEnumMemberPath, path));
                }

                enumType = segments[0];
            }

            return string.Join(" ", enumValues);
        }

        private string CheckAndReportTypeNameValidity(string name)
        {
            Debug.Assert(name != null, "name != null");
            ReadOnlySpan<char> nameSpan = name.AsSpan();

            int indexOfOpenParen = nameSpan.IndexOf('(');
            int indexOfCloseParen = nameSpan.IndexOf(')');
            ReadOnlySpan<char> collectionAsSpan = CsdlConstants.Value_Collection.AsSpan();
            ReadOnlySpan<char> refAsSpan = CsdlConstants.Value_Ref.AsSpan();
            ReadOnlySpan<char> namePrefixSpan = ReadOnlySpan<char>.Empty;
            ReadOnlySpan<char> typeNameSpan = ReadOnlySpan<char>.Empty;


            if (indexOfOpenParen >= 0)
            {
                // 1. no close paren - indexOfCloseParen = -1,
                // 2. close paren appearing before open paren - "Collection)ns.Type(",
                // 2. close paren not at the end - "Collection(ns.Type)s"
                if (indexOfCloseParen < indexOfOpenParen || nameSpan[indexOfCloseParen] != nameSpan[^1])
                {
                    this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidTypeName, Error.Format(SRResources.CsdlParser_InvalidTypeName, name));
                    return name;
                }

                namePrefixSpan = nameSpan.Slice(0, indexOfOpenParen);
                if (namePrefixSpan.IsEmpty || !(namePrefixSpan.SequenceEqual(collectionAsSpan) || namePrefixSpan.SequenceEqual(refAsSpan)))
                {
                    this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidTypeName, Error.Format(SRResources.CsdlParser_InvalidTypeName, name));
                    return name;
                }

                typeNameSpan = nameSpan.Slice(indexOfOpenParen + 1, nameSpan.Length - indexOfOpenParen - 2);
                if (typeNameSpan.IndexOf('(') >= 0 || typeNameSpan.IndexOf(')') >= 0)
                {
                    // If there are nested parentheses, it's invalid
                    this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidTypeName, Error.Format(SRResources.CsdlParser_InvalidTypeName, name));
                    return name;
                }
            }
            else if (indexOfCloseParen >= 0)
            {
                // If there is a close parenthesis without an open parenthesis, it's invalid
                this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidTypeName, Error.Format(SRResources.CsdlParser_InvalidTypeName, name));
                return name;
            }

            if (typeNameSpan.IsEmpty)
            {
                typeNameSpan = nameSpan; // If no parentheses, the whole name is the type name
            }

            // 'Collection' on its own is a valid type string
            if (namePrefixSpan.IsEmpty && typeNameSpan.SequenceEqual(collectionAsSpan))
            {
                return name;
            }
            // 'Ref' on its own is not a valid type string.
            else if (namePrefixSpan.IsEmpty && typeNameSpan.SequenceEqual(refAsSpan))
            {
                this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidTypeName, Error.Format(SRResources.CsdlParser_InvalidTypeName, name));
                return name;
            }

            // Check for whitespace in the type name
            for (int i = 0; i < typeNameSpan.Length; i++)
            {
                if (char.IsWhiteSpace(typeNameSpan[i]))
                {
                    this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidTypeName, Error.Format(SRResources.CsdlParser_InvalidTypeName, name));
                    return name;
                }
            }

            string typeName = typeNameSpan.ToString();

            if (EdmUtil.IsQualifiedName(typeName) || EdmCoreModel.Instance.GetPrimitiveTypeKind(typeName) != EdmPrimitiveTypeKind.None)
            {
                return name;
            }
            else
            {
                this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidTypeName, Error.Format(SRResources.CsdlParser_InvalidTypeName, name));
                return name;
            }
        }

        private string ValidateAlias(string name)
        {
            if (!EdmUtil.IsValidUndottedName(name))
            {
                this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidQualifiedName, Error.Format(SRResources.CsdlParser_InvalidAlias, name));
            }

            return name;
        }

        private string ValidateEntitySetPath(string path)
        {
            string[] segments = path.Split('/');
            if (!(segments.Length == 2 &&
                EdmUtil.IsValidDottedName(segments[0]) &&
                EdmUtil.IsValidUndottedName(segments[1])))
            {
                this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidEntitySetPath, Error.Format(SRResources.CsdlParser_InvalidEntitySetPath, path));
            }

            return path;
        }

        private string ValidateEnumMemberPath(string path)
        {
            string[] segments = path.Split('/');
            if (!(segments.Length == 2 &&
                EdmUtil.IsValidDottedName(segments[0]) &&
                EdmUtil.IsValidUndottedName(segments[1])))
            {
                this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidEnumMemberPath, Error.Format(SRResources.CsdlParser_InvalidEnumMemberPath, path));
            }

            return path;
        }

        private string ValidateQualifiedName(string qualifiedName)
        {
            if (!EdmUtil.IsQualifiedName(qualifiedName))
            {
                this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidQualifiedName, Error.Format(SRResources.CsdlParser_InvalidQualifiedName, qualifiedName));
            }

            return qualifiedName;
        }

        private bool IsEdmNamespace(string xmlNamespaceUri)
        {
            Debug.Assert(!string.IsNullOrEmpty(xmlNamespaceUri), "Ensure namespace URI is not null or empty before calling IsEdmNamespace");

            if (this.edmNamespaces == null)
            {
                this.edmNamespaces = new HashSetInternal<string>();
                foreach (var namespaces in CsdlConstants.SupportedVersions.Values)
                {
                    foreach (var edmNamespace in namespaces)
                    {
                        this.edmNamespaces.Add(edmNamespace);
                    }
                }
            }

            return this.edmNamespaces.Contains(xmlNamespaceUri);
        }
        #endregion
    }
}
