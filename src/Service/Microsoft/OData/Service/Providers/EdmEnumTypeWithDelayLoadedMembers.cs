//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client.Providers
#else
namespace Microsoft.OData.Service.Providers
#endif
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    #endregion Namespaces

    /// <summary>
    /// An <see cref="IEdmEnumType"/> implementation that supports delay-loading of enumeration members.
    /// </summary>
    internal class EdmEnumTypeWithDelayLoadedMembers : EdmEnumType
    {
        /// <summary>The lock object for the delayed property loading.</summary>
        private readonly object lockObject = new object();

        /// <summary>An action that is used to create the members for this type.</summary>
        private Action<EdmEnumTypeWithDelayLoadedMembers> memberLoadAction;

        /// <summary>
        /// Initializes a new instance of the EdmEnumTypeWithDelayLoadedMembers class.
        /// </summary>
        /// <param name="namespaceName">Namespace the enumeration belongs to.</param>
        /// <param name="name">Name of the enumeration Type.</param>
        /// <param name="underlyingType">The enum's underlying type, one of Edm.Byte, Edm.SByte, Edm.Int16, Edm.Int32, or Edm.Int64.</param>
        /// <param name="isFlags">The isFlags or not</param>
        /// <param name="memberLoadAction">An action that is used to create the members for this enum type.</param>
        internal EdmEnumTypeWithDelayLoadedMembers(
            string namespaceName,
            string name,
            IEdmPrimitiveType underlyingType,
            bool isFlags,
            Action<EdmEnumTypeWithDelayLoadedMembers> memberLoadAction)
            : base(namespaceName, name, underlyingType, isFlags)
        {
            Debug.Assert(memberLoadAction != null, "memberLoadAction != null");
            this.memberLoadAction = memberLoadAction;
        }

        /// <summary>
        /// Gets the enum members immediately within this type.
        /// </summary>
        public override IEnumerable<IEdmEnumMember> Members
        {
            get
            {
                this.EnsureMemberLoaded();
                return base.Members;
            }
        }

        /// <summary>
        /// Ensures that the properties have been loaded and can be used.
        /// </summary>
        private void EnsureMemberLoaded()
        {
            lock (this.lockObject)
            {
                if (this.memberLoadAction != null)
                {
                    this.memberLoadAction(this);
                    this.memberLoadAction = null;
                }
            }
        }
    }
}
