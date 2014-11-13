//   OData .NET Libraries ver. 6.8.1
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

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents an EDM action import.
    /// </summary>
    public class EdmActionImport : EdmOperationImport, IEdmActionImport
    {
        private static readonly string ActionArgumentNullParameterName = "action";

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmActionImport"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        public EdmActionImport(IEdmEntityContainer container, string name, IEdmAction action)
            : this(container, name, action, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmActionImport"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="name">The name.</param>
        /// <param name="action">The action.</param>
        /// <param name="entitySetExpression">The entity set expression.</param>
        public EdmActionImport(IEdmEntityContainer container, string name, IEdmAction action, IEdmExpression entitySetExpression)
            : base(container, action, name, entitySetExpression)
        {
            EdmUtil.CheckArgumentNull(action, "action");

            this.Action = action;
        }

        /// <summary>
        /// Gets the action type of the import.
        /// </summary>
        public IEdmAction Action { get; private set; }

        /// <summary>
        /// Gets the kind of this actionimport, which is always ActionImport.
        /// </summary>
        public override EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.ActionImport; }
        }

        /// <summary>
        /// Indicates the name of the constructor argument that is passed to EdmOperationImport.
        /// </summary>
        /// <returns>Returns the name of the operation from this import.</returns>
        protected override string OperationArgumentNullParameterName()
        {
            return ActionArgumentNullParameterName;
        }
    }
}
