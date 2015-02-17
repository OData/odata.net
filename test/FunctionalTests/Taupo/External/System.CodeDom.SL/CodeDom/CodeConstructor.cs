//---------------------------------------------------------------------
// <copyright file="CodeConstructor.cs" company="Microsoft">
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
    ///       Represents a class constructor.
    ///    </para>
    /// </devdoc>
    [
        ClassInterface(ClassInterfaceType.None),
        ComVisible(true),
#if FRAMEWORK 
        //Serializable, 
#endif
]
    public class CodeConstructor : CodeMemberMethod {
        private CodeExpressionCollection baseConstructorArgs = new CodeExpressionCollection();
        private CodeExpressionCollection chainedConstructorArgs = new CodeExpressionCollection();

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='System.CodeDom.CodeConstructor'/>.
        ///    </para>
        /// </devdoc>
        public CodeConstructor() {
            Name = ".ctor";
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the base constructor arguments.
        ///    </para>
        /// </devdoc>
        public CodeExpressionCollection BaseConstructorArgs {
            get {
                return baseConstructorArgs;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the chained constructor arguments.
        ///    </para>
        /// </devdoc>
        public CodeExpressionCollection ChainedConstructorArgs {
            get {
                return chainedConstructorArgs;
            }
        }
    }
}
