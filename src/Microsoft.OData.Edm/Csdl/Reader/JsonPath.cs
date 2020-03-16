//---------------------------------------------------------------------
// <copyright file="JsonPath.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------


using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.OData.Edm.Csdl.Reader
{
    internal class JsonPathOptions
    {
        internal static JsonPathOptions Default = new JsonPathOptions();

        public JsonPathOptions()
        {
            IsBracketNotation = false;
        }

        /// <summary>
        /// If false: $.store.book[0].title
        /// If true: $['store']['book'][0]['title']
        /// </summary>
        public bool IsBracketNotation { get; set; }
    }

    /// <summary>
    /// Represents a JSON Path: see the information: https://goessner.net/articles/JsonPath/
    /// JSONPath Syntax
    /// As XPath, JSONPath also has syntax to follow:
    /// $ – symbol refers to the root object or element.
    /// @ – symbol refers to the current object or element.
    /// . – operator is the dot-child operator, which you use to denote a child element of the current element.
    /// [ ] – is the subscript operator, which you use to denote a child element of the current element (by name or index).
    /// * – operator is a wildcard, returning all objects or elements regardless of their names.
    /// , – operator is the union operator, which returns the union of the children or indexes indicated.
    /// : – operator is the array slice operator, so you can slice collections using the syntax[start:end:step] to return a subcollection of a collection.
    /// ( ) – operator lets you pass a script expression in the underlying implementation’s script language.It’s not supported by every implementation of JSONPath, however.
    /// ? ( ) – to query all items that meet a certain criteria
    /// </summary>
    internal class JsonPath
    {
        private Stack<JsonPathSegment> _path;
        private JsonPathOptions _options;

        public JsonPath()
            : this(JsonPathOptions.Default)
        {
            _path = new Stack<JsonPathSegment>();
        }

        public JsonPath(JsonPathOptions options)
        {
            _path = new Stack<JsonPathSegment>();
            _options = options;
        }

        // We use this class to report a JSON error location.
        // So, we don't care about "*" etc.
        public void Push(JsonPathSegment segment)
        {
            _path.Push(segment);
        }

        public void Push(string node)
        {
            _path.Push(new JsonNodeSegment(node));
        }

        public void Push(int index)
        {
            _path.Push(new JsonIndexSegment(index));
        }

        public JsonPathSegment Pop()
        {
            return _path.Pop();
        }

        public override string ToString()
        {
            // $ The root object or array.
            string root = "$";

            string[] segments = new string[_path.Count + 1];
            segments[0] = root;
            int index = _path.Count;
            foreach (var segment in _path)
            {
                segments[index--] = segment.GetName(_options.IsBracketNotation);
            }

            if (_options.IsBracketNotation)
            {
                return string.Join("", segments);
            }
            else
            {
                return string.Join(".", segments);
            }
        }
    }

    public abstract class JsonPathSegment
    {
        public abstract string GetName(bool isBracketNotation);
    }

    public class JsonNodeSegment : JsonPathSegment
    {
        public JsonNodeSegment(string node)
        {
            Node = node;
        }

        public string Node { get; }

        public override string GetName(bool isBracketNotation)
        {
            if (isBracketNotation)
            {
                return "['" + Node + "']";
            }

            return Node;
        }
    }

    public class JsonIndexSegment : JsonPathSegment
    {
        public JsonIndexSegment(int index)
        {
            Index = index;
        }

        public int Index { get; }

        public override string GetName(bool isBracketNotation)
        {
            return string.Format(CultureInfo.InvariantCulture, "[{0}]", Index);
        }
    }

}
