//---------------------------------------------------------------------
// <copyright file="CodeTypeParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.CodeDom {

    using System.Diagnostics;
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [
        ClassInterface(ClassInterfaceType.None),
        ComVisible(true),
#if FRAMEWORK   
    //Serializable, 
#endif
    ]
    public class CodeTypeParameter : CodeObject {
        private string name;
        private CodeAttributeDeclarationCollection customAttributes;
        private CodeTypeReferenceCollection constraints;
        private bool hasConstructorConstraint;

        public CodeTypeParameter() {
        }

        public CodeTypeParameter(string name) {
            this.name = name;
        }

        public string Name {
            get {
                return (name == null) ? string.Empty : name;
            }
            set {
                name = value;
            }
        }

        public CodeTypeReferenceCollection Constraints {  
            get {
                if (constraints == null) {
                    constraints = new CodeTypeReferenceCollection();
                }
                return constraints;
            }
        } 

        public CodeAttributeDeclarationCollection CustomAttributes {
            get {
                if (customAttributes == null) {
                    customAttributes = new CodeAttributeDeclarationCollection();
                }
                return customAttributes;
            }
        }

        public bool HasConstructorConstraint {
            get {
                return hasConstructorConstraint;
            } 
            set {
                hasConstructorConstraint = value;
            }
        }

    }
}


