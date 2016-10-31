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

namespace Microsoft.Data.OData.Query.SemanticAst
{
    #region Namespaces
    using System;
    using Microsoft.Data.Edm;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// A rangeVariable from an Any or All that doesn't refer to an entity set
    /// </summary>
    public sealed class NonentityRangeVariable : RangeVariable
    {
        /// <summary>
        ///  The name of the associated rangeVariable
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The collection that this rangeVariable node iterates over, can be null in the case of
        /// single value nodes.
        /// </summary>
        private readonly CollectionNode collectionNode;

        /// <summary>
        /// The type of the value the range variable represents
        /// </summary>
        private readonly IEdmTypeReference typeReference;

        /// <summary>
        /// Creates a <see cref="NonentityRangeVariable"/>.
        /// </summary>
        /// <param name="name"> The name of the associated range variable.</param>
        /// <param name="typeReference">The type of the value the range variable represents.</param>
        /// <param name="collectionNode">The collection that this rangeVariable node iterates over, can be null in the case of single value nodes.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input name is null.</exception>
        /// <exception cref="ArgumentException">Throws if the input type reference is an entity type.</exception>
        public NonentityRangeVariable(string name, IEdmTypeReference typeReference, CollectionNode collectionNode)
        {
            ExceptionUtils.CheckArgumentNotNull(name, "name");
            this.name = name;
            if (typeReference != null)
            {
                if (typeReference.Definition.TypeKind == EdmTypeKind.Entity)
                {
                    throw new ArgumentException(
                        ODataErrorStrings.Nodes_NonentityParameterQueryNodeWithEntityType(typeReference.FullName()));
                }
            }

            this.typeReference = typeReference;
            this.collectionNode = collectionNode;
        }

        /// <summary>
        /// Gets the name of the associated rangevariable.
        /// </summary>
        public override string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the type of the value the range variable represents.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get { return this.typeReference; }
        }

        /// <summary>
        /// Gets the collection that this rangeVariable node iterates over, can be null in the case of single value nodes.
        /// </summary>
        public CollectionNode CollectionNode
        {
            get { return this.collectionNode; }
        }

        /// <summary>
        /// Gets the kind of this range variable.
        /// </summary>
        public override int Kind
        {
            get { return RangeVariableKind.Nonentity; }
        }
    }
}
