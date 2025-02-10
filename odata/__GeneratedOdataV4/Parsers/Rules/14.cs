namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _keyPathLiteralParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._keyPathLiteral> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._keyPathLiteral>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Rules._keyPathLiteral Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                newStart = start;
                for (; newStart < start + 4; ++newStart)
                {
                    var next = input[newStart];
                }

                return default;
            }
        }

        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._keyPathLiteral> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._keyPathLiteral>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._keyPathLiteral> Parse(IInput<char>? input)
            {
                var _pchar_1 = __GeneratedOdataV4.Parsers.Rules._pcharParser.Instance.Many().Parse(input);
if (!_pchar_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._keyPathLiteral)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._keyPathLiteral(_pchar_1.Parsed), _pchar_1.Remainder);
            }
        }
    }
    
}
