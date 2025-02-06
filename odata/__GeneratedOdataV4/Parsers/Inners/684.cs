namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⰳx20Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⰳx20> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⰳx20>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._Ⰳx20> Parse(IInput<char>? input)
            {
                var _Ⰳx20 = CombinatorParsingV2.Parse.Char((char)0x20).Parse(input);
if (!_Ⰳx20.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._Ⰳx20)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._Ⰳx20.Instance, _Ⰳx20.Remainder);
            }
        }
    }
    
}
