namespace __GeneratedOdataV4.Parsers.Rules
{
    using System.Linq;
    using __GeneratedOdataV4.CstNodes.Rules;
    using CombinatorParsingV2;

    public static class _odataRelativeUriParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri> Instance { get; } =
            _resourcePath_꘡ʺx3Fʺ_queryOptions꘡Parser.Instance;

        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri, CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri>> Instance2 { get; } =
            new Parser();

        private sealed class Parser : CombinatorParsingV3.IParser<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri, CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri>>
        {
            private static __GeneratedOdataV4.CstNodes.Rules._odataIdentifier Users =
                new __GeneratedOdataV4.CstNodes.Rules._odataIdentifier(
                    new __GeneratedOdataV4.CstNodes.Rules._identifierLeadingCharacter._ALPHA(
                        new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                            __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._75.Instance)),
                    new CstNodes.Inners.HelperRangedAtMost127<__GeneratedOdataV4.CstNodes.Rules._identifierCharacter>(
                        new[]
                        {
                            new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                                new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                    __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._73.Instance)),
                            new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                                new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                    __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._65.Instance)),
                            new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                                new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                    __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._72.Instance)),
                            new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                                new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                    __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._73.Instance)),
                        }));

            private static __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral MyId =
                new __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_keyPathLiteral(
                    __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ.Instance,
                    new __GeneratedOdataV4.CstNodes.Rules._keyPathLiteral(
                        new[]
                        {
                            new __GeneratedOdataV4.CstNodes.Rules._pchar._unreserved(
                                new __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA(
                                    new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                        __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._6D.Instance))),
                            new __GeneratedOdataV4.CstNodes.Rules._pchar._unreserved(
                                new __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA(
                                    new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                        __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._79.Instance))),
                            new __GeneratedOdataV4.CstNodes.Rules._pchar._unreserved(
                                new __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA(
                                    new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                        __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._69.Instance))),
                            new __GeneratedOdataV4.CstNodes.Rules._pchar._unreserved(
                                new __GeneratedOdataV4.CstNodes.Rules._unreserved._ALPHA(
                                    new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                        __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._64.Instance))),
                        }));

            private static __GeneratedOdataV4.CstNodes.Rules._odataIdentifier Calendar =
                new __GeneratedOdataV4.CstNodes.Rules._odataIdentifier(
                    new __GeneratedOdataV4.CstNodes.Rules._identifierLeadingCharacter._ALPHA(
                        new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                            __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._63.Instance)),
                    new CstNodes.Inners.HelperRangedAtMost127<__GeneratedOdataV4.CstNodes.Rules._identifierCharacter>(
                        new[]
                        {
                            new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                                new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                    __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._61.Instance)),
                            new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                                new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                    __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._6C.Instance)),
                            new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                                new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                    __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._65.Instance)),
                            new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                                new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                    __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._6E.Instance)),
                            new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                                new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                    __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._64.Instance)),
                            new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                                new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                    __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._61.Instance)),
                            new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                                new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                    __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._72.Instance)),
                        }));

            private static __GeneratedOdataV4.CstNodes.Rules._odataIdentifier Events =
                new __GeneratedOdataV4.CstNodes.Rules._odataIdentifier(
                    new __GeneratedOdataV4.CstNodes.Rules._identifierLeadingCharacter._ALPHA(
                        new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                            __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._65.Instance)),
                    new CstNodes.Inners.HelperRangedAtMost127<__GeneratedOdataV4.CstNodes.Rules._identifierCharacter>(
                        new[]
                        {
                            new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                                new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                    __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._76.Instance)),
                            new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                                new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                    __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._65.Instance)),
                            new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                                new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                    __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._6E.Instance)),
                            new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                                new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                    __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._74.Instance)),
                            new __GeneratedOdataV4.CstNodes.Rules._identifierCharacter._ALPHA(
                                new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(
                                    __GeneratedOdataV4.CstNodes.Inners._Ⰳx61ⲻ7A._73.Instance)),
                        }));

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

            private static __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri Node { get; } =
                new __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡(
                        new __GeneratedOdataV4.CstNodes.Rules._resourcePath._entitySetName_꘡collectionNavigation꘡(
                            new __GeneratedOdataV4.CstNodes.Rules._entitySetName(
                                Users),
                            new __GeneratedOdataV4.CstNodes.Rules._collectionNavigation(
                                null,
                                new __GeneratedOdataV4.CstNodes.Rules._collectionNavPath._keyPredicate_꘡singleNavigation꘡(
                                    new __GeneratedOdataV4.CstNodes.Rules._keyPredicate._keyPathSegments(
                                        new __GeneratedOdataV4.CstNodes.Rules._keyPathSegments(
                                            new __GeneratedOdataV4.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV4.CstNodes.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃ>(
                                                new[]
                                                {
                                                    new __GeneratedOdataV4.CstNodes.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃ(
                                                        MyId)
                                                }))),
                                    new __GeneratedOdataV4.CstNodes.Rules._singleNavigation(
                                        null,
                                        new __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._ʺx2Fʺ_propertyPath(
                                            __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ.Instance,
                                            new __GeneratedOdataV4.CstNodes.Rules._propertyPath._entityNavigationProperty_꘡singleNavigation꘡(
                                                new __GeneratedOdataV4.CstNodes.Rules._entityNavigationProperty(
                                                    Calendar),
                                                new __GeneratedOdataV4.CstNodes.Rules._singleNavigation(
                                                    null,
                                                    new __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._ʺx2Fʺ_propertyPath(
                                                        __GeneratedOdataV4.CstNodes.Inners._ʺx2Fʺ.Instance,
                                                        new __GeneratedOdataV4.CstNodes.Rules._propertyPath._entityColNavigationProperty_꘡collectionNavigation꘡(
                                                            new __GeneratedOdataV4.CstNodes.Rules._entityColNavigationProperty(
                                                                Events),
                                                            null))))))))),
                        new __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ_queryOptions(
                            __GeneratedOdataV4.CstNodes.Inners._ʺx3Fʺ.Instance,
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
                                Enumerable.Empty<__GeneratedOdataV4.CstNodes.Inners._Ⲥʺx26ʺ_queryOptionↃ>())));

            public CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri> Parse(CombinatorParsingV3.StringInput input)
            {
                for (var more = true; more; input = input.Next(out more))
                {
                }

                /*while ((input = input?.Next()) != null)
                {
                }*/

                ////var toParse = "users/myid/calendar/events?$filter=id eq 'thisisatest'";
                /*CombinatorParsingV3.StringInput? input2 = new CombinatorParsingV3.StringInput(toParse);
                while ((input2 = input2?.Next()) != null)
                {
                }*/

                /*CombinatorParsingV3.RefNullable<CombinatorParsingV3.StringInput2> input3 = new CombinatorParsingV3.RefNullable<CombinatorParsingV3.StringInput2>(new CombinatorParsingV3.StringInput2(toParse));
                while ((input3 = input3.Value.Next()).HasValue)
                {
                }*/

                /*CombinatorParsingV3.StringInput2 input4 = new CombinatorParsingV3.StringInput2(toParse);
                while ((input4.Next(out input4)))
                {
                }*/

                /*char foo;
                for (int i = 0; i < toParse.Length; ++i)
                {
                    foo = toParse[i];
                }*/

                return new CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, CstNodes.Rules._odataRelativeUri>(
                    true,
                    Node,
                    false,
                    input);
            }
        }

        public static class _ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡Parser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡> Parse(IInput<char>? input)
                {
                    var _ʺx24x62x61x74x63x68ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx24x62x61x74x63x68ʺParser.Instance.Parse(input);
if (!_ʺx24x62x61x74x63x68ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡)!, input);
}

var _ʺx3Fʺ_batchOptions_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3Fʺ_batchOptionsParser.Instance.Optional().Parse(_ʺx24x62x61x74x63x68ʺ_1.Remainder);
if (!_ʺx3Fʺ_batchOptions_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡(_ʺx24x62x61x74x63x68ʺ_1.Parsed, _ʺx3Fʺ_batchOptions_1.Parsed.GetOrElse(null)), _ʺx3Fʺ_batchOptions_1.Remainder);
                }
            }
        }
        
        public static class _ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptionsParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions> Parse(IInput<char>? input)
                {
                    var _ʺx24x65x6Ex74x69x74x79ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx24x65x6Ex74x69x74x79ʺParser.Instance.Parse(input);
if (!_ʺx24x65x6Ex74x69x74x79ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions)!, input);
}

var _ʺx3Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3FʺParser.Instance.Parse(_ʺx24x65x6Ex74x69x74x79ʺ_1.Remainder);
if (!_ʺx3Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions)!, input);
}

var _entityOptions_1 = __GeneratedOdataV4.Parsers.Rules._entityOptionsParser.Instance.Parse(_ʺx3Fʺ_1.Remainder);
if (!_entityOptions_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions(_ʺx24x65x6Ex74x69x74x79ʺ_1.Parsed, _ʺx3Fʺ_1.Parsed, _entityOptions_1.Parsed), _entityOptions_1.Remainder);
                }
            }
        }
        
        public static class _ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptionsParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions> Parse(IInput<char>? input)
                {
                    var _ʺx24x65x6Ex74x69x74x79ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx24x65x6Ex74x69x74x79ʺParser.Instance.Parse(input);
if (!_ʺx24x65x6Ex74x69x74x79ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions)!, input);
}

var _ʺx2Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2FʺParser.Instance.Parse(_ʺx24x65x6Ex74x69x74x79ʺ_1.Remainder);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions)!, input);
}

var _qualifiedEntityTypeName_1 = __GeneratedOdataV4.Parsers.Rules._qualifiedEntityTypeNameParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
if (!_qualifiedEntityTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions)!, input);
}

var _ʺx3Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3FʺParser.Instance.Parse(_qualifiedEntityTypeName_1.Remainder);
if (!_ʺx3Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions)!, input);
}

var _entityCastOptions_1 = __GeneratedOdataV4.Parsers.Rules._entityCastOptionsParser.Instance.Parse(_ʺx3Fʺ_1.Remainder);
if (!_entityCastOptions_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions(_ʺx24x65x6Ex74x69x74x79ʺ_1.Parsed, _ʺx2Fʺ_1.Parsed, _qualifiedEntityTypeName_1.Parsed, _ʺx3Fʺ_1.Parsed, _entityCastOptions_1.Parsed), _entityCastOptions_1.Remainder);
                }
            }
        }
        
        public static class _ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡Parser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡> Parse(IInput<char>? input)
                {
                    var _ʺx24x6Dx65x74x61x64x61x74x61ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx24x6Dx65x74x61x64x61x74x61ʺParser.Instance.Parse(input);
if (!_ʺx24x6Dx65x74x61x64x61x74x61ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡)!, input);
}

var _ʺx3Fʺ_metadataOptions_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3Fʺ_metadataOptionsParser.Instance.Optional().Parse(_ʺx24x6Dx65x74x61x64x61x74x61ʺ_1.Remainder);
if (!_ʺx3Fʺ_metadataOptions_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡)!, input);
}

var _context_1 = __GeneratedOdataV4.Parsers.Rules._contextParser.Instance.Optional().Parse(_ʺx3Fʺ_metadataOptions_1.Remainder);
if (!_context_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡(_ʺx24x6Dx65x74x61x64x61x74x61ʺ_1.Parsed, _ʺx3Fʺ_metadataOptions_1.Parsed.GetOrElse(null), _context_1.Parsed.GetOrElse(null)), _context_1.Remainder);
                }
            }
        }
        
        public static class _resourcePath_꘡ʺx3Fʺ_queryOptions꘡Parser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡> Parse(IInput<char>? input)
                {
                    var _resourcePath_1 = __GeneratedOdataV4.Parsers.Rules._resourcePathParser.Instance.Parse(input);

var _ʺx3Fʺ_queryOptions_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3Fʺ_queryOptionsParser.Instance.Optional().Parse(_resourcePath_1.Remainder);

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡(_resourcePath_1.Parsed, _ʺx3Fʺ_queryOptions_1.Parsed.GetOrElse(null)), _ʺx3Fʺ_queryOptions_1.Remainder);
                }
            }
        }
    }
    
}
