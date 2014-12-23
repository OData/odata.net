//   OData .NET Libraries ver. 6.9
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
