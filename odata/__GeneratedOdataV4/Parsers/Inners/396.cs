namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x46Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x46> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x46>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x46> Parse(IInput<char>? input)
            {
                var _x46 = CombinatorParsingV2.Parse.Char((char)0x46).Parse(input);
if (!_x46.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x46)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x46.Instance, _x46.Remainder);
            }
        }
    }
    
}
