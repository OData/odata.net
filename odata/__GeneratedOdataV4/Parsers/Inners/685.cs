namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⰳx09Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⰳx09> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._Ⰳx09>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._Ⰳx09> Parse(IInput<char>? input)
            {
                var _Ⰳx09 = CombinatorParsingV2.Parse.Char((char)0x09).Parse(input);
if (!_Ⰳx09.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._Ⰳx09)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._Ⰳx09.Instance, _Ⰳx09.Remainder);
            }
        }
    }
    
}
