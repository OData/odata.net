namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x34Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x34> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x34>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x34> Parse(IInput<char>? input)
            {
                var _x34 = CombinatorParsingV2.Parse.Char((char)0x34).Parse(input);
if (!_x34.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x34)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x34.Instance, _x34.Remainder);
            }
        }
    }
    
}
