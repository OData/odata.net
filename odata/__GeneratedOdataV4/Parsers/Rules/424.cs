namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _DQUOTEParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._DQUOTE> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._DQUOTE>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._DQUOTE> Parse(IInput<char>? input)
            {
                var _Ⰳx22_1 = __GeneratedOdataV4.Parsers.Inners._Ⰳx22Parser.Instance.Parse(input);
if (!_Ⰳx22_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._DQUOTE)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._DQUOTE.Instance, _Ⰳx22_1.Remainder);
            }
        }
    }
    
}
