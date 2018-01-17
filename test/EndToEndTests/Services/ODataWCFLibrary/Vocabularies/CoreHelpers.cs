//---------------------------------------------------------------------
// <copyright file="CoreHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;

namespace Microsoft.Test.OData.Services.ODataWCFService.Vocabularies
{
    public static class CoreHelpers
    {
        #region Initialization

        public static readonly IEdmEnumType PermissionType;

        internal const string CorePermission = "Org.OData.Core.V1.Permission";
        
        static CoreHelpers()
        {
            PermissionType = (IEdmEnumType)CoreVocabularyModel.Instance.FindDeclaredType(CorePermission);
        }

        #endregion

        #region ResourcePath

        public static void SetResourcePathCoreAnnotation(this EdmModel model, IEdmEntitySet entitySet, string url)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (entitySet == null) throw new ArgumentNullException("entitySet");
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");

            model.SetCoreAnnotation(entitySet, CoreVocabularyModel.ResourcePathTerm, url);
        }

        public static void SetResourcePathCoreAnnotation(this EdmModel model, IEdmSingleton singleton, string url)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (singleton == null) throw new ArgumentNullException("singleton");
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");

            model.SetCoreAnnotation(singleton, CoreVocabularyModel.ResourcePathTerm, url);
        }

        public static void SetResourcePathCoreAnnotation(this EdmModel model, IEdmActionImport import, string url)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (import == null) throw new ArgumentNullException("import");
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");

            model.SetCoreAnnotation(import, CoreVocabularyModel.ResourcePathTerm, url);
        }

        public static void SetResourcePathCoreAnnotation(this EdmModel model, IEdmFunctionImport import, string url)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (import == null) throw new ArgumentNullException("import");
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");

            model.SetCoreAnnotation(import, CoreVocabularyModel.ResourcePathTerm, url);
        }

        #endregion

        #region DereferenceableIDs

        public static void SetDereferenceableIDsCoreAnnotation(this EdmModel model, IEdmEntityContainer container, bool value)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (container == null) throw new ArgumentNullException("container");

            model.SetCoreAnnotation(container, CoreVocabularyModel.DereferenceableIDsTerm, value);
        }

        #endregion

        #region ConventionalIDs

        public static void SetConventionalIDsCoreAnnotation(this EdmModel model, IEdmEntityContainer container, bool value)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (container == null) throw new ArgumentNullException("container");

            model.SetCoreAnnotation(container, CoreVocabularyModel.ConventionalIDsTerm, value);
        }

        #endregion

        #region Permissions

        public static void SetPermissionsCoreAnnotation(this EdmModel model, IEdmProperty property, CorePermission value)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (property == null) throw new ArgumentNullException("property");

            var target = property;
            var term = CoreVocabularyModel.PermissionsTerm;
            var name = new EdmEnumTypeReference(PermissionType, false).ToStringLiteral((long)value);
            var expression = new EdmEnumMemberExpression(PermissionType.Members.Single(m => m.Name == name));
            var annotation = new EdmVocabularyAnnotation(target, term, expression);

            annotation.SetSerializationLocation(model, property.ToSerializationLocation());
            model.AddVocabularyAnnotation(annotation);
        }

        public static CorePermission? GetPermissions(this IEdmModel model, IEdmProperty property)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (property == null) throw new ArgumentNullException("property");

            return model.GetEnum<CorePermission>(property, CoreVocabularyModel.PermissionsTerm);
        }

        #endregion

        #region Immutable

        public static void SetImmutableCoreAnnotation(this EdmModel model, IEdmProperty property, bool value)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (property == null) throw new ArgumentNullException("property");

            model.SetCoreAnnotation(property, CoreVocabularyModel.ImmutableTerm, value);
        }

        public static bool? GetImmutable(this IEdmModel model, IEdmProperty property)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (property == null) throw new ArgumentNullException("property");

            return model.GetBoolean(property, CoreVocabularyModel.ImmutableTerm);
        }

        #endregion

        #region Computed

        public static void SetComputedCoreAnnotation(this EdmModel model, IEdmProperty property, bool value)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (property == null) throw new ArgumentNullException("property");

            model.SetCoreAnnotation(property, CoreVocabularyModel.ComputedTerm, value);
        }

        public static bool? GetComputed(this IEdmModel model, IEdmProperty property)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (property == null) throw new ArgumentNullException("property");

            return model.GetBoolean(property, CoreVocabularyModel.ComputedTerm);
        }

        #endregion

        #region AcceptableMediaTypes

        public static void SetAcceptableMediaTypesCoreAnnotation(this EdmModel model, IEdmEntityType entityType, IEnumerable<string> acceptableMediaTypes)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (entityType == null) throw new ArgumentNullException("entityType");

            model.SetCoreAnnotation(entityType, CoreVocabularyModel.AcceptableMediaTypesTerm, acceptableMediaTypes);
        }

        public static void SetAcceptableMediaTypesCoreAnnotation(this EdmModel model, IEdmProperty property, IEnumerable<string> acceptableMediaTypes)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (property == null) throw new ArgumentNullException("property");

            model.SetCoreAnnotation(property, CoreVocabularyModel.AcceptableMediaTypesTerm, acceptableMediaTypes);
        }

        public static IEnumerable<string> GetAcceptableMediaTypes(this IEdmModel model, IEdmEntityType entityType)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (entityType == null) throw new ArgumentNullException("entityType");

            var annotation = model.FindVocabularyAnnotation(entityType, CoreVocabularyModel.AcceptableMediaTypesTerm);
            if (annotation != null)
            {
                var collection = (IEdmCollectionExpression)annotation.Value;
                foreach (IEdmStringConstantExpression expression in collection.Elements)
                {
                    yield return expression.Value;
                }
            }
        }

        #endregion

        #region OptimisticConcurrency

        public static IEnumerable<string> GetOptimisticConcurrency(this IEdmModel model, IEdmEntitySet entitySet)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (entitySet == null) throw new ArgumentNullException("entitySet");

            var annotation = model.FindVocabularyAnnotation(entitySet, CoreVocabularyModel.ConcurrencyTerm);
            if (annotation != null)
            {
                var collection = (IEdmCollectionExpression)annotation.Value;
                foreach (IEdmPathExpression expression in collection.Elements)
                {
                    var paths = new StringBuilder();
                    foreach (var path in expression.Path)
                    {
                        paths.AppendFormat("{0}.", path);
                    }

                    paths.Remove(paths.Length - 1, 1);

                    yield return paths.ToString();
                }
            }
        }

        #endregion

        #region Helpers

        private static void SetCoreAnnotation(this EdmModel model, IEdmVocabularyAnnotatable target, IEdmTerm term, string value)
        {
            var expression = new EdmStringConstant(value);
            var annotation = new EdmVocabularyAnnotation(target, term, expression);
            annotation.SetSerializationLocation(model, target.ToSerializationLocation());
            model.AddVocabularyAnnotation(annotation);
        }

        private static void SetCoreAnnotation(this EdmModel model, IEdmVocabularyAnnotatable target, IEdmTerm term, IEnumerable<string> values)
        {
            if (values == null)
            {
                values = new string[0];
            }

            var expression = new EdmCollectionExpression(values.Select(value => new EdmStringConstant(value)));
            var annotation = new EdmVocabularyAnnotation(target, term, expression);
            annotation.SetSerializationLocation(model, target.ToSerializationLocation());
            model.AddVocabularyAnnotation(annotation);
        }

        private static void SetCoreAnnotation(this EdmModel model, IEdmVocabularyAnnotatable target, IEdmTerm term, bool value)
        {
            var expression = new EdmBooleanConstant(value);
            var annotation = new EdmVocabularyAnnotation(target, term, expression);
            annotation.SetSerializationLocation(model, target.ToSerializationLocation());
            model.AddVocabularyAnnotation(annotation);
        }

        private static bool? GetBoolean(this IEdmModel model, IEdmProperty property, IEdmTerm term)
        {
            var annotation = model.FindVocabularyAnnotation(property, term);
            if (annotation != null)
            {
                var booleanExpression = (IEdmBooleanConstantExpression)annotation.Value;
                return booleanExpression.Value;
            }

            return null;
        }

        private static T? GetEnum<T>(this IEdmModel model, IEdmProperty property, IEdmTerm term)
            where T : struct
        {
            var annotation = model.FindVocabularyAnnotation(property, term);
            if (annotation != null)
            {
                var enumMemberReference = (IEdmEnumMemberExpression)annotation.Value;
                var enumMember = enumMemberReference.EnumMembers.Single();
                return (T)Enum.Parse(typeof(T), enumMember.Name);
            }

            return null;
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
