namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x48Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x48> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x48>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x48> Parse(IInput<char>? input)
            {
                var _x48 = CombinatorParsingV2.Parse.Char((char)0x48).Parse(input);
if (!_x48.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x48)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x48.Instance, _x48.Remainder);
            }
        }
    }
    
}
