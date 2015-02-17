//---------------------------------------------------------------------
// <copyright file="CodeThrowExceptionStatement.cs" company="Microsoft">
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
    ///       Represents
    ///       a statement that throws an exception.
    ///    </para>
    /// </devdoc>
    [
        ClassInterface(ClassInterfaceType.None),
        ComVisible(true),
        //Serializable,
    ]
    public class CodeThrowExceptionStatement : CodeStatement {
        private CodeExpression toThrow;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='System.CodeDom.CodeThrowExceptionStatement'/>.
        ///    </para>
        /// </devdoc>
        public CodeThrowExceptionStatement() {
        }
        
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='System.CodeDom.CodeThrowExceptionStatement'/> using the specified statement.
        ///    </para>
        /// </devdoc>
        public CodeThrowExceptionStatement(CodeExpression toThrow) {
            ToThrow = toThrow;
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the expression to throw.
        ///    </para>
        /// </devdoc>
        public CodeExpression ToThrow {
            get {
                return toThrow;
            }
            set {
                toThrow = value;
            }
        }
    }
}
