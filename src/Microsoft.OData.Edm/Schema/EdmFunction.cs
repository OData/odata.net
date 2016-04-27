//---------------------------------------------------------------------
// <copyright file="EdmFunction.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM function.
    /// </summary>
    public class EdmFunction : EdmOperation, IEdmFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmFunction"/> class.
        /// </summary>
        /// <param name="namespaceName">Name of the namespace.</param>
        /// <param name="name">The name.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="isBound">if set to <c>true</c> [is bound].</param>
        /// <param name="entitySetPathExpression">The entity set path expression.</param>
        /// <param name="isComposable">A value indicating if the function is composable or not.</param>
        public EdmFunction(string namespaceName, string name, IEdmTypeReference returnType, bool isBound, IEdmPathExpression entitySetPathExpression, bool isComposable)
            : base(namespaceName, name, returnType, isBound, entitySetPathExpression)
        {
            EdmUtil.CheckArgumentNull(returnType, "returnType");
            this.IsComposable = isComposable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmFunction"/> class.
        /// </summary>
        /// <param name="namespaceName">Namespace of the function.</param>
        /// <param name="name">Name of the function.</param>
        /// <param name="returnType">Return type of the function.</param>
        public EdmFunction(string namespaceName, string name, IEdmTypeReference returnType)
            : this(namespaceName, name, returnType, false /*isBound*/, null, false /*isComposable*/)
        {
        }

        /// <summary>
        /// Gets the element kind of this operation, which is always Operation.
        /// virtual will be removed in the near future, stop gap to enable testing for now.
        /// </summary>
        public override EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Function; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is composable.
        /// </summary>
        public bool IsComposable { get; private set; }
    }
}
