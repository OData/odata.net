//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client
{
    using System;

    /// <summary>
    /// Encapsulates the arguments for the DataServiceContext ChangesSaved event
    /// </summary>
    internal class SaveChangesEventArgs : EventArgs
    {
        /// <summary>
        /// DataServiceContext SaveChanges response
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823", Justification = "No upstream callers.")]
        private DataServiceResponse response;

        /// <summary>
        /// Construct a DataServiceSaveChangesEventArgs object.
        /// </summary>
        /// <param name="response">DataServiceContext SaveChanges response</param>
        public SaveChangesEventArgs(DataServiceResponse response)
        {
            this.response = response;
        }
    }  
}
