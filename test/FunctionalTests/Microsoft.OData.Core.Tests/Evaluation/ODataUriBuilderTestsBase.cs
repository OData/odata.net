//---------------------------------------------------------------------
// <copyright file="ODataUriBuilderBaseTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Core.Evaluation;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Core.Tests.Evaluation
{
    public abstract class ODataUriBuilderTestsBase
    {
        private readonly Uri defaultBaseUri;
        private readonly IEdmStructuredValue defaultProductInstance;
        private readonly IEdmModel model;

        protected ODataUriBuilderTestsBase()
        {
            this.defaultBaseUri = new Uri("http://odata.org/base/");
            this.model = TestModel.BuildDefaultTestModel();
            this.defaultProductInstance = TestModel.BuildDefaultProductValue(TestModel.GetEntityType(this.model, "TestModel.Product"));
        }

        internal void BuildEntitySetUriShouldValidateArguments(ODataUriBuilder builder)
        {
            this.VerifyBaseUriAndStringNullOrEmptyArgumentValidation((baseUri, entitySet) => builder.BuildEntitySetUri(baseUri, entitySet), "entitySetName");
        }

        internal void BuildStreamEditLinkUriShouldValidateArguments(ODataUriBuilder builder)
        {
            this.VerifyBaseUriAndStringEmptyArgumentValidation((baseUri, streamPropertyName) => builder.BuildStreamEditLinkUri(baseUri, streamPropertyName), "streamPropertyName");
        }

        internal void BuildStreamReadLinkUriShouldValidateArguments(ODataUriBuilder builder)
        {
            this.VerifyBaseUriAndStringEmptyArgumentValidation((baseUri, streamPropertyName) => builder.BuildStreamReadLinkUri(baseUri, streamPropertyName), "streamPropertyName");
        }

        internal void BuildNavigationLinkUriShouldValidateArguments(ODataUriBuilder builder)
        {
            this.VerifyBaseUriAndStringNullOrEmptyArgumentValidation((baseUri, navigationPropertyName) => builder.BuildNavigationLinkUri(baseUri, navigationPropertyName), "navigationPropertyName");
        }

        internal void BuildAssociationLinkUriShouldValidateArguments(ODataUriBuilder builder)
        {
            this.VerifyBaseUriAndStringNullOrEmptyArgumentValidation((baseUri, associationLinkName) => builder.BuildAssociationLinkUri(baseUri, associationLinkName), "navigationPropertyName");
        }

        internal void BuildOperationTargetUriShouldValidateArguments(ODataUriBuilder builder)
        {
            this.VerifyBaseUriAndStringNullOrEmptyArgumentValidation((baseUri, operationName) => builder.BuildOperationTargetUri(baseUri, operationName, null, null), "operationName");
        }

        protected virtual void VerifyBaseUriArgumentValidation(Action<Uri> action)
        {
            // base class does not validate baseUri, so nothing to do here
        }

        private void VerifyStringNullOrEmptyArgumentValidation(Action<string> action, string argumentName)
        {
            action.ShouldThrowOnNullOrEmptyStringArgument(argumentName);
        }

        private void VerifyStringEmptyArgumentValidation(Action<string> action, string argumentName)
        {
            action.ShouldThrowOnEmptyStringArgument(argumentName);
        }

        private void VerifyBaseUriAndStringNullOrEmptyArgumentValidation(Action<Uri, string> action, string argumentName)
        {
            this.VerifyBaseUriArgumentValidation((baseUri) => action(baseUri, "StringValue"));
            this.VerifyStringNullOrEmptyArgumentValidation((stringArg) => action(this.defaultBaseUri, stringArg), argumentName);
        }

        private void VerifyBaseUriAndStringEmptyArgumentValidation(Action<Uri, string> action, string argumentName)
        {
            this.VerifyBaseUriArgumentValidation((baseUri) => action(baseUri, "StringValue"));
            this.VerifyStringEmptyArgumentValidation((stringArg) => action(this.defaultBaseUri, stringArg), argumentName);
        }
    }
}
