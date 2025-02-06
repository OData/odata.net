namespace __GeneratedOdataV4.Parsers.Rules
{
    using __GeneratedOdataV4.CstNodes.Rules;
    using CombinatorParsingV2;

    public static class _resourcePathParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._resourcePath, CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._resourcePath>> Instance2 { get; }
        = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._resourcePath, CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._resourcePath>>
        {
            public CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, _resourcePath> Parse(in CombinatorParsingV3.StringInput input)
            {
                //// TODO OR
                var output = _entitySetName_꘡collectionNavigation꘡Parser.Instance2.Parse(input);
                /*if (!output.Success)
                {
                    return new CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, _resourcePath>(
                        false,
                        default,
                        true,
                        input);
                }*/

                return new CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, _resourcePath>(
                    true,
                    output.Parsed,
                    output.HasRemainder,
                    output.Remainder);
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath> Instance { get; } = (_entitySetName_꘡collectionNavigation꘡Parser.Instance);

        public static class _entitySetName_꘡collectionNavigation꘡Parser
        {
            public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._resourcePath._entitySetName_꘡collectionNavigation꘡, CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._resourcePath._entitySetName_꘡collectionNavigation꘡>> Instance2 { get; } = new Parser2();

            private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._resourcePath._entitySetName_꘡collectionNavigation꘡, CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, __GeneratedOdataV4.CstNodes.Rules._resourcePath._entitySetName_꘡collectionNavigation꘡>>
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

                private static __GeneratedOdataV4.CstNodes.Rules._resourcePath._entitySetName_꘡collectionNavigation꘡ Node = 
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
                                                        null)))))))));

                public CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, _resourcePath._entitySetName_꘡collectionNavigation꘡> Parse(in CombinatorParsingV3.StringInput input)
                {
                    CombinatorParsingV3.StringInput remainder = input.Next(out var more);

                    for (int i = 0; i < 25; ++i)
                    {
                        remainder = remainder.Next(out more);
                    }

                    return new CombinatorParsingV3.Output<char, CombinatorParsingV3.StringInput, _resourcePath._entitySetName_꘡collectionNavigation꘡>(
                        true,
                        Node,
                        more,
                        input);
                }
            }

            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._entitySetName_꘡collectionNavigation꘡> Instance { get; } = new Parser();

            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._entitySetName_꘡collectionNavigation꘡>
            {
                public Parser()
                {
                }

                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._entitySetName_꘡collectionNavigation꘡> Parse(IInput<char>? input)
                {
                    var _entitySetName_1 = __GeneratedOdataV4.Parsers.Rules._entitySetNameParser.Instance.Parse(input);

                    var _collectionNavigation_1 = __GeneratedOdataV4.Parsers.Rules._collectionNavigationParser.Instance.Optional().Parse(_entitySetName_1.Remainder);

                    return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._resourcePath._entitySetName_꘡collectionNavigation꘡(_entitySetName_1.Parsed, _collectionNavigation_1.Parsed.GetOrElse(null)), _collectionNavigation_1.Remainder);
                }
            }
        }

        public static class _singletonEntity_꘡singleNavigation꘡Parser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._singletonEntity_꘡singleNavigation꘡> Instance { get; } = new Parser();

            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._singletonEntity_꘡singleNavigation꘡>
            {
                public Parser()
                {
                }

                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._singletonEntity_꘡singleNavigation꘡> Parse(IInput<char>? input)
                {
                    var _singletonEntity_1 = __GeneratedOdataV4.Parsers.Rules._singletonEntityParser.Instance.Parse(input);
                    if (!_singletonEntity_1.Success)
                    {
                        return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._resourcePath._singletonEntity_꘡singleNavigation꘡)!, input);
                    }

                    var _singleNavigation_1 = __GeneratedOdataV4.Parsers.Rules._singleNavigationParser.Instance.Optional().Parse(_singletonEntity_1.Remainder);
                    if (!_singleNavigation_1.Success)
                    {
                        return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._resourcePath._singletonEntity_꘡singleNavigation꘡)!, input);
                    }

                    return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._resourcePath._singletonEntity_꘡singleNavigation꘡(_singletonEntity_1.Parsed, _singleNavigation_1.Parsed.GetOrElse(null)), _singleNavigation_1.Remainder);
                }
            }
        }

        public static class _actionImportCallParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._actionImportCall> Instance { get; } = new Parser();

            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._actionImportCall>
            {
                public Parser()
                {
                }

                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._actionImportCall> Parse(IInput<char>? input)
                {
                    var _actionImportCall_1 = __GeneratedOdataV4.Parsers.Rules._actionImportCallParser.Instance.Parse(input);
                    if (!_actionImportCall_1.Success)
                    {
                        return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._resourcePath._actionImportCall)!, input);
                    }

                    return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._resourcePath._actionImportCall(_actionImportCall_1.Parsed), _actionImportCall_1.Remainder);
                }
            }
        }

        public static class _entityColFunctionImportCall_꘡collectionNavigation꘡Parser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._entityColFunctionImportCall_꘡collectionNavigation꘡> Instance { get; } = new Parser();

            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._entityColFunctionImportCall_꘡collectionNavigation꘡>
            {
                public Parser()
                {
                }

                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._entityColFunctionImportCall_꘡collectionNavigation꘡> Parse(IInput<char>? input)
                {
                    var _entityColFunctionImportCall_1 = __GeneratedOdataV4.Parsers.Rules._entityColFunctionImportCallParser.Instance.Parse(input);
                    if (!_entityColFunctionImportCall_1.Success)
                    {
                        return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._resourcePath._entityColFunctionImportCall_꘡collectionNavigation꘡)!, input);
                    }

                    var _collectionNavigation_1 = __GeneratedOdataV4.Parsers.Rules._collectionNavigationParser.Instance.Optional().Parse(_entityColFunctionImportCall_1.Remainder);
                    if (!_collectionNavigation_1.Success)
                    {
                        return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._resourcePath._entityColFunctionImportCall_꘡collectionNavigation꘡)!, input);
                    }

                    return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._resourcePath._entityColFunctionImportCall_꘡collectionNavigation꘡(_entityColFunctionImportCall_1.Parsed, _collectionNavigation_1.Parsed.GetOrElse(null)), _collectionNavigation_1.Remainder);
                }
            }
        }

        public static class _entityFunctionImportCall_꘡singleNavigation꘡Parser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._entityFunctionImportCall_꘡singleNavigation꘡> Instance { get; } = new Parser();

            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._entityFunctionImportCall_꘡singleNavigation꘡>
            {
                public Parser()
                {
                }

                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._entityFunctionImportCall_꘡singleNavigation꘡> Parse(IInput<char>? input)
                {
                    var _entityFunctionImportCall_1 = __GeneratedOdataV4.Parsers.Rules._entityFunctionImportCallParser.Instance.Parse(input);
                    if (!_entityFunctionImportCall_1.Success)
                    {
                        return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._resourcePath._entityFunctionImportCall_꘡singleNavigation꘡)!, input);
                    }

                    var _singleNavigation_1 = __GeneratedOdataV4.Parsers.Rules._singleNavigationParser.Instance.Optional().Parse(_entityFunctionImportCall_1.Remainder);
                    if (!_singleNavigation_1.Success)
                    {
                        return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._resourcePath._entityFunctionImportCall_꘡singleNavigation꘡)!, input);
                    }

                    return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._resourcePath._entityFunctionImportCall_꘡singleNavigation꘡(_entityFunctionImportCall_1.Parsed, _singleNavigation_1.Parsed.GetOrElse(null)), _singleNavigation_1.Remainder);
                }
            }
        }

        public static class _complexColFunctionImportCall_꘡complexColPath꘡Parser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._complexColFunctionImportCall_꘡complexColPath꘡> Instance { get; } = new Parser();

            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._complexColFunctionImportCall_꘡complexColPath꘡>
            {
                public Parser()
                {
                }

                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._complexColFunctionImportCall_꘡complexColPath꘡> Parse(IInput<char>? input)
                {
                    var _complexColFunctionImportCall_1 = __GeneratedOdataV4.Parsers.Rules._complexColFunctionImportCallParser.Instance.Parse(input);
                    if (!_complexColFunctionImportCall_1.Success)
                    {
                        return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._resourcePath._complexColFunctionImportCall_꘡complexColPath꘡)!, input);
                    }

                    var _complexColPath_1 = __GeneratedOdataV4.Parsers.Rules._complexColPathParser.Instance.Optional().Parse(_complexColFunctionImportCall_1.Remainder);
                    if (!_complexColPath_1.Success)
                    {
                        return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._resourcePath._complexColFunctionImportCall_꘡complexColPath꘡)!, input);
                    }

                    return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._resourcePath._complexColFunctionImportCall_꘡complexColPath꘡(_complexColFunctionImportCall_1.Parsed, _complexColPath_1.Parsed.GetOrElse(null)), _complexColPath_1.Remainder);
                }
            }
        }

        public static class _complexFunctionImportCall_꘡complexPath꘡Parser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._complexFunctionImportCall_꘡complexPath꘡> Instance { get; } = new Parser();

            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._complexFunctionImportCall_꘡complexPath꘡>
            {
                public Parser()
                {
                }

                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._complexFunctionImportCall_꘡complexPath꘡> Parse(IInput<char>? input)
                {
                    var _complexFunctionImportCall_1 = __GeneratedOdataV4.Parsers.Rules._complexFunctionImportCallParser.Instance.Parse(input);
                    if (!_complexFunctionImportCall_1.Success)
                    {
                        return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._resourcePath._complexFunctionImportCall_꘡complexPath꘡)!, input);
                    }

                    var _complexPath_1 = __GeneratedOdataV4.Parsers.Rules._complexPathParser.Instance.Optional().Parse(_complexFunctionImportCall_1.Remainder);
                    if (!_complexPath_1.Success)
                    {
                        return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._resourcePath._complexFunctionImportCall_꘡complexPath꘡)!, input);
                    }

                    return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._resourcePath._complexFunctionImportCall_꘡complexPath꘡(_complexFunctionImportCall_1.Parsed, _complexPath_1.Parsed.GetOrElse(null)), _complexPath_1.Remainder);
                }
            }
        }

        public static class _primitiveColFunctionImportCall_꘡primitiveColPath꘡Parser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._primitiveColFunctionImportCall_꘡primitiveColPath꘡> Instance { get; } = new Parser();

            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._primitiveColFunctionImportCall_꘡primitiveColPath꘡>
            {
                public Parser()
                {
                }

                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._primitiveColFunctionImportCall_꘡primitiveColPath꘡> Parse(IInput<char>? input)
                {
                    var _primitiveColFunctionImportCall_1 = __GeneratedOdataV4.Parsers.Rules._primitiveColFunctionImportCallParser.Instance.Parse(input);
                    if (!_primitiveColFunctionImportCall_1.Success)
                    {
                        return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._resourcePath._primitiveColFunctionImportCall_꘡primitiveColPath꘡)!, input);
                    }

                    var _primitiveColPath_1 = __GeneratedOdataV4.Parsers.Rules._primitiveColPathParser.Instance.Optional().Parse(_primitiveColFunctionImportCall_1.Remainder);
                    if (!_primitiveColPath_1.Success)
                    {
                        return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._resourcePath._primitiveColFunctionImportCall_꘡primitiveColPath꘡)!, input);
                    }

                    return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._resourcePath._primitiveColFunctionImportCall_꘡primitiveColPath꘡(_primitiveColFunctionImportCall_1.Parsed, _primitiveColPath_1.Parsed.GetOrElse(null)), _primitiveColPath_1.Remainder);
                }
            }
        }

        public static class _primitiveFunctionImportCall_꘡primitivePath꘡Parser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._primitiveFunctionImportCall_꘡primitivePath꘡> Instance { get; } = new Parser();

            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._primitiveFunctionImportCall_꘡primitivePath꘡>
            {
                public Parser()
                {
                }

                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._primitiveFunctionImportCall_꘡primitivePath꘡> Parse(IInput<char>? input)
                {
                    var _primitiveFunctionImportCall_1 = __GeneratedOdataV4.Parsers.Rules._primitiveFunctionImportCallParser.Instance.Parse(input);
                    if (!_primitiveFunctionImportCall_1.Success)
                    {
                        return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._resourcePath._primitiveFunctionImportCall_꘡primitivePath꘡)!, input);
                    }

                    var _primitivePath_1 = __GeneratedOdataV4.Parsers.Rules._primitivePathParser.Instance.Optional().Parse(_primitiveFunctionImportCall_1.Remainder);
                    if (!_primitivePath_1.Success)
                    {
                        return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._resourcePath._primitiveFunctionImportCall_꘡primitivePath꘡)!, input);
                    }

                    return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._resourcePath._primitiveFunctionImportCall_꘡primitivePath꘡(_primitiveFunctionImportCall_1.Parsed, _primitivePath_1.Parsed.GetOrElse(null)), _primitivePath_1.Remainder);
                }
            }
        }

        public static class _functionImportCallNoParensParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._functionImportCallNoParens> Instance { get; } = new Parser();

            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._functionImportCallNoParens>
            {
                public Parser()
                {
                }

                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._functionImportCallNoParens> Parse(IInput<char>? input)
                {
                    var _functionImportCallNoParens_1 = __GeneratedOdataV4.Parsers.Rules._functionImportCallNoParensParser.Instance.Parse(input);
                    if (!_functionImportCallNoParens_1.Success)
                    {
                        return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._resourcePath._functionImportCallNoParens)!, input);
                    }

                    return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._resourcePath._functionImportCallNoParens(_functionImportCallNoParens_1.Parsed), _functionImportCallNoParens_1.Remainder);
                }
            }
        }

        public static class _crossjoinParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._crossjoin> Instance { get; } = new Parser();

            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._crossjoin>
            {
                public Parser()
                {
                }

                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._crossjoin> Parse(IInput<char>? input)
                {
                    var _crossjoin_1 = __GeneratedOdataV4.Parsers.Rules._crossjoinParser.Instance.Parse(input);
                    if (!_crossjoin_1.Success)
                    {
                        return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._resourcePath._crossjoin)!, input);
                    }

                    return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._resourcePath._crossjoin(_crossjoin_1.Parsed), _crossjoin_1.Remainder);
                }
            }
        }

        public static class _ʺx24x61x6Cx6Cʺ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Parser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._ʺx24x61x6Cx6Cʺ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡> Instance { get; } = new Parser();

            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._ʺx24x61x6Cx6Cʺ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡>
            {
                public Parser()
                {
                }

                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._resourcePath._ʺx24x61x6Cx6Cʺ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡> Parse(IInput<char>? input)
                {
                    var _ʺx24x61x6Cx6Cʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx24x61x6Cx6CʺParser.Instance.Parse(input);
                    if (!_ʺx24x61x6Cx6Cʺ_1.Success)
                    {
                        return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._resourcePath._ʺx24x61x6Cx6Cʺ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡)!, input);
                    }

                    var _ʺx2Fʺ_qualifiedEntityTypeName_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional().Parse(_ʺx24x61x6Cx6Cʺ_1.Remainder);
                    if (!_ʺx2Fʺ_qualifiedEntityTypeName_1.Success)
                    {
                        return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._resourcePath._ʺx24x61x6Cx6Cʺ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡)!, input);
                    }

                    return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._resourcePath._ʺx24x61x6Cx6Cʺ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡(_ʺx24x61x6Cx6Cʺ_1.Parsed, _ʺx2Fʺ_qualifiedEntityTypeName_1.Parsed.GetOrElse(null)), _ʺx2Fʺ_qualifiedEntityTypeName_1.Remainder);
                }
            }
        }
    }

}
