//---------------------------------------------------------------------
// <copyright file="ODataPayloadElement.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Abstract base class for all payload representation elements. Infers the PayloadElementType from the actual CLR type.
    /// </summary>
    [DebuggerDisplay("{StringRepresentation}")]
    public abstract class ODataPayloadElement : IAnnotatable<ODataPayloadElementAnnotation>
    {
        /// <summary>
        /// Map which stores the mapping between type name and the respective element type enum.
        /// </summary>
        /// <remarks>
        /// This acts like a cache to avoid expensive enum parsing.
        /// </remarks>
        private static Dictionary<string, ODataPayloadElementType> typeNameToElementTypeMap = new Dictionary<string, ODataPayloadElementType>();

        /// <summary>
        /// Initializes static members of the ODataPayloadElement class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "We need to compute the value of the map to not duplicate the list.")]
        static ODataPayloadElement()
        {
            // Since the ODataPayloadElementType is static (it's a type) cache the mapping between type name and the element type
            // to avoid expensive enum parsing each time we construct a new OData payload element (which is VERY often)
            foreach (object payloadElementType in Enum.GetValues(typeof(ODataPayloadElementType)))
            {
                typeNameToElementTypeMap.Add(Enum.GetName(typeof(ODataPayloadElementType), payloadElementType), (ODataPayloadElementType)payloadElementType);
            }
        }

        /// <summary>
        /// Initializes a new instance of the ODataPayloadElement class. 
        /// Infers the ElementType using ODataPayloadElement.GetElementType with the current instance's type
        /// </summary>
        protected ODataPayloadElement()
        {
            this.ElementType = ODataPayloadElement.GetElementType(this.GetType());
            this.Annotations = new List<ODataPayloadElementAnnotation>();
        }

        /// <summary>
        /// Gets the type of the element as an enum
        /// </summary>
        public ODataPayloadElementType ElementType { get; private set; }

        /// <summary>
        /// Gets the list of annotations for this element
        /// </summary>
        public IList<ODataPayloadElementAnnotation> Annotations { get; private set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public abstract string StringRepresentation
        {
            get;
        }

        /// <summary>
        /// Adds an annotation to the payload element
        /// </summary>
        /// <param name="toAdd">The annotation to add</param>
        public void Add(ODataPayloadElementAnnotation toAdd)
        {
            this.Annotations.Add(toAdd);
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this element.</param>
        /// <returns>The result of visiting this expression.</returns>
        public abstract TResult Accept<TResult>(IODataPayloadElementVisitor<TResult> visitor);

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that does not return a result.
        /// </summary>
        /// <param name="visitor">The visitor that is visiting this element.</param>
        public abstract void Accept(IODataPayloadElementVisitor visitor);
        
        /// <summary>
        /// Generic version of GetElementType, which returns the Enum value for the given payload element type
        /// </summary>
        /// <typeparam name="T">A non-abstract type which derives from PayloadElement</typeparam>
        /// <returns>The enum representation for the given type</returns>
        internal static ODataPayloadElementType GetElementType<T>() where T : ODataPayloadElement
        {
            return GetElementType(typeof(T));
        }

        /// <summary>
        /// Returns the Enum value for the given payload element type
        /// </summary>
        /// <param name="t">A non-abstract type which derives from PayloadElement</param>
        /// <returns>The enum representation for the given type</returns>
        internal static ODataPayloadElementType GetElementType(Type t)
        {
            ExceptionUtilities.Assert(typeof(ODataPayloadElement).IsAssignableFrom(t), "!typeof(ODataPayloadElement).IsAssignableFrom(t)");

            ODataPayloadElementType payloadElementType;
            ExceptionUtilities.Assert(typeNameToElementTypeMap.TryGetValue(t.Name, out payloadElementType), "!Enum.IsDefined(typeof(ODataPayloadElementType), t.Name)");
            return payloadElementType;
        }
    }
}
