//---------------------------------------------------------------------
// <copyright file="ReaderDefaultODataObjectModelValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Common;
    #endregion Namespaces

    /// <summary>
    /// Implementation of IODataObjectModelValidator which uses the default set of validators for readers.
    /// </summary>
    [ImplementationName(typeof(IODataObjectModelValidator), "ReaderDefaultODataObjectModelValidator")]
    public class ReaderDefaultODataObjectModelValidator : AggregateODataObjectModelValidator
    {
        [InjectDependency(IsRequired = true)]
        public ReaderEnumerablesODataObjectModelValidator EnumerableValidator { get; set; }

        [InjectDependency(IsRequired = true)]
        public ReaderAbsoluteUriODataObjectModelValidator AbsoluteUriValidator { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ReaderDefaultODataObjectModelValidator()
        {
        }

        /// <summary>
        /// Called before the validation is performed to initialize validators.
        /// </summary>
        protected override void InitializeValidators()
        {
            base.InitializeValidators();

            // Add validators
            this.AddValidator(this.EnumerableValidator);
            this.AddValidator(this.AbsoluteUriValidator);
        }
    }
}