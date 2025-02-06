namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x77Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x77> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x77>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x77> Parse(IInput<char>? input)
            {
                var _x77 = CombinatorParsingV2.Parse.Char((char)0x77).Parse(input);
if (!_x77.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x77)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x77.Instance, _x77.Remainder);
            }
        }
    }
    
}
