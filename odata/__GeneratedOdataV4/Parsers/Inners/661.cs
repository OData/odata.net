namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _x21Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._x21> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._x21>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._x21> Parse(IInput<char>? input)
            {
                var _x21 = CombinatorParsingV2.Parse.Char((char)0x21).Parse(input);
if (!_x21.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._x21)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._x21.Instance, _x21.Remainder);
            }
        }
    }
    
}
