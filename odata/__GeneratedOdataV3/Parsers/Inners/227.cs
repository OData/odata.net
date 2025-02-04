namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x43Parser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._x43> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._x43>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._x43> Parse(IInput<char>? input)
            {
                var _x43 = CombinatorParsingV2.Parse.Char((char)0x43).Parse(input);
if (!_x43.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._x43)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._x43.Instance, _x43.Remainder);
            }
        }
    }
    
}
