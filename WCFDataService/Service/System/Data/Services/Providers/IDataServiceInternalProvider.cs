//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
