//---------------------------------------------------------------------
// <copyright file="CodeTypeOfExpression.cs" company="Microsoft">
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
    ///       Represents a TypeOf expression.
    ///    </para>
    /// </devdoc>
    [
        ClassInterface(ClassInterfaceType.None),
        ComVisible(true),
#if FRAMEWORK 
        //Serializable, 
#endif
]
    public class CodeTypeOfExpression : CodeExpression {
        private CodeTypeReference type;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='System.CodeDom.CodeTypeOfExpression'/>.
        ///    </para>
        /// </devdoc>
        public CodeTypeOfExpression() {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='System.CodeDom.CodeTypeOfExpression'/>.
        ///    </para>
        /// </devdoc>
        public CodeTypeOfExpression(CodeTypeReference type) {
            Type = type;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeTypeOfExpression(string type) {
            Type = new CodeTypeReference(type);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeTypeOfExpression(Type type) {
            Type = new CodeTypeReference(type);
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the data type.
        ///    </para>
        /// </devdoc>
        public CodeTypeReference Type {
            get {
                if (type == null) {
                    type = new CodeTypeReference("");
                }
                return type;
            }
            set {
                type = value;
            }
        }
    }
}
