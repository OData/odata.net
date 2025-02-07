namespace __GeneratedOdataV4.Parsers.Rules
{
    using __GeneratedOdataV4.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _collectionNavigationParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._collectionNavigation, CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._collectionNavigation>> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._collectionNavigation, CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._collectionNavigation>>
        {
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

            private static __GeneratedOdataV4.CstNodes.Rules._collectionNavigation Node =
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
                                                null))))))));

            public CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, _collectionNavigation> Parse(in CombinatorParsingV3.StringInput input)
            {
                CombinatorParsingV3.StringInput remainder = input.Next(out var more);

                for (int i = 0; i < 20; ++i)
                {
                    remainder = remainder.Next(out more);
                }

                return new CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, _collectionNavigation>(
                    true,
                    Node,
                    more,
                    remainder);
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._collectionNavigation> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._collectionNavigation>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._collectionNavigation> Parse(IInput<char>? input)
            {

var _collectionNavPath_1 = __GeneratedOdataV4.Parsers.Rules._collectionNavPathParser.Instance.Optional().Parse(input);

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._collectionNavigation(null, _collectionNavPath_1.Parsed.GetOrElse(null)), _collectionNavPath_1.Remainder);
            }
        }
    }
    
}
