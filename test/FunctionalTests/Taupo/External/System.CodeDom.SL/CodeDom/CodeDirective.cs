//---------------------------------------------------------------------
// <copyright file="CodeDirective.cs" company="Microsoft">
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
    public class CodeDirective: CodeObject {

    }
}

