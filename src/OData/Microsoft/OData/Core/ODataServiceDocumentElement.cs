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

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    #endregion Namespaces

    /// <summary>
    /// Abstract class representing an element (EntitySet, Singleton) in a service document.
    /// </summary>
    public abstract class ODataServiceDocumentElement : ODataAnnotatable
    {
        /// <summary>Gets or sets the URI representing the Unified Resource Locator (URL) to the element.</summary>
        /// <returns>The URI representing the Unified Resource Locator (URL) to the element.</returns>
        public Uri Url
        {
            get;
            set;
        }

        /// <summary>Gets or sets the name of the element; this is the entity set or singleton name in JSON and the HREF in Atom.</summary>
        /// <returns>The name of the element.</returns>
        /// <remarks>
        /// This property is required when reading and writing the JSON light format.
        /// If present in ATOM, it will be used to populate the title element.
        /// </remarks>
        public string Name
        {
            get;
            set;
        }

        /// <summary>Gets or sets the title of the element; this is the title in JSON.</summary>
        /// <returns>The title of the element.</returns>
        /// <remarks>
        /// This property is optional in JSON light format, containing a human-readable, language-dependent title for the object.
        /// The value is null if it is not present.
        /// </remarks>
        public string Title
        {
            get;
            set;
        }
    }
}
