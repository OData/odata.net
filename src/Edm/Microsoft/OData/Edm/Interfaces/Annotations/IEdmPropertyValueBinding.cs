//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Annotations
{
    /// <summary>
    /// Represents a property binding specified as part of an EDM type annotation.
    /// </summary>
    public interface IEdmPropertyValueBinding : IEdmElement
    {
        /// <summary>
        /// Gets the property that is given a value by the annotation.
        /// </summary>
        IEdmProperty BoundProperty { get; }

        /// <summary>
        /// Gets the expression producing the value of the annotation.
        /// </summary>
        IEdmExpression Value { get; }
    }
} 
