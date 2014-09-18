//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    #endregion Namespaces

    /// <summary>Enumerates the behavior of readers when reading undeclared property.</summary>
    [Flags]
    public enum ODataUndeclaredPropertyBehaviorKinds
    {
        /// <summary>
        /// The default behavior - the reader will fail if it finds a property which is not declared by the model
        /// and the type is not open.
        /// </summary>
        None = 0,

        /// <summary>
        /// The reader will skip reading the property if it's not declared by the model and the current type is not open.
        /// </summary>
        /// <remarks>
        /// This flag can only be used when reading responses.
        /// All information about the undeclared property is going to be ignored, so for example ATOM metadata related to that property
        /// will not be reported either.
        /// </remarks>
        IgnoreUndeclaredValueProperty = 1,

        /// <summary>
        /// The reader will read and report link properties which are not declared by the model.
        /// </summary>
        /// <remarks>
        /// This flag can only be used when reading responses.
        /// If a link property in the payload is defined in the model it will be read as usual. If it is not declared
        /// it will still be read and reported, but it won't be validated against the model.
        /// 
        /// Link properties are:
        /// - Navigation links
        /// - Association links
        /// - Stream properties
        /// </remarks>
        ReportUndeclaredLinkProperty = 2,
    }
}
