//---------------------------------------------------------------------
// <copyright file="ExpectedErrorMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Represents an expected error message
    /// </summary>
    public class ExpectedErrorMessage
    {
        private string identifier;
        
        /// <summary>
        /// Initializes a new instance of the ExpectedErrorMessage class.
        /// </summary>
        /// <param name="identifier">Resource Identifier</param>
        /// <param name="args">Args to be added to resource string</param>
        public ExpectedErrorMessage(string identifier, params string[] args)
        {
            ExceptionUtilities.CheckArgumentNotNull(identifier, "identifier");
            ExceptionUtilities.CheckArgumentNotNull(args, "args");

            this.identifier = identifier;
            this.Arguments = new Collection<string>();
            args.ForEach(a => this.Arguments.Add(a.Length > 1024 ? a.Substring(0, 1024 - 3) + "..." : a));

            this.SkipInnerErrorVerification = false;
        }

        /// <summary>
        /// Gets or sets the resource identifier
        /// </summary>
        public string ResourceIdentifier
        {
            get 
            {
                return this.identifier; 
            }

            set
            {
                ExceptionUtilities.CheckArgumentNotNull(value, "value");
                this.identifier = value;
            }
        }

        /// <summary>
        /// Gets the collection of arguments for the error
        /// </summary>
        public ICollection<string> Arguments { get; private set; }

        /// <summary>
        /// Gets or sets the optional specific verifier for this error. This is primarily used when inner exceptions are coming from different assemblies.
        /// </summary>
        public IStringResourceVerifier Verifier { get; set; }
        
        /// <summary>
        /// Gets or sets the inner error
        /// </summary>
        public ExpectedErrorMessage InnerError { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to skip inner error verification or not
        /// </summary>
        public bool SkipInnerErrorVerification { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Microsoft.Test.Taupo.Astoria.Contracts.ExpectedErrorMessage"/>.
        /// </summary>
        /// <param name="resourceIdentifier">Resource identifier for the error.</param>
        /// <returns>The result of the conversion.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Not needed here.")]
        public static implicit operator ExpectedErrorMessage(string resourceIdentifier)
        {
            return new ExpectedErrorMessage(resourceIdentifier);
        }

        /// <summary>
        /// Wraps the current error with the specified parent error and returns the parent error.
        /// </summary>
        /// <param name="parentErrorMessage">The parent error to wrap this error in.</param>
        /// <returns>The parent error</returns>
        public ExpectedErrorMessage AddParent(ExpectedErrorMessage parentErrorMessage)
        {
            parentErrorMessage.InnerError = this;
            if (parentErrorMessage.Verifier == null)
            {
                parentErrorMessage.Verifier = this.Verifier;
            }

            return parentErrorMessage;
        }
    }
}