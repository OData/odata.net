//---------------------------------------------------------------------
// <copyright file="DynamicExpressionUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;

namespace System.Data.Test.Astoria.Expressions
{
    public class DynamicExpressionUtils
    {
        //Get the expression tree
        public static String ProcessExpressionTree(Workspace w, QueryTreeInfo queryTreeInfo)
        {
            // Retrieve expression tree.
            AstoriaResponse rs;
            RequestUtil.GetAndVerifyStatusCode(w, w.ServiceUri + "/ExpToXml", HttpStatusCode.OK, out rs);
            if (rs.ActualStatusCode != HttpStatusCode.OK && rs.ActualStatusCode != HttpStatusCode.NotFound)
            {
                AstoriaTestLog.WriteLine("/ExpToXml returned error code " + rs.ActualStatusCode.ToString() + ", payload:");
                AstoriaTestLog.FailAndThrow(rs.Payload);
            }

            // Extract XML document from response.
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(rs.Payload);

            // Skip verification in retail builds as they do not contain expression capture hook.
            string embeddedDocument = xml.DocumentElement.InnerText;
            AstoriaTestLog.WriteLine(embeddedDocument);
            if (embeddedDocument.StartsWith("WARNING"))
            {
                // Warn the user.
                AstoriaTestLog.WriteLine("WARNING: missing expression tree!");
                AstoriaTestLog.Skip("Test variation skipped");
            }

            // Separate string form and XML form.
            string[] pair = embeddedDocument.Split(new string[] { "[[===]]" }, StringSplitOptions.None);
            xml.LoadXml(pair[1].Replace("utf-16", "utf-8"));
            AstoriaTestLog.WriteLine(xml.OuterXml);
            if (queryTreeInfo != null)
            {
                queryTreeInfo.currentTreeXml = xml;
            }
            //return xml;
            return pair[0];
        }
    }
}
