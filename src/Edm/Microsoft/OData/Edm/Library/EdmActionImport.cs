//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
