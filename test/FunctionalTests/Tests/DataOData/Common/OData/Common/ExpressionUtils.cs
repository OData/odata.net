//---------------------------------------------------------------------
// <copyright file="ExpressionUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System.Linq.Expressions;
    using System.Xml;
    using System.Xml.Linq;
    using System;
    #endregion Namespaces

    /// <summary>
    /// Helper methods to work with expressions
    /// </summary>
    public static class ExpressionUtils
    {
        public static string ToXml(this Expression expression)
        {
#if SILVERLIGHT
            throw new PlatformNotSupportedException("This method is not supported on non-desktop platforms");
#else
            return ExpressionTreeToXmlSerializer.SerializeToString(expression);
#endif
        }
#if !SILVERLIGHT
        public static XmlDocument ToXmlDocument(this Expression expression)
        {
            return ExpressionTreeToXmlSerializer.SerializeToXmlDocument(expression);
        }
#endif
        public static XDocument ToXDocument(this Expression expression)
        {
#if SILVERLIGHT
            throw new PlatformNotSupportedException("This method is not supported on non-desktop platforms");
#else
            return ExpressionTreeToXmlSerializer.SerializeToXDocument(expression);
#endif
        }
    }
}
