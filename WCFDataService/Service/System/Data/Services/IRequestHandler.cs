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

namespace System.Data.Services
{
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Web;    
    
    /// <summary>
    /// This interface declares the service contract for a DataWeb
    /// service.
    /// </summary>
    [ServiceContract]
    public interface IRequestHandler
    {
        /// <summary>Provides an entry point for the request. </summary>
        /// <returns>The resulting message for the supplied request.</returns>
        /// <param name="messageBody">The <see cref="T:System.IO.Stream" /> object that contains the request.</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "*", Method = "*")]
        Message ProcessRequestForMessage(Stream messageBody);
    }
}
