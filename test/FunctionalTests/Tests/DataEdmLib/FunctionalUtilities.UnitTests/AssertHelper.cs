//---------------------------------------------------------------------
// <copyright file="AssertHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities.UnitTests
{
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class AssertHelper
    {
        public static void AssertXElementEquals(string expected, XElement actual)
        {
            XElement expectedXElement = XElement.Parse(expected);
            Assert.AreEqual(expectedXElement.ToString(), actual.ToString());
        }
    }
}
