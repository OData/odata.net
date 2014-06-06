//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Providers
{
    /// <summary>
    /// Enumeration for classifying the different kinds of operation parameter binding.
    /// Defect #1917144 - OperationParameterBindingKind should be refacored in Service/Test as we use bound or unbound to indicate binding kind now.
    /// </summary>
    public enum OperationParameterBindingKind
    {
        /// <summary>
        /// Used when the first parameter of a service action is not a binding parameter.
        /// </summary>
        Never = 0,

        /// <summary>
        /// Used when the first parameter of a service action is a binding parameter and some or all instances of the binding parameter type 
        /// may be bound to the service action.
        /// </summary>
        Sometimes = 1,

        /// <summary>
        /// Used when the first parameter of a service action is a binding parameter and all instances of the binding parameter type 
        /// must be bound to the service action.
        /// </summary>
        /// <remarks>When this value is set, the <see cref="IDataServiceActionProvider.AdvertiseServiceAction"/> method will not be called for the service action."/> </remarks>
        Always = 2,
    }
}
