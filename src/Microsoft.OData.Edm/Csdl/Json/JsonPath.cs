//---------------------------------------------------------------------
// <copyright file="JsonPath.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.OData.Edm.Csdl.Json
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
    /// 
    /// </summary>
    public interface IJsonPath
    {
        string Path { get; }

        void Push(string node);

        void Push(int index);

        void Pop();
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
    internal class JsonPath : IJsonPath
    {
        // It's only include "string" for property, and Int32 for the index of array
        private Stack<object> _nodes;
        private JsonPathOptions _options;
        private string _source;

        public JsonPath(string source = null)
            : this(JsonPathOptions.Default, source)
        {
        }

        public JsonPath(JsonPathOptions options, string source = null)
        {
            _nodes = new Stack<object>();
            _options = options;
            _source = source;
        }

        public JsonPath(JsonPath jsonPath)
        {
            _source = jsonPath._source;
            _options = jsonPath._options;
            _nodes = new Stack<object>(jsonPath._nodes);
        }


        public void Push(string node)
        {
            _nodes.Push(node);
        }

        public void Push(int index)
        {
            _nodes.Push(index);
        }

        public object Pop()
        {
            return _nodes.Pop();
        }

        public string Path { get { return ToString(); } }

        public override string ToString()
        {
            // $ The root object or array.
            string root = "$";
            if (_source != null)
            {
                root = "(" + _source + ")$";
            }

            string[] segments = new string[_nodes.Count + 1];
            segments[0] = root;
            int index = _nodes.Count;
            foreach (var segment in _nodes)
            {
                segments[index--] = GetName(segment);
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

        private string GetName(object node)
        {
            string strNode = node as string;
            if (strNode != null)
            {
                if (_options.IsBracketNotation)
                {
                    return "['" + strNode + "']";
                }

                return strNode;
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture, "[{0}]", (int)node);
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
