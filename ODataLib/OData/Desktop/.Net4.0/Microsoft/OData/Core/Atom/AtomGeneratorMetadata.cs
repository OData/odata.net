//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System;
    #endregion Namespaces

    /// <summary>
    /// Atom metadata description of a content generator.
    /// </summary>
    public sealed class AtomGeneratorMetadata
    {
        /// <summary>Gets or sets the human readable name of the generator of the content.</summary>
        /// <returns>The human readable name of the generator of the content.</returns>
        public string Name
        {
            get;
            set;
        }

        /// <summary>Gets or sets the (optional) URI describing the generator of the content.</summary>
        /// <returns>The (optional) URI describing the generator of the content.</returns>
        public Uri Uri
        {
            get;
            set;
        }

        /// <summary>Gets or sets the (optional) version of the generator.</summary>
        /// <returns>The (optional) version of the generator.</returns>
        public string Version
        {
            get;
            set;
        }
    }
}
