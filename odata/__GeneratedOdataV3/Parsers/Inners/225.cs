namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x23Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x23> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x23>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x23> Parse(IInput<char>? input)
            {
                var _x23 = CombinatorParsingV2.Parse.Char((char)0x23).Parse(input);
if (!_x23.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x23)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x23.Instance, _x23.Remainder);
            }
        }
    }
    
}
