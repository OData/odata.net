//---------------------------------------------------------------------
// <copyright file="JsonPath.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------


using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Reader
{
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
    public class JsonPath
    {
        

        private Stack<JsonPathSegment> _path;

        public JsonPath()
        {
            _path = new Stack<JsonPathSegment>();
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
    }

    public abstract class JsonPathSegment
    {

    }

    public class JsonNodeSegment : JsonPathSegment
    {
        public JsonNodeSegment(string node)
        {

        }
    }

    public class JsonIndexSegment : JsonPathSegment
    {
        public JsonIndexSegment(int index)
        {

        }
    }

}
