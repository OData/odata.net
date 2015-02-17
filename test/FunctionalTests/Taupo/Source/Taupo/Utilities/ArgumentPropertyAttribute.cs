//---------------------------------------------------------------------
// <copyright file="ArgumentPropertyAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;

    /// <summary>
    /// An Attribute to apply to Properties in a CommandLineProcessRunner to help automatically generate the arguments string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ArgumentPropertyAttribute : Attribute
    {
        private readonly string argumentName;
        private readonly ArgumentPropertyType propertyType;

        /// <summary>
        /// Initializes a new instance of the ArgumentPropertyAttribute class
        /// </summary>
        public ArgumentPropertyAttribute()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ArgumentPropertyAttribute class
        /// </summary>
        /// <param name="argumentName">The format of the argument.</param>
        /// <param name="propertyType">The type of the argument.</param>
        public ArgumentPropertyAttribute(string argumentName, ArgumentPropertyType propertyType = ArgumentPropertyType.Standard)
        {
            this.argumentName = argumentName;
            this.propertyType = propertyType;
        }

        /// <summary>
        /// Gets the format of the argument.
        /// </summary>
        public string ArgumentName
        {
            get { return this.argumentName; }
        }

        /// <summary>
        /// Gets a value indicating the Type of the Argument Property.
        /// </summary>
        public ArgumentPropertyType PropertyType
        {
            get { return this.propertyType; }
        }
    }
}
