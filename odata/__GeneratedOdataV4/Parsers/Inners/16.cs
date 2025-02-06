namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x24Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x24> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x24>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x24> Parse(IInput<char>? input)
            {
                var _x24 = CombinatorParsingV2.Parse.Char((char)0x24).Parse(input);
if (!_x24.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x24)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x24.Instance, _x24.Remainder);
            }
        }
    }
    
}
