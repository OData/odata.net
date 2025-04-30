//---------------------------------------------------------------------
// <copyright file="MetadataExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.E2E.Tests;

/// <summary>
/// Extension methods that make writing tests easier
/// </summary>
public static class MetadataExtensionMethods
{

    /// <summary>
    /// Gets the member properties that correspond to a given path for the entity type
    /// </summary>
    /// <param name="entityType">The entity type</param>
    /// <param name="pathPieces">The property names from the path</param>
    /// <returns>The series of properties for the path</returns>
    public static IEnumerable<IEdmProperty> GetPropertiesForPath(this IEdmEntityType entityType, string[] pathPieces)
    {
        var currentProperties = entityType.Properties();
        foreach (string propertyName in pathPieces)
        {
            IEdmProperty property = currentProperties.Single(p => p.Name == propertyName);

            var complexDataType = property.Type as IEdmComplexTypeReference;
            if (complexDataType != null)
            {
                currentProperties = complexDataType.ComplexDefinition().Properties();
            }

            var collectionDataType = property.Type as IEdmCollectionTypeReference;
            if (collectionDataType != null)
            {
                var complexElementType = collectionDataType.GetCollectionItemType() as IEdmComplexTypeReference;
                if (complexElementType != null)
                {
                    currentProperties = complexElementType.ComplexDefinition().Properties();
                }
            }

            yield return property;
        }
    }

    /// <summary>
    /// Gets the <see cref="IEdmValue"/> of a property of a term type that has been applied to the type of a value.
    /// </summary>
    /// <param name="model">Model to search for annotations.</param>
    /// <param name="context">Value to use as context in evaluation.</param>
    /// <param name="term">Term to search for annotations.</param>
    /// <param name="property">Property to evaluate.</param>
    /// <param name="expressionEvaluator">Evaluator to use to perform expression evaluation.</param>
    /// <returns>Value of the property evaluated against the supplied value, or null if no relevant annotation exists.</returns>
    public static IEdmValue GetPropertyValue(this IEdmModel model, IEdmStructuredValue context, IEdmTerm term, IEdmProperty property, EdmExpressionEvaluator expressionEvaluator)
    {
        ExceptionUtilities.CheckArgumentNotNull(model, "model");
        ExceptionUtilities.CheckArgumentNotNull(context, "context");
        ExceptionUtilities.CheckArgumentNotNull(property, "property");
        ExceptionUtilities.CheckArgumentNotNull(expressionEvaluator, "expressionEvaluator");

        return GetPropertyValue<IEdmValue>(model, context, context.Type.AsEntity().EntityDefinition(), term, property, null, expressionEvaluator.Evaluate);
    }

    /// <summary>
    /// Gets the <see cref="IEdmValue"/> of a property of a term type that has been applied to the type of a value.
    /// </summary>
    /// <param name="model">Model to search for annotations.</param>
    /// <param name="context">Value to use as context in evaluation.</param>
    /// <param name="term">Term to search for annotations.</param>
    /// <param name="property">Property to evaluate.</param>
    /// <param name="qualifier">Qualifier to apply.</param>
    /// <param name="expressionEvaluator">Evaluator to use to perform expression evaluation.</param>
    /// <returns>Value of the property evaluated against the supplied value, or null if no relevant annotation exists.</returns>
    public static IEdmValue GetPropertyValue(this IEdmModel model, IEdmStructuredValue context, IEdmTerm term, IEdmProperty property, string qualifier, EdmExpressionEvaluator expressionEvaluator)
    {
        ExceptionUtilities.CheckArgumentNotNull(model, "model");
        ExceptionUtilities.CheckArgumentNotNull(context, "context");
        ExceptionUtilities.CheckArgumentNotNull(property, "property");
        ExceptionUtilities.CheckArgumentNotNull(expressionEvaluator, "expressionEvaluator");

        return GetPropertyValue<IEdmValue>(model, context, context.Type.AsEntity().EntityDefinition(), term, property, qualifier, expressionEvaluator.Evaluate);
    }

    /// <summary>
    /// Gets the CLR value of a property of a term type that has been applied to the type of a value.
    /// </summary>
    /// <typeparam name="T">The CLR type of the value to be returned.</typeparam>
    /// <param name="model">Model to search for annotations.</param>
    /// <param name="context">Value to use as context in evaluation.</param>
    /// <param name="term">Term to search for annotations.</param>
    /// <param name="property">Property to evaluate.</param>
    /// <param name="evaluator">Evaluator to use to perform expression evaluation.</param>
    /// <returns>Value of the property evaluated against the supplied value, or default(<typeparamref name="T"/>) if no relevant annotation exists.</returns>
    public static T GetPropertyValue<T>(this IEdmModel model, IEdmStructuredValue context, IEdmTerm term, IEdmProperty property, EdmToClrEvaluator evaluator)
    {
        ExceptionUtilities.CheckArgumentNotNull(model, "model");
        ExceptionUtilities.CheckArgumentNotNull(context, "context");
        ExceptionUtilities.CheckArgumentNotNull(property, "property");
        ExceptionUtilities.CheckArgumentNotNull(evaluator, "evaluator");

        return GetPropertyValue<T>(model, context, context.Type.AsEntity().EntityDefinition(), term, property, null, evaluator.EvaluateToClrValue<T>);
    }

    /// <summary>
    /// Gets the CLR value of a property of a term type that has been applied to the type of a value.
    /// </summary>
    /// <typeparam name="T">The CLR type of the value to be returned.</typeparam>
    /// <param name="model">Model to search for annotations.</param>
    /// <param name="context">Value to use as context in evaluation.</param>
    /// <param name="term">Term to search for annotations.</param>
    /// <param name="property">Property to evaluate.</param>
    /// <param name="qualifier">Qualifier to apply.</param>
    /// <param name="evaluator">Evaluator to use to perform expression evaluation.</param>
    /// <returns>Value of the property evaluated against the supplied value, or default(<typeparamref name="T"/>) if no relevant annotation exists.</returns>
    public static T GetPropertyValue<T>(this IEdmModel model, IEdmStructuredValue context, IEdmTerm term, IEdmProperty property, string qualifier, EdmToClrEvaluator evaluator)
    {
        ExceptionUtilities.CheckArgumentNotNull(model, "model");
        ExceptionUtilities.CheckArgumentNotNull(context, "context");
        ExceptionUtilities.CheckArgumentNotNull(property, "property");
        ExceptionUtilities.CheckArgumentNotNull(evaluator, "evaluator");

        return GetPropertyValue<T>(model, context, context.Type.AsEntity().EntityDefinition(), term, property, qualifier, evaluator.EvaluateToClrValue<T>);
    }

    private static T GetPropertyValue<T>(this IEdmModel model, IEdmStructuredValue context, IEdmEntityType contextType, IEdmTerm term, IEdmProperty property, string qualifier, Func<IEdmExpression, IEdmStructuredValue, T> evaluator)
    {
        IEnumerable<IEdmVocabularyAnnotation> annotations = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(contextType, term, qualifier);

        if (annotations.Count() != 1)
        {
            throw new InvalidOperationException("Type " + contextType.ToTraceString() + " must have a single annotation with term " + term.ToTraceString());
        }

        var annotationValue = annotations.Single().Value as IEdmRecordExpression;

        if (annotationValue == null)
        {
            throw new InvalidOperationException("Type " + contextType.ToTraceString() + " must have a single annotation containing a record expression with term " + term.ToTraceString());
        }

        var propertyConstructor = annotationValue.FindProperty(property.Name);
        return propertyConstructor != null ? evaluator(propertyConstructor.Value, context) : default(T);
    }
}
