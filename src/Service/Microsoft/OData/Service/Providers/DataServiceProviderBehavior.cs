//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System;
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
