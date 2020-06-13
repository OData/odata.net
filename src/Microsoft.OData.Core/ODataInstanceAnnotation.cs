//---------------------------------------------------------------------
// <copyright file="ODataInstanceAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using System.Xml;
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
        /// <param name="isCustomAnnotation">If the name is not for built-in OData annotation.</param>
        internal ODataInstanceAnnotation(string annotationName, ODataValue annotationValue, bool isCustomAnnotation)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(annotationName, "annotationName");
            if (!isCustomAnnotation && ODataAnnotationNames.IsODataAnnotationName(annotationName))
            {
                // isCustomAnnotation==true includes '@odata.<unknown name>', which won't cause the below exception.
                throw new ArgumentException(Strings.ODataInstanceAnnotation_ReservedNamesNotAllowed(annotationName, JsonLightConstants.ODataAnnotationNamespacePrefix));
            }

            ValidateName(annotationName);
            ValidateValue(annotationValue);
            this.Name = annotationName;
            this.Value = annotationValue;
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
        /// Validates that the given <paramref name="name"/> is a valid instance annotation name.
        /// </summary>
        /// <param name="name">Name to validate.</param>
        internal static void ValidateName(string name)
        {
            if (name.IndexOf('.') < 0 || name[0] == '.' || name[name.Length - 1] == '.')
            {
                throw new ArgumentException(Strings.ODataInstanceAnnotation_NeedPeriodInName(name));
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
                throw new ArgumentException(Strings.ODataInstanceAnnotation_ValueCannotBeODataStreamReferenceValue, nameof(value));
            }
        }
    }
}