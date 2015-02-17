//---------------------------------------------------------------------
// <copyright file="CodeRegionMode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.CodeDom {

    using System.Diagnostics;
    using System.Runtime.InteropServices;

    [
        ComVisible(true),
#if FRAMEWORK
        //Serializable,
#endif
]
    public enum CodeRegionMode {
        None = 0,
        Start = 1,
        End = 2,
    }
}
