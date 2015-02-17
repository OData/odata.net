//---------------------------------------------------------------------
// <copyright file="CodeMemberAutoImplementedProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.CodeDomExtensions
{
    using System.CodeDom;

    /// <summary>
    /// Automatically-implemented property.
    /// </summary>
    public class CodeMemberAutoImplementedProperty : CodeMemberProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeMemberAutoImplementedProperty"/> class.
        /// </summary>
        public CodeMemberAutoImplementedProperty()
        {
            this.GetAttributes = MemberAttributes.Public;
            this.SetAttributes = MemberAttributes.Public;
        }

        /// <summary>
        /// Gets or sets the <see cref="MemberAttributes"/> applied to the get method
        /// of the generated property. Note that only access modifiers are respected.
        /// </summary>
        public MemberAttributes GetAttributes { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="MemberAttributes"/> applied to the set method
        /// of the generated property. Note that only access modifiers are respected.
        /// </summary>
        public MemberAttributes SetAttributes { get; set; }
    }
}