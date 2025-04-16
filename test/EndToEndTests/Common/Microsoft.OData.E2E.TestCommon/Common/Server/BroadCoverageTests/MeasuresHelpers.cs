﻿//---------------------------------------------------------------------
// <copyright file="MeasuresHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Reflection;
using System.Xml;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.E2E.TestCommon.Common.Server.BroadCoverageTests;

public static class MeasuresHelpers
{
    #region Initialization

    public static readonly IEdmModel Instance;
    public static readonly IEdmTerm ISOCurrencyTerm;
    public static readonly IEdmTerm ScaleTerm;

    internal const string MeasuresISOCurrency = "Org.OData.Measures.V1.ISOCurrency";
    internal const string MeasuresScale = "Org.OData.Measures.V1.Scale";

    static MeasuresHelpers()
    {
        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.OData.E2E.TestCommon.Common.Server.BroadCoverageTests.Vocabularies.MeasuresVocabularies.xml"))
        {
            IEnumerable<EdmError> errors;
            SchemaReader.TryParse(new[] { XmlReader.Create(stream) }, out Instance, out errors);
        }

        ISOCurrencyTerm = Instance.FindDeclaredTerm(MeasuresISOCurrency);
        ScaleTerm = Instance.FindDeclaredTerm(MeasuresScale);
    }

    #endregion

    #region ISOCurrency

    public static void SetISOCurrencyMeasuresAnnotation(this EdmModel model, IEdmProperty property, string isoCurrency)
    {
        if (model == null) throw new ArgumentNullException("model");
        if (property == null) throw new ArgumentNullException("property");

        var target = property;
        var term = ISOCurrencyTerm;
        var expression = new EdmStringConstant(isoCurrency);
        var annotation = new EdmVocabularyAnnotation(target, term, expression);
        annotation.SetSerializationLocation(model, property.ToSerializationLocation());
        model.AddVocabularyAnnotation(annotation);
    }

    #endregion

    #region Scale

    public static void SetScaleMeasuresAnnotation(this EdmModel model, IEdmProperty property, byte scale)
    {
        if (model == null) throw new ArgumentNullException("model");
        if (property == null) throw new ArgumentNullException("property");

        var target = property;
        var term = ScaleTerm;
        var expression = new EdmIntegerConstant(scale);
        var annotation = new EdmVocabularyAnnotation(target, term, expression);
        annotation.SetSerializationLocation(model, property.ToSerializationLocation());
        model.AddVocabularyAnnotation(annotation);
    }

    #endregion

    #region Helpers

    private static EdmVocabularyAnnotationSerializationLocation ToSerializationLocation(this IEdmVocabularyAnnotatable target)
    {
        return target is IEdmEntityContainer ? EdmVocabularyAnnotationSerializationLocation.OutOfLine : EdmVocabularyAnnotationSerializationLocation.Inline;
    }

    #endregion
}
