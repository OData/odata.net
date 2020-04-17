//---------------------------------------------------------------------
// <copyright file="IJsonPath.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Json
{
    /// <summary>
    /// Represents a JSON Path: see the information: https://goessner.net/articles/JsonPath/
    /// For simplicity, it's only supporting the property and array index
    /// </summary>
    internal interface IJsonPath
    {
        /// <summary>
        /// Gets the path string.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Push a property node.
        /// </summary>
        /// <param name="node">The string node.</param>
        void Push(string node);

        /// <summary>
        /// Push an array index node.
        /// </summary>
        /// <param name="index">The index node</param>
        void Push(int index);

        /// <summary>
        /// Pop the current node.
        /// </summary>
        void Pop();
    }
}
