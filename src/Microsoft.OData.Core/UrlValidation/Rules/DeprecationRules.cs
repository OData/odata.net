//---------------------------------------------------------------------
// <copyright file="DeprecationRules.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//--------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;

namespace Microsoft.OData.UriParser.Validation.Rules
{
    /// <summary>
    /// Rules for validating that a Url doesn't use properties or types marked as deprecated.
    /// </summary>
    internal class DeprecationRules
    {
        private const string DefaultCoreAliasWithRevisionTerm = "Core.Revisions";
        private const string RevisionVersionProperty = "Version";
        private const string RevisionKindProperty = "Kind";
        private const string RevisionDateProperty = "Date";
        private const string RevisionRemovalDateProperty = "RemovalDate";
        private const string RevisionElementNameProperty = "ElementName";
        private const string RevisionDescriptionProperty = "Description";
        private const string RevisionKindDeprecated = "deprecated";

        /// <summary>
        /// Validates that a property is not deprecated
        /// </summary>
        public static ODataUrlValidationRule<IEdmProperty> DeprecatedPropertyRule = new ODataUrlValidationRule<IEdmProperty>(
        "DeprecatedPropertyRule",
        (ODataUrlValidationContext context, IEdmProperty segment) =>
        {
            Validate(segment, segment.Type.Definition.AsElementType(), context);
        },
        /*includeImpliedProperties*/ true);

        /// <summary>
        /// Validates that a navigation source is not deprecated
        /// </summary>
        public static ODataUrlValidationRule<IEdmNavigationSource> DeprecatedNavigationSourceRule = new ODataUrlValidationRule<IEdmNavigationSource>(
        "DeprecatedNavigationSourceRule",
        (ODataUrlValidationContext context, IEdmNavigationSource source) =>
        {
            Validate(source, source.EntityType(), context);
        },
        /*includeImpliedProperties*/ true);

        /// <summary>
        /// Validates that a type is not deprecated
        /// </summary>
        public static ODataUrlValidationRule<IEdmType> DeprecatedTypeRule = new ODataUrlValidationRule<IEdmType>(
        "DeprecatedTypeRule",
        (ODataUrlValidationContext context, IEdmType edmType) =>
        {
            string message;
            string version;
            Date? date;
            Date? removalDate;
            if (IsDeprecated(context.Model, edmType, out message, out version, out date, out removalDate))
            {
                context.Messages.Add(CreateUrlValidationMessage(edmType.FullTypeName(), message, version, date, removalDate));
            }
        },
        /*includeImpliedProperties*/ true);

        /// <summary>
        /// Helper function to create an ODataUrlValidationMessage
        /// </summary>
        /// <param name="elementName">The name of the element marked as deprecated.</param>
        /// <param name="message">The deprecation message.</param>
        /// <param name="version">The deprecation version, if specified.</param>
        /// <param name="date">The deprecation date, if specified.</param>
        /// <param name="removalDate">The removal date, if specified.</param>
        /// <returns>An ODataUrlValidationMessage representing the deprecated element.</returns>
        private static ODataUrlValidationMessage CreateUrlValidationMessage(string elementName, string message, string version, Date? date, Date? removalDate)
        {
            ODataUrlValidationMessage error = new ODataUrlValidationMessage(ODataUrlValidationMessageCodes.DeprecatedElement, message, Severity.Warning);
            if (date != null)
            {
                error.ExtendedProperties.Add(RevisionDateProperty, date);
            }

            if (removalDate != null)
            {
                error.ExtendedProperties.Add(RevisionRemovalDateProperty, removalDate);
            }

            if (!String.IsNullOrEmpty(version))
            {
                error.ExtendedProperties.Add(RevisionVersionProperty, version);
            }

            if (!String.IsNullOrEmpty(elementName))
            {
                error.ExtendedProperties.Add(RevisionElementNameProperty, elementName);
            }

            return error;
        }
        
        /// <summary>
        /// Validate a model element (and its type) to see if it has been marked as deprecated
        /// </summary>
        /// <param name="element">The element being validated.</param>
        /// <param name="elementType">The type of element being validated.</param>
        /// <param name="context">The ODataUrlValidationContext providing validation context.</param>
        private static void Validate(IEdmNamedElement element, IEdmType elementType, ODataUrlValidationContext context)
        {
            string message;
            string version;
            Date? date;
            Date? removalDate;
            if (IsDeprecated(context.Model, element, out message, out version, out date, out removalDate))
            {
                context.Messages.Add(CreateUrlValidationMessage(element.Name, message, version, date, removalDate));
            }
        }

        /// <summary>
        /// Given a model element, see if it is marked as deprecated.
        /// </summary>
        /// <param name="model">The model containing the element</param>
        /// <param name="element">The IEdmElement to be validated to see if it's marked as deprecated.</param>
        /// <param name="message">The returned deprecation message if the element is deprecated.</param>
        /// <param name="version">The returned version if the element is deprecated, if specified.</param>
        /// <param name="date">The returned date if the element is deprecated, if specified.</param>
        /// <param name="removalDate">The returned removal date if the element is deprecated, if specified.</param>
        /// <returns>True if the element is marked as deprecated, otherwise false.</returns>
        private static bool IsDeprecated(IEdmModel model, IEdmElement element, out string message, out string version, out Date? date, out Date? removalDate)
        {
            if (!(element is IEdmPrimitiveType))
            {
                IEdmVocabularyAnnotatable annotatedElement = element as IEdmVocabularyAnnotatable;
                if (annotatedElement != null)
                {
                    foreach (IEdmVocabularyAnnotation annotation in GetRevisionAnnotations(model, annotatedElement))
                    {
                        IEdmCollectionExpression collectionExpression = annotation.Value as IEdmCollectionExpression;
                        if (collectionExpression != null)
                        {
                            foreach (IEdmExpression versionRecord in collectionExpression.Elements)
                            {
                                bool isDeprecated = false;
                                message = string.Empty;
                                version = string.Empty;
                                date = null;
                                removalDate = null;
                                IEdmRecordExpression record = versionRecord as IEdmRecordExpression;
                                if (record != null)
                                {
                                    foreach (IEdmPropertyConstructor property in record.Properties)
                                    {
                                        switch (property.Name)
                                        {
                                            case RevisionKindProperty:
                                                IEdmEnumMemberExpression enumValue = property.Value as IEdmEnumMemberExpression;
                                                if (enumValue != null)
                                                {
                                                    if (string.Equals(enumValue.EnumMembers.FirstOrDefault().Name, RevisionKindDeprecated, StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        isDeprecated = true;
                                                    }
                                                    else
                                                    {
                                                        continue;
                                                    }
                                                }

                                                break;

                                            case RevisionVersionProperty:
                                                IEdmStringConstantExpression versionValue = property.Value as IEdmStringConstantExpression;
                                                if (versionValue != null)
                                                {
                                                    version = versionValue.Value;
                                                }

                                                break;

                                            case RevisionDescriptionProperty:
                                                IEdmStringConstantExpression descriptionValue = property.Value as IEdmStringConstantExpression;
                                                if (descriptionValue != null)
                                                {
                                                    message = descriptionValue.Value;
                                                }

                                                break;

                                            case RevisionDateProperty:
                                                IEdmDateConstantExpression dateValue = property.Value as IEdmDateConstantExpression;
                                                if (dateValue != null)
                                                {
                                                    date = dateValue.Value;
                                                }

                                                break;

                                            case RevisionRemovalDateProperty:
                                                IEdmDateConstantExpression removalDateValue = property.Value as IEdmDateConstantExpression;
                                                if (removalDateValue != null)
                                                {
                                                    removalDate = removalDateValue.Value;
                                                }

                                                break;

                                            default:
                                                break;
                                        }
                                    }
                                }

                                if (isDeprecated)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            message = version = string.Empty;
            date = removalDate = null;
            return false;
        }

        /// <summary>
        /// Get Revision annotations directly applied to a model element (not including inherited annotations)
        /// </summary>
        /// <param name="model">The root model to search for annotations (including referenced models).</param>
        /// <param name="annotatedElement">The element to search for annotations on.</param>
        /// <returns>An IEnumerable of all annotations defined on the annotatedElement.</returns>
        private static IEnumerable<IEdmVocabularyAnnotation> GetRevisionAnnotations(IEdmModel model, IEdmVocabularyAnnotatable annotatedElement)
        {
            foreach(IEdmVocabularyAnnotation annotation in model.FindDeclaredVocabularyAnnotations(annotatedElement))
            {
                if (isRevisionsAnnotation(annotation))
                {
                    yield return annotation;
                }
            }

            // look in referenced models
            foreach (IEdmModel referencedModel in model.ReferencedModels)
            {
                // Omit the default models
                if (referencedModel != EdmCoreModel.Instance && referencedModel != CoreVocabularyModel.Instance && referencedModel != CapabilitiesVocabularyModel.Instance)
                {
                    foreach(IEdmVocabularyAnnotation annotation in referencedModel.FindDeclaredVocabularyAnnotations(annotatedElement))
                    {
                        if (isRevisionsAnnotation(annotation))
                        {
                            yield return annotation;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if the annotation is the Revisions annotation, otherwise false
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        /// <returns>True if the annotation is the Revisions annotation, otherwise false.</returns>
        private static bool isRevisionsAnnotation(IEdmVocabularyAnnotation annotation)
        {
            return string.Equals(annotation.Term.FullName(), CoreVocabularyConstants.Revisions, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(annotation.Term.FullName(), DefaultCoreAliasWithRevisionTerm, StringComparison.OrdinalIgnoreCase);
        }
    }
}