namespace __GeneratedOdata.Parsers.Rules
{
    using __GeneratedOdata.CstNodes.Inners;
    using __GeneratedOdata.CstNodes.Rules;
    using CombinatorParsingV2;
    using System.Linq;

    public static class _odataRelativeUriParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri> Instance { get; } = (_ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri>(_ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptionsParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri>(_ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptionsParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri>(_ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡Parser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri>(_resourcePath_꘡ʺx3Fʺ_queryOptions꘡Parser.Instance);
                                                                                                           ////new Parser();
            //// PERF
            ////_resourcePath_꘡ʺx3Fʺ_queryOptions꘡Parser.Instance;

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri>
        {
            private static _odataIdentifier Users = 
                new _odataIdentifier(
                    new _identifierLeadingCharacter._ALPHA(
                        new _ALPHA._Ⰳx61ⲻ7A(
                            new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._75(
                                CstNodes.Inners._7.Instance,
                                CstNodes.Inners._5.Instance))),
                    new CstNodes.Inners.HelperRangedAtMost127<_identifierCharacter>(
                        new[]
                        {
                            new _identifierCharacter._ALPHA(
                                new _ALPHA._Ⰳx61ⲻ7A(
                                    new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._73(
                                        CstNodes.Inners._7.Instance,
                                        CstNodes.Inners._3.Instance))),
                            new _identifierCharacter._ALPHA(
                                new _ALPHA._Ⰳx61ⲻ7A(
                                    new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._65(
                                        CstNodes.Inners._6.Instance,
                                        CstNodes.Inners._5.Instance))),
                            new _identifierCharacter._ALPHA(
                                new _ALPHA._Ⰳx61ⲻ7A(
                                    new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._72(
                                        CstNodes.Inners._7.Instance,
                                        CstNodes.Inners._2.Instance))),
                            new _identifierCharacter._ALPHA(
                                new _ALPHA._Ⰳx61ⲻ7A(
                                    new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._73(
                                        CstNodes.Inners._7.Instance,
                                        CstNodes.Inners._3.Instance))),
                        }));

            private static _ʺx2Fʺ_keyPathLiteral MyId =
                new CstNodes.Inners._ʺx2Fʺ_keyPathLiteral(
                    new CstNodes.Inners._ʺx2Fʺ(
                        CstNodes.Inners._x2F.Instance),
                    new _keyPathLiteral(
                        new[]
                        {
                            new _pchar._unreserved(
                                new _unreserved._ALPHA(
                                    new _ALPHA._Ⰳx61ⲻ7A(
                                        new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._6D(
                                            CstNodes.Inners._6.Instance,
                                            CstNodes.Inners._D.Instance)))),
                            new _pchar._unreserved(
                                new _unreserved._ALPHA(
                                    new _ALPHA._Ⰳx61ⲻ7A(
                                        new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._79(
                                            CstNodes.Inners._7.Instance,
                                            CstNodes.Inners._9.Instance)))),
                            new _pchar._unreserved(
                                new _unreserved._ALPHA(
                                    new _ALPHA._Ⰳx61ⲻ7A(
                                        new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._69(
                                            CstNodes.Inners._6.Instance,
                                            CstNodes.Inners._9.Instance)))),
                            new _pchar._unreserved(
                                new _unreserved._ALPHA(
                                    new _ALPHA._Ⰳx61ⲻ7A(
                                        new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._64(
                                            CstNodes.Inners._6.Instance,
                                            CstNodes.Inners._4.Instance)))),
                        }));

            private static _odataIdentifier Calendar =
                new _odataIdentifier(
                    new _identifierLeadingCharacter._ALPHA(
                        new _ALPHA._Ⰳx61ⲻ7A(
                            new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._63(
                                CstNodes.Inners._6.Instance,
                                CstNodes.Inners._3.Instance))),
                    new CstNodes.Inners.HelperRangedAtMost127<_identifierCharacter>(
                        new[]
                        {
                            new _identifierCharacter._ALPHA(
                                new _ALPHA._Ⰳx61ⲻ7A(
                                    new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._61(
                                        CstNodes.Inners._6.Instance,
                                        CstNodes.Inners._1.Instance))),
                            new _identifierCharacter._ALPHA(
                                new _ALPHA._Ⰳx61ⲻ7A(
                                    new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._6C(
                                        CstNodes.Inners._6.Instance,
                                        CstNodes.Inners._C.Instance))),
                            new _identifierCharacter._ALPHA(
                                new _ALPHA._Ⰳx61ⲻ7A(
                                    new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._65(
                                        CstNodes.Inners._6.Instance,
                                        CstNodes.Inners._5.Instance))),
                            new _identifierCharacter._ALPHA(
                                new _ALPHA._Ⰳx61ⲻ7A(
                                    new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._6E(
                                        CstNodes.Inners._6.Instance,
                                        CstNodes.Inners._E.Instance))),
                            new _identifierCharacter._ALPHA(
                                new _ALPHA._Ⰳx61ⲻ7A(
                                    new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._64(
                                        CstNodes.Inners._6.Instance,
                                        CstNodes.Inners._4.Instance))),
                            new _identifierCharacter._ALPHA(
                                new _ALPHA._Ⰳx61ⲻ7A(
                                    new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._61(
                                        CstNodes.Inners._6.Instance,
                                        CstNodes.Inners._1.Instance))),
                            new _identifierCharacter._ALPHA(
                                new _ALPHA._Ⰳx61ⲻ7A(
                                    new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._72(
                                        CstNodes.Inners._7.Instance,
                                        CstNodes.Inners._2.Instance))),
                        }));

            private static _odataIdentifier Events =
                new _odataIdentifier(
                    new _identifierLeadingCharacter._ALPHA(
                        new _ALPHA._Ⰳx61ⲻ7A(
                            new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._65(
                                CstNodes.Inners._6.Instance,
                                CstNodes.Inners._5.Instance))),
                    new CstNodes.Inners.HelperRangedAtMost127<_identifierCharacter>(
                        new[]
                        {
                            new _identifierCharacter._ALPHA(
                                new _ALPHA._Ⰳx61ⲻ7A(
                                    new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._76(
                                        CstNodes.Inners._7.Instance,
                                        CstNodes.Inners._6.Instance))),
                            new _identifierCharacter._ALPHA(
                                new _ALPHA._Ⰳx61ⲻ7A(
                                    new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._65(
                                        CstNodes.Inners._6.Instance,
                                        CstNodes.Inners._5.Instance))),
                            new _identifierCharacter._ALPHA(
                                new _ALPHA._Ⰳx61ⲻ7A(
                                    new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._6E(
                                        CstNodes.Inners._6.Instance,
                                        CstNodes.Inners._E.Instance))),
                            new _identifierCharacter._ALPHA(
                                new _ALPHA._Ⰳx61ⲻ7A(
                                    new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._74(
                                        CstNodes.Inners._7.Instance,
                                        CstNodes.Inners._4.Instance))),
                            new _identifierCharacter._ALPHA(
                                new _ALPHA._Ⰳx61ⲻ7A(
                                    new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._73(
                                        CstNodes.Inners._7.Instance,
                                        CstNodes.Inners._3.Instance))),
                        }));

            private static CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺ Filter =
                new CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺ(
                    CstNodes.Inners._x24.Instance,
                    CstNodes.Inners._x66.Instance,
                    CstNodes.Inners._x69.Instance,
                    CstNodes.Inners._x6C.Instance,
                    CstNodes.Inners._x74.Instance,
                    CstNodes.Inners._x65.Instance,
                    CstNodes.Inners._x72.Instance);

            private static _odataIdentifier Id =
                new _odataIdentifier(
                    new _identifierLeadingCharacter._ALPHA(
                        new _ALPHA._Ⰳx61ⲻ7A(
                            new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._69(
                                CstNodes.Inners._6.Instance,
                                CstNodes.Inners._9.Instance))),
                    new CstNodes.Inners.HelperRangedAtMost127<_identifierCharacter>(
                        new[]
                        {
                            new _identifierCharacter._ALPHA(
                                new _ALPHA._Ⰳx61ⲻ7A(
                                    new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._64(
                                        CstNodes.Inners._6.Instance,
                                        CstNodes.Inners._4.Instance))),
                        }));

            private static System.Collections.Generic.IEnumerable<__GeneratedOdata.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ> ThisIsATest =
                new[]
                {
                    new __GeneratedOdata.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdata.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new _unreserved._ALPHA(
                                    new _ALPHA._Ⰳx61ⲻ7A(
                                        new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._74(
                                            _7.Instance,
                                            _4.Instance)))))),
                    new __GeneratedOdata.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdata.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new _unreserved._ALPHA(
                                    new _ALPHA._Ⰳx61ⲻ7A(
                                        new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._68(
                                            _6.Instance,
                                            _8.Instance)))))),
                    new __GeneratedOdata.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdata.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new _unreserved._ALPHA(
                                    new _ALPHA._Ⰳx61ⲻ7A(
                                        new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._69(
                                            _6.Instance,
                                            _9.Instance)))))),
                    new __GeneratedOdata.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdata.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new _unreserved._ALPHA(
                                    new _ALPHA._Ⰳx61ⲻ7A(
                                        new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._73(
                                            _7.Instance,
                                            _3.Instance)))))),
                    new __GeneratedOdata.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdata.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new _unreserved._ALPHA(
                                    new _ALPHA._Ⰳx61ⲻ7A(
                                        new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._69(
                                            _6.Instance,
                                            _9.Instance)))))),
                    new __GeneratedOdata.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdata.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new _unreserved._ALPHA(
                                    new _ALPHA._Ⰳx61ⲻ7A(
                                        new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._73(
                                            _7.Instance,
                                            _3.Instance)))))),
                    new __GeneratedOdata.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdata.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new _unreserved._ALPHA(
                                    new _ALPHA._Ⰳx61ⲻ7A(
                                        new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._61(
                                            _6.Instance,
                                            _1.Instance)))))),
                    new __GeneratedOdata.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdata.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new _unreserved._ALPHA(
                                    new _ALPHA._Ⰳx61ⲻ7A(
                                        new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._74(
                                            _7.Instance,
                                            _4.Instance)))))),
                    new __GeneratedOdata.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdata.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new _unreserved._ALPHA(
                                    new _ALPHA._Ⰳx61ⲻ7A(
                                        new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._65(
                                            _6.Instance,
                                            _5.Instance)))))),
                    new __GeneratedOdata.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdata.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new _unreserved._ALPHA(
                                    new _ALPHA._Ⰳx61ⲻ7A(
                                        new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._73(
                                            _7.Instance,
                                            _3.Instance)))))),
                    new __GeneratedOdata.CstNodes.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ(
                        new __GeneratedOdata.CstNodes.Inners._SQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTE._pcharⲻnoⲻSQUOTE(
                            new __GeneratedOdata.CstNodes.Rules._pcharⲻnoⲻSQUOTE._unreserved(
                                new _unreserved._ALPHA(
                                    new _ALPHA._Ⰳx61ⲻ7A(
                                        new __GeneratedOdata.CstNodes.Inners._Ⰳx61ⲻ7A._74(
                                            _7.Instance,
                                            _4.Instance)))))),
                };

            public IOutput<char, _odataRelativeUri> Parse(IInput<char>? input)
            {
                var node =
                    new _odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡(
                        new _resourcePath._entitySetName_꘡collectionNavigation꘡(
                            new _entitySetName(
                                Users),
                            new _collectionNavigation(
                                null,
                                new _collectionNavPath._keyPredicate_꘡singleNavigation꘡(
                                    new _keyPredicate._keyPathSegments(
                                        new _keyPathSegments(
                                            new CstNodes.Inners.HelperRangedAtLeast1<CstNodes.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃ>(
                                                new[]
                                                {
                                                    new CstNodes.Inners._Ⲥʺx2Fʺ_keyPathLiteralↃ(
                                                        MyId)
                                                }))),
                                    new _singleNavigation(
                                        null,
                                        new __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._ʺx2Fʺ_propertyPath(
                                            new CstNodes.Inners._ʺx2Fʺ(
                                                CstNodes.Inners._x2F.Instance),
                                            new _propertyPath._entityNavigationProperty_꘡singleNavigation꘡(
                                                new _entityNavigationProperty(
                                                    Calendar),
                                                new _singleNavigation(
                                                    null,
                                                    new __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_propertyPathⳆboundOperationⳆrefⳆvalue._ʺx2Fʺ_propertyPath(
                                                        new CstNodes.Inners._ʺx2Fʺ(
                                                            CstNodes.Inners._x2F.Instance),
                                                        new _propertyPath._entityColNavigationProperty_꘡collectionNavigation꘡(
                                                            new _entityColNavigationProperty(
                                                                Events),
                                                            null))))))))),
                        new CstNodes.Inners._ʺx3Fʺ_queryOptions(
                            new CstNodes.Inners._ʺx3Fʺ(
                                CstNodes.Inners._x3F.Instance),
                            new _queryOptions(
                                new _queryOption._systemQueryOption(
                                    new _systemQueryOption._filter(
                                        new _filter(
                                            new CstNodes.Inners._Ⲥʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺↃ(
                                                new __GeneratedOdata.CstNodes.Inners._ʺx24x66x69x6Cx74x65x72ʺⳆʺx66x69x6Cx74x65x72ʺ._ʺx24x66x69x6Cx74x65x72ʺ(
                                                    Filter)),
                                            new _EQ(
                                                new CstNodes.Inners._ʺx3Dʺ(
                                                    CstNodes.Inners._x3D.Instance)),
                                            new _boolCommonExpr(
                                                new _commonExpr(
                                                    new CstNodes.Inners._ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ(
                                                        new __GeneratedOdata.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._firstMemberExpr(
                                                            new _firstMemberExpr._memberExpr(
                                                                new _memberExpr(
                                                                    null,
                                                                    new CstNodes.Inners._ⲤpropertyPathExprⳆboundFunctionExprⳆannotationExprↃ(
                                                                        new __GeneratedOdata.CstNodes.Inners._propertyPathExprⳆboundFunctionExprⳆannotationExpr._propertyPathExpr(
                                                                            new _propertyPathExpr(
                                                                                new CstNodes.Inners._ⲤentityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡Ↄ(
                                                                                    new _entityColNavigationProperty_꘡collectionNavigationExpr꘡ⳆentityNavigationProperty_꘡singleNavigationExpr꘡ⳆcomplexColProperty_꘡complexColPathExpr꘡ⳆcomplexProperty_꘡complexPathExpr꘡ⳆprimitiveColProperty_꘡collectionPathExpr꘡ⳆprimitiveProperty_꘡primitivePathExpr꘡ⳆstreamProperty_꘡primitivePathExpr꘡._primitiveProperty_꘡primitivePathExpr꘡(
                                                                                        new _primitiveProperty._primitiveKeyProperty(
                                                                                            new _primitiveKeyProperty(
                                                                                                Id)),
                                                                                        null))))))))),
                                                    null,
                                                    new __GeneratedOdata.CstNodes.Inners._eqExprⳆneExprⳆltExprⳆleExprⳆgtExprⳆgeExprⳆhasExprⳆinExpr._eqExpr(
                                                        new _eqExpr(
                                                            new _RWS(
                                                                new HelperRangedAtLeast1<_ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ>(
                                                                    new[]
                                                                    {
                                                                        new _ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ(
                                                                            new __GeneratedOdata.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._SP(
                                                                                new _SP(
                                                                                    new _Ⰳx20(
                                                                                        _2.Instance,
                                                                                        _0.Instance)))),
                                                                    })),
                                                            new _ʺx65x71ʺ(
                                                                _x65.Instance,
                                                                _x71.Instance),
                                                            new _RWS(
                                                                new HelperRangedAtLeast1<_ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ>(
                                                                    new[]
                                                                    {
                                                                        new _ⲤSPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺↃ(
                                                                            new __GeneratedOdata.CstNodes.Inners._SPⳆHTABⳆʺx25x32x30ʺⳆʺx25x30x39ʺ._SP(
                                                                                new _SP(
                                                                                    new _Ⰳx20(
                                                                                        _2.Instance,
                                                                                        _0.Instance)))),
                                                                    })),
                                                            new _commonExpr(
                                                                new _ⲤprimitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExprↃ(
                                                                    new __GeneratedOdata.CstNodes.Inners._primitiveLiteralⳆarrayOrObjectⳆrootExprⳆfirstMemberExprⳆfunctionExprⳆnegateExprⳆmethodCallExprⳆparenExprⳆlistExprⳆcastExprⳆisofExprⳆnotExpr._primitiveLiteral(
                                                                        new _primitiveLiteral._string(
                                                                            new _string(
                                                                                new _SQUOTE._ʺx27ʺ(
                                                                                    new _ʺx27ʺ(
                                                                                        _x27.Instance)),
                                                                                ThisIsATest,
                                                                                new _SQUOTE._ʺx27ʺ(
                                                                                    new _ʺx27ʺ(
                                                                                        _x27.Instance)))))),
                                                                null,
                                                                null,
                                                                null))),
                                                    null))))),
                                Enumerable.Empty<__GeneratedOdata.CstNodes.Inners._Ⲥʺx26ʺ_queryOptionↃ>())));
                return Output.Create(
                    true,
                    node,
                    (IInput<char>?)null);
            }
        }

        public static class _ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡> Instance { get; } = from _ʺx24x62x61x74x63x68ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx24x62x61x74x63x68ʺParser.Instance
from _ʺx3Fʺ_batchOptions_1 in __GeneratedOdata.Parsers.Inners._ʺx3Fʺ_batchOptionsParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡(_ʺx24x62x61x74x63x68ʺ_1, _ʺx3Fʺ_batchOptions_1.GetOrElse(null));
        }
        
        public static class _ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptionsParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions> Instance { get; } = from _ʺx24x65x6Ex74x69x74x79ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx24x65x6Ex74x69x74x79ʺParser.Instance
from _ʺx3Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3FʺParser.Instance
from _entityOptions_1 in __GeneratedOdata.Parsers.Rules._entityOptionsParser.Instance
select new __GeneratedOdata.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions(_ʺx24x65x6Ex74x69x74x79ʺ_1, _ʺx3Fʺ_1, _entityOptions_1);
        }
        
        public static class _ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptionsParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions> Instance { get; } = from _ʺx24x65x6Ex74x69x74x79ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx24x65x6Ex74x69x74x79ʺParser.Instance
from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _qualifiedEntityTypeName_1 in __GeneratedOdata.Parsers.Rules._qualifiedEntityTypeNameParser.Instance
from _ʺx3Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx3FʺParser.Instance
from _entityCastOptions_1 in __GeneratedOdata.Parsers.Rules._entityCastOptionsParser.Instance
select new __GeneratedOdata.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions(_ʺx24x65x6Ex74x69x74x79ʺ_1, _ʺx2Fʺ_1, _qualifiedEntityTypeName_1, _ʺx3Fʺ_1, _entityCastOptions_1);
        }
        
        public static class _ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri._ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡> Instance { get; } = from _ʺx24x6Dx65x74x61x64x61x74x61ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx24x6Dx65x74x61x64x61x74x61ʺParser.Instance
from _ʺx3Fʺ_metadataOptions_1 in __GeneratedOdata.Parsers.Inners._ʺx3Fʺ_metadataOptionsParser.Instance.Optional()
from _context_1 in __GeneratedOdata.Parsers.Rules._contextParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._odataRelativeUri._ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡(_ʺx24x6Dx65x74x61x64x61x74x61ʺ_1, _ʺx3Fʺ_metadataOptions_1.GetOrElse(null), _context_1.GetOrElse(null));
        }
        
        public static class _resourcePath_꘡ʺx3Fʺ_queryOptions꘡Parser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡> Instance { get; } = from _resourcePath_1 in __GeneratedOdata.Parsers.Rules._resourcePathParser.Instance
from _ʺx3Fʺ_queryOptions_1 in __GeneratedOdata.Parsers.Inners._ʺx3Fʺ_queryOptionsParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡(_resourcePath_1, _ʺx3Fʺ_queryOptions_1.GetOrElse(null));

            //// PERF
            /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡> Instance { get; } =
                new Parser();*/

            private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡>
            {
                public IOutput<char, _odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡> Parse(IInput<char>? input)
                {
                    var _resourcePath_1 = __GeneratedOdata.Parsers.Rules._resourcePathParser.Instance.Parse(input);
                    var _ʺx3Fʺ_queryOptions_1 = __GeneratedOdata.Parsers.Inners._ʺx3Fʺ_queryOptionsParser.Instance.Optional().Parse(_resourcePath_1.Remainder);
                    return Output.Create(
                        true, 
                        new __GeneratedOdata.CstNodes.Rules._odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡(_resourcePath_1.Parsed, _ʺx3Fʺ_queryOptions_1.Parsed.GetOrElse(null)), 
                        _ʺx3Fʺ_queryOptions_1.Remainder);
                }
            }
        }
    }
    
}
