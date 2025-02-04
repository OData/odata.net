namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x72Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x72> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x72>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x72> Parse(IInput<char>? input)
            {
                var _x72 = CombinatorParsingV2.Parse.Char((char)0x72).Parse(input);
if (!_x72.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x72)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x72.Instance, _x72.Remainder);
            }
        }
    }
    
}
