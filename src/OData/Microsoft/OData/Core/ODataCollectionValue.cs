//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    #endregion Namespaces

    /// <summary>
    /// OData representation of a Collection.
    /// </summary>
    public sealed class ODataCollectionValue : ODataValue
    {
        /// <summary>Gets or sets the type of the collection value.</summary>
        /// <returns>The type of the collection value.</returns>
        public string TypeName
        {
            get;
            set;
        }

        /// <summary>Gets or sets the items in the bag value.</summary>
        /// <returns>The items in the bag value.</returns>
        public IEnumerable Items
        {
            get;
            set;
        }
    }
}
