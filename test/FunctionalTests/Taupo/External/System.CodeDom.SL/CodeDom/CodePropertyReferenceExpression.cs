//---------------------------------------------------------------------
// <copyright file="CodePropertyReferenceExpression.cs" company="Microsoft">
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
    ///       Represents a reference to a property.
    ///    </para>
    /// </devdoc>
    [
        ClassInterface(ClassInterfaceType.None),
        ComVisible(true),
        //Serializable,
    ]
    public class CodePropertyReferenceExpression : CodeExpression {
        private CodeExpression targetObject;
        private string propertyName;
        private CodeExpressionCollection parameters = new CodeExpressionCollection();

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='System.CodeDom.CodePropertyReferenceExpression'/>.
        ///    </para>
        /// </devdoc>
        public CodePropertyReferenceExpression() {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='System.CodeDom.CodePropertyReferenceExpression'/> using the specified target object and property
        ///       name.
        ///    </para>
        /// </devdoc>
        public CodePropertyReferenceExpression(CodeExpression targetObject, string propertyName) {
            TargetObject = targetObject;
            PropertyName = propertyName;
        }

        /// <devdoc>
        ///    <para>
        ///       The target object containing the property this <see cref='System.CodeDom.CodePropertyReferenceExpression'/> references.
        ///    </para>
        /// </devdoc>
        public CodeExpression TargetObject {
            get {
                return targetObject;
            }
            set {
                targetObject = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       The name of the property to reference.
        ///    </para>
        /// </devdoc>
        public string PropertyName {
            get {
                return (propertyName == null) ? string.Empty : propertyName;
            }
            set {
                propertyName = value;
            }
        }
    }
}
