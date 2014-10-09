using System;
using System.CodeDom;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.OData.Core.Aggregation;
using Xbehave;

namespace AggregationTestProject
{
   
    public class HelpersTest
    {
        [Scenario]
        public void TrimOneTest()
        {
            string res = string.Empty;
            string str = "hello(1234))";
            "executing trim one".Then(() => res = str.TrimOne('(', ')'));
            "only last charterer was removed".Then(() => res.Should().Be("hello(1234)"));

        }

        [Scenario]
        public void TrimMethodCallPrefixTest()
        {
            string res = string.Empty;
            string str = "round(product";
            "executing TrimMethodCallPrefix".Then(() => res = str.TrimMethodCallPrefix());
            "method call string is removed".Then(() => res.Should().Be("product"));
        }

        [Scenario]
        public void TrimMethodCallSufixTest()
        {
            string res = string.Empty;
            string str = "taxRate)";
            "executing TrimMethodCallSuefix".Then(() => res = str.TrimMethodCallSufix());
            "last parenthesis is removed".Then(() => res.Should().Be("taxRate"));
        }
    }
}
