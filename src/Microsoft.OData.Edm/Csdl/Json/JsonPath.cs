//---------------------------------------------------------------------
// <copyright file="JsonPath.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.OData.Edm.Csdl.Json
{
    /// <summary>
    /// Simply implement a JSON Path: see the information: https://goessner.net/articles/JsonPath/
    /// </summary>
    internal class JsonPath : IJsonPath
    {
        /// <summary>
        /// It's only include "string" for property, and Int32 for the index of array
        /// </summary>
        private Stack<object> nodes;

        /// <summary>
        /// The source of this path.
        /// </summary>
        private string source;

        /// <summary>
        /// If true: $['store']['book'][0]['title']
        /// If false: $.store.book[0].title
        /// </summary>
        private bool isBracketNotation;

        /// <summary>
        /// a value indicating whether this JSON path is changed outside.
        /// </summary>
        private bool dirty;

        /// <summary>
        /// a value caching the calcuated path.
        /// </summary>
        private string path;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPath"/> class.
        /// </summary>
        /// <param name="source">The source of this path, can be null..</param>
        /// <param name="isBracketNotation">If false, output path using "'", otherwise, output path using "[" and "]" </param>
        public JsonPath(string source = null, bool isBracketNotation = false)
        {
            this.source = source;
            this.isBracketNotation = isBracketNotation;
            this.nodes = new Stack<object>();
            this.dirty = true;
        }

        /// <summary>
        /// Gets the path string.
        /// </summary>
        public string Path
        {
            get
            {
                if (this.dirty)
                {
                    BuildPath();
                }

                return this.path;
            }
        }

        /// <summary>
        /// Push a property node.
        /// </summary>
        /// <param name="node">The string node.</param>
        public void Push(string node)
        {
            this.nodes.Push(node);
            this.dirty = true;
        }

        /// <summary>
        /// Push an array index node.
        /// </summary>
        /// <param name="index">The index node</param>
        public void Push(int index)
        {
            // Boxing is ok? There's a tradeoff, compare to create a wrapper,
            // Let's allow the index to boxing to "object".
            this.nodes.Push(index);
            this.dirty = true;
        }

        /// <summary>
        /// Pop the current node.
        /// </summary>
        public void Pop()
        {
            Debug.Assert(this.nodes.Count > 0);

            this.nodes.Pop();
            this.dirty = true;
        }

        /// <summary>
        /// Get the string of this JSON path.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            if (this.dirty)
            {
                BuildPath();
            }

            return this.path;
        }

        /// <summary>
        /// Construct the json path string
        /// </summary>
        private void BuildPath()
        {
            // $ for The root object or array.
            string root = "$";
            if (this.source != null)
            {
                root = "(" + this.source + ")$";
            }

            string[] segments = new string[this.nodes.Count + 1];
            segments[0] = root;
            int index = this.nodes.Count;
            foreach (var segment in this.nodes)
            {
                segments[index--] = GetName(segment);
            }

            if (this.isBracketNotation)
            {
                this.path = string.Join("", segments);
            }
            else
            {
                this.path = string.Join(".", segments);
            }

            this.dirty = false;
        }

        private string GetName(object node)
        {
            string strNode = node as string;
            if (strNode != null)
            {
                if (this.isBracketNotation)
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
}