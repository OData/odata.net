//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Validation;
using Microsoft.Data.Edm.Values;

namespace Microsoft.Data.Edm
{
    /// <summary>
    /// Contains extension methods for <see cref="IEdmModel"/> interfaces.
    /// </summary>
    public static class ExtensionMethods
    {
        #region IEdmModel

        private static readonly Func<IEdmModel, string, IEdmSchemaType> findType = (model, qualifiedName) => model.FindDeclaredType(qualifiedName);
        private static readonly Func<IEdmModel, string, IEdmValueTerm> findValueTerm = (model, qualifiedName) => model.FindDeclaredValueTerm(qualifiedName);
        private static readonly Func<IEdmModel, string, IEnumerable<IEdmFunction>> findFunctions = (model, qualifiedName) => model.FindDeclaredFunctions(qualifiedName);
        private static readonly Func<IEdmModel, string, IEdmEntityContainer> findEntityContainer = (model, qualifiedName) => model.FindDeclaredEntityContainer(qualifiedName);
        private static readonly Func<IEnumerable<IEdmFunction>, IEnumerable<IEdmFunction>, IEnumerable<IEdmFunction>> mergeFunctions = (f1, f2) => Enumerable.Concat(f1, f2);

        /// <summary>
        /// Gets the value for the EDM version of the <paramref name="model"/>.
        /// </summary>
        /// <param name="model">Model the version has been set for.</param>
        /// <returns>The version.</returns>
        public static Version GetEdmVersion(this IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            return model.GetAnnotationValue<Version>(model, EdmConstants.InternalUri, EdmConstants.EdmVersionAnnotation);
        }

        /// <summary>
        /// Sets a value of EDM version attribute of the <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The model the version should be set for.</param>
        /// <param name="version">The version.</param>
        public static void SetEdmVersion(this IEdmModel model, Version version)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            model.SetAnnotationValue(model, EdmConstants.InternalUri, EdmConstants.EdmVersionAnnotation, version);
        }

        /// <summary>
        /// Searches for a type with the given name in this model and all referenced models and returns null if no such type exists.
        /// </summary>
        /// <param name="model">The model to search.</param>
        /// <param name="qualifiedName">The qualified name of the type being found.</param>
        /// <returns>The requested type, or null if no such type exists.</returns>
        public static IEdmSchemaType FindType(this IEdmModel model, string qualifiedName)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(qualifiedName, "qualifiedName");

            return FindAcrossModels(model, qualifiedName, findType, Internal.RegistrationHelper.CreateAmbiguousTypeBinding);
        }

        /// <summary>
        /// Searches for a value term with the given name in this model and all referenced models and returns null if no such value term exists.
        /// </summary>
        /// <param name="model">The model to search.</param>
        /// <param name="qualifiedName">The qualified name of the value term being found.</param>
        /// <returns>The requested value term, or null if no such value term exists.</returns>
        public static IEdmValueTerm FindValueTerm(this IEdmModel model, string qualifiedName)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(qualifiedName, "qualifiedName");

            return FindAcrossModels(model, qualifiedName, findValueTerm, Internal.RegistrationHelper.CreateAmbiguousValueTermBinding);
        }

        /// <summary>
        /// Searches for functions with the given name in this model and all referenced models and returns an empty enumerable if no such functions exist.
        /// </summary>
        /// <param name="model">The model to search.</param>
        /// <param name="qualifiedName">The qualified name of the functions being found.</param>
        /// <returns>The requested functions.</returns>
        public static IEnumerable<IEdmFunction> FindFunctions(this IEdmModel model, string qualifiedName)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(qualifiedName, "qualifiedName");

            return FindAcrossModels(model, qualifiedName, findFunctions, mergeFunctions);
        }

        /// <summary>
        /// Searches for an entity container with the given name in this model and all referenced models and returns null if no such entity container exists.
        /// </summary>
        /// <param name="model">The model to search.</param>
        /// <param name="qualifiedName">The qualified name of the entity container being found.</param>
        /// <returns>The requested entity container, or null if no such entity container exists.</returns>
        public static IEdmEntityContainer FindEntityContainer(this IEdmModel model, string qualifiedName)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(qualifiedName, "qualifiedName");

            return FindAcrossModels(model, qualifiedName, findEntityContainer, Internal.RegistrationHelper.CreateAmbiguousEntityContainerBinding);
        }

        /// <summary>
        /// Gets the entity containers belonging to this model.
        /// </summary>
        /// <param name="model">Model to search for entity containers.</param>
        /// <returns>Entity containers belonging to this model.</returns>
        public static IEnumerable<IEdmEntityContainer> EntityContainers(this IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            return model.SchemaElements.OfType<IEdmEntityContainer>();
        }

        /// <summary>
        /// Gets an annotatable element's vocabulary annotations defined in a specific model and models referenced by that model.
        /// </summary>
        /// <param name="model">The model to search.</param>
        /// <param name="element">Element to check for annotations.</param>
        /// <returns>Annotations attached to the element (or, if the element is a type, to its base types) by this model or by models referenced by this model.</returns>
        public static IEnumerable<IEdmVocabularyAnnotation> FindVocabularyAnnotationsIncludingInheritedAnnotations(this IEdmModel model, IEdmVocabularyAnnotatable element)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");

            IEnumerable<IEdmVocabularyAnnotation> result = model.FindDeclaredVocabularyAnnotations(element);

            IEdmStructuredType typeElement = element as IEdmStructuredType;
            if (typeElement != null)
            {
                typeElement = typeElement.BaseType;
                while (typeElement != null)
                {
                    IEdmVocabularyAnnotatable annotatableElement = typeElement as IEdmVocabularyAnnotatable;
                    if (annotatableElement != null)
                    {
                        result = result.Concat(model.FindDeclaredVocabularyAnnotations(annotatableElement));
                    }

                    typeElement = typeElement.BaseType;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets an annotatable element's vocabulary annotations defined in a specific model and models referenced by that model.
        /// </summary>
        /// <param name="model">The model to search.</param>
        /// <param name="element">Element to check for annotations.</param>
        /// <returns>Annotations attached to the element by this model or by models referenced by this model.</returns>
        public static IEnumerable<IEdmVocabularyAnnotation> FindVocabularyAnnotations(this IEdmModel model, IEdmVocabularyAnnotatable element)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");

            IEnumerable<IEdmVocabularyAnnotation> result = model.FindVocabularyAnnotationsIncludingInheritedAnnotations(element);
            foreach (IEdmModel referencedModel in model.ReferencedModels)
            {
                result = result.Concat(referencedModel.FindVocabularyAnnotationsIncludingInheritedAnnotations(element));
            }

            return result;
        }

        /// <summary>
        /// Gets an annotatable element's vocabulary annotations that bind a particular term.
        /// </summary>
        /// <typeparam name="T">Type of the annotation being returned.</typeparam>
        /// <param name="model">Model to search.</param>
        /// <param name="element">Element to check for annotations.</param>
        /// <param name="term">Term to search for.</param>
        /// <returns>Annotations attached to the element by this model or by models referenced by this model that bind the term.</returns>
        public static IEnumerable<T> FindVocabularyAnnotations<T>(this IEdmModel model, IEdmVocabularyAnnotatable element, IEdmTerm term) where T : IEdmVocabularyAnnotation
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");
            EdmUtil.CheckArgumentNull(term, "term");

            return FindVocabularyAnnotations<T>(model, element, term, null);
        }

        /// <summary>
        /// Gets an annotatable element's vocabulary annotations that bind a particular term.
        /// </summary>
        /// <typeparam name="T">Type of the annotation being returned.</typeparam>
        /// <param name="model">Model to search.</param>
        /// <param name="element">Element to check for annotations.</param>
        /// <param name="term">Term to search for.</param>
        /// <param name="qualifier">Qualifier to apply.</param>
        /// <returns>Annotations attached to the element by this model or by models referenced by this model that bind the term with the given qualifier.</returns>
        public static IEnumerable<T> FindVocabularyAnnotations<T>(this IEdmModel model, IEdmVocabularyAnnotatable element, IEdmTerm term, string qualifier) where T : IEdmVocabularyAnnotation
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");
            EdmUtil.CheckArgumentNull(term, "term");

            List<T> result = null;

            foreach (T annotation in model.FindVocabularyAnnotations(element).OfType<T>())
            {
                if (annotation.Term == term && (qualifier == null || qualifier == annotation.Qualifier))
                {
                    if (result == null)
                    {
                        result = new List<T>();
                    }

                    result.Add(annotation);
                }
            }

            return result ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Gets an annotatable element's vocabulary annotations that bind a particular term.
        /// </summary>
        /// <typeparam name="T">Type of the annotation being returned.</typeparam>
        /// <param name="model">Model to search.</param>
        /// <param name="element">Element to check for annotations.</param>
        /// <param name="termName">Name of the term to search for.</param>
        /// <returns>Annotations attached to the element by this model or by models referenced by this model that bind the term.</returns>
        public static IEnumerable<T> FindVocabularyAnnotations<T>(this IEdmModel model, IEdmVocabularyAnnotatable element, string termName) where T : IEdmVocabularyAnnotation
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");
            EdmUtil.CheckArgumentNull(termName, "termName");

            return FindVocabularyAnnotations<T>(model, element, termName, null);
        }

        /// <summary>
        /// Gets an annotatable element's vocabulary annotations that bind a particular term.
        /// </summary>
        /// <typeparam name="T">Type of the annotation being returned.</typeparam>
        /// <param name="model">Model to search.</param>
        /// <param name="element">Element to check for annotations.</param>
        /// <param name="termName">Name of the term to search for.</param>
        /// <param name="qualifier">Qualifier to apply.</param>
        /// <returns>Annotations attached to the element by this model or by models referenced by this model that bind the term with the given qualifier.</returns>
        public static IEnumerable<T> FindVocabularyAnnotations<T>(this IEdmModel model, IEdmVocabularyAnnotatable element, string termName, string qualifier) where T : IEdmVocabularyAnnotation
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");
            EdmUtil.CheckArgumentNull(termName, "termName");
            
            // Look up annotations on the element by name. There's no particular advantage in searching for a term first.
            string name;
            string namespaceName;

            if (EdmUtil.TryGetNamespaceNameFromQualifiedName(termName, out namespaceName, out name))
            {
                foreach (T annotation in model.FindVocabularyAnnotations(element).OfType<T>())
                {
                    IEdmTerm annotationTerm = annotation.Term;
                    if (annotationTerm.Namespace == namespaceName && annotationTerm.Name == name && (qualifier == null || qualifier == annotation.Qualifier))
                    {
                        yield return annotation;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="IEdmValue"/> of a property of a term type that has been applied to the type of a value.
        /// </summary>
        /// <param name="model">Model to search for type annotations.</param>
        /// <param name="context">Value to use as context in evaluation.</param>
        /// <param name="property">Property to evaluate.</param>
        /// <param name="expressionEvaluator">Evaluator to use to perform expression evaluation.</param>
        /// <returns>Value of the property evaluated against the supplied value, or null if no relevant type annotation exists.</returns>
        public static IEdmValue GetPropertyValue(this IEdmModel model, IEdmStructuredValue context, IEdmProperty property, Evaluation.EdmExpressionEvaluator expressionEvaluator)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(context, "context");
            EdmUtil.CheckArgumentNull(property, "property");
            EdmUtil.CheckArgumentNull(expressionEvaluator, "expressionEvaluator");

            return GetPropertyValue<IEdmValue>(model, context, context.Type.AsEntity().EntityDefinition(), property, null, expressionEvaluator.Evaluate);
        }

        /// <summary>
        /// Gets the <see cref="IEdmValue"/> of a property of a term type that has been applied to the type of a value.
        /// </summary>
        /// <param name="model">Model to search for type annotations.</param>
        /// <param name="context">Value to use as context in evaluation.</param>
        /// <param name="property">Property to evaluate.</param>
        /// <param name="qualifier">Qualifier to apply.</param>
        /// <param name="expressionEvaluator">Evaluator to use to perform expression evaluation.</param>
        /// <returns>Value of the property evaluated against the supplied value, or null if no relevant type annotation exists.</returns>
        public static IEdmValue GetPropertyValue(this IEdmModel model, IEdmStructuredValue context, IEdmProperty property, string qualifier, Evaluation.EdmExpressionEvaluator expressionEvaluator)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(context, "context");
            EdmUtil.CheckArgumentNull(property, "property");
            EdmUtil.CheckArgumentNull(expressionEvaluator, "expressionEvaluator");

            return GetPropertyValue<IEdmValue>(model, context, context.Type.AsEntity().EntityDefinition(), property, qualifier, expressionEvaluator.Evaluate);
        }

        /// <summary>
        /// Gets the CLR value of a property of a term type that has been applied to the type of a value.
        /// </summary>
        /// <typeparam name="T">The CLR type of the value to be returned.</typeparam>
        /// <param name="model">Model to search for type annotations.</param>
        /// <param name="context">Value to use as context in evaluation.</param>
        /// <param name="property">Property to evaluate.</param>
        /// <param name="evaluator">Evaluator to use to perform expression evaluation.</param>
        /// <returns>Value of the property evaluated against the supplied value, or default(<typeparamref name="T"/>) if no relevant type annotation exists.</returns>
        public static T GetPropertyValue<T>(this IEdmModel model, IEdmStructuredValue context, IEdmProperty property, Evaluation.EdmToClrEvaluator evaluator)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(context, "context");
            EdmUtil.CheckArgumentNull(property, "property");
            EdmUtil.CheckArgumentNull(evaluator, "evaluator");

            return GetPropertyValue<T>(model, context, context.Type.AsEntity().EntityDefinition(), property, null, evaluator.EvaluateToClrValue<T>);
        }

        /// <summary>
        /// Gets the CLR value of a property of a term type that has been applied to the type of a value.
        /// </summary>
        /// <typeparam name="T">The CLR type of the value to be returned.</typeparam>
        /// <param name="model">Model to search for type annotations.</param>
        /// <param name="context">Value to use as context in evaluation.</param>
        /// <param name="property">Property to evaluate.</param>
        /// <param name="qualifier">Qualifier to apply.</param>
        /// <param name="evaluator">Evaluator to use to perform expression evaluation.</param>
        /// <returns>Value of the property evaluated against the supplied value, or default(<typeparamref name="T"/>) if no relevant type annotation exists.</returns>
        public static T GetPropertyValue<T>(this IEdmModel model, IEdmStructuredValue context, IEdmProperty property, string qualifier, Evaluation.EdmToClrEvaluator evaluator)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(context, "context");
            EdmUtil.CheckArgumentNull(property, "property");
            EdmUtil.CheckArgumentNull(evaluator, "evaluator");

            return GetPropertyValue<T>(model, context, context.Type.AsEntity().EntityDefinition(), property, qualifier, evaluator.EvaluateToClrValue<T>);
        }

        /// <summary>
        /// Gets the <see cref="IEdmValue "/> of a vocabulary term that has been applied to the type of a value.
        /// </summary>
        /// <param name="model">Model to search for term annotations.</param>
        /// <param name="context">Value to use as context in evaluation.</param>
        /// <param name="termName">Name of the term to evaluate.</param>
        /// <param name="expressionEvaluator">Evaluator to use to perform expression evaluation.</param>
        /// <returns>Value of the term evaluated against the supplied value.</returns>
        public static IEdmValue GetTermValue(this IEdmModel model, IEdmStructuredValue context, string termName, Evaluation.EdmExpressionEvaluator expressionEvaluator)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(context, "context");
            EdmUtil.CheckArgumentNull(termName, "termName");
            EdmUtil.CheckArgumentNull(expressionEvaluator, "expressionEvaluator");

            return GetTermValue<IEdmValue>(model, context, context.Type.AsEntity().EntityDefinition(), termName, null, expressionEvaluator.Evaluate);
        }

        /// <summary>
        /// Gets the <see cref="IEdmValue "/> of a vocabulary term that has been applied to the type of a value.
        /// </summary>
        /// <param name="model">Model to search for term annotations.</param>
        /// <param name="context">Value to use as context in evaluation.</param>
        /// <param name="termName">Name of the term to evaluate.</param>
        /// <param name="qualifier">Qualifier to apply.</param>
        /// <param name="expressionEvaluator">Evaluator to use to perform expression evaluation.</param>
        /// <returns>Value of the term evaluated against the supplied value.</returns>
        public static IEdmValue GetTermValue(this IEdmModel model, IEdmStructuredValue context, string termName, string qualifier, Evaluation.EdmExpressionEvaluator expressionEvaluator)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(context, "context");
            EdmUtil.CheckArgumentNull(termName, "termName");
            EdmUtil.CheckArgumentNull(expressionEvaluator, "expressionEvaluator");

            return GetTermValue<IEdmValue>(model, context, context.Type.AsEntity().EntityDefinition(), termName, qualifier, expressionEvaluator.Evaluate);
        }

        /// <summary>
        /// Gets the <see cref="IEdmValue "/> of a vocabulary term that has been applied to the type of a value.
        /// </summary>
        /// <param name="model">Model to search for term annotations.</param>
        /// <param name="context">Value to use as context in evaluation.</param>
        /// <param name="term">Term to evaluate.</param>
        /// <param name="expressionEvaluator">Evaluator to use to perform expression evaluation.</param>
        /// <returns>Value of the term evaluated against the supplied value.</returns>
        public static IEdmValue GetTermValue(this IEdmModel model, IEdmStructuredValue context, IEdmValueTerm term, Evaluation.EdmExpressionEvaluator expressionEvaluator)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(context, "context");
            EdmUtil.CheckArgumentNull(term, "term");
            EdmUtil.CheckArgumentNull(expressionEvaluator, "expressionEvaluator");

            return GetTermValue<IEdmValue>(model, context, context.Type.AsEntity().EntityDefinition(), term, null, expressionEvaluator.Evaluate);
        }

        /// <summary>
        /// Gets the <see cref="IEdmValue "/> of a vocabulary term that has been applied to the type of a value.
        /// </summary>
        /// <param name="model">Model to search for term annotations.</param>
        /// <param name="context">Value to use as context in evaluation.</param>
        /// <param name="term">Term to evaluate.</param>
        /// <param name="qualifier">Qualifier to apply.</param>
        /// <param name="expressionEvaluator">Evaluator to use to perform expression evaluation.</param>
        /// <returns>Value of the term evaluated against the supplied value.</returns>
        public static IEdmValue GetTermValue(this IEdmModel model, IEdmStructuredValue context, IEdmValueTerm term, string qualifier, Evaluation.EdmExpressionEvaluator expressionEvaluator)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(context, "context");
            EdmUtil.CheckArgumentNull(term, "term");
            EdmUtil.CheckArgumentNull(expressionEvaluator, "expressionEvaluator");

            return GetTermValue<IEdmValue>(model, context, context.Type.AsEntity().EntityDefinition(), term, qualifier, expressionEvaluator.Evaluate);
        }

        /// <summary>
        /// Gets the CLR value of a vocabulary term that has been applied to the type of a value.
        /// </summary>
        /// <typeparam name="T">The CLR type of the value to be returned.</typeparam>
        /// <param name="model">Model to search for term annotations.</param>
        /// <param name="context">Value to use as context in evaluation.</param>
        /// <param name="termName">Name of the term to evaluate.</param>
        /// <param name="evaluator">Evaluator to use to perform expression evaluation.</param>
        /// <returns>Value of the term evaluated against the supplied value.</returns>
        public static T GetTermValue<T>(this IEdmModel model, IEdmStructuredValue context, string termName, Evaluation.EdmToClrEvaluator evaluator)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(context, "context");
            EdmUtil.CheckArgumentNull(termName, "termName");
            EdmUtil.CheckArgumentNull(evaluator, "evaluator");

            return GetTermValue<T>(model, context, context.Type.AsEntity().EntityDefinition(), termName, null, evaluator.EvaluateToClrValue<T>);
        }

        /// <summary>
        /// Gets the CLR value of a vocabulary term that has been applied to the type of a value.
        /// </summary>
        /// <typeparam name="T">The CLR type of the value to be returned.</typeparam>
        /// <param name="model">Model to search for term annotations.</param>
        /// <param name="context">Value to use as context in evaluation.</param>
        /// <param name="termName">Name of the term to evaluate.</param>
        /// <param name="qualifier">Qualifier to apply.</param>
        /// <param name="evaluator">Evaluator to use to perform expression evaluation.</param>
        /// <returns>Value of the term evaluated against the supplied value.</returns>
        public static T GetTermValue<T>(this IEdmModel model, IEdmStructuredValue context, string termName, string qualifier, Evaluation.EdmToClrEvaluator evaluator)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(context, "context");
            EdmUtil.CheckArgumentNull(termName, "termName");
            EdmUtil.CheckArgumentNull(evaluator, "evaluator");

            return GetTermValue<T>(model, context, context.Type.AsEntity().EntityDefinition(), termName, qualifier, evaluator.EvaluateToClrValue<T>);
        }

        /// <summary>
        /// Gets the CLR value of a vocabulary term that has been applied to the type of a value.
        /// </summary>
        /// <typeparam name="T">The CLR type of the value to be returned.</typeparam>
        /// <param name="model">Model to search for term annotations.</param>
        /// <param name="context">Value to use as context in evaluation.</param>
        /// <param name="term">Term to evaluate.</param>
        /// <param name="evaluator">Evaluator to use to perform expression evaluation.</param>
        /// <returns>Value of the term evaluated against the supplied value.</returns>
        public static T GetTermValue<T>(this IEdmModel model, IEdmStructuredValue context, IEdmValueTerm term, Evaluation.EdmToClrEvaluator evaluator)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(context, "context");
            EdmUtil.CheckArgumentNull(term, "term");
            EdmUtil.CheckArgumentNull(evaluator, "evaluator");

            return GetTermValue<T>(model, context, context.Type.AsEntity().EntityDefinition(), term, null, evaluator.EvaluateToClrValue<T>);
        }

        /// <summary>
        /// Gets the CLR value of a vocabulary term that has been applied to the type of a value.
        /// </summary>
        /// <typeparam name="T">The CLR type of the value to be returned.</typeparam>
        /// <param name="model">Model to search for term annotations.</param>
        /// <param name="context">Value to use as context in evaluation.</param>
        /// <param name="term">Term to evaluate.</param>
        /// <param name="qualifier">Qualifier to apply.</param>
        /// <param name="evaluator">Evaluator to use to perform expression evaluation.</param>
        /// <returns>Value of the term evaluated against the supplied value.</returns>
        public static T GetTermValue<T>(this IEdmModel model, IEdmStructuredValue context, IEdmValueTerm term, string qualifier, Evaluation.EdmToClrEvaluator evaluator)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(context, "context");
            EdmUtil.CheckArgumentNull(term, "term");
            EdmUtil.CheckArgumentNull(evaluator, "evaluator");

            return GetTermValue<T>(model, context, context.Type.AsEntity().EntityDefinition(), term, qualifier, evaluator.EvaluateToClrValue<T>);
        }

        /// <summary>
        /// Gets the <see cref="IEdmValue "/> of a vocabulary term that has been applied to an element.
        /// </summary>
        /// <param name="model">Model to search for term annotations.</param>
        /// <param name="element">Annotated element.</param>
        /// <param name="termName">Name of the term to evaluate.</param>
        /// <param name="expressionEvaluator">Evaluator to use to perform expression evaluation.</param>
        /// <returns>Value of the term evaluated against the supplied value.</returns>
        public static IEdmValue GetTermValue(this IEdmModel model, IEdmVocabularyAnnotatable element, string termName, Evaluation.EdmExpressionEvaluator expressionEvaluator)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");
            EdmUtil.CheckArgumentNull(termName, "termName");
            EdmUtil.CheckArgumentNull(expressionEvaluator, "evaluator");

            return GetTermValue<IEdmValue>(model, element, termName, null, expressionEvaluator.Evaluate);
        }

        /// <summary>
        /// Gets the <see cref="IEdmValue "/> of a vocabulary term that has been applied to an element.
        /// </summary>
        /// <param name="model">Model to search for term annotations.</param>
        /// <param name="element">Annotated element.</param>
        /// <param name="termName">Name of the term to evaluate.</param>
        /// <param name="qualifier">Qualifier to apply.</param>
        /// <param name="expressionEvaluator">Evaluator to use to perform expression evaluation.</param>
        /// <returns>Value of the term evaluated against the supplied value.</returns>
        public static IEdmValue GetTermValue(this IEdmModel model, IEdmVocabularyAnnotatable element, string termName, string qualifier, Evaluation.EdmExpressionEvaluator expressionEvaluator)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");
            EdmUtil.CheckArgumentNull(termName, "termName");
            EdmUtil.CheckArgumentNull(expressionEvaluator, "evaluator");

            return GetTermValue<IEdmValue>(model, element, termName, qualifier, expressionEvaluator.Evaluate);
        }

        /// <summary>
        /// Gets the <see cref="IEdmValue "/> of a vocabulary term that has been applied to an element.
        /// </summary>
        /// <param name="model">Model to search for term annotations.</param>
        /// <param name="element">Annotated element.</param>
        /// <param name="term">Term to evaluate.</param>
        /// <param name="expressionEvaluator">Evaluator to use to perform expression evaluation.</param>
        /// <returns>Value of the term evaluated against the supplied value.</returns>
        public static IEdmValue GetTermValue(this IEdmModel model, IEdmVocabularyAnnotatable element, IEdmValueTerm term, Evaluation.EdmExpressionEvaluator expressionEvaluator)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");
            EdmUtil.CheckArgumentNull(term, "term");
            EdmUtil.CheckArgumentNull(expressionEvaluator, "evaluator");

            return GetTermValue<IEdmValue>(model, element, term, null, expressionEvaluator.Evaluate);
        }

        /// <summary>
        /// Gets the <see cref="IEdmValue "/> of a vocabulary term that has been applied to an element.
        /// </summary>
        /// <param name="model">Model to search for term annotations.</param>
        /// <param name="element">Annotated element.</param>
        /// <param name="term">Term to evaluate.</param>
        /// <param name="qualifier">Qualifier to apply.</param>
        /// <param name="expressionEvaluator">Evaluator to use to perform expression evaluation.</param>
        /// <returns>Value of the term evaluated against the supplied value.</returns>
        public static IEdmValue GetTermValue(this IEdmModel model, IEdmVocabularyAnnotatable element, IEdmValueTerm term, string qualifier, Evaluation.EdmExpressionEvaluator expressionEvaluator)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");
            EdmUtil.CheckArgumentNull(term, "term");
            EdmUtil.CheckArgumentNull(expressionEvaluator, "evaluator");

            return GetTermValue<IEdmValue>(model, element, term, qualifier, expressionEvaluator.Evaluate);
        }

        /// <summary>
        /// Gets the CLR value of a vocabulary term that has been applied to an element.
        /// </summary>
        /// <typeparam name="T">The CLR type of the value to be returned.</typeparam>
        /// <param name="model">Model to search for term annotations.</param>
        /// <param name="element">Annotated element.</param>
        /// <param name="termName">Name of the term to evaluate.</param>
        /// <param name="evaluator">Evaluator to use to perform expression evaluation.</param>
        /// <returns>Value of the term evaluated against the supplied value.</returns>
        public static T GetTermValue<T>(this IEdmModel model, IEdmVocabularyAnnotatable element, string termName, Evaluation.EdmToClrEvaluator evaluator)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");
            EdmUtil.CheckArgumentNull(termName, "termName");
            EdmUtil.CheckArgumentNull(evaluator, "evaluator");

            return GetTermValue<T>(model, element, termName, null, evaluator.EvaluateToClrValue<T>);
        }

        /// <summary>
        /// Gets the CLR value of a vocabulary term that has been applied to an element.
        /// </summary>
        /// <typeparam name="T">The CLR type of the value to be returned.</typeparam>
        /// <param name="model">Model to search for term annotations.</param>
        /// <param name="element">Annotated element.</param>
        /// <param name="termName">Name of the term to evaluate.</param>
        /// <param name="qualifier">Qualifier to apply.</param>
        /// <param name="evaluator">Evaluator to use to perform expression evaluation.</param>
        /// <returns>Value of the term evaluated against the supplied value.</returns>
        public static T GetTermValue<T>(this IEdmModel model, IEdmVocabularyAnnotatable element, string termName, string qualifier, Evaluation.EdmToClrEvaluator evaluator)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");
            EdmUtil.CheckArgumentNull(termName, "termName");
            EdmUtil.CheckArgumentNull(evaluator, "evaluator");

            return GetTermValue<T>(model, element, termName, qualifier, evaluator.EvaluateToClrValue<T>);
        }

        /// <summary>
        /// Gets the CLR value of a vocabulary term that has been applied to an element.
        /// </summary>
        /// <typeparam name="T">The CLR type of the value to be returned.</typeparam>
        /// <param name="model">Model to search for term annotations.</param>
        /// <param name="element">Annotated element.</param>
        /// <param name="term">Term to evaluate.</param>
        /// <param name="evaluator">Evaluator to use to perform expression evaluation.</param>
        /// <returns>Value of the term evaluated against the supplied value.</returns>
        public static T GetTermValue<T>(this IEdmModel model, IEdmVocabularyAnnotatable element, IEdmValueTerm term, Evaluation.EdmToClrEvaluator evaluator)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");
            EdmUtil.CheckArgumentNull(term, "term");
            EdmUtil.CheckArgumentNull(evaluator, "evaluator");

            return GetTermValue<T>(model, element, term, null, evaluator.EvaluateToClrValue<T>);
        }

        /// <summary>
        /// Gets the CLR value of a vocabulary term that has been applied to an element.
        /// </summary>
        /// <typeparam name="T">The CLR type of the value to be returned.</typeparam>
        /// <param name="model">Model to search for term annotations.</param>
        /// <param name="element">Annotated element.</param>
        /// <param name="term">Term to evaluate.</param>
        /// <param name="qualifier">Qualifier to apply.</param>
        /// <param name="evaluator">Evaluator to use to perform expression evaluation.</param>
        /// <returns>Value of the term evaluated against the supplied value.</returns>
        public static T GetTermValue<T>(this IEdmModel model, IEdmVocabularyAnnotatable element, IEdmValueTerm term, string qualifier, Evaluation.EdmToClrEvaluator evaluator)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");
            EdmUtil.CheckArgumentNull(term, "term");
            EdmUtil.CheckArgumentNull(evaluator, "evaluator");

            return GetTermValue<T>(model, element, term, qualifier, evaluator.EvaluateToClrValue<T>);
        }

        /// <summary>
        /// Gets documentation for a specified element.
        /// </summary>
        /// <param name="model">The model containing the documentation.</param>
        /// <param name="element">The element.</param>
        /// <returns>Documentation that exists on the element. Otherwise, null.</returns>
        public static IEdmDocumentation GetDocumentation(this IEdmModel model, IEdmElement element)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");

            return (IEdmDocumentation)model.GetAnnotationValue(element, EdmConstants.DocumentationUri, EdmConstants.DocumentationAnnotation);
        }

        /// <summary>
        /// Sets documentation for a specified element.
        /// </summary>
        /// <param name="model">The model containing the documentation.</param>
        /// <param name="element">The element.</param>
        /// <param name="documentation">Documentation to set.</param>
        public static void SetDocumentation(this IEdmModel model, IEdmElement element, IEdmDocumentation documentation)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");

            model.SetAnnotationValue(element, EdmConstants.DocumentationUri, EdmConstants.DocumentationAnnotation, documentation);
        }

        /// <summary>
        /// Gets an annotation value corresponding to the given namespace and name provided.
        /// </summary>
        /// <param name="model">The model containing the annotation.</param>
        /// <param name="element">The annotated element.</param>
        /// <param name="namespaceName">Namespace of the annotation.</param>
        /// <param name="localName">Name of the annotation inside the namespace.</param>
        /// <returns>The requested annotation value, if it exists. Otherwise, null.</returns>
        public static object GetAnnotationValue(this IEdmModel model, IEdmElement element, string namespaceName, string localName)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");

            return model.DirectValueAnnotationsManager.GetAnnotationValue(element, namespaceName, localName);
        }

        /// <summary>
        /// Gets an annotation value corresponding to the given namespace and name provided.
        /// </summary>
        /// <typeparam name="T">Type of the annotation being returned.</typeparam>
        /// <param name="model">The model containing the annotation.</param>
        /// <param name="element">The annotated element.</param>
        /// <param name="namespaceName">Namespace of the annotation.</param>
        /// <param name="localName">Name of the annotation inside the namespace.</param>
        /// <returns>The requested annotation value, if it exists. Otherwise, null.</returns>
        public static T GetAnnotationValue<T>(this IEdmModel model, IEdmElement element, string namespaceName, string localName) where T : class
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");

            return AnnotationValue<T>(model.GetAnnotationValue(element, namespaceName, localName));
        }

        /// <summary>
        /// Gets an annotation value from an annotatable element.
        /// </summary>
        /// <typeparam name="T">Type of the annotation being returned.</typeparam>
        /// <param name="model">The model containing the annotation.</param>
        /// <param name="element">The annotated element.</param>
        /// <returns>The requested annotation, if it exists. Otherwise, null.</returns>
        /// <remarks>
        /// Strongly-typed wrappers for unnamed annotations keyed by CLR type.
        /// </remarks>
        public static T GetAnnotationValue<T>(this IEdmModel model, IEdmElement element) where T : class
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");

            return model.GetAnnotationValue<T>(element, EdmConstants.InternalUri, TypeName<T>.LocalName);
        }

        /// <summary>
        /// Sets an annotation value for an EDM element. If the value is null, no annotation is added and an existing annotation with the same name is removed.
        /// </summary>
        /// <param name="model">The model containing the annotation.</param>
        /// <param name="element">The annotated element.</param>
        /// <param name="namespaceName">Namespace that the annotation belongs to.</param>
        /// <param name="localName">Name of the annotation within the namespace.</param>
        /// <param name="value">Value of the new annotation.</param>
        public static void SetAnnotationValue(this IEdmModel model, IEdmElement element, string namespaceName, string localName, object value)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");

            model.DirectValueAnnotationsManager.SetAnnotationValue(element, namespaceName, localName, value);
        }

        /// <summary>
        /// Gets all schema elements from the model, and models referenced by it.
        /// </summary>
        /// <param name="model">Model to search for elements</param>
        /// <returns>Schema elements from the model, and models referenced by it.</returns>
        public static IEnumerable<IEdmSchemaElement> SchemaElementsAcrossModels(this IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(model, "model");

            IEnumerable<IEdmSchemaElement> result = new IEdmSchemaElement[] { };
            foreach (IEdmModel referencedModel in model.ReferencedModels)
            {
                result = result.Concat(referencedModel.SchemaElements);
            }

            result = result.Concat(model.SchemaElements);
            return result;
        }

        /// <summary>
        /// Finds a list of types that derive from the supplied type directly or indirectly, and across models.
        /// </summary>
        /// <param name="model">The model types are being found on.</param>
        /// <param name="baseType">The base type that derived types are being searched for.</param>
        /// <returns>A list of types that derive from the type.</returns>
        public static IEnumerable<IEdmStructuredType> FindAllDerivedTypes(this IEdmModel model, IEdmStructuredType baseType)
        {
            List<IEdmStructuredType> result = new List<IEdmStructuredType>();
            if (baseType is IEdmSchemaElement)
            {
                model.DerivedFrom(baseType, new HashSetInternal<IEdmStructuredType>(), result);
            }

            return result;
        }

        /// <summary>
        /// Sets an annotation value on an annotatable element.
        /// </summary>
        /// <typeparam name="T">Type of the annotation being set.</typeparam>
        ///  <param name="model">The model containing the annotation.</param>
        /// <param name="element">The annotated element.</param>
        /// <param name="value">Value of the new annotation.</param>
        public static void SetAnnotationValue<T>(this IEdmModel model, IEdmElement element, T value) where T : class
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");

            model.SetAnnotationValue(element, EdmConstants.InternalUri, TypeName<T>.LocalName, value);
        }

        /// <summary>
        /// Retrieves a set of annotation values. For each requested value, returns null if no annotation with the given name exists for the given element.
        /// </summary>
        /// <param name="model">The model in which to find the annotations.</param>
        /// <param name="annotations">The set of requested annotations.</param>
        /// <returns>Returns values that correspond to the provided annotations. A value is null if no annotation with the given name exists for the given element.</returns>
        public static object[] GetAnnotationValues(this IEdmModel model, IEnumerable<IEdmDirectValueAnnotationBinding> annotations)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(annotations, "annotations");

            return model.DirectValueAnnotationsManager.GetAnnotationValues(annotations);
        }

        /// <summary>
        /// Sets a set of annotation values. If a supplied value is null, no annotation is added and an existing annotation with the same name is removed.
        /// </summary>
        /// <param name="model">The model in which to set the annotations.</param>
        /// <param name="annotations">The annotations to set.</param>
        public static void SetAnnotationValues(this IEdmModel model, IEnumerable<IEdmDirectValueAnnotationBinding> annotations)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(annotations, "annotations");

            model.DirectValueAnnotationsManager.SetAnnotationValues(annotations);
        }

        /// <summary>
        /// Gets the direct value annotations for an element.
        /// </summary>
        /// <param name="model">The model containing the annotations.</param>
        /// <param name="element">The annotated element.</param>
        /// <returns>The immediate value annotations of the element.</returns>
        public static IEnumerable<IEdmDirectValueAnnotation> DirectValueAnnotations(this IEdmModel model, IEdmElement element)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(element, "element");
           
            return model.DirectValueAnnotationsManager.GetDirectValueAnnotations(element);
        }

        #endregion

        #region IEdmElement

        /// <summary>
        /// Gets the location of this element.
        /// </summary>
        /// <param name="item">Reference to the calling object.</param>
        /// <returns>The location of the element.</returns>
        public static EdmLocation Location(this IEdmElement item)
        {
            EdmUtil.CheckArgumentNull(item, "item");
            IEdmLocatable locatable = item as IEdmLocatable;
            return locatable != null && locatable.Location != null ? locatable.Location : new ObjectLocation(item);
        }

        #endregion

        #region IEdmVocabularyAnnotatable

        /// <summary>
        /// Gets an annotatable element's vocabulary annotations as seen from a particular model.
        /// </summary>
        /// <param name="element">Reference to the calling object.</param>
        /// <param name="model">Model to check for annotations.</param>
        /// <returns>Annotations attached to the element by the model or by models referenced by the model.</returns>
        public static IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations(this IEdmVocabularyAnnotatable element, IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(element, "element");
            EdmUtil.CheckArgumentNull(model, "model");
            return model.FindVocabularyAnnotations(element);
        }

        #endregion

        #region IEdmSchemaElement
        /// <summary>
        /// Gets the full name of the element.
        /// </summary>
        /// <param name="element">Reference to the calling object.</param>
        /// <returns>The full name of the element.</returns>
        public static string FullName(this IEdmSchemaElement element)
        {
            EdmUtil.CheckArgumentNull(element, "element");
            return (element.Namespace ?? String.Empty) + "." + (element.Name ?? String.Empty);
        }
        #endregion

        #region IEdmEntityContainer

        /// <summary>
        /// Returns entity sets belonging to an IEdmEntityContainer.
        /// </summary>
        /// <param name="container">Reference to the calling object.</param>
        /// <returns>Entity sets belonging to an IEdmEntityContainer.</returns>
        public static IEnumerable<IEdmEntitySet> EntitySets(this IEdmEntityContainer container)
        {
            EdmUtil.CheckArgumentNull(container, "container");
            return container.Elements.OfType<IEdmEntitySet>();
        }

        /// <summary>
        /// Returns function imports belonging to an IEdmEntityContainer.
        /// </summary>
        /// <param name="container">Reference to the calling object.</param>
        /// <returns>Function imports belonging to an IEdmEntityContainer.</returns>
        public static IEnumerable<IEdmFunctionImport> FunctionImports(this IEdmEntityContainer container)
        {
            EdmUtil.CheckArgumentNull(container, "container");
            return container.Elements.OfType<IEdmFunctionImport>();
        }
        #endregion

        #region IEdmTypeReference
        /// <summary>
        /// Gets the type kind of the type references definition.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The type kind of the reference.</returns>
        public static EdmTypeKind TypeKind(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmType typeDefinition = type.Definition;
            return typeDefinition != null ? typeDefinition.TypeKind : EdmTypeKind.None;
        }

        /// <summary>
        /// Gets the full name of the definition referred to by the type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The full name of this references definition.</returns>
        public static string FullName(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            var namedDefinition = type.Definition as IEdmSchemaElement;
            return namedDefinition != null ? namedDefinition.FullName() : null;
        }

        #endregion

        #region IEdmPrimitiveTypeReference
        /// <summary>
        /// Gets the definition of this primitive type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>Definition of this primitive type reference.</returns>
        public static IEdmPrimitiveType PrimitiveDefinition(this IEdmPrimitiveTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return (IEdmPrimitiveType)type.Definition;
        }

        /// <summary>
        /// Gets the primitive kind of the definition referred to by this type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>Primitive kind of the definition of this reference.</returns>
        public static EdmPrimitiveTypeKind PrimitiveKind(this IEdmPrimitiveTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmPrimitiveType primitive = type.PrimitiveDefinition();
            return primitive != null ? primitive.PrimitiveKind : EdmPrimitiveTypeKind.None;
        }
        #endregion

        #region IEdmStructuredTypeDefinition
        /// <summary>
        /// Gets all properties of the structured type definition and its base types.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>Properties of this type.</returns>
        public static IEnumerable<IEdmProperty> Properties(this IEdmStructuredType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            if (type.BaseType != null)
            {
                foreach (IEdmProperty baseProperty in type.BaseType.Properties())
                {
                    yield return baseProperty;
                }
            }

            if (type.DeclaredProperties != null)
            {
                foreach (IEdmProperty declaredProperty in type.DeclaredProperties)
                {
                    yield return declaredProperty;
                }
            }
        }

        /// <summary>
        /// Gets all structural properties declared in the IEdmStructuredTypeDefinition.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>All structural properties declared in the IEdmStructuredTypeDefinition.</returns>
        public static IEnumerable<IEdmStructuralProperty> DeclaredStructuralProperties(this IEdmStructuredType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.DeclaredProperties.OfType<IEdmStructuralProperty>();
        }

        /// <summary>
        /// Gets the structural properties declared in this type definition and all base types.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The structural properties declared in this type definition and all base types.</returns>
        public static IEnumerable<IEdmStructuralProperty> StructuralProperties(this IEdmStructuredType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.Properties().OfType<IEdmStructuralProperty>();
        }
        #endregion

        #region IEdmStructuredTypeReference
        /// <summary>
        /// Gets the definition of this structured type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The definition of this structured type reference.</returns>
        public static IEdmStructuredType StructuredDefinition(this IEdmStructuredTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return (IEdmStructuredType)type.Definition;
        }

        /// <summary>
        /// Returns true if the definition of this reference is abstract.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>If the definition of this reference is abstract.</returns>
        public static bool IsAbstract(this IEdmStructuredTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.StructuredDefinition().IsAbstract;
        }

        /// <summary>
        /// Returns true if the definition of this reference is open.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>If the definition of this reference is open.</returns>
        public static bool IsOpen(this IEdmStructuredTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.StructuredDefinition().IsOpen;
        }

        /// <summary>
        /// Returns the base type of the definition of this reference. 
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The base type of the definition of this reference. </returns>
        public static IEdmStructuredType BaseType(this IEdmStructuredTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.StructuredDefinition().BaseType;
        }

        /// <summary>
        /// Gets all structural properties declared in the definition of this reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>All structural properties declared in the definition of this reference.</returns>
        public static IEnumerable<IEdmStructuralProperty> DeclaredStructuralProperties(this IEdmStructuredTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.StructuredDefinition().DeclaredStructuralProperties();
        }

        /// <summary>
        /// Gets all structural properties declared in the definition of this reference and all its base types.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>All structural properties declared in the definition of this reference and all its base types.</returns>
        public static IEnumerable<IEdmStructuralProperty> StructuralProperties(this IEdmStructuredTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.StructuredDefinition().StructuralProperties();
        }

        /// <summary>
        /// Finds a property from the definition of this reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <param name="name">Name of the property to find.</param>
        /// <returns>The requested property if it exists. Otherwise, null.</returns>
        public static IEdmProperty FindProperty(this IEdmStructuredTypeReference type, string name)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            EdmUtil.CheckArgumentNull(name, "name");
            return type.StructuredDefinition().FindProperty(name);
        }
        #endregion

        #region IEdmEntityTypeDefinition
        /// <summary>
        /// Gets the base type of this entity type definition.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The base type of this entity type definition.</returns>
        public static IEdmEntityType BaseEntityType(this IEdmEntityType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.BaseType as IEdmEntityType;
        }

        /// <summary>
        /// Gets the navigation properties declared in this entity definition.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The navigation properties declared in this entity definition.</returns>
        public static IEnumerable<IEdmNavigationProperty> DeclaredNavigationProperties(this IEdmEntityType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.DeclaredProperties.OfType<IEdmNavigationProperty>();
        }

        /// <summary>
        /// Get the navigation properties declared in this entity type and all base types.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The navigation properties declared in this entity type and all base types.</returns>
        public static IEnumerable<IEdmNavigationProperty> NavigationProperties(this IEdmEntityType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.Properties().OfType<IEdmNavigationProperty>();
        }

        /// <summary>
        /// Gets the declared key of the most defined entity with a declared key present.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>Key of this type.</returns>
        public static IEnumerable<IEdmStructuralProperty> Key(this IEdmEntityType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmEntityType checkingType = type;
            while (checkingType != null)
            {
                if (checkingType.DeclaredKey != null)
                {
                    return checkingType.DeclaredKey;
                }

                checkingType = checkingType.BaseEntityType();
            }

            return Enumerable.Empty<IEdmStructuralProperty>();
        }

        /// <summary>
        /// Checks whether the given entity type has the <paramref name="property"/> as one of the key properties.
        /// </summary>
        /// <param name="entityType">Given entity type.</param>
        /// <param name="property">Property to be searched for.</param>
        /// <returns><c>true</c> if the type or base types has given property declared as key. <c>false</c> otherwise.</returns>
        public static bool HasDeclaredKeyProperty(this IEdmEntityType entityType, IEdmProperty property)
        {
            EdmUtil.CheckArgumentNull(entityType, "entityType");
            EdmUtil.CheckArgumentNull(property, "property");

            while (entityType != null)
            {
                if (entityType.DeclaredKey != null && entityType.DeclaredKey.Any(k => k == property))
                {
                    return true;
                }

                entityType = entityType.BaseEntityType();
            }

            return false;
        }

        #endregion

        #region IEdmEntityTypeReference
        /// <summary>
        /// Gets the definition of this entity reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The definition of this entity reference.</returns>
        public static IEdmEntityType EntityDefinition(this IEdmEntityTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return (IEdmEntityType)type.Definition;
        }

        /// <summary>
        /// Gets the base type of the definition of this reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The base type of the definition of this reference.</returns>
        public static IEdmEntityType BaseEntityType(this IEdmEntityTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.EntityDefinition().BaseEntityType();
        }

        /// <summary>
        /// Gets the entity key of the definition of this reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The entity key of the definition of this reference.</returns>
        public static IEnumerable<IEdmStructuralProperty> Key(this IEdmEntityTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.EntityDefinition().Key();
        }

        /// <summary>
        /// Gets the navigation properties declared in the definition of this reference and its base types.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The navigation properties declared in the definition of this reference and its base types.</returns>
        public static IEnumerable<IEdmNavigationProperty> NavigationProperties(this IEdmEntityTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.EntityDefinition().NavigationProperties();
        }

        /// <summary>
        /// Gets the navigation properties declared in the definition of this reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The navigation properties declared in the definition of this reference.</returns>
        public static IEnumerable<IEdmNavigationProperty> DeclaredNavigationProperties(this IEdmEntityTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.EntityDefinition().DeclaredNavigationProperties();
        }

        /// <summary>
        /// Finds a navigation property declared in the definition of this reference by name.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <param name="name">Name of the navigation property to find.</param>
        /// <returns>The requested navigation property if it exists. Otherwise, null.</returns>
        public static IEdmNavigationProperty FindNavigationProperty(this IEdmEntityTypeReference type, string name)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            EdmUtil.CheckArgumentNull(name, "name");
            return type.EntityDefinition().FindProperty(name) as IEdmNavigationProperty;
        }
        #endregion

        #region IEdmComplexTypeDefinition
        /// <summary>
        /// Gets the base type of this references definition.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The base type of this references definition.</returns>
        public static IEdmComplexType BaseComplexType(this IEdmComplexType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.BaseType as IEdmComplexType;
        }
        #endregion

        #region IEdmComplexTypeReference
        /// <summary>
        /// Gets the definition of this reference typed as an IEdmComplexTypeDefinition.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The definition of this reference typed as an IEdmComplexTypeDefinition.</returns>
        public static IEdmComplexType ComplexDefinition(this IEdmComplexTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return (IEdmComplexType)type.Definition;
        }

        /// <summary>
        /// Gets the base type of this reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The base type of this reference.</returns>
        public static IEdmComplexType BaseComplexType(this IEdmComplexTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.ComplexDefinition().BaseComplexType();
        }
        #endregion

        #region IEdmEntityReferenceTypeReference
        /// <summary>
        /// Gets the definition of this entity reference type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The definition of this entity reference type reference.</returns>
        public static IEdmEntityReferenceType EntityReferenceDefinition(this IEdmEntityReferenceTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return (IEdmEntityReferenceType)type.Definition;
        }

        /// <summary>
        /// Gets the entity type referred to by the definition of this entity reference type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The entity type referred to by the definition of this entity reference type reference.</returns>
        public static IEdmEntityType EntityType(this IEdmEntityReferenceTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.EntityReferenceDefinition().EntityType;
        }
        #endregion

        #region IEdmCollectionTypeReference
        /// <summary>
        /// Gets the definition of this collection reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The definition of this collection reference.</returns>
        public static IEdmCollectionType CollectionDefinition(this IEdmCollectionTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return (IEdmCollectionType)type.Definition;
        }

        /// <summary>
        /// Gets the element type of the definition of this collection reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The element type of the definition of this collection reference.</returns>
        public static IEdmTypeReference ElementType(this IEdmCollectionTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.CollectionDefinition().ElementType;
        }

        #endregion

        #region IEdmEnumTypeReference

        /// <summary>
        /// Gets the definition of this enumeration reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The definition of this enumeration reference.</returns>
        public static IEdmEnumType EnumDefinition(this IEdmEnumTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return (IEdmEnumType)type.Definition;
        }

        #endregion

        #region IEdmNavigationProperty

        /// <summary>
        /// Gets the multiplicity of this end of a bidirectional relationship between this navigation property and its partner.
        /// </summary>
        /// <param name="property">Reference to the calling object.</param>
        /// <returns>The multiplicity of this end of the relationship.</returns>
        public static EdmMultiplicity Multiplicity(this IEdmNavigationProperty property)
        {
            EdmUtil.CheckArgumentNull(property, "property");
            IEdmNavigationProperty partner = property.Partner;
            if (partner != null)
            {
                IEdmTypeReference partnerType = partner.Type;
                if (partnerType.IsCollection())
                {
                    return EdmMultiplicity.Many;
                }

                return partnerType.IsNullable ? EdmMultiplicity.ZeroOrOne : EdmMultiplicity.One;
            }

            return EdmMultiplicity.One;
        }

        /// <summary>
        /// Gets the entity type targeted by this navigation property.
        /// </summary>
        /// <param name="property">Reference to the calling object.</param>
        /// <returns>The entity type targeted by this navigation property.</returns>
        public static IEdmEntityType ToEntityType(this IEdmNavigationProperty property)
        {
            IEdmType target = property.Type.Definition;
            if (target.TypeKind == EdmTypeKind.Collection)
            {
                target = ((IEdmCollectionType)target).ElementType.Definition;
            }

            if (target.TypeKind == EdmTypeKind.EntityReference)
            {
                target = ((IEdmEntityReferenceType)target).EntityType;
            }

            return target as IEdmEntityType;
        }

        /// <summary>
        /// Gets the entity type declaring this navigation property.
        /// </summary>
        /// <param name="property">Reference to the calling object.</param>
        /// <returns>The entity type that declares this navigation property.</returns>
        public static IEdmEntityType DeclaringEntityType(this IEdmNavigationProperty property)
        {
            return (IEdmEntityType)property.DeclaringType;
        }

        #endregion

        #region IEdmRowTypeReference
        /// <summary>
        /// Gets the definition of this row type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The definition of this row type reference.</returns>
        public static IEdmRowType RowDefinition(this IEdmRowTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return (IEdmRowType)type.Definition;
        }
        #endregion

        #region IEdmTypeAnnotation
        /// <summary>
        /// Gets the binding of a property of the type term of a type annotation.
        /// </summary>
        /// <param name="annotation">Annotation to search.</param>
        /// <param name="property">Property to search for.</param>
        /// <returns>The binding of the property in the type annotation, or null if no binding exists.</returns>
        public static IEdmPropertyValueBinding FindPropertyBinding(this IEdmTypeAnnotation annotation, IEdmProperty property)
        {
            EdmUtil.CheckArgumentNull(annotation, "annotation");
            EdmUtil.CheckArgumentNull(property, "property");

            foreach (IEdmPropertyValueBinding propertyBinding in annotation.PropertyValueBindings)
            {
                if (propertyBinding.BoundProperty == property)
                {
                    return propertyBinding;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the binding of a property of the type term of a type annotation.
        /// </summary>
        /// <param name="annotation">Annotation to search.</param>
        /// <param name="propertyName">Name of the property to search for.</param>
        /// <returns>The binding of the property in the type annotation, or null if no binding exists.</returns>
        public static IEdmPropertyValueBinding FindPropertyBinding(this IEdmTypeAnnotation annotation, string propertyName)
        {
            EdmUtil.CheckArgumentNull(annotation, "annotation");
            EdmUtil.CheckArgumentNull(propertyName, "propertyName");

            foreach (IEdmPropertyValueBinding propertyBinding in annotation.PropertyValueBindings)
            {
                if (propertyBinding.BoundProperty.Name.EqualsOrdinal(propertyName))
                {
                    return propertyBinding;
                }
            }

            return null;
        }

        #endregion

        #region IEdmValueAnnotation
        /// <summary>
        /// Gets the value term of this value annotation.
        /// </summary>
        /// <param name="annotation">Reference to the calling object.</param>
        /// <returns>The value term of this value annotation.</returns>
        public static IEdmValueTerm ValueTerm(this IEdmValueAnnotation annotation)
        {
            EdmUtil.CheckArgumentNull(annotation, "annotation");
            return (IEdmValueTerm)annotation.Term;
        }
        #endregion

        #region IEdmFunctionImport

        /// <summary>
        /// Analyzes <see cref="IEdmFunctionImport"/>.EntitySet expression and returns a static <see cref="IEdmEntitySet"/> reference if available.
        /// </summary>
        /// <param name="functionImport">The function import containing the entity set expression.</param>
        /// <param name="entitySet">The static entity set of the function import.</param>
        /// <returns>True if the entity set expression of the <paramref name="functionImport"/> contains a static reference to an <see cref="IEdmEntitySet"/>, otherwise false.</returns>
        public static bool TryGetStaticEntitySet(this IEdmFunctionImport functionImport, out IEdmEntitySet entitySet)
        {
            var entitySetReference = functionImport.EntitySet as IEdmEntitySetReferenceExpression;
            entitySet = entitySetReference != null ? entitySetReference.ReferencedEntitySet : null;
            return entitySet != null;
        }

        /// <summary>
        /// Analyzes <see cref="IEdmFunctionImport"/>.EntitySet expression and returns a relative path to an <see cref="IEdmEntitySet"/> if available.
        /// The path starts with the <paramref name="parameter"/> and may have optional sequence of <see cref="IEdmNavigationProperty"/> and type casts segments.
        /// </summary>
        /// <param name="functionImport">The function import containing the entity set expression.</param>
        /// <param name="model">The model containing the function import.</param>
        /// <param name="parameter">The function import parameter from which the relative entity set path starts.</param>
        /// <param name="path">The optional sequence of navigation properties.</param>
        /// <returns>True if the entity set expression of the <paramref name="functionImport"/> contains a relative path an <see cref="IEdmEntitySet"/>, otherwise false.</returns>
        public static bool TryGetRelativeEntitySetPath(this IEdmFunctionImport functionImport, IEdmModel model, out IEdmFunctionParameter parameter, out IEnumerable<IEdmNavigationProperty> path)
        {
            parameter = null;
            path = null;

            var entitySetPath = functionImport.EntitySet as IEdmPathExpression;
            if (entitySetPath == null)
            {
                return false;
            }

            var pathToResolve = entitySetPath.Path.ToList();
            if (pathToResolve.Count == 0)
            {
                return false;
            }

            // Resolve the first segment as a parameter.
            parameter = functionImport.FindParameter(pathToResolve[0]);
            if (parameter == null)
            {
                return false;
            }

            if (pathToResolve.Count == 1)
            {
                path = Enumerable.Empty<IEdmNavigationProperty>();
                return true;
            }
            else
            {
                // Get the entity type of the parameter, treat the rest of the path as a sequence of navprops.
                IEdmEntityType entityType = GetPathSegmentEntityType(parameter.Type);
                List<IEdmNavigationProperty> pathList = new List<IEdmNavigationProperty>();
                for (int i = 1; i < pathToResolve.Count; ++i)
                {
                    string segment = pathToResolve[i];
                    if (EdmUtil.IsQualifiedName(segment))
                    {
                        if (i == pathToResolve.Count - 1)
                        {
                            // The last segment must not be type cast.
                            return false;
                        }

                        IEdmEntityType subType = model.FindDeclaredType(segment) as IEdmEntityType;
                        if (subType == null || !subType.IsOrInheritsFrom(entityType))
                        {
                            return false;
                        }

                        entityType = subType;
                    }
                    else
                    {
                        IEdmNavigationProperty navProp = entityType.FindProperty(segment) as IEdmNavigationProperty;
                        if (navProp == null)
                        {
                            return false;
                        }

                        pathList.Add(navProp);
                        entityType = GetPathSegmentEntityType(navProp.Type);
                    }
                }

                path = pathList;
                return true;
            }
        }

        #endregion

        #region IEdmRecordExpression

        /// <summary>
        /// Finds a property of a record expression.
        /// </summary>
        /// <param name="expression">The record expression.</param>
        /// <param name="name">Name of the property to find.</param>
        /// <returns>The property, if found, otherwise null.</returns>
        public static IEdmPropertyConstructor FindProperty(this IEdmRecordExpression expression, string name)
        {
            foreach (IEdmPropertyConstructor propertyConstructor in expression.Properties)
            {
                if (propertyConstructor.Name == name)
                {
                    return propertyConstructor;
                }
            }

            return null;
        }

        #endregion

        /// <summary>
        /// This method is only used for the function import entity set path parsing.
        /// </summary>
        /// <param name="segmentType">The type of the segment.</param>
        /// <returns>Non-null entity type that may be bad.</returns>
        internal static IEdmEntityType GetPathSegmentEntityType(IEdmTypeReference segmentType)
        {
            return (segmentType.IsCollection() ? segmentType.AsCollection().ElementType() : segmentType).AsEntity().EntityDefinition();
        }

        private static T FindAcrossModels<T>(this IEdmModel model, string qualifiedName, Func<IEdmModel, string, T> finder, Func<T, T, T> ambiguousCreator)
        {
            T candidate = finder(model, qualifiedName);

            foreach (IEdmModel reference in model.ReferencedModels)
            {
                T fromReference = finder(reference, qualifiedName);
                if (fromReference != null)
                {
                    candidate = candidate == null ? fromReference : ambiguousCreator(candidate, fromReference);
                }
            }

            return candidate;
        }

        private static T GetPropertyValue<T>(this IEdmModel model, IEdmStructuredValue context, IEdmEntityType contextType, IEdmProperty property, string qualifier, Func<IEdmExpression, IEdmStructuredValue, T> evaluator)
        {
            IEdmEntityType termType = (IEdmEntityType)property.DeclaringType;
            IEnumerable<IEdmTypeAnnotation> annotations = model.FindVocabularyAnnotations<IEdmTypeAnnotation>(contextType, termType, qualifier);

            if (annotations.Count() != 1)
            {
                throw new InvalidOperationException(Edm.Strings.Edm_Evaluator_NoTermTypeAnnotationOnType(contextType.ToTraceString(), termType.ToTraceString()));
            }

            IEdmPropertyValueBinding propertyBinding = annotations.Single().FindPropertyBinding(property);
            return propertyBinding != null ? evaluator(propertyBinding.Value, context) : default(T);
        }

        private static T GetTermValue<T>(this IEdmModel model, IEdmStructuredValue context, IEdmEntityType contextType, IEdmValueTerm term, string qualifier, Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, T> evaluator)
        {
            IEnumerable<IEdmValueAnnotation> annotations = model.FindVocabularyAnnotations<IEdmValueAnnotation>(contextType, term, qualifier);

            if (annotations.Count() != 1)
            {
                throw new InvalidOperationException(Edm.Strings.Edm_Evaluator_NoValueAnnotationOnType(contextType.ToTraceString(), term.ToTraceString()));
            }

            return evaluator(annotations.Single().Value, context, term.Type);
        }

        private static T GetTermValue<T>(this IEdmModel model, IEdmStructuredValue context, IEdmEntityType contextType, string termName, string qualifier, Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, T> evaluator)
        {
            IEnumerable<IEdmValueAnnotation> annotations = model.FindVocabularyAnnotations<IEdmValueAnnotation>(contextType, termName, qualifier);

            if (annotations.Count() != 1)
            {
                throw new InvalidOperationException(Edm.Strings.Edm_Evaluator_NoValueAnnotationOnType(contextType.ToTraceString(), termName));
            }

            IEdmValueAnnotation valueAnnotation = annotations.Single();
            return evaluator(valueAnnotation.Value, context, valueAnnotation.ValueTerm().Type);
        }

        private static T GetTermValue<T>(this IEdmModel model, IEdmVocabularyAnnotatable element, IEdmValueTerm term, string qualifier, Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, T> evaluator)
        {
            IEnumerable<IEdmValueAnnotation> annotations = model.FindVocabularyAnnotations<IEdmValueAnnotation>(element, term, qualifier);

            if (annotations.Count() != 1)
            {
                throw new InvalidOperationException(Edm.Strings.Edm_Evaluator_NoValueAnnotationOnElement(term.ToTraceString()));
            }

            return evaluator(annotations.Single().Value, null, term.Type);
        }

        private static T GetTermValue<T>(this IEdmModel model, IEdmVocabularyAnnotatable element, string termName, string qualifier, Func<IEdmExpression, IEdmStructuredValue, IEdmTypeReference, T> evaluator)
        {
            IEnumerable<IEdmValueAnnotation> annotations = model.FindVocabularyAnnotations<IEdmValueAnnotation>(element, termName, qualifier);

            if (annotations.Count() != 1)
            {
                throw new InvalidOperationException(Edm.Strings.Edm_Evaluator_NoValueAnnotationOnElement(termName));
            }

            IEdmValueAnnotation valueAnnotation = annotations.Single();
            return evaluator(valueAnnotation.Value, null, valueAnnotation.ValueTerm().Type);
        }

        private static T AnnotationValue<T>(object annotation) where T : class
        {
            if (annotation != null)
            {
                T specificAnnotation = annotation as T;
                if (specificAnnotation != null)
                {
                    return specificAnnotation;
                }

                IEdmValue valueAnnotation = annotation as IEdmValue;
                if (valueAnnotation != null)
                {
                }

                throw new InvalidOperationException(Edm.Strings.Annotations_TypeMismatch(annotation.GetType().Name, typeof(T).Name));
            }

            return null;
        }

        private static void DerivedFrom(this IEdmModel model, IEdmStructuredType baseType, HashSetInternal<IEdmStructuredType> visited, List<IEdmStructuredType> derivedTypes)
        {
            if (visited.Add(baseType))
            {
                IEnumerable<IEdmStructuredType> candidates = model.FindDirectlyDerivedTypes(baseType);
                if (candidates != null && candidates.Any())
                {
                    foreach (IEdmStructuredType derivedType in candidates)
                    {
                        derivedTypes.Add(derivedType);
                        model.DerivedFrom(derivedType, visited, derivedTypes);
                    }
                }

                foreach (IEdmModel referenced in model.ReferencedModels)
                {
                    candidates = referenced.FindDirectlyDerivedTypes(baseType);
                    if (candidates != null && candidates.Any())
                    {
                        foreach (IEdmStructuredType derivedType in candidates)
                        {
                            derivedTypes.Add(derivedType);
                            model.DerivedFrom(derivedType, visited, derivedTypes);
                        }
                    }
                }
            }
        }

        internal static class TypeName<T>
        {
            // Use the name of the type as its local name for annotations.
            // Filter out special characters to produce a valid name:
            // '.'                      Appears in qualified names.
            // '`', '[', ']', ','       Appear in generic instantiations.
            // '+'                      Appears in names of local classes.
            public static readonly string LocalName = typeof(T).ToString().Replace("_", "_____").Replace('.', '_').Replace("[", "").Replace("]", "").Replace(",", "__").Replace("`", "___").Replace("+", "____");
        }
    }
}
