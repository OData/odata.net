﻿//---------------------------------------------------------------------
// <copyright file="PathFunctionalTestsUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
{
    /// <summary>
    /// Class that contains common methods to test URI Path related Functional tests
    /// </summary>
    public class PathFunctionalTestsUtil
    {
        internal static ODataPath RunParsePath(string path)
        {
            return RunParsePath(path, HardCodedTestModel.TestModel);
        }

        internal static ODataPath RunParsePath(string path, IEdmModel model)
        {
            ODataUriParser parser = new ODataUriParser(model, new Uri("http://gobbldgook/"), new Uri("http://gobbldygook/" + path));
            return parser.ParsePath();
        }

        internal static void RunParseErrorPath(string path, string expectedMessage)
        {
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldgook/"), new Uri("http://gobbldygook/" + path)) { Resolver = new ODataUriResolver() { EnableCaseInsensitive = false } };
            Action parse = () => parser.ParsePath();
            parse.ShouldThrow<ODataException>().WithMessage(expectedMessage);
        }
    }
}
