//---------------------------------------------------------------------
// <copyright file="ODataUrlValidationContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser.Validation.ValidationEngine;

namespace Microsoft.OData.UriParser.Validation
{
    /// <summary>
    /// Context for validating an OData Url.
    /// </summary>
    public sealed class ODataUrlValidationContext
    {
        /// <summary>
        /// List of <see cref="ODataUrlValidationError"/>s discovered while validating the OData Url.
        /// </summary>
        public List<ODataUrlValidationError> Errors { get; private set; }

        /// <summary>
        /// The model against which the OData Url is to be validated.
        /// </summary>
        public IEdmModel Model { get; private set; }

        /// <summary>
        /// The ODataUrlValidator associated with this validation context.
        /// </summary>
        internal ODataUrlValidator UrlValidator { get; private set; }

        /// <summary>
        /// The ExpressionValidator used to validate an expression within the Url.
        /// </summary>
        internal ExpressionValidator ExpressionValidator { get; private set; }

        /// <summary>
        /// The ODataUrlValidator associated with this validation context.
        /// </summary>
        internal PathSegmentValidator PathValidator { get; private set; }

        /// <summary>
        /// Construct an ODataUrlValidationContext for a given model, associated with the specified <see cref="ODataUrlValidator"/>.
        /// </summary>
        /// <param name="model">The model against which teh OData Url is to be validated.</param>
        /// <param name="urlValidator">The <see cref="ODataUrlValidator"/> associated with this validation context.</param>
        internal ODataUrlValidationContext(IEdmModel model, ODataUrlValidator urlValidator)
        {
            this.Model = model;
            this.Errors = new List<ODataUrlValidationError>();
            this.UrlValidator = urlValidator;
            this.ExpressionValidator = new ExpressionValidator((item) => urlValidator.ValidateItem(item, this));
            this.PathValidator = new PathSegmentValidator(this);
        }

        /// <summary>
        /// Add an <see cref="ODataUrlValidationError"/> to the collection of validation errors.
        /// </summary>
        /// <param name="code">The error code of the error.</param>
        /// <param name="message">The error message.</param>
        /// <param name="severity">The severity of the error.</param>
        public void AddError(string code, string message, Severity severity)
        {
            this.Errors.Add(new ODataUrlValidationError(code, message, severity));
        }
    }
}