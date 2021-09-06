//---------------------------------------------------------------------
// <copyright file="SafeCodeGeneratorOptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;

    /// <summary>
    /// Provides a way to access the functionality of <see cref="CodeGeneratorOptions"/>
    /// in partial trust scenarios.
    /// </summary>
    public class SafeCodeGeneratorOptions
    {
        private CodeGeneratorOptions options = new CodeGeneratorOptions();

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeCodeGeneratorOptions"/> class.
        /// </summary>
        [SecuritySafeCritical]
        public SafeCodeGeneratorOptions()
        {
            this.options = new CodeGeneratorOptions();
        }

        /// <summary>
        /// Gets or sets a value indicating whether to insert blank lines between members.
        /// </summary>
        public bool BlankLinesBetweenMembers
        {
            [SecuritySafeCritical]
            get { return this.options.BlankLinesBetweenMembers; }

            [SecuritySafeCritical]
            set { this.options.BlankLinesBetweenMembers = value; }
        }

        /// <summary>
        /// Gets or sets the style to use for bracing.
        /// </summary>
        public string BracingStyle
        {
            [SecuritySafeCritical]
            get { return this.options.BracingStyle; }

            [SecuritySafeCritical]
            set { this.options.BracingStyle = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to append an else, catch, or finally block,
        /// including brackets, at the closing line of each previous if or try block.
        /// </summary>
        public bool ElseOnClosing
        {
            [SecuritySafeCritical]
            get { return this.options.ElseOnClosing; }

            [SecuritySafeCritical]
            set { this.options.ElseOnClosing = value; }
        }

        /// <summary>
        /// Gets or sets the string to use for indentations.
        /// </summary>
        public string IndentString
        {
            [SecuritySafeCritical]
            get { return this.options.IndentString; }

            [SecuritySafeCritical]
            set { this.options.IndentString = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to generate members in the order
        /// in which they occur in member collections.
        /// </summary>
        public bool VerbatimOrder
        {
            [SecuritySafeCritical]
            get { return this.options.VerbatimOrder; }

            [SecuritySafeCritical]
            set { this.options.VerbatimOrder = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        /// <param name="index">The name associated with the object to retrieve.</param>
        public object this[string index]
        {
            [SecuritySafeCritical]
            get { return this.options[index]; }

            [SecuritySafeCritical]
            set { this.options[index] = value; }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Microsoft.Test.Taupo.Contracts.SafeCodeGeneratorOptions"/> to
        /// <see cref="System.CodeDom.Compiler.CodeGeneratorOptions"/>.
        /// </summary>
        /// <param name="options">The options to convert.</param>
        /// <returns>The result of the conversion.</returns>
        [SecuritySafeCritical]
        public static implicit operator CodeGeneratorOptions(SafeCodeGeneratorOptions options)
        {
            return options == null ? null : options.ToCodeGeneratorOptions();
        }

        /// <summary>
        /// Converts this <see cref="Microsoft.Test.Taupo.Contracts.SafeCodeGeneratorOptions"/> instance to
        /// <see cref="System.CodeDom.Compiler.CodeGeneratorOptions"/>.
        /// </summary>
        /// <returns>The result of the conversion.</returns>
        [SecuritySafeCritical]
        public CodeGeneratorOptions ToCodeGeneratorOptions()
        {
            return new CodeGeneratorOptions()
            {
                BlankLinesBetweenMembers = this.BlankLinesBetweenMembers,
                BracingStyle = this.BracingStyle,
                ElseOnClosing = this.ElseOnClosing,
                IndentString = this.IndentString,
                VerbatimOrder = this.VerbatimOrder
            };
        }
    }
}
