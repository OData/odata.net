namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x79Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x79> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x79>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x79> Parse(IInput<char>? input)
            {
                var _x79 = CombinatorParsingV2.Parse.Char((char)0x79).Parse(input);
if (!_x79.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x79)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x79.Instance, _x79.Remainder);
            }
        }
    }
    
}
