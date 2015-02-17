//---------------------------------------------------------------------
// <copyright file="CodeArgumentReferenceExpression.cs" company="Microsoft">
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
        //Serializable,
    ]
    public class CodeArgumentReferenceExpression : CodeExpression {
        private string parameterName;

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeArgumentReferenceExpression() {
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeArgumentReferenceExpression(string parameterName) {
            this.parameterName = parameterName;
        }


        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string ParameterName {
            get {
                return (parameterName == null) ? string.Empty : parameterName;
            }
            set {
                parameterName = value;
            }
        }
    }
}
