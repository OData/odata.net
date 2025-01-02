using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;

namespace Microsoft.OData.Edm.Tests
{
    public class ImmutableModelTests
    {

        private IEdmModel CreateModel()
        {
            EdmModel model = new EdmModel();
            string ns = "NS";

            EdmEntityType typeA = model.AddEntityType(ns, "TypeA");
            var idProperty = typeA.AddStructuralProperty("Id", EdmPrimitiveTypeKind.String);
            typeA.AddKeys(idProperty);

            var container = model.AddEntityContainer(ns, "Container");
            container.AddEntitySet("TypeAs", typeA);
            var term = model.AddTerm(ns, "MyTerm", EdmPrimitiveTypeKind.Boolean);
            var annotation = new EdmVocabularyAnnotation(typeA, term, new EdmBooleanConstant(true));
            model.AddVocabularyAnnotation(annotation);
            
            return model;
        }

        [Fact]
        public void ModelIsNotImmutableByDefault()
        {
            IEdmModel model = CreateModel();
            bool isImmutable = model.IsImmutable();
            Assert.False(isImmutable);
        }

        [Fact]
        public void ModelIsImmutableTrue_WhenMarkAsImmutableIsCalled()
        {
            IEdmModel model = CreateModel();
            model.MarkAsImmutable();

            bool isImmutable = model.IsImmutable();
            Assert.True(isImmutable);
        }

        [Fact]
        public void DoNotCacheVocabularyAnnotationsIfModelIsNotImmutable()
        {
            IEdmModel model = CreateModel();
            var entityType = model.FindType("NS.TypeA");

            IEnumerable<IEdmVocabularyAnnotation> annotations = model.FindVocabularyAnnotations(entityType);
            Assert.True(annotations.Count() >= 1);
            var annotation = annotations.FirstOrDefault(a => a.Term.FullName() == "NS.MyTerm");
            Assert.NotNull(annotation);

            var cache = model.GetVocabularyAnnotationCache();
            bool cacheHit = cache.TryGetVocabularyAnnotations(entityType, out var cachedAnnotations);
            Assert.False(cacheHit);
        }

        [Fact]
        public void CacheVocabularyAnnotationsIfModelIsImmutable()
        {
            IEdmModel model = CreateModel();
            model.MarkAsImmutable();
            var entityType = model.FindType("NS.TypeA");

            IEnumerable<IEdmVocabularyAnnotation> annotations = model.FindVocabularyAnnotations(entityType);
            int count = annotations.Count();
            Assert.True(count >= 1);
            var annotation = annotations.FirstOrDefault(a => a.Term.FullName() == "NS.MyTerm");
            Assert.NotNull(annotation);

            var cache = model.GetVocabularyAnnotationCache();
            bool cacheHit = cache.TryGetVocabularyAnnotations(entityType, out var cachedAnnotations);
            Assert.True(cacheHit);
            Assert.Equal(count, cachedAnnotations.Count());
            var cachedAnnotation = annotations.FirstOrDefault(a => a.Term.FullName() == "NS.MyTerm");
            Assert.Equal(annotation, cachedAnnotation);
        }
    }
}
