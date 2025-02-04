namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⰳx22Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx22> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx22>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⰳx22> Parse(IInput<char>? input)
            {
                var _Ⰳx22 = CombinatorParsingV2.Parse.Char((char)0x22).Parse(input);
if (!_Ⰳx22.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⰳx22)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._Ⰳx22.Instance, _Ⰳx22.Remainder);
            }
        }
    }
    
}
