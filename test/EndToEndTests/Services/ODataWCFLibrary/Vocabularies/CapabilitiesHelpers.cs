//---------------------------------------------------------------------
// <copyright file="CapabilitiesHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.Vocabularies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;

    public static class CapabilitiesHelpers
    {
        #region Initialization

        public static readonly IEdmModel Instance;
        public static readonly IEdmTerm ConformanceLevelTerm;
        public static readonly IEdmTerm SupportedFormatsTerm;
        public static readonly IEdmTerm AsynchronousRequestsSupportedTerm;
        public static readonly IEdmTerm BatchContinueOnErrorSupportedTerm;
        public static readonly IEdmTerm ChangeTrackingTerm;
        public static readonly IEdmTerm NavigationRestrictionsTerm;
        public static readonly IEdmTerm FilterFunctionsTerm;
        public static readonly IEdmTerm SearchRestrictionsTerm;
        public static readonly IEdmTerm InsertRestrictionsTerm;
        public static readonly IEdmTerm UpdateRestrictionsTerm;
        public static readonly IEdmTerm DeleteRestrictionsTerm;
        public static readonly IEdmEnumType ConformanceLevelTypeType;
        public static readonly IEdmEnumType NavigationTypeType;
        public static readonly IEdmEnumType SearchExpressionsType;

        internal const string CapabilitiesConformanceLevel = "Org.OData.Capabilities.V1.ConformanceLevel";
        internal const string CapabilitiesSupportedFormats = "Org.OData.Capabilities.V1.SupportedFormats";
        internal const string CapabilitiesAsynchronousRequestsSupported = "Org.OData.Capabilities.V1.AsynchronousRequestsSupported";
        internal const string CapabilitiesBatchContinueOnErrorSupported = "Org.OData.Capabilities.V1.BatchContinueOnErrorSupported";
        internal const string CapabilitiesChangeTracking = "Org.OData.Capabilities.V1.ChangeTracking";
        internal const string CapabilitiesNavigationRestrictions = "Org.OData.Capabilities.V1.NavigationRestrictions";
        internal const string CapabilitiesFilterFunctions = "Org.OData.Capabilities.V1.FilterFunctions";
        internal const string CapabilitiesSearchRestrictions = "Org.OData.Capabilities.V1.SearchRestrictions";
        internal const string CapabilitiesInsertRestrictions = "Org.OData.Capabilities.V1.InsertRestrictions";
        internal const string CapabilitiesUpdateRestrictions = "Org.OData.Capabilities.V1.UpdateRestrictions";
        internal const string CapabilitiesDeleteRestrictions = "Org.OData.Capabilities.V1.DeleteRestrictions";
        internal const string CapabilitiesConformanceLevelType = "Org.OData.Capabilities.V1.ConformanceLevelType";
        internal const string CapabilitiesNavigationType = "Org.OData.Capabilities.V1.NavigationType";
        internal const string CapabilitiesSearchExpressions = "Org.OData.Capabilities.V1.SearchExpressions";

        static CapabilitiesHelpers()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ODataSamples.Services.Core.Vocabularies.CapabilitiesVocabularies.xml"))
            {
                IEnumerable<EdmError> errors;
                SchemaReader.TryParse(new[] { XmlReader.Create(stream) }, out Instance, out errors);
            }

            ConformanceLevelTerm = Instance.FindDeclaredTerm(CapabilitiesConformanceLevel);
            SupportedFormatsTerm = Instance.FindDeclaredTerm(CapabilitiesSupportedFormats);
            AsynchronousRequestsSupportedTerm = Instance.FindDeclaredTerm(CapabilitiesAsynchronousRequestsSupported);
            BatchContinueOnErrorSupportedTerm = Instance.FindDeclaredTerm(CapabilitiesBatchContinueOnErrorSupported);
            ChangeTrackingTerm = Instance.FindDeclaredTerm(CapabilitiesChangeTracking);
            NavigationRestrictionsTerm = Instance.FindDeclaredTerm(CapabilitiesNavigationRestrictions);
            FilterFunctionsTerm = Instance.FindDeclaredTerm(CapabilitiesFilterFunctions);
            SearchRestrictionsTerm = Instance.FindDeclaredTerm(CapabilitiesSearchRestrictions);
            InsertRestrictionsTerm = Instance.FindDeclaredTerm(CapabilitiesInsertRestrictions);
            UpdateRestrictionsTerm = Instance.FindDeclaredTerm(CapabilitiesUpdateRestrictions);
            DeleteRestrictionsTerm = Instance.FindDeclaredTerm(CapabilitiesDeleteRestrictions);
            ConformanceLevelTypeType = (IEdmEnumType)Instance.FindDeclaredType(CapabilitiesConformanceLevelType);
            NavigationTypeType = (IEdmEnumType)Instance.FindDeclaredType(CapabilitiesNavigationType);
            SearchExpressionsType = (IEdmEnumType)Instance.FindDeclaredType(CapabilitiesSearchExpressions);
        }

        #endregion

        #region ConformanceLevel

        public static void SetConformanceLevelCapabilitiesAnnotation(this EdmModel model, IEdmEntityContainer container, CapabilitiesConformanceLevelType level)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (container == null) throw new ArgumentNullException("container");

            var target = container;
            var term = ConformanceLevelTerm;
            var name = new EdmEnumTypeReference(ConformanceLevelTypeType, false).ToStringLiteral((long)level);
            var expression = new EdmEnumMemberExpression(ConformanceLevelTypeType.Members.Single(m => m.Name == name));
            var annotation = new EdmVocabularyAnnotation(target, term, expression);

            annotation.SetSerializationLocation(model, container.ToSerializationLocation());
            model.AddVocabularyAnnotation(annotation);
        }

        #endregion

        #region SupportedFormats

        public static void SetSupportedFormatsCapabilitiesAnnotation(this EdmModel model, IEdmEntityContainer container, IEnumerable<string> supportedFormats)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (container == null) throw new ArgumentNullException("container");

            model.SetCapabilitiesAnnotation(container, SupportedFormatsTerm, supportedFormats);
        }

        #endregion

        #region AsynchronousRequestsSupported

        public static void SetAsynchronousRequestsSupportedCapabilitiesAnnotation(this EdmModel model, IEdmEntityContainer container, bool value)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (container == null) throw new ArgumentNullException("container");

            model.SetCapabilitiesAnnotation(container, AsynchronousRequestsSupportedTerm, value);
        }

        #endregion

        #region BatchContinueOnErrorSupported

        public static void SetBatchContinueOnErrorSupportedCapabilitiesAnnotation(this EdmModel model, IEdmEntityContainer container, bool value)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (container == null) throw new ArgumentNullException("container");

            model.SetCapabilitiesAnnotation(container, BatchContinueOnErrorSupportedTerm, value);
        }

        #endregion

        #region NavigationRestrictions

        public static void SetNavigationRestrictionsCapabilitiesAnnotation(this EdmModel model, IEdmEntitySet entitySet, CapabilitiesNavigationType type, IEnumerable<Tuple<IEdmNavigationProperty, CapabilitiesNavigationType>> properties)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (entitySet == null) throw new ArgumentNullException("entitySet");

            if (properties == null)
            {
                properties = new Tuple<IEdmNavigationProperty, CapabilitiesNavigationType>[0];
            }

            var target = entitySet;
            var term = NavigationRestrictionsTerm;
            // handle type
            var typeLiteral = new EdmEnumTypeReference(NavigationTypeType, false).ToStringLiteral((long)type);
            // handle properties
            var propertiesExpression = properties.Select(p =>
            {
                var name = new EdmEnumTypeReference(NavigationTypeType, false).ToStringLiteral((long)p.Item2);
                return new EdmRecordExpression(new IEdmPropertyConstructor[]
                {
                    new EdmPropertyConstructor("NavigationProperty", new EdmNavigationPropertyPathExpression(p.Item1.Name)),
                    new EdmPropertyConstructor("Navigability", new EdmEnumMemberExpression(NavigationTypeType.Members.Single(m => m.Name == name))),
                });
            });

            var record = new EdmRecordExpression(new IEdmPropertyConstructor[]
            {
                new EdmPropertyConstructor("Navigability", new EdmEnumMemberExpression(NavigationTypeType.Members.Single(m => m.Name == typeLiteral))),
                new EdmPropertyConstructor("RestrictedProperties", new EdmCollectionExpression(propertiesExpression))
            });

            var annotation = new EdmVocabularyAnnotation(target, term, record);
            annotation.SetSerializationLocation(model, entitySet.ToSerializationLocation());
            model.AddVocabularyAnnotation(annotation);
        }

        #endregion

        #region FilterFunctions

        public static void SetFilterFunctionsCapabilitiesAnnotation(this EdmModel model, IEdmEntityContainer container, IEnumerable<string> functions)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (container == null) throw new ArgumentNullException("container");

            model.SetCapabilitiesAnnotation(container, FilterFunctionsTerm, functions);
        }

        public static void SetFilterFunctionsCapabilitiesAnnotation(this EdmModel model, IEdmEntitySet entitySet, IEnumerable<string> functions)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (entitySet == null) throw new ArgumentNullException("entitySet");

            model.SetCapabilitiesAnnotation(entitySet, FilterFunctionsTerm, functions);
        }

        #endregion

        #region SearchRestrictions

        public static void SetSearchRestrictionsCapabilitiesAnnotation(this EdmModel model, IEdmEntitySet entitySet, bool searchable, CapabilitiesSearchExpressions unsupported)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (entitySet == null) throw new ArgumentNullException("entitySet");

            var target = entitySet;
            var term = SearchRestrictionsTerm;
            var name = new EdmEnumTypeReference(SearchExpressionsType, false).ToStringLiteral((long)unsupported);
            var properties = new IEdmPropertyConstructor[]
            {
                new EdmPropertyConstructor("Searchable", new EdmBooleanConstant(searchable)),
                new EdmPropertyConstructor("UnsupportedExpressions", new EdmEnumMemberExpression(SearchExpressionsType.Members.Single(m => m.Name == name))),
            };
            var record = new EdmRecordExpression(properties);

            var annotation = new EdmVocabularyAnnotation(target, term, record);
            annotation.SetSerializationLocation(model, entitySet.ToSerializationLocation());
            model.AddVocabularyAnnotation(annotation);
        }

        #endregion

        #region InsertRestrictions

        public static void SetInsertRestrictionsCapabilitiesAnnotation(this EdmModel model, IEdmEntitySet entitySet, bool insertable, IEnumerable<IEdmNavigationProperty> nonInsertableProperties)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (entitySet == null) throw new ArgumentNullException("entitySet");

            model.SetCapabilitiesAnnotation(entitySet, InsertRestrictionsTerm, insertable, nonInsertableProperties, "Insertable", "NonInsertableNavigationProperties");
        }

        public static void GetInsertRestrictions(this IEdmModel model, IEdmEntitySet entitySet, out bool? insertable, out IEnumerable<string> nonInsertableProperties)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (entitySet == null) throw new ArgumentNullException("entitySet");

            model.GetBooleanAndPathCollection(entitySet, InsertRestrictionsTerm, "Insertable", "NonInsertableNavigationProperties", out insertable, out nonInsertableProperties);
        }

        #endregion

        #region UpdateRestrictions

        public static void SetUpdateRestrictionsCapabilitiesAnnotation(this EdmModel model, IEdmEntitySet entitySet, bool updatable, IEnumerable<IEdmNavigationProperty> nonUpdatableProperties)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (entitySet == null) throw new ArgumentNullException("entitySet");

            model.SetCapabilitiesAnnotation(entitySet, UpdateRestrictionsTerm, updatable, nonUpdatableProperties, "Updatable", "NonUpdatableNavigationProperties");
        }

        #endregion

        #region DeleteRestrictions

        public static void SetDeleteRestrictionsCapabilitiesAnnotation(this EdmModel model, IEdmEntitySet entitySet, bool deletable, IEnumerable<IEdmNavigationProperty> nonDeletableProperties)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (entitySet == null) throw new ArgumentNullException("entitySet");

            model.SetCapabilitiesAnnotation(entitySet, DeleteRestrictionsTerm, deletable, nonDeletableProperties, "Deletable", "NonDeletableNavigationProperties");
        }

        public static void GetDeleteRestrictions(this IEdmModel model, IEdmEntitySet entitySet, out bool? deletable, out IEnumerable<string> nonDeletableProperties)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (entitySet == null) throw new ArgumentNullException("entitySet");

            model.GetBooleanAndPathCollection(entitySet, DeleteRestrictionsTerm, "Deletable", "NonDeletableNavigationProperties", out deletable, out nonDeletableProperties);
        }

        #endregion

        #region Helpers

        private static void SetCapabilitiesAnnotation(this EdmModel model, IEdmVocabularyAnnotatable target, IEdmTerm term, bool value, IEnumerable<IEdmNavigationProperty> navigationProperties, string name1, string name2)
        {
            if (navigationProperties == null)
            {
                navigationProperties = new IEdmNavigationProperty[0];
            }

            var properties = new IEdmPropertyConstructor[]
            {
                new EdmPropertyConstructor(name1, new EdmBooleanConstant(value)),
                new EdmPropertyConstructor(name2, new EdmCollectionExpression(navigationProperties.Select(p => new EdmNavigationPropertyPathExpression(p.Name)))),
            };
            var record = new EdmRecordExpression(properties);

            var annotation = new EdmVocabularyAnnotation(target, term, record);
            annotation.SetSerializationLocation(model, target.ToSerializationLocation());
            model.AddVocabularyAnnotation(annotation);
        }

        private static void SetCapabilitiesAnnotation(this EdmModel model, IEdmVocabularyAnnotatable target, IEdmTerm term, IEnumerable<string> values)
        {
            if (values == null)
            {
                values = new string[0];
            }

            var expression = new EdmCollectionExpression(values.Select(function => new EdmStringConstant(function)));
            var annotation = new EdmVocabularyAnnotation(target, term, expression);
            annotation.SetSerializationLocation(model, target.ToSerializationLocation());
            model.AddVocabularyAnnotation(annotation);
        }

        private static void SetCapabilitiesAnnotation(this EdmModel model, IEdmVocabularyAnnotatable target, IEdmTerm term, bool value)
        {
            var expression = new EdmBooleanConstant(value);
            var annotation = new EdmVocabularyAnnotation(target, term, expression);
            annotation.SetSerializationLocation(model, target.ToSerializationLocation());
            model.AddVocabularyAnnotation(annotation);
        }

        private static void GetBooleanAndPathCollection(this IEdmModel model, IEdmEntitySet entitySet, IEdmTerm term, string booleanPropertyName, string pathsPropertyName, out bool? boolean, out IEnumerable<string> paths)
        {
            boolean = null;
            paths = new string[0];

            var annotation = model.FindVocabularyAnnotation(entitySet, term);
            if (annotation == null)
            {
                return;
            }

            var recordExpression = (IEdmRecordExpression)annotation.Value;
            var booleanExpression = (IEdmBooleanConstantExpression)recordExpression.Properties.Single(p => p.Name == booleanPropertyName).Value;
            var collectionExpression = (IEdmCollectionExpression)recordExpression.Properties.Single(p => p.Name == pathsPropertyName).Value;
            var pathsTemp = new List<string>();

            foreach (IEdmPathExpression pathExpression in collectionExpression.Elements)
            {
                var pathBuilder = new StringBuilder();
                foreach (var path in pathExpression.Path)
                {
                    pathBuilder.AppendFormat("{0}.", path);
                }

                pathBuilder.Remove(pathBuilder.Length - 1, 1);

                pathsTemp.Add(paths.ToString());
            }

            boolean = booleanExpression.Value;
            paths = pathsTemp;
        }

        private static EdmVocabularyAnnotationSerializationLocation ToSerializationLocation(this IEdmVocabularyAnnotatable target)
        {
            return target is IEdmEntityContainer ? EdmVocabularyAnnotationSerializationLocation.OutOfLine : EdmVocabularyAnnotationSerializationLocation.Inline;
        }

        private static IEdmVocabularyAnnotation FindVocabularyAnnotation(this IEdmModel model, IEdmVocabularyAnnotatable target, IEdmTerm term)
        {
            var result = default(IEdmVocabularyAnnotation);

            var annotations = model.FindVocabularyAnnotations(target);
            if (annotations != null)
            {
                var annotation = annotations.FirstOrDefault(a => a.Term.Namespace == term.Namespace && a.Term.Name == term.Name);
                result = annotation;
            }

            return result;
        }

        #endregion
    }
}
