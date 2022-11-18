using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Binders
{
    public class InBinderTests
    {

        [Fact]
        public void NormalizeStringCollectionItems()
        {
            Assert.Equal("[\"ABC\"]", InBinder.NormalizeStringCollectionItems("(\"ABC\")"));
            Assert.Equal("[null,null]", InBinder.NormalizeStringCollectionItems("[null,null]"));
            Assert.Equal("[]", InBinder.NormalizeStringCollectionItems("[]"));
            Assert.Equal("[\"\\\"\\\"\"]", InBinder.NormalizeStringCollectionItems("['']"));
            Assert.Equal("[\"\\\"\\\"\"]", InBinder.NormalizeStringCollectionItems("[\"\"]"));
            Assert.Equal("[\"\\\"\\\"\",\"\\\"\\\"\"]", InBinder.NormalizeStringCollectionItems("['','']"));
            Assert.Equal("[\"\\\"\\\"\",\"\\\"\\\"\"]", InBinder.NormalizeStringCollectionItems("[\"\", \"\"]"));
            Assert.Equal("[\"'\"]", InBinder.NormalizeStringCollectionItems("['''']"));
            Assert.Equal("[\"\\\"\"]", InBinder.NormalizeStringCollectionItems("['\"']"));
            Assert.Equal("[\"'\"]", InBinder.NormalizeStringCollectionItems("[\"'\"]"));
            Assert.Equal("[\"'''\"]", InBinder.NormalizeStringCollectionItems("['''''''']"));
            Assert.Equal("[\"A\",,,,\"B\",\"C\"]", InBinder.NormalizeStringCollectionItems("['A',,,,'B','C']"));
            Assert.Equal("[\"[AB'''CD\",\"(EF)GH&IJ\",null]", InBinder.NormalizeStringCollectionItems("[\"[AB'''CD\",'(EF)GH&IJ', null]"));
            Assert.Equal("[\"ABC\",\"DEF\"]", InBinder.NormalizeStringCollectionItems("['ABC',   'DEF]"));
            Assert.Equal("[\"ABC\",\"DEF\"]", InBinder.NormalizeStringCollectionItems("[\"ABC\",   \"DEF]"));
            // The following tests failed in the previous version of the "NormalizeStringCollectionItems" method 
            Assert.Equal("[\"[AB)'\",\"(CD)EF&GH\"]", InBinder.NormalizeStringCollectionItems("['[AB)''','(CD)EF&GH']"));
            Assert.Equal("[\"\\\"\\\"\\\"\"]", InBinder.NormalizeStringCollectionItems("[\"\\\"\\\"\\\"\"]"));
            Assert.Equal("", InBinder.NormalizeStringCollectionItems(""));
            Assert.Equal("\"ABC\"", InBinder.NormalizeStringCollectionItems("\"ABC\""));
            Assert.Equal("", InBinder.NormalizeStringCollectionItems(""));
            Assert.Equal("null", InBinder.NormalizeStringCollectionItems(null));
        }

    }
}
