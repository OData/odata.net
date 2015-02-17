//---------------------------------------------------------------------
// <copyright file="CodeObject.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.CodeDom {

    using System.Collections;
    using System.Collections.Specialized;
    using System.Runtime.Serialization;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;

    /// <devdoc>
    ///    <para>
    ///       The base class for CodeDom objects
    ///    </para>
    /// </devdoc>
    [
        ClassInterface(ClassInterfaceType.None),
        ComVisible(true),
        //Serializable,
    ]
    public class CodeObject 
    {
#if WIN8
        private IDictionary<object, object> userData = null;
#else
        private IDictionary userData = null;
#endif
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeObject() {
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
#if WIN8
        public IDictionary<object, object> UserData {
            get {
                if (userData == null) {
                    userData = new Dictionary<object, object>();
                }

                return userData;
            }
        }
#else
        public IDictionary UserData {
            get {
                if (userData == null) {
                    userData = new Dictionary<object, object>();
                }

                return userData;
            }
        }
#endif
    }
}
