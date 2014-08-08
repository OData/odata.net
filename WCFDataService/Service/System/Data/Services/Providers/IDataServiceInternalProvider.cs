//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace System.Data.Services.Providers
{
    #region Namespaces
    using System.Collections.Generic;
    #endregion

    /// <summary>
    /// Internal interface whose implementation loads known types, validates the model and sets 
    /// all the metadata objects to read-only. Also used for obtaining entity container annotations.
    /// </summary>
    public interface IDataServiceInternalProvider
    {
        /// <summary>
        /// Called by the service to let the provider perform data model validation.
        /// </summary>
        /// <param name="knownTypes">Collection of known types.</param>
        /// <param name="useMetadataCacheOrder">Whether to use metadata cache ordering instead of default ordering.</param>
        void FinalizeMetadataModel(IEnumerable<Type> knownTypes, bool useMetadataCacheOrder);

        /// <summary>
        /// Return the list of custom annotation for the entity container with the given name.
        /// </summary>
        /// <param name="entityContainerName">Name of the EntityContainer.</param>
        /// <returns>Return the list of custom annotation for the entity container with the given name.</returns>
        IEnumerable<KeyValuePair<string, object>> GetEntityContainerAnnotations(string entityContainerName);
    }
}
