namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _keyPathLiteralParser
    {
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
