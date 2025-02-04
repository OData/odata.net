namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x54Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x54> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x54>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x54> Parse(IInput<char>? input)
            {
                var _x54 = CombinatorParsingV2.Parse.Char((char)0x54).Parse(input);
if (!_x54.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x54)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x54.Instance, _x54.Remainder);
            }
        }
    }
    
}
