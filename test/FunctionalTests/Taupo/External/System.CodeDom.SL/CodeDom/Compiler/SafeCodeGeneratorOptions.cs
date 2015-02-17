//---------------------------------------------------------------------
// <copyright file="SafeCodeGeneratorOptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.CodeDom.Compiler {
    using System;
    using System.CodeDom;
    using System.Collections;
    using System.Collections.Specialized;
#if !WIN8
    using System.Security.Permissions;
#endif
    using System.Collections.Generic;


    /// <devdoc>
    ///    <para>
    ///       Represents options used in code generation
    ///    </para>
    /// </devdoc>
#if FRAMEWORK
    //[PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
    //[PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
#endif
    public class SafeCodeGeneratorOptions {
        private Dictionary<string, object> options = new Dictionary<string, object>();

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public SafeCodeGeneratorOptions() {
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public object this[string index] {
            get {
                return options[index];
            }
            set {
                options[index] = value;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string IndentString {
            get {
                object o = GetOption("IndentString");
                return ((o == null) ? "    " : (string)o);
            }
            set {
                options["IndentString"] = value;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string BracingStyle {
            get {
                object o = GetOption("BracingStyle");
                return ((o == null) ? "Block" : (string)o);
            }
            set {
                options["BracingStyle"] = value;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool ElseOnClosing {
            get {
                object o = GetOption("ElseOnClosing");
                return ((o == null) ? false : (bool)o);
            }
            set {
                options["ElseOnClosing"] = value;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public bool BlankLinesBetweenMembers {
            get {
                object o = GetOption("BlankLinesBetweenMembers");
                return ((o == null) ? true : (bool)o);
            }
            set {
                options["BlankLinesBetweenMembers"] = value;
            }
        }

        [System.Runtime.InteropServices.ComVisible(false)]
        public bool VerbatimOrder {
            get {
                object o = GetOption("VerbatimOrder");
                return ((o == null) ? false : (bool)o);
            }
            set {
                options["VerbatimOrder"] = value;
            }
        }

        private object GetOption(string name)
        {
            if (this.options.ContainsKey(name))
            {
                return this.options[name];
            }
            else
            {
                return null;
            }
        }
    }
}
