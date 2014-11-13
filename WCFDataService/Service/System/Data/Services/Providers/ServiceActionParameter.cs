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
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    /// <summary>Use this type to represent a parameter on a service action.</summary>
    [DebuggerVisualizer("ServiceActionParameter={Name}")]
    public class ServiceActionParameter : OperationParameter
    {
        /// <summary>Empty parameter collection.</summary>
        internal static readonly ReadOnlyCollection<ServiceActionParameter> EmptyServiceActionParameterCollection = new ReadOnlyCollection<ServiceActionParameter>(new ServiceActionParameter[0]);

        /// <summary> Initializes a new <see cref="T:System.Data.Services.Providers.ServiceActionParameter" />. </summary>
        /// <param name="name">Name of parameter.</param>
        /// <param name="parameterType">resource type of parameter value.</param>
        public ServiceActionParameter(string name, ResourceType parameterType)
            : base(name, parameterType)
        {
        }
    }
}
