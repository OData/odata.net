//---------------------------------------------------------------------
// <copyright file="CodeDefaultValueExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.CodeDom {

    using System.Diagnostics;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    [
        ClassInterface(ClassInterfaceType.None),
        ComVisible(true),
#if FRAMEWORK
        //Serializable,
#endif
    ]
    public class CodeDefaultValueExpression : CodeExpression {
        private CodeTypeReference type;

        public CodeDefaultValueExpression() {
        }

        public CodeDefaultValueExpression(CodeTypeReference type) {
            this.type = type;
        }

        public CodeTypeReference Type {
            get {
                if( type == null) {
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
