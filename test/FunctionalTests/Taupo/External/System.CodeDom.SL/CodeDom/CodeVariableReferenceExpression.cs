//---------------------------------------------------------------------
// <copyright file="CodeVariableReferenceExpression.cs" company="Microsoft">
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
    ///       Represents a reference to a field.
    ///    </para>
    /// </devdoc>
    [
        ClassInterface(ClassInterfaceType.None),
        ComVisible(true),
#if FRAMEWORK 
        //Serializable, 
#endif
]
    public class CodeVariableReferenceExpression : CodeExpression {
        private string variableName;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='System.CodeDom.CodeVariableReferenceExpression'/>.
        ///    </para>
        /// </devdoc>
        public CodeVariableReferenceExpression() {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='System.CodeDom.CodeArgumentReferenceExpression'/>.
        ///    </para>
        /// </devdoc>
        public CodeVariableReferenceExpression(string variableName) {
            this.variableName = variableName;
        }


        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string VariableName {
            get {
                return (variableName == null) ? string.Empty : variableName;
            }
            set {
                variableName = value;
            }
        }
    }
}
