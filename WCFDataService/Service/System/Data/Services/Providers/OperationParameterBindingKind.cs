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
    /// <summary> Enumeration for classifying the different kinds of operation parameter binding. </summary>
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
