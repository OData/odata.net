namespace __GeneratedOdataV4.Parsers.Rules
{
    using System.Linq;

    using __GeneratedOdataV4.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _queryOptionsParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._queryOptions, CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._queryOptions>> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._queryOptions, CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._queryOptions>>
        {
            private static __GeneratedOdataV4.CstNodes.Rules._odataIdentifier Id =
                new __GeneratedOdataV4.CstNodes.Rules._odataIdentifier(
                    new __GeneratedOdataV4.CstNodes.Rules._identifierLeadingCharacter._ALPHA(
                        new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                            __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._69.Instance)),
                    new CstNodes.Inners.HelperRangedAtMost127<__GeneratedOdataV4.CstNodes.Rules._identifierCharacter>(
                        new[]
                        {
                        new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                            new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._64.Instance)),
                        }));

            private static System.Collections.Generic.IEnumerable<__GeneratedOdataV4.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ> ThisIsATest =
                new[]
                {
                    new __GeneratedOdataV4.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdataV4.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdataV4.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA(
                                    new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                        __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._74.Instance))))),
                    new __GeneratedOdataV4.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdataV4.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdataV4.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA(
                                    new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                        __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._68.Instance))))),
                    new __GeneratedOdataV4.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdataV4.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdataV4.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA(
                                    new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                        __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._69.Instance))))),
                    new __GeneratedOdataV4.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdataV4.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdataV4.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA(
                                    new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                        __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._73.Instance))))),
                    new __GeneratedOdataV4.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdataV4.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdataV4.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA(
                                    new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                        __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._69.Instance))))),
                    new __GeneratedOdataV4.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdataV4.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdataV4.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA(
                                    new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                        __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._73.Instance))))),
                    new __GeneratedOdataV4.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdataV4.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdataV4.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA(
                                    new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                        __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._61.Instance))))),
                    new __GeneratedOdataV4.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdataV4.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdataV4.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA(
                                    new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                        __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._74.Instance))))),
                    new __GeneratedOdataV4.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdataV4.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdataV4.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA(
                                    new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                        __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._65.Instance))))),
                    new __GeneratedOdataV4.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdataV4.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdataV4.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA(
                                    new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                        __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._73.Instance))))),
                    new __GeneratedOdataV4.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdataV4.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdataV4.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA(
                                    new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                        __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._74.Instance))))),
                };

            public static __GeneratedOdataV4.CstNodes.Rules._queryOptions Node { get; } =
                new __GeneratedOdataV4.CstNodes.Rules._queryOptions(
                    new __GeneratedOdataV4.CstNodes.Rules._queryOption._systemQueryOption(
                        new __GeneratedOdataV4.CstNodes.Rules._systemQueryOption._filter(
                            new __GeneratedOdataV4.CstNodes.Rules._filter(
                                new CstNodes.Inners._Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃ(
                                    __GeneratedOdataV4.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx24x66x69x6Cx74x65x72ʺ.Instance),
                                __GeneratedOdataV4.CstNodes.Rules._EQ.Instance,
                                new __GeneratedOdataV4.CstNodes.Rules._boolCommonExpr(
                                    new __GeneratedOdataV4.CstNodes.Rules._commonExpr(
                                        new __GeneratedOdataV4.CstNodes.Inners._ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ(
                                            new __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._firstMemberExpr(
                                                new __GeneratedOdataV4.CstNodes.Rules._firstMemberExpr._memberExpr(
                                                    new __GeneratedOdataV4.CstNodes.Rules._memberExpr(
                                                        null,
                                                        new __GeneratedOdataV4.CstNodes.Inners._ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ(
                                                            new __GeneratedOdataV4.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._propertyPathExpr(
                                                                new __GeneratedOdataV4.CstNodes.Rules._propertyPathExpr(
                                                                    new CstNodes.Inners._ⲤentityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡Ↄ(
                                                                        new __GeneratedOdataV4.CstNodes.Inners._entityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡._primitiveProperty_꘡primitivePathExpr꘡(
                                                                            new __GeneratedOdataV4.CstNodes.Rules._primitiveProperty._primitiveKeyProperty(
                                                                                new __GeneratedOdataV4.CstNodes.Rules._primitiveKeyProperty(
                                                                                    Id)),
                                                                            null))))))))),
                                        null,
                                        new __GeneratedOdataV4.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._eqExpr(
                                            new __GeneratedOdataV4.CstNodes.Rules._eqExpr(
                                                new __GeneratedOdataV4.CstNodes.Rules._RWS(
                                                    new __GeneratedOdataV4.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV4.CstNodes.Inners._ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ>(
                                                        new[]
                                                        {
                                                            new __GeneratedOdataV4.CstNodes.Inners._ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ(
                                                                __GeneratedOdataV4.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._SP.Instance),
                                                        })),
                                                __GeneratedOdataV4.CstNodes.Inners._ʺx65x71ʺ.Instance,
                                                new __GeneratedOdataV4.CstNodes.Rules._RWS(
                                                    new __GeneratedOdataV4.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV4.CstNodes.Inners._ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ>(
                                                        new[]
                                                        {
                                                            new __GeneratedOdataV4.CstNodes.Inners._ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ(
                                                                __GeneratedOdataV4.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._SP.Instance),
                                                        })),
                                                new __GeneratedOdataV4.CstNodes.Rules._commonExpr(
                                                    new __GeneratedOdataV4.CstNodes.Inners._ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ(
                                                        new __GeneratedOdataV4.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._primitiveLiteral(
                                                            new __GeneratedOdataV4.CstNodes.Rules._primitiveLiteral._string(
                                                                new __GeneratedOdataV4.CstNodes.Rules._string(
                                                                    __GeneratedOdataV4.CstNodes.Rules._SQUOTE._ʺx27ʺ.Instance,
                                                                    ThisIsATest,
                                                                    __GeneratedOdataV4.CstNodes.Rules._SQUOTE._ʺx27ʺ.Instance)))),
                                                    null,
                                                    null,
                                                    null))),
                                        null))))),
                    Enumerable.Empty<__GeneratedOdataV4.CstNodes.Inners._Ⲥʺx26ʺ_queryOptionↃ>());

            public CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, _queryOptions> Parse(in CombinatorParsingV3.StringInput input)
            {
                var remainder = input.Next(out var more);
                for (int i = 0; i < 26; ++i)
                {
                    remainder = remainder.Next(out more);
                }

                return new CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, _queryOptions>(
                    true,
                    default,
                    more,
                    remainder);
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._queryOptions> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._queryOptions>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._queryOptions> Parse(IInput<char>? input)
            {
                var _queryOption_1 = __GeneratedOdataV4.Parsers.Rules._queryOptionParser.Instance.Parse(input);

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._queryOptions(_queryOption_1.Parsed, System.Linq.Enumerable.Empty<__GeneratedOdataV4.CstNodes.Inners._Ⲥʺx26ʺ_queryOptionↃ>()), _queryOption_1.Remainder);
            }
        }
    }
    
}
