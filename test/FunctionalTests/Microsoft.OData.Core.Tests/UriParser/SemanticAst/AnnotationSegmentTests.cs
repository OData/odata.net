//---------------------------------------------------------------------
// <copyright file="AnnotationSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
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
            Assert.Throws<ArgumentNullException>("term", createWithNullProperty);
        }

        [Fact]
        public void IdentifierSetToAnnotationName()
        {
            AnnotationSegment annotationSegment = new AnnotationSegment(HardCodedTestModel.GetPrimitiveAnnotationTerm());
            Assert.Equal(HardCodedTestModel.GetPrimitiveAnnotationTerm().FullName(), annotationSegment.Term.FullName());
        }
        
        [Fact]
        public void TargetEdmTypeIsAnnotationTypeDefinition()
        {
            AnnotationSegment annotationSegment = new AnnotationSegment(HardCodedTestModel.GetPrimitiveAnnotationTerm());
            Assert.Same(HardCodedTestModel.GetPrimitiveAnnotationTerm().Type.Definition, annotationSegment.TargetEdmType);
        }

        [Fact]
        public void TermSetCorrectly()
        {
            IEdmTerm term = HardCodedTestModel.GetPrimitiveAnnotationTerm();
            AnnotationSegment segment = new AnnotationSegment(term);
            Assert.Same(term, segment.Term);
        }

        [Fact]
        public void EqualityCorrect()
        {
            AnnotationSegment annotationSegment1 = new AnnotationSegment(HardCodedTestModel.GetPrimitiveAnnotationTerm());
            AnnotationSegment annotationSegment2 = new AnnotationSegment(HardCodedTestModel.GetPrimitiveAnnotationTerm());
            Assert.True(annotationSegment1.Equals(annotationSegment2));
        }

        [Fact]
        public void InequalityCorrect()
        {
            AnnotationSegment annotationSegment1 = new AnnotationSegment(HardCodedTestModel.GetPrimitiveAnnotationTerm());
            AnnotationSegment annotationSegment2 = new AnnotationSegment(HardCodedTestModel.GetComplexAnnotationTerm());
            BatchSegment batchSegment = BatchSegment.Instance;
            Assert.False(annotationSegment1.Equals(annotationSegment2));
            Assert.False(annotationSegment1.Equals(batchSegment));
        }
    }
}
