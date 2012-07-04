//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Represents a single entity.
    /// </summary>
    [DebuggerDisplay("Id: {Id} TypeName: {TypeName}")]
    public sealed class ODataEntry : ODataItem
    {
        /// <summary>
        /// Entry ETag.
        /// </summary>
        public string ETag 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Entry ID.
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Link used to edit the entry.
        /// </summary>
        public Uri EditLink
        {
            get;
            set;
        }

        /// <summary>
        /// The default media resource of the media link entry.
        /// The default media resource does not have a name.
        /// </summary>
        public ODataStreamReferenceValue MediaResource
        {
            get;
            set;
        }

        /// <summary>
        /// The association links.
        /// </summary>
        public IEnumerable<ODataAssociationLink> AssociationLinks
        {
            get;
            set;
        }

        /// <summary>
        /// The actions advertised for the entry.
        /// </summary>
        public IEnumerable<ODataAction> Actions
        {
            get;
            set;
        }

        /// <summary>
        /// The functions advertised for the entry.
        /// </summary>
        public IEnumerable<ODataFunction> Functions
        {
            get;
            set;
        }
        
        /// <summary>
        /// Properties of the entry.
        /// </summary>
        /// <remarks>
        /// Non-property content goes to annotations.
        /// </remarks>
        public IEnumerable<ODataProperty> Properties
        { 
            get; 
            set; 
        }

        /// <summary>
        /// A link that can be used to read the entry.
        /// </summary>
        public Uri ReadLink
        {
            get;
            set;
        }

        /// <summary>
        /// Name of entry's type.
        /// </summary>
        public string TypeName
        {
            get;
            set;
        }
    }
}
