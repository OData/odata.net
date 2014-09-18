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

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM value term.
    /// </summary>
    public interface IEdmValueTerm : IEdmSchemaElement, IEdmTerm
    {
        /// <summary>
        /// Gets the type of this term.
        /// </summary>
        IEdmTypeReference Type { get; }

        /// <summary>
        /// Gets the AppliesTo of this term.
        /// </summary>
        string AppliesTo { get; }

        /// <summary>
        /// Gets the DefaultValue of this term.
        /// </summary>
        string DefaultValue { get; }
    }
}
