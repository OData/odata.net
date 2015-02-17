//---------------------------------------------------------------------
// <copyright file="NonentityRangeVariable.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces
    using System;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
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