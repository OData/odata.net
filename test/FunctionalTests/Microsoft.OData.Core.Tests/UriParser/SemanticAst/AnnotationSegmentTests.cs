//---------------------------------------------------------------------
// <copyright file="AnnotationSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class AnnotationSegmentTests
    {
        [Fact]
        public void AnnotationCannotBeNull()
        {
            Action createWithNullProperty = () => new AnnotationSegment(null);
            createWithNullProperty.ShouldThrow<Exception>(Error.ArgumentNull("property").ToString());
        }

        [Fact]
        public void IdentifierSetToAnnotationName()
        {
            AnnotationSegment annotationSegment = new AnnotationSegment(HardCodedTestModel.GetPrimitiveAnnotationTerm());
            annotationSegment.Term.FullName().Should().Be(HardCodedTestModel.GetPrimitiveAnnotationTerm().FullName());
        }
        
        [Fact]
        public void TargetEdmTypeIsAnnotationTypeDefinition()
        {
            AnnotationSegment annotationSegment = new AnnotationSegment(HardCodedTestModel.GetPrimitiveAnnotationTerm());
            annotationSegment.TargetEdmType.Should().BeSameAs(HardCodedTestModel.GetPrimitiveAnnotationTerm().Type.Definition);
        }

        [Fact]
        public void TermSetCorrectly()
        {
            IEdmTerm term = HardCodedTestModel.GetPrimitiveAnnotationTerm();
            AnnotationSegment segment = new AnnotationSegment(term);
            segment.Term.Should().Be(term);
        }

        [Fact]
        public void EqualityCorrect()
        {
            AnnotationSegment annotationSegment1 = new AnnotationSegment(HardCodedTestModel.GetPrimitiveAnnotationTerm());
            AnnotationSegment annotationSegment2 = new AnnotationSegment(HardCodedTestModel.GetPrimitiveAnnotationTerm());
            annotationSegment1.Equals(annotationSegment2).Should().BeTrue();
        }

        [Fact]
        public void InequalityCorrect()
        {
            AnnotationSegment annotationSegment1 = new AnnotationSegment(HardCodedTestModel.GetPrimitiveAnnotationTerm());
            AnnotationSegment annotationSegment2 = new AnnotationSegment(HardCodedTestModel.GetComplexAnnotationTerm());
            BatchSegment batchSegment = BatchSegment.Instance;
            annotationSegment1.Equals(annotationSegment2).Should().BeFalse();
            annotationSegment1.Equals(batchSegment).Should().BeFalse();
        }
    }
}
