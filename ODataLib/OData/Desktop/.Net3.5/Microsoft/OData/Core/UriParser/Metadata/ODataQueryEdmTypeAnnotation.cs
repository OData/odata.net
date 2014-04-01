//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Metadata
{
    #region Namespaces
    using System;
    using System.Diagnostics.CodeAnalysis;
    #endregion Namespaces

    /// <summary>
    /// In-memory annotation class to associate CLR instance types with 
    /// (non-primitive) EDM types.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Edm", Justification = "Camel-casing in class name.")]
    internal sealed class ODataQueryEdmTypeAnnotation
    {
        /// <summary>
        /// true if reflection over the instance type is supported; otherwise false.
        /// </summary>
        public bool CanReflectOnInstanceType
        {
            get;
            set;
        }

        /// <summary>
        /// The instance type represented by this annotation.
        /// </summary>
        public Type InstanceType
        {
            get;
            set;
        }
    }
}
