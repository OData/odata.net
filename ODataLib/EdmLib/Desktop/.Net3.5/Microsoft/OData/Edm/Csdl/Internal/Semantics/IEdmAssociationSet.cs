//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Represents an EDM association set.
    /// </summary>
    internal interface IEdmAssociationSet : IEdmNamedElement
    {
        /// <summary>
        /// Gets the association of this association set.
        /// </summary>
        IEdmAssociation Association { get; }

        /// <summary>
        /// Gets the first end of this association set.
        /// </summary>
        IEdmAssociationSetEnd End1 { get; }

        /// <summary>
        /// Gets the second end of this association set.
        /// </summary>
        IEdmAssociationSetEnd End2 { get; }

        IEdmEntityContainer Container { get; }
    }
}
