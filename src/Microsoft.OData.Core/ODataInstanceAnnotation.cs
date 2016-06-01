//---------------------------------------------------------------------
// <copyright file="ODataInstanceAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using System.Xml;
    using System.Diagnostics;
    using Microsoft.OData.JsonLight;

    /// <summary>
    /// Represents an instance annotation.
    /// </summary>
    public sealed class ODataInstanceAnnotation : ODataAnnotatable
    {
        /// <summary>
        /// Constructs a new <see cref="ODataInstanceAnnotation"/> instance.
        /// </summary>
        /// <param name="name">The name of the instance annotation.</param>
        /// <param name="value">The value of the instance annotation.</param>
        public ODataInstanceAnnotation(string name, ODataValue value)
            : this(name, value, false)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="ODataInstanceAnnotation"/> instance.
        /// </summary>
        /// <param name="annotationName">The name of the instance annotation.</param>
        /// <param name="annotationValue">The value of the instance annotation.</param>
        /// <param name="isForUntypedProperty">If it is annotating an untyped property value.</param>
        internal ODataInstanceAnnotation(string annotationName, ODataValue annotationValue, bool isForUntypedProperty)
        {
            Debug.Assert((!isForUntypedProperty) || (annotationValue is ODataUntypedValue),
                "(!isForUntypedProperty) || (annotationValue is ODataUntypedValue)");
            if (!isForUntypedProperty)
            {
                ValidateName(annotationName);
                ValidateValue(annotationValue);
            }

            this.Name = annotationName;
            this.Value = annotationValue;
            this.IsForUntypedProperty = IsForUntypedProperty;
        }

        /// <summary>
        /// Instance annotation name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Instance annotation value.
        /// </summary>
        public ODataValue Value { get; private set; }

        /// <summary>
        /// Gets if it is annotating an untyped property value.
        /// </summary>
        internal bool IsForUntypedProperty { get; set; }

        /// <summary>
        /// Validates that the given <paramref name="name"/> is a valid instance annotation name.
        /// </summary>
        /// <param name="name">Name to validate.</param>
        internal static void ValidateName(string name)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(name, "name");

            if (name.IndexOf('.') < 0 || name[0] == '.' || name[name.Length - 1] == '.')
            {
                throw new ArgumentException(Strings.ODataInstanceAnnotation_NeedPeriodInName(name));
            }

            if (ODataAnnotationNames.IsODataAnnotationName(name))
            {
                throw new ArgumentException(Strings.ODataInstanceAnnotation_ReservedNamesNotAllowed(name, JsonLightConstants.ODataAnnotationNamespacePrefix));
            }

            try
            {
                XmlConvert.VerifyNCName(name);
            }
            catch (XmlException e)
            {
                throw new ArgumentException(Strings.ODataInstanceAnnotation_BadTermName(name), e);
            }
        }

        /// <summary>
        /// Validates the given <paramref name="value"/> is a valid instance annotation value.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        internal static void ValidateValue(ODataValue value)
        {
            ExceptionUtils.CheckArgumentNotNull(value, "value");

            if (value is ODataStreamReferenceValue)
            {
                throw new ArgumentException(Strings.ODataInstanceAnnotation_ValueCannotBeODataStreamReferenceValue, "value");
            }
        }
    }
}