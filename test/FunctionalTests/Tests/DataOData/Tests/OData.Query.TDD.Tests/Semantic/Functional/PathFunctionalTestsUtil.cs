//---------------------------------------------------------------------
// <copyright file="PathFunctionalTestsUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.OData.Core.UriParser.Semantic;

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic.Functional
{
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Edm;

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
            ODataUriParser parser = new ODataUriParser(HardCodedTestModel.TestModel, new Uri("http://gobbldgook/"), new Uri("http://gobbldygook/" + path));
            Action parse = () => parser.ParsePath();
            parse.ShouldThrow<ODataException>().WithMessage(expectedMessage);
        }
    }
}
