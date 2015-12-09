//---------------------------------------------------------------------
// <copyright file="SelectExpandPathBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Metadata;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.Syntactic;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.Binders
{
    public class SelectExpandPathBinderTests
    {
        [Fact]
        public void SingleLevelTypeSegmentWorks()
        {
            NonSystemToken typeSegment = new NonSystemToken("Fully.Qualified.Namespace.Employee", null, new NonSystemToken("WorkEmail", null, null));
            PathSegmentToken firstNonTypeToken;
            IEdmStructuredType entityType = HardCodedTestModel.GetPersonType();
            var result = SelectExpandPathBinder.FollowTypeSegments(typeSegment, HardCodedTestModel.TestModel, 800, ODataUriResolver.Default, ref entityType, out firstNonTypeToken);
            result.Should().OnlyContain(x => x.Equals(new TypeSegment(HardCodedTestModel.GetEmployeeType(), null)));
            entityType.Should().Be(HardCodedTestModel.GetEmployeeType());
            firstNonTypeToken.ShouldBeNonSystemToken("WorkEmail");
        }

        [Fact]
        public void DeepPath()
        {
            NonSystemToken typeSegment = new NonSystemToken("Fully.Qualified.Namespace.Employee", null, new NonSystemToken("Fully.Qualified.Namespace.Manager", null, new NonSystemToken("NumberOfReports", null, null)));
            PathSegmentToken firstNonTypeToken;
            IEdmStructuredType entityType = HardCodedTestModel.GetPersonType();
            var result = SelectExpandPathBinder.FollowTypeSegments(typeSegment, HardCodedTestModel.TestModel, 800, ODataUriResolver.Default, ref entityType, out firstNonTypeToken);
            result.Should().Contain(x => x.As<TypeSegment>().EdmType == HardCodedTestModel.GetEmployeeType())
                .And.Contain(x => x.As<TypeSegment>().EdmType == HardCodedTestModel.GetManagerType());
            entityType.Should().Be(HardCodedTestModel.GetManagerType());
            firstNonTypeToken.ShouldBeNonSystemToken("NumberOfReports");
        }

        [Fact]
        public void InvalidTypeSegmentThrowsException()
        {
            NonSystemToken typeSegment = new NonSystemToken("Stuff", null, new NonSystemToken("stuff", null, null));
            PathSegmentToken firstNonTypeToken;
            IEdmStructuredType entityType = HardCodedTestModel.GetPersonType();
            Action followInvalidTypeSegment = () => SelectExpandPathBinder.FollowTypeSegments(typeSegment, HardCodedTestModel.TestModel, 800, ODataUriResolver.Default, ref entityType, out firstNonTypeToken);
            followInvalidTypeSegment.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.SelectExpandPathBinder_FollowNonTypeSegment("Stuff"));
        }

        [Fact]
        public void MaxRecursiveDepthIsRespected()
        {
            NonSystemToken typeSegment = new NonSystemToken("Fully.Qualified.Namespace.Employee", null, new NonSystemToken("Fully.Qualified.Namespace.Manager", null, new NonSystemToken("NumberOfReports", null, null)));
            PathSegmentToken firstNonTypeToken;
            IEdmStructuredType entityType = HardCodedTestModel.GetPersonType();
            Action followLongChain = () => SelectExpandPathBinder.FollowTypeSegments(typeSegment, HardCodedTestModel.TestModel, 1, ODataUriResolver.Default, ref entityType, out firstNonTypeToken);
            followLongChain.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpandItemBinder_PathTooDeep);
        }
    }
}
