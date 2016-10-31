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
    using System.Diagnostics;
    #endregion

    /// <summary>
    /// Default implementation of IDataServiceProviderBehavior interface.
    /// </summary>
    internal class DataServiceProviderBehavior : IDataServiceProviderBehavior
    {
        /// <summary>
        /// Cached custom provider behavior instance.
        /// </summary>
        private static readonly DataServiceProviderBehavior customProviderBehavior = new DataServiceProviderBehavior(new ProviderBehavior(ProviderQueryBehaviorKind.CustomProviderQueryBehavior));

        /// <summary>
        /// Constructs the default DataServiceProviderBehavior using the given behavior information.
        /// </summary>
        /// <param name="providerBehavior">Provider behavior information.</param>
        internal DataServiceProviderBehavior(ProviderBehavior providerBehavior)
        {
            Debug.Assert(providerBehavior != null, "providerBehavior != null");
            this.ProviderBehavior = providerBehavior;           
        }

        #region IDataServiceProviderBehavior
        
        /// <summary>
        /// ProviderBehavior information.
        /// </summary>
        public ProviderBehavior ProviderBehavior
        {
              get;
              private set;
        }

        #endregion IDataServiceProviderBehavior

        /// <summary>
        /// Default behavior object for custom providers.
        /// </summary>
        internal static IDataServiceProviderBehavior CustomDataServiceProviderBehavior
        {
            get
            {
                return DataServiceProviderBehavior.customProviderBehavior;
            }
        }

        /// <summary>
        /// Checks whether provider behaves like EntityFramework or Reflection service providers.
        /// </summary>
        /// <param name="providerBehavior">Provider behavior.</param>
        /// <returns>true if EntityFramework or Reflection provider behavior, false otherwise.</returns>
        internal static bool HasReflectionOrEntityFrameworkProviderQueryBehavior(IDataServiceProviderBehavior providerBehavior)
        {
            return GetBehavior(providerBehavior).ProviderQueryBehavior == ProviderQueryBehaviorKind.ReflectionProviderQueryBehavior ||
                   GetBehavior(providerBehavior).ProviderQueryBehavior == ProviderQueryBehaviorKind.EntityFrameworkProviderQueryBehavior;
        }

        /// <summary>
        /// Checks whether provider behaves like EntityFramework provider.
        /// </summary>
        /// <param name="providerBehavior">Provider behavior.</param>
        /// <returns>true if EntityFramework provider.</returns>
        internal static bool HasEntityFrameworkProviderQueryBehavior(IDataServiceProviderBehavior providerBehavior)
        {
            return GetBehavior(providerBehavior).ProviderQueryBehavior == ProviderQueryBehaviorKind.EntityFrameworkProviderQueryBehavior;
        }

        /// <summary>
        /// Checks whether provider behaves like Reflection provider.
        /// </summary>
        /// <param name="providerBehavior">Provider behavior.</param>
        /// <returns>true if Reflection provider.</returns>
        internal static bool HasReflectionProviderQueryBehavior(IDataServiceProviderBehavior providerBehavior)
        {
            return GetBehavior(providerBehavior).ProviderQueryBehavior == ProviderQueryBehaviorKind.ReflectionProviderQueryBehavior;
        }

        /// <summary>
        /// Gets the ProviderBehavior instance obtained from IDataServiceProviderBehavior interface.
        /// </summary>
        /// <param name="providerBehavior">IDataServiceProviderBehavior interface implementation.</param>
        /// <returns>ProviderBehavior instance.</returns>
        internal static ProviderBehavior GetBehavior(IDataServiceProviderBehavior providerBehavior)
        {
            ProviderBehavior behavior = providerBehavior.ProviderBehavior;
            if (behavior == null)
            {
                throw new InvalidOperationException(Strings.DataServiceProviderBehavior_ProviderBehaviorMustBeNonNull);
            }

            return behavior;
        }
    }
}
