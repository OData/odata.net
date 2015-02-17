//---------------------------------------------------------------------
// <copyright file="AggregateODataObjectModelValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Common
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Contracts;
    #endregion Namespaces

    /// <summary>
    /// Implementation of IODataObjectModelValidator which aggregates multiple other validators.
    /// </summary>
    public class AggregateODataObjectModelValidator : IODataObjectModelValidator
    {
        /// <summary>
        /// List of validators to use.
        /// </summary>
        private List<IODataObjectModelValidator> validators;

        /// <summary>
        /// Set to true once validators have been initialized.
        /// </summary>
        private bool validatorsInitialized = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AggregateODataObjectModelValidator()
        {
            this.validators = new List<IODataObjectModelValidator>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public AggregateODataObjectModelValidator(AggregateODataObjectModelValidator original)
        {
            this.validators = new List<IODataObjectModelValidator>(original.validators);
        }

        /// <summary>
        /// Adds a new validator to the list of validators.
        /// </summary>
        /// <param name="validator">The validator to add.</param>
        public void AddValidator(IODataObjectModelValidator validator)
        {
            ExceptionUtilities.CheckArgumentNotNull(validator, "validator");

            this.validators.Add(validator);
        }

        /// <summary>
        /// Validates the OData object model.
        /// </summary>
        /// <param name="objectModelRoot">The root of the object model.</param>
        public void ValidateODataObjectModel(object objectModelRoot)
        {
            if (!this.validatorsInitialized)
            {
                this.validatorsInitialized = true;
                this.InitializeValidators();
            }

            foreach (IODataObjectModelValidator validator in this.validators)
            {
                validator.ValidateODataObjectModel(objectModelRoot);
            }
        }

        /// <summary>
        /// Called before the validation is performed to initialize validators.
        /// </summary>
        protected virtual void InitializeValidators()
        {
        }
    }
}
