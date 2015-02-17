//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementDefaultAtomAnnotationExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Atom
{
    #region Namespaces
    using System;
    using System.Globalization;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    #endregion Namespaces

    /// <summary>
    /// Payload Extensions to add Default Atom Annotations to EntityInstance, EntitySetInstance and possibly other types.
    /// </summary>
    public static class ODataPayloadElementDefaultAtomAnnotationExtensions
    {
        /// <summary>
        /// Adds Default Atom annotations to entry represented as EntityInstance
        /// </summary>
        /// <typeparam name="T">EntityInstance</typeparam>
        /// <param name="payloadElement">Entry to add annotations to</param>
        /// <returns>EntityInstance with default annotations added</returns>
        public static T WithDefaultAtomEntryAnnotations<T>(this T payloadElement)
            where T : EntityInstance
        {
            return (T) payloadElement.WithContentType("application/xml");
        }

        /// <summary>
        /// Adds default Atom annotations for feed represented as EntitySetInstance
        /// </summary>
        /// <typeparam name="T">EntitySetInstance</typeparam>
        /// <param name="entitySet">EntitySetInstance to add annotations to</param>
        /// <returns>EntitySetInstance with default annotations added</returns>
        public static T WithDefaultAtomFeedAnnotations<T>(this T entitySet)
            where T : EntitySetInstance
        {
            return (T)entitySet.WithDefaultAtomIDAnnotation()
                .WithDefaultAtomUpdatedAnnotation()
                .WithDefaultAtomTitleAnnotation() 
                .WithTitleAttribute(string.Empty);
        }

        public static T WithDefaultAtomIDAnnotation<T>(this T payloadElement)
            where T : ODataPayloadElement
        {
            XmlTreeAnnotation id = XmlTreeAnnotation.Atom(TestAtomConstants.AtomIdElementName, "urn:" + TestAtomConstants.AtomGuid);
            payloadElement.Annotations.Add(id);
            return payloadElement;
        }

        private static T WithDefaultAtomTitleAnnotation<T>(this T payloadElement)
            where T : ODataPayloadElement
        {
            XmlTreeAnnotation title = XmlTreeAnnotation.Atom("title", null);
            title.Children.Add(XmlTreeAnnotation.AtomAttribute("type", "text"));
            payloadElement.Annotations.Add(title);
            return payloadElement;
        }

        private static T WithDefaultAtomUpdatedAnnotation<T>(this T payloadElement)
            where T : ODataPayloadElement
        {
            XmlTreeAnnotation updated = XmlTreeAnnotation.Atom(TestAtomConstants.AtomUpdatedElementName,
                "2013-08-13T01:03:16.7800000-7");
            payloadElement.Annotations.Add(updated);
            return payloadElement;
        }
    }
}
