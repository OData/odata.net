namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _keyPredicateParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._keyPredicate> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._keyPredicate>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Rules._keyPredicate Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                var segment = _keyPathSegmentsParser.Instance2.Parse(input, start, out newStart);

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._keyPredicate> Instance { get; } = _keyPathSegmentsParser.Instance;
        
        public static class _simpleKeyParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._keyPredicate._simpleKey> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._keyPredicate._simpleKey>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._keyPredicate._simpleKey> Parse(IInput<char>? input)
                {
                    var _simpleKey_1 = __GeneratedOdataV4.Parsers.Rules._simpleKeyParser.Instance.Parse(input);
if (!_simpleKey_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._keyPredicate._simpleKey)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._keyPredicate._simpleKey(_simpleKey_1.Parsed), _simpleKey_1.Remainder);
                }
            }
        }
        
        public static class _compoundKeyParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._keyPredicate._compoundKey> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._keyPredicate._compoundKey>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._keyPredicate._compoundKey> Parse(IInput<char>? input)
                {
                    var _compoundKey_1 = __GeneratedOdataV4.Parsers.Rules._compoundKeyParser.Instance.Parse(input);
if (!_compoundKey_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._keyPredicate._compoundKey)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._keyPredicate._compoundKey(_compoundKey_1.Parsed), _compoundKey_1.Remainder);
                }
            }
        }
        
        public static class _keyPathSegmentsParser
        {
            public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._keyPredicate._keyPathSegments> Instance2 { get; } = new Parser2();

            private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._keyPredicate._keyPathSegments>
            {
                [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
                public __GeneratedOdataV4.CstNodes.Rules._keyPredicate._keyPathSegments Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
                {
                    var _keyPathSegments_1 = __GeneratedOdataV4.Parsers.Rules._keyPathSegmentsParser.Instance2.Parse(input, start, out newStart);

                    return default;
                }
            }

            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._keyPredicate._keyPathSegments> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._keyPredicate._keyPathSegments>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._keyPredicate._keyPathSegments> Parse(IInput<char>? input)
                {
                    var _keyPathSegments_1 = __GeneratedOdataV4.Parsers.Rules._keyPathSegmentsParser.Instance.Parse(input);
if (!_keyPathSegments_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._keyPredicate._keyPathSegments)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._keyPredicate._keyPathSegments(_keyPathSegments_1.Parsed), _keyPathSegments_1.Remainder);
                }
            }
        }
    }
    
}
