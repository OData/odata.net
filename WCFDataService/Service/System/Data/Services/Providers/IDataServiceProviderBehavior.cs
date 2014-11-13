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
    /// <summary>
    /// The kind of query processing behavior from the provider.
    /// </summary>
    public enum ProviderQueryBehaviorKind
    {
        /// <summary>
        /// Treat the provider query processing behavior similar to the reflection based provider.
        /// </summary>
        ReflectionProviderQueryBehavior,

        /// <summary>
        /// Treat the provider query processing behavior similar to the entity framework based provider.
        /// </summary>
        EntityFrameworkProviderQueryBehavior,

        /// <summary>
        /// Treat the provider query processing behavior as a custom provider.
        /// </summary>
        CustomProviderQueryBehavior
    }

    /// <summary>
    /// Used by the service writer to define the behavior of the providers.
    /// </summary>
    public interface IDataServiceProviderBehavior
    {
        /// <summary>
        /// Instance of provider behavior that defines the assumptions service should make
        /// about the provider.
        /// </summary>
        ProviderBehavior ProviderBehavior
        {
            get;
        }
    }

    /// <summary>
    /// Provider behavior encapsulates the runtime behavior of the provider. The service
    /// will check various properties the <see cref="ProviderBehavior"/> exposed by the <see cref="IDataServiceProviderBehavior"/> 
    /// to process the request.
    /// </summary>
    public class ProviderBehavior
    {
        /// <summary>
        /// Constructs a new instance of <see cref="ProviderBehavior"/>.
        /// </summary>
        /// <param name="queryBehaviorKind">Kind of query processing behavior for the provider.</param>
        public ProviderBehavior(ProviderQueryBehaviorKind queryBehaviorKind)
        {
            if (queryBehaviorKind != ProviderQueryBehaviorKind.CustomProviderQueryBehavior &&
                queryBehaviorKind != ProviderQueryBehaviorKind.ReflectionProviderQueryBehavior &&
                queryBehaviorKind != ProviderQueryBehaviorKind.EntityFrameworkProviderQueryBehavior)
            {
                throw new ArgumentOutOfRangeException("queryBehaviorKind");
            }

            this.ProviderQueryBehavior = queryBehaviorKind;
        }

        /// <summary>
        /// The kind of behavior service should assume from the provider.
        /// </summary>
        public ProviderQueryBehaviorKind ProviderQueryBehavior 
        { 
            get; 
            private set; 
        }
    }
}
