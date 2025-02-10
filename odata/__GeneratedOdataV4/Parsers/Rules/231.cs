namespace __GeneratedOdataV4.Parsers.Rules
{
    using __GeneratedOdataV4.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _entitySetNameParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._entitySetName> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._entitySetName>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public _entitySetName Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                var _odataIdentifier = __GeneratedOdataV4.Parsers.Rules._odataIdentifierParser.Instance2.Parse(input, start, out newStart);

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._entitySetName> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._entitySetName>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._entitySetName> Parse(IInput<char>? input)
            {
                var _odataIdentifier_1 = __GeneratedOdataV4.Parsers.Rules._odataIdentifierParser.Instance.Parse(input);

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._entitySetName(_odataIdentifier_1.Parsed), _odataIdentifier_1.Remainder);
            }
        }
    }
    
}
