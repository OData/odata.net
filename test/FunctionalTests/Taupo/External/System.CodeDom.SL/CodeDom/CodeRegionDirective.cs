//---------------------------------------------------------------------
// <copyright file="CodeRegionDirective.cs" company="Microsoft">
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
    public class CodeRegionDirective: CodeDirective {
        private string regionText;
        private CodeRegionMode regionMode;

        public CodeRegionDirective() {
        }
        
        public CodeRegionDirective(CodeRegionMode regionMode, string regionText) {
            this.RegionText = regionText;
            this.regionMode = regionMode;
        }

        public string RegionText {
            get {
                return (regionText == null) ? string.Empty : regionText;
            }
            set {
                regionText = value;
            }
        }
                
        public CodeRegionMode RegionMode {
            get {
                return regionMode;
            }
            set {
                regionMode = value;
            }
        }
    }
}
