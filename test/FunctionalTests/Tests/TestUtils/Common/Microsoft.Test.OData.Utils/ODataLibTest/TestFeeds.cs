//---------------------------------------------------------------------
// <copyright file="TestFeeds.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.ODataLibTest
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Helper class to create all interesting entity set instances used in payloads.
    /// </summary>
    public static class TestFeeds
    {
        /// <summary>
        /// Creates a feed containing entities of types that derive from the same base 
        /// </summary>
        /// <param name="model">The entity model schema. The method will modify the model and call Fixup().</param>
        /// <param name="withTypeNames">True if the payloads should specify type names.</param>
        /// <returns>The feed containing derived typed entities.</returns>
        public static IEnumerable<Taupo.OData.Common.PayloadTestDescriptor> GetFeeds(EdmModel model, bool withTypeNames)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            List<Taupo.OData.Common.PayloadTestDescriptor> payloads = new List<Taupo.OData.Common.PayloadTestDescriptor>();

            EdmEntityType baseType = model.EntityType("MyBaseType").KeyProperty("Id", (EdmPrimitiveTypeReference) EdmCoreModel.Instance.GetGuid(false));
            model.Fixup();

            EntityInstance instance = PayloadBuilder.Entity(withTypeNames ? "TestModel." + baseType.Name : null).Property("Id", PayloadBuilder.PrimitiveValue(Guid.NewGuid()));
            instance.Id = "urn:id";

            EntitySetInstance emptySet = PayloadBuilder.EntitySet().WithTypeAnnotation(baseType);

            payloads.Add(new Taupo.OData.Common.PayloadTestDescriptor() 
            { 
                PayloadEdmModel = model, 
                PayloadElement = emptySet
            });

            var emptySetWithInlineCount = emptySet.DeepCopy();
            emptySetWithInlineCount.InlineCount = 0;

            // Inline count (note we skip for v1 and request because inline count is valid on response only on V2 and above.
            payloads.Add(new Taupo.OData.Common.PayloadTestDescriptor()
            {
                PayloadEdmModel = model,
                PayloadElement = emptySetWithInlineCount,
                SkipTestConfiguration = (tc => tc.IsRequest)
            });

            var emptySetWithNextLinkAndCount = emptySet.DeepCopy();
            emptySetWithNextLinkAndCount.InlineCount = 0;
            emptySetWithNextLinkAndCount.NextLink = "http://www.odata.org/Feed";
            
            // inline count + next link
            payloads.Add(new Taupo.OData.Common.PayloadTestDescriptor()
            {
                PayloadEdmModel = model,
                PayloadElement = emptySetWithNextLinkAndCount,
                SkipTestConfiguration = (tc => tc.IsRequest)
            });

            var emptySetWithNextLink = emptySet.DeepCopy();
            emptySetWithNextLink.NextLink = "http://www.odata.org/Feed";

            // next link
            payloads.Add(new Taupo.OData.Common.PayloadTestDescriptor()
            {
                PayloadEdmModel = model,
                PayloadElement = emptySetWithNextLink,
                SkipTestConfiguration = (tc => tc.IsRequest)
            });

            EntitySetInstance singleEntity = PayloadBuilder.EntitySet().Append(instance).WithTypeAnnotation(baseType);

            payloads.Add(new Taupo.OData.Common.PayloadTestDescriptor()
            {
                PayloadEdmModel = model,
                PayloadElement = singleEntity.DeepCopy()
            });

            var singleEntityWithInlineCount = singleEntity.DeepCopy();
            singleEntityWithInlineCount.InlineCount = 1;

            // inline count
            payloads.Add(new Taupo.OData.Common.PayloadTestDescriptor()
            {
                PayloadEdmModel = model,
                PayloadElement = singleEntityWithInlineCount,
                SkipTestConfiguration = (tc => tc.IsRequest)
            });

            var singleEntityWithNextLinkAndCount = singleEntity.DeepCopy();
            singleEntityWithInlineCount.InlineCount = 1;
            singleEntityWithNextLinkAndCount.NextLink = "http://www.odata.org/Feed";

            // inline count + next link
            payloads.Add(new Taupo.OData.Common.PayloadTestDescriptor()
            {
                PayloadEdmModel = model,
                PayloadElement = singleEntityWithNextLinkAndCount,
                SkipTestConfiguration = (tc => tc.IsRequest)
            });

            var singleEntityWithNextLink = singleEntity.DeepCopy();
            singleEntityWithNextLink.NextLink = "http://www.odata.org/Feed";

            // next link 
            payloads.Add(new Taupo.OData.Common.PayloadTestDescriptor()
            {
                PayloadEdmModel = model,
                PayloadElement = singleEntityWithNextLink,
                SkipTestConfiguration = (tc => tc.IsRequest)
            });

            EntitySetInstance multipleEntity = PayloadBuilder.EntitySet().Append(instance.GenerateSimilarEntries(3)).WithTypeAnnotation(baseType);

            payloads.Add(new Taupo.OData.Common.PayloadTestDescriptor()
            {
                PayloadEdmModel = model,
                PayloadElement = multipleEntity
            });

            var multipleEntityWithInlineCount = multipleEntity.DeepCopy();
            multipleEntityWithInlineCount.InlineCount = 3;

            // inline count
            payloads.Add(new Taupo.OData.Common.PayloadTestDescriptor()
            {
                PayloadEdmModel = model,
                PayloadElement = multipleEntityWithInlineCount,
                SkipTestConfiguration = (tc => tc.IsRequest)
            });

            var multipleEntityWithNextLinkAndCount = multipleEntity.DeepCopy();
            multipleEntityWithNextLinkAndCount.InlineCount = 3;
            multipleEntityWithNextLinkAndCount.NextLink = "http://www.odata.org/Feed";

            // inline count + next link
            payloads.Add(new Taupo.OData.Common.PayloadTestDescriptor()
            {
                PayloadEdmModel = model,
                PayloadElement = multipleEntityWithNextLinkAndCount,
                SkipTestConfiguration = (tc => tc.IsRequest)
            });

            var multipleEntityWithNextLink = multipleEntity.DeepCopy();
            multipleEntityWithNextLink.NextLink = "http://www.odata.org/Feed";

            // next link
            payloads.Add(new Taupo.OData.Common.PayloadTestDescriptor()
            {
                PayloadEdmModel = model,
                PayloadElement = multipleEntityWithNextLink,
                SkipTestConfiguration = (tc => tc.IsRequest)
            });

            return payloads;
        }
    }
}
