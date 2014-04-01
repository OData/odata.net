//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Represents a definition of an EDM association type.
    /// </summary>
    internal interface IEdmAssociation : IEdmNamedElement
    {
        /// <summary>
        /// Gets the namespace this association belongs to.
        /// </summary>
        string Namespace { get; }

        /// <summary>
        /// Gets the first end of the association.
        /// </summary>
        IEdmAssociationEnd End1 { get; }

        /// <summary>
        /// Gets the second end of the association.
        /// </summary>
        IEdmAssociationEnd End2 { get; }

        /// <summary>
        /// Gets the referential constraint of the association.
        /// </summary>
        CsdlSemanticsReferentialConstraint ReferentialConstraint { get; }
    }
}
