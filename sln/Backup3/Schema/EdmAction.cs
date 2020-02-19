//---------------------------------------------------------------------
// <copyright file="EdmAction.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM action.
    /// </summary>
    public class EdmAction : EdmOperation, IEdmAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmAction"/> class.
        /// </summary>
        /// <param name="namespaceName">Name of the namespace.</param>
        /// <param name="name">The name.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="isBound">if set to <c>true</c> [is bound].</param>
        /// <param name="entitySetPathExpression">The entity set path expression.</param>
        public EdmAction(string namespaceName, string name, IEdmTypeReference returnType, bool isBound, IEdmPathExpression entitySetPathExpression)
            : base(namespaceName, name, returnType, isBound, entitySetPathExpression)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmAction"/> class.
        /// </summary>
        /// <param name="namespaceName">Name of the namespace.</param>
        /// <param name="name">The name.</param>
        /// <param name="returnType">Type of the return.</param>
        public EdmAction(string namespaceName, string name, IEdmTypeReference returnType)
            : base(namespaceName, name, returnType)
        {
        }

        /// <summary>
        /// Gets the element kind of this schema element kind which is an Action.
        /// </summary>
        public override EdmSchemaElementKind SchemaElementKind
        {
            get { return EdmSchemaElementKind.Action; }
        }
    }
}
