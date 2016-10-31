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
