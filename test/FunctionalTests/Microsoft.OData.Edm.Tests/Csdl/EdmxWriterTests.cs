//---------------------------------------------------------------------
// <copyright file="EdmxWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Annotations;
using Microsoft.OData.Edm.Library.Values;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies.V1;
using Xunit;
using ErrorStrings = Microsoft.OData.Edm.Strings;

namespace Microsoft.OData.Edm.Tests.Csdl
{
    public class EdmxWriterTests
    {
        #region Annotation - Computed, OptimisticConcurrency

        [Fact]
        public void VerifyAnnotationComputedConcurrency()
        {
            var model = new EdmModel();
            var entity = new EdmEntityType("NS1", "Product");
            var entityId = entity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            entity.AddKeys(entityId);
            EdmStructuralProperty name1 = entity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            EdmStructuralProperty timeVer = entity.AddStructuralProperty("UpdatedTime", EdmCoreModel.Instance.GetDate(false));
            model.AddElement(entity);

            SetComputedAnnotation(model, entityId);  // semantic meaning is V3's 'Identity' for Key profperty
            SetComputedAnnotation(model, timeVer);   // semantic meaning is V3's 'Computed' for non-key profperty

            var entityContainer = new EdmEntityContainer("NS1", "Container");
            model.AddElement(entityContainer);
            EdmEntitySet set1 = new EdmEntitySet(entityContainer, "Products", entity);
            model.SetOptimisticConcurrencyAnnotation(set1, new IEdmStructuralProperty[] { entityId, timeVer });
            entityContainer.AddElement(set1);

            string csdlStr = GetEdmx(model, EdmxTarget.OData);
            Assert.Equal(@"<?xml version=""1.0"" encoding=""utf-16""?><edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx""><edmx:DataServices><Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm""><EntityType Name=""Product""><Key><PropertyRef Name=""Id"" /></Key><Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false""><Annotation Term=""Org.OData.Core.V1.Computed"" Bool=""true"" /></Property><Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" /><Property Name=""UpdatedTime"" Type=""Edm.Date"" Nullable=""false""><Annotation Term=""Org.OData.Core.V1.Computed"" Bool=""true"" /></Property></EntityType><EntityContainer Name=""Container""><EntitySet Name=""Products"" EntityType=""NS1.Product""><Annotation Term=""Org.OData.Core.V1.OptimisticConcurrency""><Collection><PropertyPath>Id</PropertyPath><PropertyPath>UpdatedTime</PropertyPath></Collection></Annotation></EntitySet></EntityContainer></Schema></edmx:DataServices></edmx:Edmx>", csdlStr);
        }

        public static void SetComputedAnnotation(EdmModel model, IEdmProperty target)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(target, "target");

            IEdmBooleanConstantExpression val = new EdmBooleanConstant(true);
            IEdmValueTerm term = CoreVocabularyModel.ComputedTerm;

            Debug.Assert(term != null, "term!=null");
            EdmAnnotation annotation = new EdmAnnotation(target, term, val);
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(annotation);
        }

        #endregion

        private string GetEdmx(IEdmModel model, EdmxTarget target)
        {
            string edmx = string.Empty;

            using (StringWriter sw = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = System.Text.Encoding.UTF8;

                using (XmlWriter xw = XmlWriter.Create(sw, settings))
                {
                    IEnumerable<EdmError> errors;
                    EdmxWriter.TryWriteEdmx(model, xw, target, out errors);
                    xw.Flush();
                }

                edmx = sw.ToString();
            }

            return edmx;
        }
    }
}