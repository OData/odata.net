//---------------------------------------------------------------------
// <copyright file="CodeSnippetExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.CodeDom {

    using System.Diagnostics;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    /// <devdoc>
    ///    <para>
    ///       Represents a snippet expression.
    ///    </para>
    /// </devdoc>
    [
        ClassInterface(ClassInterfaceType.None),
        ComVisible(true),
#if FRAMEWORK 
        //Serializable, 
#endif
]
    public class CodeSnippetExpression : CodeExpression {
        private string value;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='System.CodeDom.CodeSnippetExpression'/>.
        ///    </para>
        /// </devdoc>
        public CodeSnippetExpression() {
        }
        
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='System.CodeDom.CodeSnippetExpression'/> using the specified snippet
        ///       expression.
        ///    </para>
        /// </devdoc>
        public CodeSnippetExpression(string value) {
            Value = value;
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the snippet expression.
        ///    </para>
        /// </devdoc>
        public string Value {
            get {
                return (value == null) ? string.Empty : value;
            }
            set {
                this.value = value;
            }
        }
    }
}
