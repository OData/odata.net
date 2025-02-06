namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _SPParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._SP> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._SP>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._SP> Parse(IInput<char>? input)
            {
                var _Ⰳx20_1 = __GeneratedOdataV4.Parsers.Inners._Ⰳx20Parser.Instance.Parse(input);
if (!_Ⰳx20_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._SP)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._SP.Instance, _Ⰳx20_1.Remainder);
            }
        }
    }
    
}
