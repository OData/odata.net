//---------------------------------------------------------------------
// <copyright file="CodeGotoStatement.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.CodeDom {

    using System.Diagnostics;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [
        ClassInterface(ClassInterfaceType.None),
        ComVisible(true),
#if FRAMEWORK 
        //Serializable, 
#endif
]
    public class CodeGotoStatement : CodeStatement {
        private string label;

        public CodeGotoStatement() {
        }
        
        public CodeGotoStatement(string label) {
            Label = label;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string Label {
            get {
                return label;
            }
            set {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");
                    
                this.label = value;
            }
        }
    }
}
